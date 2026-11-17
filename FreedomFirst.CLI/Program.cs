using FreedomFirst.CLI.Reporting;
using FreedomFirst.Core.Detectors;
using FreedomFirst.Core.Services;

namespace FreedomFirst.CLI;

public class Program
{
    public static int Main(string[] args)
    {
        // The exit code will be returned to the OS. 0 = clean, 1 = issues found, 2 = tool failed.
        int exitCode;

        try
        {
            // Set the folder variable to the folder the exe is in.
            string folder = args.Length > 0 ? args[0] : AppContext.BaseDirectory;
            Console.WriteLine($"Analyzing reports in: {folder}");

            var analyzer = new ReportAnalyzer(new IIssueDetector[] { new ErrorKeywordDetector(), new DataRowDetector() });
            var result = analyzer.Analyze(folder);

            Console.WriteLine($"{result.TotalFindings} issue(s) found in {result.ReportsProcessed} report(s).");

            // Default output: <exe folder>/Reports/report.pdf
            string outputPath = args.Length > 1
                ? args[1]
                : Path.Combine(folder, "Reports", $"report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);
            new PdfReportWriter().Write(result, outputPath);
            Console.WriteLine($"Report written to {Path.GetFullPath(outputPath)}");
            Console.WriteLine("Analysis complete.");

            return result.HasIssues ? 1 : 0;
        }
        catch (Exception ex)
        {
            // Show a readable message instead of the window just disappearing.
            // A different exit code (2) tells scripts "the TOOL failed," not "the reports had issues."
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 2;
        }
        finally
        {
            // Runs on every path, including after the returns above.
            if (!Console.IsInputRedirected)
            {
                Console.WriteLine();
                Console.Write("Press Enter to close this window...");
                Console.ReadLine();
            }
        }

        return exitCode;
    }
}