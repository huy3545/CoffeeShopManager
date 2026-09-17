using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KhachHangController : ControllerBase
{
    private readonly IKhachHangService _khachHangService;

    public KhachHangController(IKhachHangService khachHangService)
    {
        _khachHangService = khachHangService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<KhachHangDto>>> GetAll()
    {
        try
        {
            var khachHangs = await _khachHangService.GetAllKhachHangsAsync();
            return Ok(khachHangs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<KhachHangDto>> GetById(int id)
    {
        try
        {
            var khachHang = await _khachHangService.GetKhachHangByIdAsync(id);
            if (khachHang == null)
                return NotFound(new { message = $"Không tìm thấy khách hàng với mã {id}" });

            return Ok(khachHang);
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

    [HttpGet("sdt/{soDienThoai}")]
    public async Task<ActionResult<KhachHangDto>> GetBySoDienThoai(string soDienThoai)
    {
        try
        {
            var khachHang = await _khachHangService.GetKhachHangBySoDienThoaiAsync(soDienThoai);
            if (khachHang == null)
                return NotFound(new { message = $"Không tìm thấy khách hàng với số điện thoại {soDienThoai}" });

            return Ok(khachHang);
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

    [HttpGet("top/{top}")]
    public async Task<ActionResult<IEnumerable<KhachHangDto>>> GetTop(int top)
    {
        try
        {
            var khachHangs = await _khachHangService.GetTopKhachHangsAsync(top);
            return Ok(khachHangs);
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

    [HttpPost]
    public async Task<ActionResult<KhachHangDto>> Create([FromBody] CreateKhachHangDto createDto)
    {
        try
        {
            var khachHang = await _khachHangService.CreateKhachHangAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = khachHang.MaKH }, khachHang);
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

    [HttpPut("{id}")]
    public async Task<ActionResult<KhachHangDto>> Update(int id, [FromBody] UpdateKhachHangDto updateDto)
    {
        try
        {
            var khachHang = await _khachHangService.UpdateKhachHangAsync(id, updateDto);
            return Ok(khachHang);
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
            await _khachHangService.DeleteKhachHangAsync(id);
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
}

