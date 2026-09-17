using CoffeeShopManager.BLL.DTOs;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;

namespace CoffeeShopManager.BLL.Services;

public interface IExportService
{
    Task<byte[]> ExportDoanhThuToExcelAsync(IEnumerable<DoanhThuTheoNgayDto> data, string title);
    Task<byte[]> ExportHoaDonToPdfAsync(HoaDonDto hoaDon);
}

public class ExportService : IExportService
{
    public ExportService()
    {
        // Set EPPlus license context
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        // Register code page encodings for Vietnamese support
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public async Task<byte[]> ExportDoanhThuToExcelAsync(IEnumerable<DoanhThuTheoNgayDto> data, string title)
    {
        try
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Doanh thu");

            // Header
            worksheet.Cells[1, 1].Value = title;
            worksheet.Cells[1, 1, 1, 3].Merge = true;
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // Column headers
            worksheet.Cells[3, 1].Value = "Ngày";
            worksheet.Cells[3, 2].Value = "Số hóa đơn";
            worksheet.Cells[3, 3].Value = "Tổng doanh thu";
            worksheet.Cells[3, 1, 3, 3].Style.Font.Bold = true;

            // Data
            int row = 4;
            foreach (var item in data)
            {
                worksheet.Cells[row, 1].Value = item.Ngay.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 2].Value = item.SoHoaDon;
                worksheet.Cells[row, 3].Value = item.TongDoanhThu;
                worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0";
                row++;
            }

            // Total
            worksheet.Cells[row, 1].Value = "TỔNG CỘNG";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Value = data.Sum(d => d.SoHoaDon);
            worksheet.Cells[row, 2].Style.Font.Bold = true;
            worksheet.Cells[row, 3].Value = data.Sum(d => d.TongDoanhThu);
            worksheet.Cells[row, 3].Style.Font.Bold = true;
            worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0";

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();

            return await Task.FromResult(package.GetAsByteArray());
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi xuất Excel: {ex.Message}", ex);
        }
    }

    public async Task<byte[]> ExportHoaDonToPdfAsync(HoaDonDto hoaDon)
    {
        try
        {
            using var ms = new MemoryStream();
            var document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(document, ms);
            document.Open();

            // Create BaseFont with Unicode support for Vietnamese
            // Initialize with default fonts first (will be replaced if font file found)
            BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            BaseFont baseFontBold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            
            // Try to find a font that supports Vietnamese characters
            var regularFontPaths = new[]
            {
                // macOS paths
                "/System/Library/Fonts/Supplemental/Arial.ttf",
                "/Library/Fonts/Arial.ttf",
                "/System/Library/Fonts/Supplemental/Arial Unicode.ttf",
                // Windows paths
                "c:\\windows\\fonts\\arial.ttf",
                // Linux paths
                "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf"
            };

            var boldFontPaths = new[]
            {
                // macOS paths
                "/System/Library/Fonts/Supplemental/Arial Bold.ttf",
                "/Library/Fonts/Arial Bold.ttf",
                "/System/Library/Fonts/Supplemental/Arial Unicode.ttf",
                // Windows paths
                "c:\\windows\\fonts\\arialbd.ttf",
                // Linux paths
                "/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf"
            };
            
            bool fontFound = false;
            
            // Try to find a font file that exists
            for (int i = 0; i < regularFontPaths.Length && !fontFound; i++)
            {
                if (File.Exists(regularFontPaths[i]))
                {
                    try
                    {
                        baseFont = BaseFont.CreateFont(regularFontPaths[i], BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        
                        // Try to find corresponding bold font
                        if (i < boldFontPaths.Length && File.Exists(boldFontPaths[i]))
                        {
                            baseFontBold = BaseFont.CreateFont(boldFontPaths[i], BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        }
                        else
                        {
                            // Use same font for bold if bold version not found
                            baseFontBold = baseFont;
                        }
                        fontFound = true;
                    }
                    catch { }
                }
            }
            
            // If no font file found, use built-in fonts with encoding that supports Vietnamese
            if (!fontFound)
            {
                try
                {
                    // Try CP1258 (Vietnamese Windows encoding) - supports Vietnamese characters
                    // Use encoding string directly since BaseFont may not have CP1258 constant
                    baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, "Cp1258", BaseFont.NOT_EMBEDDED);
                    baseFontBold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, "Cp1258", BaseFont.NOT_EMBEDDED);
                }
                catch
                {
                    try
                    {
                        // Try UTF-8 encoding
                        baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                        baseFontBold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    }
                    catch
                    {
                        // Final fallback: use CP1252 (Western European) - may not display all Vietnamese characters correctly
                        baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                        baseFontBold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    }
                }
            }

            // Create Font objects with Unicode support
            var titleFont = new Font(baseFontBold, 20);
            var headerFont = new Font(baseFontBold, 12);
            var normalFont = new Font(baseFont, 10);
            var boldFont = new Font(baseFontBold, 10);

            // Title
            var titleParagraph = new Paragraph("HÓA ĐƠN THANH TOÁN", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10
            };
            document.Add(titleParagraph);

            // Shop info
            var shopInfo = new Paragraph("☕ COFFEE SHOP MANAGER", normalFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(shopInfo);

            // Invoice info table
            var infoTable = new PdfPTable(2) { WidthPercentage = 100, SpacingBefore = 10 };
            infoTable.SetWidths(new float[] { 30, 70 });

            AddInfoCell(infoTable, "Mã hóa đơn:", $"{hoaDon.MaHD}", normalFont, boldFont);
            AddInfoCell(infoTable, "Bàn:", hoaDon.TenBan, normalFont, boldFont);
            AddInfoCell(infoTable, "Thời gian:", hoaDon.ThoiGianTao.ToString("dd/MM/yyyy HH:mm"), normalFont, boldFont);
            AddInfoCell(infoTable, "Trạng thái:", hoaDon.TrangThai, normalFont, boldFont);
            
            if (!string.IsNullOrEmpty(hoaDon.PhuongThucThanhToan))
            {
                AddInfoCell(infoTable, "Phương thức thanh toán:", hoaDon.PhuongThucThanhToan, normalFont, boldFont);
            }
            
            if (!string.IsNullOrEmpty(hoaDon.TenKH))
                AddInfoCell(infoTable, "Khách hàng:", hoaDon.TenKH, normalFont, boldFont);
            if (!string.IsNullOrEmpty(hoaDon.TenNV))
                AddInfoCell(infoTable, "Nhân viên:", hoaDon.TenNV, normalFont, boldFont);

            document.Add(infoTable);
            document.Add(new Paragraph(" ", normalFont));

            // Items table
            var table = new PdfPTable(5) { WidthPercentage = 100, SpacingBefore = 10 };
            table.SetWidths(new float[] { 8, 40, 15, 15, 22 });

            // Table header
            AddTableCell(table, "STT", headerFont, true);
            AddTableCell(table, "Tên món", headerFont, true);
            AddTableCell(table, "Đơn giá", headerFont, true);
            AddTableCell(table, "SL", headerFont, true);
            AddTableCell(table, "Thành tiền", headerFont, true);

            // Table rows
            int stt = 1;
            foreach (var chiTiet in hoaDon.ChiTiets)
            {
                var donGia = chiTiet.ThanhTien / chiTiet.SoLuong;
                AddTableCell(table, stt.ToString(), normalFont, false);
                AddTableCell(table, chiTiet.TenMon, normalFont, false);
                AddTableCell(table, $"{donGia:N0} ₫", normalFont, false);
                AddTableCell(table, chiTiet.SoLuong.ToString(), normalFont, false);
                AddTableCell(table, $"{chiTiet.ThanhTien:N0} ₫", normalFont, false);
                stt++;
            }

            document.Add(table);

            // Total
            document.Add(new Paragraph(" ", normalFont));
            var totalParagraph = new Paragraph($"TỔNG TIỀN: {hoaDon.TongTien:N0} ₫", headerFont)
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingBefore = 10
            };
            document.Add(totalParagraph);

            // Footer
            document.Add(new Paragraph(" ", normalFont));
            document.Add(new Paragraph(" ", normalFont));
            var footer = new Paragraph("Cảm ơn quý khách đã sử dụng dịch vụ!", normalFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingBefore = 20
            };
            document.Add(footer);

            document.Close();

            return await Task.FromResult(ms.ToArray());
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi xuất PDF: {ex.Message}", ex);
        }
    }

    private void AddInfoCell(PdfPTable table, string label, string value, Font normalFont, Font boldFont)
    {
        var labelCell = new PdfPCell(new Phrase(label, boldFont))
        {
            Border = Rectangle.NO_BORDER,
            Padding = 5,
            HorizontalAlignment = Element.ALIGN_LEFT
        };
        table.AddCell(labelCell);

        var valueCell = new PdfPCell(new Phrase(value, normalFont))
        {
            Border = Rectangle.NO_BORDER,
            Padding = 5,
            HorizontalAlignment = Element.ALIGN_LEFT
        };
        table.AddCell(valueCell);
    }

    private void AddTableCell(PdfPTable table, string text, Font font, bool isHeader)
    {
        var cell = new PdfPCell(new Phrase(text, font))
        {
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 5
        };

        if (isHeader)
        {
            cell.BackgroundColor = new BaseColor(211, 211, 211); // Light Gray
        }

        table.AddCell(cell);
    }
}

