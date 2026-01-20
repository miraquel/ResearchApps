using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ResearchApps.Service.Vm;

namespace ResearchApps.Web.Reports;

/// <summary>
/// QuestPDF document implementation for Customer Order Summary Report
/// </summary>
public class CoSummaryReportDocument : IDocument
{
    private readonly CoSummaryReportVm _model;

    public CoSummaryReportDocument(CoSummaryReportVm model)
    {
        _model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(5);
            
            // Company/System name
            column.Item().Text("ResearchApps")
                .FontSize(14)
                .Bold()
                .FontColor(Colors.Grey.Darken3);
            
            // Title
            column.Item().PaddingTop(10).Text("Customer Order Summary Report")
                .FontSize(20)
                .Bold()
                .FontColor(Colors.Blue.Darken3);
            
            // Date range in a subtle box
            column.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(text =>
                    {
                        text.Span("Report Period: ").FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                        text.Span($"{_model.StartDate:dd MMMM yyyy}").FontSize(10).FontColor(Colors.Grey.Darken1);
                        text.Span(" to ").FontSize(10).FontColor(Colors.Grey.Darken2);
                        text.Span($"{_model.EndDate:dd MMMM yyyy}").FontSize(10).FontColor(Colors.Grey.Darken1);
                    });
                });
                
                row.ConstantItem(120).AlignRight().Text(text =>
                {
                    text.Span("Generated: ").FontSize(9).SemiBold().FontColor(Colors.Grey.Darken2);
                    text.Span(DateTime.Now.ToString("dd MMM yyyy")).FontSize(9).FontColor(Colors.Grey.Darken1);
                });
            });
            
            // Decorative line
            column.Item().PaddingTop(10).PaddingBottom(15).LineHorizontal(2).LineColor(Colors.Blue.Darken3);
        });
    }

    void ComposeContent(IContainer container)
    {
        if (!_model.Items.Any())
        {
            container.AlignCenter().AlignMiddle().Column(column =>
            {
                column.Spacing(15);
                column.Item().Text("No data available for the selected period")
                    .FontSize(14)
                    .FontColor(Colors.Grey.Medium);
                column.Item().Text("Please select a different date range")
                    .FontSize(11)
                    .FontColor(Colors.Grey.Lighten1);
            });
            return;
        }

        container.Table(table =>
        {
            // Define columns with better proportions
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60);      // NO
                columns.RelativeColumn(4);       // CUSTOMER NAME
                columns.ConstantColumn(120);     // AMOUNT
            });

            // Header with professional styling
            table.Header(header =>
            {
                header.Cell().Element(HeaderCellStyle).AlignCenter().Text("No.");
                header.Cell().Element(HeaderCellStyle).Text("Customer Name");
                header.Cell().Element(HeaderCellStyle).AlignRight().Text("Amount (IDR)");
            });

            // Data rows with alternating colors
            var rowIndex = 0;
            foreach (var item in _model.Items)
            {
                var isEvenRow = rowIndex % 2 == 0;
                
                table.Cell().Element(container => DataCellStyle(container, isEvenRow))
                    .AlignCenter().Text(item.No.ToString());
                table.Cell().Element(container => DataCellStyle(container, isEvenRow))
                    .Text(item.CustomerName);
                table.Cell().Element(container => DataCellStyle(container, isEvenRow))
                    .AlignRight().Text($"{item.Amount:N0}");
                
                rowIndex++;
            }

            // Summary row with total
            var totalAmount = _model.Items.Sum(x => x.Amount);
            
            table.Cell().ColumnSpan(2).Element(TotalLabelCellStyle)
                .AlignRight().PaddingRight(10)
                .Text("GRAND TOTAL").Bold();
            table.Cell().Element(TotalValueCellStyle)
                .AlignRight()
                .Text($"{totalAmount:N0}").Bold().FontSize(11);
            
            // Count summary
            table.Cell().ColumnSpan(3).Element(FooterInfoStyle)
                .Text($"Total {_model.Items.Count} customer(s)")
                .FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Row(row =>
        {
            row.RelativeItem().AlignLeft().Text(text =>
            {
                text.Span("ResearchApps © 2026").FontSize(8).FontColor(Colors.Grey.Medium);
            });

            row.RelativeItem().AlignCenter().Text(text =>
            {
                text.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                text.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                text.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
            });
            
            row.RelativeItem().AlignRight().Text(text =>
            {
                text.Span(DateTime.Now.ToString("HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
            });
        });
    }

    IContainer HeaderCellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Background(Colors.Blue.Darken3)
            .PaddingVertical(10)
            .PaddingHorizontal(8)
            .DefaultTextStyle(x => x.SemiBold().FontSize(10).FontColor(Colors.White));
    }

    IContainer DataCellStyle(IContainer container, bool isEvenRow)
    {
        var backgroundColor = isEvenRow ? Colors.Grey.Lighten5 : Colors.White;
        
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Background(backgroundColor)
            .PaddingVertical(8)
            .PaddingHorizontal(8)
            .DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));
    }

    IContainer TotalLabelCellStyle(IContainer container)
    {
        return container
            .BorderTop(2)
            .BorderBottom(2)
            .BorderLeft(1)
            .BorderRight(1)
            .BorderColor(Colors.Blue.Darken3)
            .Background(Colors.Blue.Lighten5)
            .PaddingVertical(10)
            .PaddingHorizontal(8)
            .DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Blue.Darken3));
    }

    IContainer TotalValueCellStyle(IContainer container)
    {
        return container
            .BorderTop(2)
            .BorderBottom(2)
            .BorderLeft(1)
            .BorderRight(1)
            .BorderColor(Colors.Blue.Darken3)
            .Background(Colors.Blue.Lighten4)
            .PaddingVertical(10)
            .PaddingHorizontal(8)
            .DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Blue.Darken3));
    }

    IContainer FooterInfoStyle(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderLeft(1)
            .BorderRight(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Background(Colors.Grey.Lighten4)
            .PaddingVertical(6)
            .PaddingHorizontal(8);
    }
}
