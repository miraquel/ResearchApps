using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ResearchApps.Service.Vm;
using System.Data;

namespace ResearchApps.Web.Services;

/// <summary>
/// Interface for generating PDF reports.
/// </summary>
public interface IReportGeneratorService
{
    /// <summary>
    /// Generates a fully templated PDF report.
    /// </summary>
    byte[] GenerateFullReport(ReportGenerateVm reportData, DataTable data);
    
    /// <summary>
    /// Generates a partially templated PDF for continuous paper printing.
    /// </summary>
    byte[] GeneratePartialReport(ReportGenerateVm reportData, DataTable data);
    
    /// <summary>
    /// Generates a partially templated PDF using field coordinates for continuous paper printing.
    /// </summary>
    byte[] GeneratePartialReportWithCoordinates(ReportGenerateVm reportData, DataTable data, List<ReportFieldCoordinateVm> fieldCoordinates);
}

/// <summary>
/// Service for generating PDF reports using QuestPDF.
/// </summary>
public class ReportGeneratorService : IReportGeneratorService
{
    public byte[] GenerateFullReport(ReportGenerateVm reportData, DataTable data)
    {
        // Configure QuestPDF license (Community license for open-source)
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                // Set page size
                var pageSize = GetPageSize();
                page.Size(pageSize);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                // Header
                page.Header().Element(subContainer => ComposeHeader(subContainer, reportData));

                // Content - Table with data
                page.Content().Element(subContainer => ComposeContent(subContainer, data));

                // Footer
                page.Footer().Element(subContainer => ComposeFooter(subContainer, reportData));
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    public byte[] GeneratePartialReport(ReportGenerateVm reportData, DataTable data)
    {
        // Configure QuestPDF license
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                // For continuous paper, use custom page size
                page.Size(new PageSize(21, 14, Unit.Centimetre)); // Standard continuous paper size
                page.MarginHorizontal(0.5f, Unit.Centimetre);
                page.MarginVertical(0.3f, Unit.Centimetre);
                page.PageColor(Colors.Transparent);
                page.DefaultTextStyle(x => x.FontSize(10));

                // Only the variable data content - no header/footer decorations
                page.Content().Element(subContainer => ComposePartialContent(subContainer, reportData, data));
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    public byte[] GeneratePartialReportWithCoordinates(ReportGenerateVm reportData, DataTable data, List<ReportFieldCoordinateVm> fieldCoordinates)
    {
        // Configure QuestPDF license
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                // For continuous paper, use custom page size
                page.Size(new PageSize(21, 14, Unit.Centimetre)); // Standard continuous paper size
                page.MarginHorizontal(0, Unit.Millimetre);
                page.MarginVertical(0, Unit.Millimetre);
                page.PageColor(Colors.Transparent);
                page.DefaultTextStyle(x => x.FontSize(10));

                // Use Layers for precise positioning without deprecated Canvas API
                page.Content().Layers(layers =>
                {
                    // Group coordinates by repeating vs non-repeating
                    var staticFields = fieldCoordinates.Where(f => !f.IsRepeating).OrderBy(f => f.DisplayOrder).ToList();
                    var repeatingFields = fieldCoordinates.Where(f => f.IsRepeating).OrderBy(f => f.DisplayOrder).ToList();

                    // Create a layer for static fields
                    foreach (var field in staticFields)
                    {
                        var value = GetFieldValue(field, reportData, data, 0);
                        layers.Layer().Element(e => DrawFieldUsingPadding(e, field, value));
                    }

                    // Create layers for repeating fields for each data row
                    if (data.Rows.Count > 0 && repeatingFields.Count > 0)
                    {
                        for (var rowIndex = 0; rowIndex < data.Rows.Count; rowIndex++)
                        {
                            foreach (var field in repeatingFields)
                            {
                                var adjustedField = new ReportFieldCoordinateVm
                                {
                                    FieldName = field.FieldName,
                                    DisplayLabel = field.DisplayLabel,
                                    XPosition = field.XPosition,
                                    YPosition = field.YPosition + (field.RowIncrement * rowIndex),
                                    Width = field.Width,
                                    Height = field.Height,
                                    FontSize = field.FontSize,
                                    FontFamily = field.FontFamily,
                                    Alignment = field.Alignment,
                                    IsBold = field.IsBold,
                                    IsItalic = field.IsItalic,
                                    DataBinding = field.DataBinding,
                                    FormatString = field.FormatString
                                };
                                var value = GetFieldValue(adjustedField, reportData, data, rowIndex);
                                layers.Layer().Element(e => DrawFieldUsingPadding(e, adjustedField, value));
                            }
                        }
                    }
                });
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    private static void DrawFieldUsingPadding(IContainer container, ReportFieldCoordinateVm field, string value)
    {
        // Convert mm to points (approximately 2.834645669 points per mm)
        var xPadding = (float)field.XPosition;
        var yPadding = (float)field.YPosition;
        
        container
            .PaddingLeft(xPadding, Unit.Millimetre)
            .PaddingTop(yPadding, Unit.Millimetre)
            .Width((float)field.Width, Unit.Millimetre)
            .Height((float)field.Height, Unit.Millimetre)
            .Text(text =>
            {
                // Apply alignment
                switch (field.Alignment)
                {
                    case 2: // Center
                        text.AlignCenter();
                        break;
                    case 3: // Right
                        text.AlignRight();
                        break;
                    default: // Left
                        text.AlignLeft();
                        break;
                }
                
                // Build the text style
                var textSpan = text.Span(value);
                textSpan.FontSize((float)field.FontSize);
                textSpan.FontFamily(field.FontFamily);
                
                if (field.IsBold)
                    textSpan.Bold();
                if (field.IsItalic)
                    textSpan.Italic();
            });
    }

    private static string GetFieldValue(ReportFieldCoordinateVm field, ReportGenerateVm reportData, DataTable data, int rowIndex)
    {
        // First try to get from DataTable using DataBinding
        if (!string.IsNullOrEmpty(field.DataBinding) && data.Columns.Contains(field.DataBinding) && rowIndex < data.Rows.Count)
        {
            var rawValue = data.Rows[rowIndex][field.DataBinding];
            return FormatValue(rawValue, field.FormatString);
        }

        // Then try to get from parameter values
        if (reportData.ParameterValues.TryGetValue(field.FieldName, out var paramValue))
        {
            return paramValue ?? string.Empty;
        }

        return string.Empty;
    }

    private static string FormatValue(object? value, string? formatString)
    {
        if (value == null || value == DBNull.Value) return string.Empty;

        if (string.IsNullOrEmpty(formatString)) return value.ToString() ?? string.Empty;

        return value switch
        {
            decimal decimalValue => decimalValue.ToString(formatString),
            double doubleValue => doubleValue.ToString(formatString),
            float floatValue => floatValue.ToString(formatString),
            int intValue => intValue.ToString(formatString),
            DateTime dateValue => dateValue.ToString(formatString),
            _ => value.ToString() ?? string.Empty
        };
    }

    private static PageSize GetPageSize()
    {
        // Default to A4 if not specified or invalid
        var pageSize = "A4"; // Default page size

        var size = pageSize.ToUpperInvariant() switch
        {
            "A3" => PageSizes.A3,
            "A4" => PageSizes.A4,
            "A5" => PageSizes.A5,
            "LETTER" => PageSizes.Letter,
            "LEGAL" => PageSizes.Legal,
            _ => PageSizes.A4
        };

        // Check orientation from the first parameter's report or default to portrait
        // For now, default to portrait
        return size;
    }

    private void ComposeHeader(IContainer container, ReportGenerateVm reportData)
    {
        container
            .PaddingBottom(10)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Medium)
            .Column(column =>
            {
                column.Item()
                    .Text(reportData.ReportName)
                    .Bold()
                    .FontSize(16)
                    .FontColor(Colors.Blue.Darken2);

                column.Item()
                    .Text($"Generated on: {DateTime.Now:dd MMM yyyy HH:mm:ss}")
                    .FontSize(8)
                    .FontColor(Colors.Grey.Medium);

                // Display parameters used
                if (reportData.ParameterValues.Count > 0)
                {
                    column.Item()
                        .PaddingTop(5)
                        .Row(row =>
                        {
                            foreach (var param in reportData.ParameterValues.Where(p => !string.IsNullOrEmpty(p.Value)))
                            {
                                var paramInfo = reportData.Parameters.FirstOrDefault(p => p.ParameterName == param.Key);
                                if (paramInfo != null)
                                {
                                    row.AutoItem()
                                        .PaddingRight(15)
                                        .Text(text =>
                                        {
                                            text.Span($"{paramInfo.DisplayLabel}: ").SemiBold().FontSize(8);
                                            text.Span(param.Value ?? "-").FontSize(8);
                                        });
                                }
                            }
                        });
                }
            });
    }

    private static void ComposeContent(IContainer container, DataTable data)
    {
        container.PaddingVertical(10).Column(column =>
        {
            if (data.Rows.Count == 0)
            {
                column.Item()
                    .AlignCenter()
                    .PaddingVertical(50)
                    .Text("No data available for the selected parameters.")
                    .FontSize(12)
                    .FontColor(Colors.Grey.Medium);
                return;
            }

            column.Item().Table(table =>
            {
                // Define columns based on DataTable columns
                table.ColumnsDefinition(columns =>
                {
                    foreach (DataColumn _ in data.Columns)
                    {
                        columns.RelativeColumn();
                    }
                });

                // Header row
                table.Header(header =>
                {
                    foreach (DataColumn col in data.Columns)
                    {
                        header.Cell()
                            .Background(Colors.Blue.Darken2)
                            .Padding(5)
                            .Text(col.ColumnName)
                            .FontColor(Colors.White)
                            .Bold()
                            .FontSize(9);
                    }
                });

                // Data rows
                var rowIndex = 0;
                foreach (DataRow row in data.Rows)
                {
                    var bgColor = rowIndex % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                    
                    foreach (DataColumn col in data.Columns)
                    {
                        table.Cell()
                            .Background(bgColor)
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(5)
                            .Text(row[col].ToString() ?? "")
                            .FontSize(9);
                    }
                    rowIndex++;
                }
            });

            // Summary row
            column.Item()
                .PaddingTop(10)
                .AlignRight()
                .Text($"Total Records: {data.Rows.Count}")
                .FontSize(9)
                .FontColor(Colors.Grey.Darken1);
        });
    }

    private void ComposePartialContent(IContainer container, ReportGenerateVm reportData, DataTable data)
    {
        // For partial reports, we only print the variable data
        // This is designed for pre-printed continuous paper forms
        container.Column(column =>
        {
            // Print parameter values in specific positions for the form
            foreach (var param in reportData.Parameters.OrderBy(p => p.DisplayOrder))
            {
                var value = reportData.ParameterValues.GetValueOrDefault(param.ParameterName, "");
                
                column.Item()
                    .PaddingVertical(2)
                    .Row(row =>
                    {
                        // Position label (for reference - can be hidden in final print)
                        row.ConstantItem(100)
                            .Text(param.DisplayLabel + ":")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Medium);
                        
                        // Value to be printed
                        row.RelativeItem()
                            .Text(value ?? "")
                            .FontSize(10);
                    });
            }

            // If there's data, print minimal table info
            if (data.Rows.Count > 0)
            {
                column.Item()
                    .PaddingTop(5)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (DataColumn _ in data.Columns)
                            {
                                columns.RelativeColumn();
                            }
                        });

                        // Only data - no headers (they're pre-printed)
                        foreach (DataRow row in data.Rows)
                        {
                            foreach (DataColumn col in data.Columns)
                            {
                                table.Cell()
                                    .PaddingHorizontal(2)
                                    .PaddingVertical(1)
                                    .Text(row[col].ToString() ?? "")
                                    .FontSize(9);
                            }
                        }
                    });
            }
        });
    }

    private void ComposeFooter(IContainer container, ReportGenerateVm reportData)
    {
        container
            .PaddingTop(10)
            .BorderTop(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Row(row =>
            {
                row.RelativeItem()
                    .AlignLeft()
                    .Text(text =>
                    {
                        text.Span("Report: ").FontSize(8).FontColor(Colors.Grey.Medium);
                        text.Span(reportData.ReportName).FontSize(8);
                    });

                row.RelativeItem()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Page ").FontSize(8);
                        text.CurrentPageNumber().FontSize(8);
                        text.Span(" of ").FontSize(8);
                        text.TotalPages().FontSize(8);
                    });

                row.RelativeItem()
                    .AlignRight()
                    .Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                    .FontSize(8)
                    .FontColor(Colors.Grey.Medium);
            });
    }
}

