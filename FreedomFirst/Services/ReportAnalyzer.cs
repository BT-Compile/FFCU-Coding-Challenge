using System;
using System.Collections.Generic;
using System.Text;
using FreedomFirst.Core.Detectors;
using FreedomFirst.Core.Models;

namespace FreedomFirst.Core.Services
{
    public interface IReportAnalyzer
    {
        AnalysisResult Analyze(string folder);
    }

    // The ReportAnalyzer class is responsible for analyzing log files using a collection of issue detectors.
    public class ReportAnalyzer : IReportAnalyzer
    {
        // A collection of issue detectors used to analyze log files.
        private readonly IEnumerable<IIssueDetector> _detectors;
        public ReportAnalyzer(IEnumerable<IIssueDetector> detectors)
        {
            _detectors = detectors;
        }

        // Analyzes the log files in the specified folder and returns the analysis result, including findings and the number of reports processed.
        public AnalysisResult Analyze(string folder)
        {
            var findings = new List<Finding>();
            string[] files = Directory.GetFiles(folder, "*.txt");

            foreach (string file in files)
            {
                string[] lines = File.ReadAllLines(file);
                string reportName = Path.GetFileName(file);

                foreach (IIssueDetector detector in _detectors)
                    findings.AddRange(detector.Detect(reportName, lines));
            }

            return new AnalysisResult(findings, files.Length);
        }
    }
}
