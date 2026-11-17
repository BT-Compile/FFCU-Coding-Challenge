using System;
using System.Collections.Generic;
using System.Text;
using FreedomFirst.Core.Models;

namespace FreedomFirst.Core.Detectors
{
    public interface IIssueDetector
    {
        // This method detects issues in the provided lines of a report.
        IEnumerable<Finding> Detect(string reportName, IReadOnlyList<string> lines);
    }
}
