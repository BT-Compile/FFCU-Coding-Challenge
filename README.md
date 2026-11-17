# Batch Report Analyzer

A small .NET console tool that scans a folder of batch job output reports (`.txt` files), detects likely failures and suspicious data, prints a summary, and writes the findings to a PDF report.

## How to use it

Download the .exe in the releases tab and drop it in a folder with all the sent coding challenge forms and run it. It should generate a report folder and a report with all found issues.

## What it does

1. Reads every `.txt` file in a folder (the folder you pass in, or the folder the `.exe` is in).
2. Runs each report through a set of detectors.
3. Prints a one-line summary to the console.
4. Writes a PDF to `<folder>\Reports\report.pdf` listing each finding.

### Exit codes

| Code | Meaning |
|---|---|
| 0 | All reports checked, no issues found |
| 1 | Issues found (see the PDF) |
| 2 | The tool itself failed (an error message is printed) |

## Technologies and dependencies

| Item | Purpose |
|---|---|
| C# / .NET 10 | Language and runtime |
| PDFsharp-MigraDoc 6.2.4 | Builds the PDF report |

PDFsharp-MigraDoc is pinned to 6.2.4 on purpose: it is MIT-licensed, while versions 6.5.0 and later require licenses.

## Assumptions

- Every `.txt` file in the folder is treated as a report. Subfolders are not scanned, so the `Reports` output folder is never read as input.
- A report's column header starts with `ACCT`, and customer rows start with `*`.
- Columns are separated by two or more spaces. Single spaces stay inside a value.
- The years limit (70) is a fixed value in `DataRowDetector` since Freedom First has been around for 70 years.

## Developer Guide

```
FreedomFirst.Core/        Class library: the analysis logic (no UI or PDF code)
  Models/                 Finding, Severity, AnalysisResult
  Detectors/              IIssueDetector and the detector classes
  Services/               ReportAnalyzer (reads files, runs detectors, collects findings)
FreedomFirst.CLI/         Console app: entry point, argument handling, exit codes
  Reporting/              PdfReportWriter (builds the PDF)
```

### How the pieces fit

```
folder -> ReportAnalyzer -> each detector -> Findings -> AnalysisResult -> console summary + PDF
```

- `ReportAnalyzer` is given its detectors through its constructor (dependency injection), so adding a rule means writing one class and registering it in `Program.cs`.
- Detectors implement `IIssueDetector` and return `Finding` objects. They know nothing about the PDF.
- The PDF library lives only in the CLI project, so Core has no third-party dependencies.

## Possible improvements

- Load detection rules from a config so no code needs to be change (ie, the years the bank has been open).
- Keep going when a single report cannot be read, and report that file as a finding.
- Stream large files line by line and summarize very large result sets in the PDF.
- Duplicate members and ID's but this maybe intentional as a member can have multiple accounts.
