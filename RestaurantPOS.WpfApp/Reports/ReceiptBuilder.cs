using System.Drawing;
using System.IO;
using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.WpfApp.Reports;

// Builds a minimal invoice PDF for a paid Order. There's no real printer to test
// against here, so "printing" means generating a PDF and opening it with the OS
// default viewer — see PaymentViewModel.PrintReceiptCommand.
public static class ReceiptBuilder
{
    private const float RowHeightMm = 6f;

    public static string BuildPdf(Order order)
    {
        using var report = new Report();
        var page = new ReportPage { PaperWidth = 80, PaperHeight = 200 };
        report.Pages.Add(page);

        var band = new ReportTitleBand { Height = Units.Millimeters * (55 + RowHeightMm * order.OrderItems.Count) };
        page.ReportTitle = band;

        float y = 0;
        AddText(band, 0, y, 70, 6, "HÓA ĐƠN THANH TOÁN", bold: true); y += 8;
        AddText(band, 0, y, 70, 5, $"Bàn: {order.Table.TableName}"); y += 5;
        AddText(band, 0, y, 70, 5, $"Đơn hàng: #{order.OrderId}"); y += 5;
        AddText(band, 0, y, 70, 5, $"Ngày: {(order.Payment?.PaidAt ?? DateTime.Now):dd/MM/yyyy HH:mm}"); y += 7;

        AddText(band, 0, y, 40, 5, "Món", bold: true);
        AddText(band, 40, y, 10, 5, "SL", bold: true);
        AddText(band, 50, y, 20, 5, "T.Tiền", bold: true);
        y += 6;

        foreach (var item in order.OrderItems)
        {
            AddText(band, 0, y, 40, RowHeightMm, item.MenuItem.ItemName);
            AddText(band, 40, y, 10, RowHeightMm, item.Quantity.ToString());
            AddText(band, 50, y, 20, RowHeightMm, (item.UnitPrice * item.Quantity).ToString("N0"));
            y += RowHeightMm;
        }

        y += 3;
        AddText(band, 0, y, 70, 6, $"Tổng cộng: {order.Total:N0} đ", bold: true); y += 6;
        var methodLabel = order.Payment?.Method == PaymentMethod.BankTransfer ? "Chuyển khoản" : "Tiền mặt";
        AddText(band, 0, y, 70, 5, $"Hình thức: {methodLabel}");

        report.Prepare();

        var pdfPath = Path.Combine(Path.GetTempPath(), $"receipt-order-{order.OrderId}.pdf");
        report.Export(new PDFSimpleExport(), pdfPath);
        return pdfPath;
    }

    private static void AddText(ReportTitleBand band, float xMm, float yMm, float widthMm, float heightMm, string text, bool bold = false)
    {
        var obj = new TextObject
        {
            Bounds = new RectangleF(xMm * Units.Millimeters, yMm * Units.Millimeters, widthMm * Units.Millimeters, heightMm * Units.Millimeters),
            Text = text,
            Font = new Font("Arial", 9, bold ? FontStyle.Bold : FontStyle.Regular)
        };
        band.Objects.Add(obj);
    }
}
