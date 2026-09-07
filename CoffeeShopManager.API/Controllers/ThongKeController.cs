using CoffeeShopManager.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThongKeController : ControllerBase
{
    private readonly IThongKeService _thongKeService;
    private readonly IExportService _exportService;

    public ThongKeController(IThongKeService thongKeService, IExportService exportService)
    {
        _thongKeService = thongKeService;
        _exportService = exportService;
    }

    [HttpGet("doanhthu/ngay")]
    public async Task<ActionResult> GetDoanhThuTheoNgay([FromQuery] DateTime tuNgay, [FromQuery] DateTime denNgay)
    {
        try
        {
            var data = await _thongKeService.GetDoanhThuTheoNgayAsync(tuNgay, denNgay);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("doanhthu/thang")]
    public async Task<ActionResult> GetDoanhThuTheoThang([FromQuery] int nam)
    {
        try
        {
            var data = await _thongKeService.GetDoanhThuTheoThangAsync(nam);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("monbanchay")]
    public async Task<ActionResult> GetMonBanChay([FromQuery] DateTime tuNgay, [FromQuery] DateTime denNgay, [FromQuery] int top = 10)
    {
        try
        {
            var data = await _thongKeService.GetMonBanChayAsync(tuNgay, denNgay, top);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("tongdoanhthu")]
    public async Task<ActionResult> GetTongDoanhThu([FromQuery] DateTime tuNgay, [FromQuery] DateTime denNgay)
    {
        try
        {
            var tongDoanhThu = await _thongKeService.GetTongDoanhThuAsync(tuNgay, denNgay);
            return Ok(new { tongDoanhThu });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("export/excel")]
    public async Task<ActionResult> ExportDoanhThuToExcel([FromQuery] DateTime tuNgay, [FromQuery] DateTime denNgay)
    {
        try
        {
            var data = await _thongKeService.GetDoanhThuTheoNgayAsync(tuNgay, denNgay);
            var title = $"BÁO CÁO DOANH THU TỪ {tuNgay:dd/MM/yyyy} ĐẾN {denNgay:dd/MM/yyyy}";
            var excelFile = await _exportService.ExportDoanhThuToExcelAsync(data, title);

            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"DoanhThu_{tuNgay:yyyyMMdd}_{denNgay:yyyyMMdd}.xlsx");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}

