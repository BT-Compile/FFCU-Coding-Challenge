using System;
using System.Collections.Generic;
using System.Text;

namespace FreedomFirst.Core.Models;

// This record represents a finding (issue) detected in a report.
public record Finding (
    string ReportName,
    string IssueType,
    Severity Severity,
    int? LineNumber,
    string Details
    );