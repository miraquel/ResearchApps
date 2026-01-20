using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ResearchApps.Service.Vm;

namespace ResearchApps.Web.Reports;

public class CoDetailReportDocument : IDocument
{
    private readonly CoDetailReportVm _model;

    public CoDetailReportDocument(CoDetailReportVm model)
    {
        _model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            // Company name
            column.Item().Text(text =>
            {
                text.Span("ResearchApps").Bold().FontSize(16).FontColor(Colors.Blue.Darken3);
            });
            
            // Report title
            column.Item().PaddingTop(5).Text(text =>
            {
                text.Span("Customer Order Detail Report").Bold().FontSize(14).FontColor(Colors.Grey.Darken2);
            });
            
            // Date range
            column.Item().PaddingTop(3).Text(text =>
            {
                text.Span("Period: ").FontSize(10).FontColor(Colors.Grey.Darken1);
                text.Span($"{_model.StartDate:dd MMM yyyy}").FontSize(10).Bold().FontColor(Colors.Blue.Darken2);
                text.Span(" to ").FontSize(10).FontColor(Colors.Grey.Darken1);
                text.Span($"{_model.EndDate:dd MMM yyyy}").FontSize(10).Bold().FontColor(Colors.Blue.Darken2);
            });
            
            // Generated date
            column.Item().Text(text =>
            {
                text.Span($"Generated on: {DateTime.Now:dd MMM yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
            });
            
            // Decorative line
            column.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Blue.Darken2);
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingTop(10).Table(table =>
        {
            // Define columns
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(35);  // No
                columns.ConstantColumn(60);  // CO ID
                columns.RelativeColumn(2);   // Customer Name
                columns.RelativeColumn(1.5f); // PO Customer
                columns.RelativeColumn(2);   // Item Name
                columns.ConstantColumn(50);  // Unit
                columns.ConstantColumn(75);  // Wanted Delivery
                columns.ConstantColumn(60);  // Qty CO
                columns.ConstantColumn(60);  // Qty DO
                columns.ConstantColumn(60);  // Qty OS
            });

            // Header
            table.Header(header =>
            {
                header.Cell().Element(HeaderCellStyle).Text("No.");
                header.Cell().Element(HeaderCellStyle).Text("CO ID");
                header.Cell().Element(HeaderCellStyle).Text("Customer Name");
                header.Cell().Element(HeaderCellStyle).Text("PO Customer");
                header.Cell().Element(HeaderCellStyle).Text("Item Name");
                header.Cell().Element(HeaderCellStyle).Text("Unit");
                header.Cell().Element(HeaderCellStyle).Text("Wanted Delivery");
                header.Cell().Element(HeaderCellStyle).Text("Qty CO");
                header.Cell().Element(HeaderCellStyle).Text("Qty DO");
                header.Cell().Element(HeaderCellStyle).Text("Qty OS");
            });

            // Data rows
            var rowNumber = 0;
            foreach (var item in _model.Items)
            {
                rowNumber++;
                var isEvenRow = rowNumber % 2 == 0;

                table.Cell().Element(c => DataCellStyle(c, isEvenRow)).AlignRight().Text(rowNumber.ToString());
                table.Cell().Element(c => DataCellStyle(c, isEvenRow)).Text(item.CoId);
                table.Cell().Element(c => DataCellStyle(c, isEvenRow)).Text(item.CustomerName);
                table.Cell().Element(c => DataCellStyle(c, isEvenRow)).Text(item.PoCustomer);
                table.Cell().Element(c => DataCellStyle(c, isEvenRow)).Text(item.ItemName);
                table.Cell().Element(c => DataCellStyle(c, isEvenRow)).Text(item.UnitName);
                table.Cell().Element(c => DataCellStyle(c, isEvenRow)).Text(item.WantedDeliveryDateStr);
                table.Cell().Element(c => DataCellStyle(c, isEvenRow)).AlignRight().Text(item.QtyCo.ToString("N2"));
                table.Cell().Element(c => DataCellStyle(c, isEvenRow)).AlignRight().Text(item.QtyDo.ToString("N2"));
                table.Cell().Element(c => DataCellStyle(c, isEvenRow)).AlignRight().Text(item.QtyOs.ToString("N2"));
            }

            // Summary section
            if (_model.Items.Any())
            {
                var totalQtyCo = _model.Items.Sum(x => x.QtyCo);
                var totalQtyDo = _model.Items.Sum(x => x.QtyDo);
                var totalQtyOs = _model.Items.Sum(x => x.QtyOs);
                
                // Total row
                table.Cell().ColumnSpan(7).Element(TotalLabelCellStyle).AlignRight().Text("TOTAL:").Bold();
                table.Cell().Element(TotalValueCellStyle).AlignRight().Text(totalQtyCo.ToString("N2")).Bold();
                table.Cell().Element(TotalValueCellStyle).AlignRight().Text(totalQtyDo.ToString("N2")).Bold();
                table.Cell().Element(TotalValueCellStyle).AlignRight().Text(totalQtyOs.ToString("N2")).Bold();
                
                // Record count row
                table.Cell().ColumnSpan(10).Element(SummaryCellStyle).Text($"Total Records: {_model.Items.Count}").FontSize(8).Italic();
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignBottom().Row(row =>
        {
            // Left: Copyright
            row.RelativeItem().AlignLeft().Text(text =>
            {
                text.Span("© 2026 ResearchApps").FontSize(7).FontColor(Colors.Grey.Medium);
            });

            // Center: Page numbers
            row.RelativeItem().AlignCenter().Text(text =>
            {
                text.CurrentPageNumber().FontSize(8);
                text.Span(" / ").FontSize(8);
                text.TotalPages().FontSize(8);
            });

            // Right: Print time
            row.RelativeItem().AlignRight().Text(text =>
            {
                text.Span($"Printed: {DateTime.Now:HH:mm:ss}").FontSize(7).FontColor(Colors.Grey.Medium);
            });
        });
    }

    private IContainer HeaderCellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Blue.Darken3)
            .Background(Colors.Blue.Darken3)
            .Padding(5)
            .DefaultTextStyle(x => x.Bold().FontColor(Colors.White).FontSize(9));
    }

    private IContainer DataCellStyle(IContainer container, bool isEvenRow)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Background(isEvenRow ? Colors.Grey.Lighten5 : Colors.White)
            .Padding(5);
    }

    private IContainer TotalLabelCellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Blue.Darken2)
            .Background(Colors.Blue.Lighten4)
            .Padding(5);
    }

    private IContainer TotalValueCellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Blue.Darken2)
            .Background(Colors.Blue.Lighten3)
            .Padding(5);
    }

    private IContainer SummaryCellStyle(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingTop(5)
            .PaddingLeft(5);
    }
}
