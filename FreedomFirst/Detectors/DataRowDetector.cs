using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FreedomFirst.Core.Models;

namespace FreedomFirst.Core.Detectors
{
    public class DataRowDetector : IIssueDetector
    {
        public IEnumerable<Finding> Detect(string reportName, IReadOnlyList<string> lines)
        {
            var findings = new List<Finding>();

            // Variable to hold the header line, containing the column names.
            string? header = lines.FirstOrDefault(l => l.StartsWith("ACCT"));
            if (header == null) return findings;

            // Creates an array of column names by detecting two+ blank spaces and trims whitespace.
            string[] columns = Regex.Split(header.Trim(), @"\s{2,}");

            // Position of the years column
            int yearsColumn = Array.IndexOf(columns, "YEARS WITH CREDIT UNION");

            // Iterate through each line in the log file and check for data row issues.
            for (int i = 0; i < lines.Count; i++)
            {
                // Rows start with *; skip everything else.
                if (!lines[i].StartsWith("*")) continue;

                string[] fields = Regex.Split(lines[i].Trim(), @"\s{2,}");

                // Check to see if the row is missing ID and Balance.
                if (fields.Length < columns.Length)
                {
                    string missing = string.Join(", ", columns[fields.Length..]);
                    findings.Add(new Finding(reportName, "Incomplete row", Severity.Warning,
                        i + 1, $"Missing {missing} for account {fields[0]}"));
                    continue; 
                }

                // Check if the years value is above 70.
                if (yearsColumn >= 0
                    && int.TryParse(fields[yearsColumn], out int years)
                    && years > 70)
                {
                    findings.Add(new Finding(reportName, "Value out of range", Severity.Warning,
                        i + 1, $"Years with credit union is {years} (limit {70}) for account {fields[0]}"));
                }
            }

            return findings;
        }
    }
}
