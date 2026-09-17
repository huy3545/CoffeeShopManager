using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BanController : ControllerBase
{
    private readonly IBanService _banService;

    public BanController(IBanService banService)
    {
        _banService = banService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BanDto>>> GetAll()
    {
        try
        {
            var bans = await _banService.GetAllBansAsync();
            return Ok(bans);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BanDto>> GetById(int id)
    {
        try
        {
            var ban = await _banService.GetBanByIdAsync(id);
            if (ban == null)
                return NotFound(new { message = $"Không tìm thấy bàn với mã {id}" });

            return Ok(ban);
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

    [HttpGet("trangthai/{trangThai}")]
    public async Task<ActionResult<IEnumerable<BanDto>>> GetByTrangThai(string trangThai)
    {
        try
        {
            var bans = await _banService.GetBansByTrangThaiAsync(trangThai);
            return Ok(bans);
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
    public async Task<ActionResult<BanDto>> Create([FromBody] CreateBanDto createDto)
    {
        try
        {
            var ban = await _banService.CreateBanAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = ban.MaBan }, ban);
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
    public async Task<ActionResult<BanDto>> Update(int id, [FromBody] UpdateBanDto updateDto)
    {
        try
        {
            var ban = await _banService.UpdateBanAsync(id, updateDto);
            return Ok(ban);
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
            await _banService.DeleteBanAsync(id);
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

