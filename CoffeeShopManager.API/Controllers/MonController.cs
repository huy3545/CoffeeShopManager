using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonController : ControllerBase
{
    private readonly IMonService _monService;

    public MonController(IMonService monService)
    {
        _monService = monService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MonDto>>> GetAll()
    {
        try
        {
            var mons = await _monService.GetAllMonsAsync();
            return Ok(mons);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MonDto>> GetById(int id)
    {
        try
        {
            var mon = await _monService.GetMonByIdAsync(id);
            if (mon == null)
                return NotFound(new { message = $"Không tìm thấy món với mã {id}" });

            return Ok(mon);
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

    [HttpGet("loai/{loai}")]
    public async Task<ActionResult<IEnumerable<MonDto>>> GetByLoai(string loai)
    {
        try
        {
            var mons = await _monService.GetMonsByLoaiAsync(loai);
            return Ok(mons);
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
    public async Task<ActionResult<MonDto>> Create([FromBody] CreateMonDto createDto)
    {
        try
        {
            var mon = await _monService.CreateMonAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = mon.MaMon }, mon);
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
    public async Task<ActionResult<MonDto>> Update(int id, [FromBody] UpdateMonDto updateDto)
    {
        try
        {
            var mon = await _monService.UpdateMonAsync(id, updateDto);
            return Ok(mon);
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
            await _monService.DeleteMonAsync(id);
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

