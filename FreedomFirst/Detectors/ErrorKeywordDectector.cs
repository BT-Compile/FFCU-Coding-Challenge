using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FreedomFirst.Core.Models;

namespace FreedomFirst.Core.Detectors
{
    public class ErrorKeywordDetector : IIssueDetector
    {
        // This regex pattern matches common error keywords in log files.
        private static readonly Regex ErrorPattern =
        new(@"\b(ERROR|FATAL|EXCEPTION|FAILED)\b", RegexOptions.IgnoreCase);

        // This method detects error keywords in the provided lines of a report.
        public IEnumerable<Finding> Detect(string reportName, IReadOnlyList<string> lines)
        {
            // Iterate through each line in the log file and check for error keywords using the regex pattern.
            for (int i = 0; i < lines.Count; i++)
            {
                // If the current line does not match the error pattern, skip to the next line.
                if (!ErrorPattern.IsMatch(lines[i])) continue;

                // If an error keyword is found, extract the context of the error.
                yield return new Finding(
                    reportName, "Error message", Severity.Error,
                    i + 1, lines[i].Trim());
            }
        }
    }
}
