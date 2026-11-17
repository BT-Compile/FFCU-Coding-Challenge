using System;
using System.Collections.Generic;
using System.Text;
using FreedomFirst.Core.Detectors;
using FreedomFirst.Core.Models;

namespace FreedomFirst.Core.Models
{
    public record AnalysisResult(IReadOnlyList<Finding> Findings, int ReportsProcessed)
    {
        // A record that represents the result of analyzing log files, including the findings and the number of reports processed.
        public int TotalFindings => Findings.Count;
        public bool HasIssues => Findings.Count > 0;
    }
}
