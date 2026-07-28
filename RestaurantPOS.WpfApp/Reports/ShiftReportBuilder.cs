using System.Drawing;
using System.IO;
using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using RestaurantPOS.Services;

namespace RestaurantPOS.WpfApp.Reports;

// Builds a shift reconciliation report PDF — same no-real-printer approach as
// ReceiptBuilder (generate a PDF, open it with the OS default viewer), but on
// A4 rather than a thermal-roll page since this is an office document, not a receipt.
public static class ShiftReportBuilder
{
    public static string BuildPdf(ShiftReport report)
    {
        using var reportDoc = new Report();
        var page = new ReportPage { PaperWidth = 210, PaperHeight = 297 };
        reportDoc.Pages.Add(page);

        var band = new ReportTitleBand { Height = Units.Millimeters * 90 };
        page.ReportTitle = band;

        float y = 0;
        AddText(band, 0, y, 180, 8, "BÁO CÁO CA LÀM VIỆC", bold: true); y += 12;
        AddText(band, 0, y, 180, 6, $"Nhân viên: {report.UserFullName}"); y += 7;
        AddText(band, 0, y, 180, 6, $"Mở ca lúc: {report.OpenedAt:HH:mm dd/MM/yyyy}"); y += 7;
        AddText(band, 0, y, 180, 6, $"Đóng ca lúc: {report.ClosedAt:HH:mm dd/MM/yyyy}"); y += 7;
        AddText(band, 0, y, 180, 6, $"Thời gian làm việc: {report.WorkedDuration:hh\\:mm}"); y += 10;

        AddText(band, 0, y, 180, 6, $"Tiền đầu ca: {report.OpeningCash:N0} đ"); y += 7;
        AddText(band, 0, y, 180, 6, $"Doanh thu tiền mặt: {report.CashPayments:N0} đ"); y += 7;
        AddText(band, 0, y, 180, 6, $"Tiền dự kiến trong két: {report.ExpectedCash:N0} đ"); y += 7;
        AddText(band, 0, y, 180, 6, $"Tiền cuối ca (kiểm đếm thực tế): {report.ClosingCash:N0} đ"); y += 7;
        AddText(band, 0, y, 180, 7, $"Chênh lệch: {report.Variance:N0} đ", bold: true); y += 10;

        reportDoc.Prepare();

        var pdfPath = Path.Combine(Path.GetTempPath(), $"shift-report-{report.ShiftId}.pdf");
        reportDoc.Export(new PDFSimpleExport(), pdfPath);
        return pdfPath;
    }

    private static void AddText(ReportTitleBand band, float xMm, float yMm, float widthMm, float heightMm, string text, bool bold = false)
    {
        var obj = new TextObject
        {
            Bounds = new RectangleF(xMm * Units.Millimeters, yMm * Units.Millimeters, widthMm * Units.Millimeters, heightMm * Units.Millimeters),
            Text = text,
            Font = new Font("Arial", 11, bold ? FontStyle.Bold : FontStyle.Regular)
        };
        band.Objects.Add(obj);
    }
}
