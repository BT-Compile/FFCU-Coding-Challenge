# Batch Report Analyzer

This is a small .NET console tool that scans a folder of sample reports, detects likely failures and suspicious data, prints a summary, and writes the findings to a PDF report.

## How to use it

Download the .exe in the releases tab and drop it in a folder with all the sent coding challenge forms and run it. It should generate a report folder and a report with all found issues.

## What it does

1. Reads every .txt file in a folder.
2. Runs each report through a set of detectors.
3. Prints a short summary to the console.
4. Writes a PDF to the folder the application exists in listing each finding.

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

PDFsharp-MigraDoc is set to 6.2.4 on purpose. Versions 6.5.0 and later require licenses.

## Developer Guide

```
FreedomFirst.Core/        Class library
  Models/                 Finding, Severity, AnalysisResult
  Detectors/              IIssueDetector and the detector classes
  Services/               ReportAnalyzer (reads files, runs detectors, collects findings)
FreedomFirst.CLI/         Console app
  Reporting/              PdfReportWriter (builds the PDF)
```

### Development Process

I wanted to keep the application really simple. Talking and working with end users for my previous roles has taught me to follow the KISS principles and try to tailor
software to keep it easy for them to access and use. Originally, I was going to make a simple browser application to display all the data but looking at the constraints and business scenario, 
it seemed to add complexity to a already complex situation for the user. In the end the drag and drop console application seemed like a solid approach.

When I set off writing this application, I wanted to add modularity in all aspects of the program. I broke it down into two parts the core and the CLI application. I wrote all of the data classes
(Analysis results, Finding, and Severity) first to cover the basics of what I wanted the application to do. Afterwards, I built the detectors and looked through the data. 
I noticed the obvious errors and dug a little deeper making some assumptions and mental flags. Then I added a nuget package I'm familiar with (PDFSharp) as its open source, free, and easy to use.
Finally, I set up the report, built the simple program, and started some quick testing.

- ReportAnalyzer is given its detectors through its constructor, so adding a rule means writing one class and registering it in Program.cs.
- Detectors implement IIssueDetector and return "Finding" objects.
- The PDF library lives only in the CLI project, so Core has no third-party dependencies.

## Assumptions

- Every .txt file in the folder is treated as a report. Subfolders are not scanned, so the Reports output folder is never read as input.
- A report's column header starts with ACCT, and customer rows start with *.
- Columns are separated by two or more spaces. Single spaces stay inside a value.
- The years limit (70) is a fixed value in DataRowDetector since Freedom First has been around for 70 years.

## Possible improvements

- Load detection rules from a config so no code needs to be change (ie, the years the bank has been open, or what error keywords to detect).
- Keep going when a single report cannot be read, and report that file as a finding.
- Stream large files line by line and summarize very large result sets in the PDF.
- Duplicate members and ID's, unsure if this is intentional as a member can have multiple accounts.
