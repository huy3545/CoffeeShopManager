using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NhanVienController : ControllerBase
{
    private readonly INhanVienService _nhanVienService;

    public NhanVienController(INhanVienService nhanVienService)
    {
        _nhanVienService = nhanVienService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NhanVienDto>>> GetAll()
    {
        try
        {
            var nhanViens = await _nhanVienService.GetAllNhanViensAsync();
            return Ok(nhanViens);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NhanVienDto>> GetById(int id)
    {
        try
        {
            var nhanVien = await _nhanVienService.GetNhanVienByIdAsync(id);
            if (nhanVien == null)
                return NotFound(new { message = $"Không tìm thấy nhân viên với mã {id}" });

            return Ok(nhanVien);
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

    [HttpGet("calam/{caLam}")]
    public async Task<ActionResult<IEnumerable<NhanVienDto>>> GetByCaLam(string caLam)
    {
        try
        {
            var nhanViens = await _nhanVienService.GetNhanViensByCaLamAsync(caLam);
            return Ok(nhanViens);
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
    public async Task<ActionResult<NhanVienDto>> Create([FromBody] CreateNhanVienDto createDto)
    {
        try
        {
            var nhanVien = await _nhanVienService.CreateNhanVienAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = nhanVien.MaNV }, nhanVien);
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
    public async Task<ActionResult<NhanVienDto>> Update(int id, [FromBody] UpdateNhanVienDto updateDto)
    {
        try
        {
            var nhanVien = await _nhanVienService.UpdateNhanVienAsync(id, updateDto);
            return Ok(nhanVien);
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
            await _nhanVienService.DeleteNhanVienAsync(id);
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

