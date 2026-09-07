using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HoaDonController : ControllerBase
{
    private readonly IHoaDonService _hoaDonService;
    private readonly IExportService _exportService;

    public HoaDonController(IHoaDonService hoaDonService, IExportService exportService)
    {
        _hoaDonService = hoaDonService;
        _exportService = exportService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HoaDonDto>>> GetAll()
    {
        try
        {
            var hoaDons = await _hoaDonService.GetAllHoaDonsAsync();
            return Ok(hoaDons);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HoaDonDto>> GetById(int id)
    {
        try
        {
            var hoaDon = await _hoaDonService.GetHoaDonByIdAsync(id);
            if (hoaDon == null)
                return NotFound(new { message = $"Không tìm thấy hóa đơn với mã {id}" });

            return Ok(hoaDon);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("ban/{maBan}")]
    public async Task<ActionResult<IEnumerable<HoaDonDto>>> GetByBan(int maBan)
    {
        try
        {
            var hoaDons = await _hoaDonService.GetHoaDonsByBanAsync(maBan);
            return Ok(hoaDons);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("daterange")]
    public async Task<ActionResult<IEnumerable<HoaDonDto>>> GetByDateRange([FromQuery] DateTime tuNgay, [FromQuery] DateTime denNgay)
    {
        try
        {
            var hoaDons = await _hoaDonService.GetHoaDonsByDateRangeAsync(tuNgay, denNgay);
            return Ok(hoaDons);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<HoaDonDto>> Create([FromBody] CreateHoaDonDto createDto)
    {
        try
        {
            var hoaDon = await _hoaDonService.CreateHoaDonAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = hoaDon.MaHD }, hoaDon);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("goimon")]
    public async Task<ActionResult<HoaDonDto>> GoiMon([FromBody] GoiMonDto goiMonDto)
    {
        try
        {
            var hoaDon = await _hoaDonService.GoiMonAsync(goiMonDto);
            return Ok(hoaDon);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("thanhtoan")]
    public async Task<ActionResult<HoaDonDto>> ThanhToan([FromBody] ThanhToanDto thanhToanDto)
    {
        try
        {
            var hoaDon = await _hoaDonService.ThanhToanAsync(thanhToanDto);
            return Ok(hoaDon);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<HoaDonDto>> Update(int id, [FromBody] UpdateHoaDonDto updateDto)
    {
        try
        {
            var hoaDon = await _hoaDonService.UpdateHoaDonAsync(id, updateDto);
            return Ok(hoaDon);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _hoaDonService.DeleteHoaDonAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("chitiet/{maChiTiet}")]
    public async Task<ActionResult<HoaDonDto>> XoaMon(int maChiTiet)
    {
        try
        {
            var hoaDon = await _hoaDonService.XoaMonAsync(maChiTiet);
            return Ok(hoaDon);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}/export/pdf")]
    public async Task<ActionResult> ExportToPdf(int id)
    {
        try
        {
            var hoaDon = await _hoaDonService.GetHoaDonByIdAsync(id);
            if (hoaDon == null)
                return NotFound(new { message = $"Không tìm thấy hóa đơn với mã {id}" });

            var pdfBytes = await _exportService.ExportHoaDonToPdfAsync(hoaDon);
            return File(pdfBytes, "application/pdf", $"HoaDon_{hoaDon.MaHD}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}

