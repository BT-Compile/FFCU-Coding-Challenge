using System;
using FreedomFirst.Core.Models;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;

namespace FreedomFirst.CLI.Reporting;

public class PdfReportWriter
{

    // Writes the analysis result to a PDF file
    public void Write(AnalysisResult result, string outputPath)
    {
        // Use windows fonts
        GlobalFontSettings.UseWindowsFontsUnderWindows = true;

        var document = new Document();
        document.Info.Title = "Batch Job Report Analysis";

        
        Section section = document.AddSection();
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.Orientation = Orientation.Landscape;
        section.PageSetup.LeftMargin = Unit.FromCentimeter(1.5);
        section.PageSetup.RightMargin = Unit.FromCentimeter(1.5);

        // Title
        Paragraph title = section.AddParagraph("Batch Job Report Analysis");
        title.Format.Font.Size = 18;
        title.Format.Font.Bold = true;

        // Short sub heading of title
        Paragraph summary = section.AddParagraph(
            $"{result.TotalFindings} issue(s) found in {result.ReportsProcessed} report(s) " +
            $"- generated {DateTime.Now:yyyy-MM-dd HH:mm}");
        summary.Format.SpaceAfter = Unit.FromPoint(10);

        // The findings table
        Table table = section.AddTable();
        table.Format.Font.Size = 9;
        table.Borders.Width = 0.25;
        table.Borders.Color = Colors.Gray;
        table.LeftPadding = 3;
        table.RightPadding = 3;
        table.TopPadding = 2;
        table.BottomPadding = 2;

        // Column widths
        table.AddColumn(Unit.FromCentimeter(4));
        table.AddColumn(Unit.FromCentimeter(4));
        table.AddColumn(Unit.FromCentimeter(2));
        table.AddColumn(Unit.FromCentimeter(1.5));
        table.AddColumn(Unit.FromCentimeter(15));

        // Header rows
        Row header = table.AddRow();
        header.HeadingFormat = true;
        header.Format.Font.Bold = true;
        header.Shading.Color = Colors.LightGray;
        header.Cells[0].AddParagraph("Report");
        header.Cells[1].AddParagraph("Issue");
        header.Cells[2].AddParagraph("Severity");
        header.Cells[3].AddParagraph("Line");
        header.Cells[4].AddParagraph("Details");

        // One row per finding
        foreach (Finding finding in result.Findings)
        {
            Row row = table.AddRow();
            row.Cells[0].AddParagraph(finding.ReportName);
            row.Cells[1].AddParagraph(finding.IssueType);

            // SeverityColor method maps each enum value to a color
            Paragraph severityText = row.Cells[2].AddParagraph(finding.Severity.ToString());
            severityText.Format.Font.Bold = true;
            severityText.Format.Font.Color = SeverityColor(finding.Severity);
            row.Cells[3].AddParagraph(finding.LineNumber?.ToString() ?? "-");
            row.Cells[4].AddParagraph(finding.Details);
        }

        // Footer
        Paragraph footer = section.Footers.Primary.AddParagraph();
        footer.Format.Alignment = ParagraphAlignment.Center;
        footer.AddText("Page ");
        footer.AddPageField();
        footer.AddText(" of ");
        footer.AddNumPagesField();

        // Render the MigraDoc model to a PDF and save it
        var renderer = new PdfDocumentRenderer { Document = document };
        renderer.RenderDocument();
        renderer.PdfDocument.Save(outputPath);
    }

    // A switch expression maps each enum value to a color
    private static Color SeverityColor(Severity severity) => severity switch
    {
        Severity.Critical => Colors.DarkRed,
        Severity.Error => Colors.Red,
        Severity.Warning => Colors.Orange,
        _ => Colors.Gray
    };
}