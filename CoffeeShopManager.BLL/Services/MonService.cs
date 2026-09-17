using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;

namespace CoffeeShopManager.BLL.Services;

public interface IMonService
{
    Task<IEnumerable<MonDto>> GetAllMonsAsync();
    Task<MonDto?> GetMonByIdAsync(int maMon);
    Task<MonDto> CreateMonAsync(CreateMonDto createDto);
    Task<MonDto> UpdateMonAsync(int maMon, UpdateMonDto updateDto);
    Task DeleteMonAsync(int maMon);
    Task<IEnumerable<MonDto>> GetMonsByLoaiAsync(string loai);
}

public class MonService : IMonService
{
    private readonly IMonRepository _monRepository;

    public MonService(IMonRepository monRepository)
    {
        _monRepository = monRepository;
    }

    public async Task<IEnumerable<MonDto>> GetAllMonsAsync()
    {
        try
        {
            var mons = await _monRepository.GetAllAsync();
            return mons.Select(m => MapToDto(m));
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy danh sách món: {ex.Message}", ex);
        }
    }

    public async Task<MonDto?> GetMonByIdAsync(int maMon)
    {
        try
        {
            if (maMon <= 0)
                throw new ArgumentException("Mã món không hợp lệ");

            var mon = await _monRepository.GetByIdAsync(maMon);
            return mon != null ? MapToDto(mon) : null;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy thông tin món: {ex.Message}", ex);
        }
    }

    public async Task<MonDto> CreateMonAsync(CreateMonDto createDto)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(createDto.TenMon))
                throw new ArgumentException("Tên món không được để trống");

            if (createDto.DonGia <= 0)
                throw new ArgumentException("Đơn giá phải lớn hơn 0");

            if (string.IsNullOrWhiteSpace(createDto.Loai))
                throw new ArgumentException("Loại món không được để trống");

            var mon = new Mon
            {
                TenMon = createDto.TenMon.Trim(),
                DonGia = createDto.DonGia,
                Loai = createDto.Loai.Trim()
            };

            var createdMon = await _monRepository.AddAsync(mon);
            return MapToDto(createdMon);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi tạo món mới: {ex.Message}", ex);
        }
    }

    public async Task<MonDto> UpdateMonAsync(int maMon, UpdateMonDto updateDto)
    {
        try
        {
            if (maMon <= 0)
                throw new ArgumentException("Mã món không hợp lệ");

            var mon = await _monRepository.GetByIdAsync(maMon);
            if (mon == null)
                throw new KeyNotFoundException($"Không tìm thấy món với mã {maMon}");

            // Update fields
            if (!string.IsNullOrWhiteSpace(updateDto.TenMon))
                mon.TenMon = updateDto.TenMon.Trim();

            if (updateDto.DonGia.HasValue)
            {
                if (updateDto.DonGia.Value <= 0)
                    throw new ArgumentException("Đơn giá phải lớn hơn 0");
                mon.DonGia = updateDto.DonGia.Value;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Loai))
                mon.Loai = updateDto.Loai.Trim();

            await _monRepository.UpdateAsync(mon);
            return MapToDto(mon);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi cập nhật món: {ex.Message}", ex);
        }
    }

    public async Task DeleteMonAsync(int maMon)
    {
        try
        {
            if (maMon <= 0)
                throw new ArgumentException("Mã món không hợp lệ");

            var exists = await _monRepository.ExistsAsync(maMon);
            if (!exists)
                throw new KeyNotFoundException($"Không tìm thấy món với mã {maMon}");

            await _monRepository.DeleteAsync(maMon);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi xóa món: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<MonDto>> GetMonsByLoaiAsync(string loai)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(loai))
                throw new ArgumentException("Loại món không được để trống");

            var mons = await _monRepository.GetMonsByLoaiAsync(loai);
            return mons.Select(m => MapToDto(m));
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy danh sách món theo loại: {ex.Message}", ex);
        }
    }

    private static MonDto MapToDto(Mon mon)
    {
        return new MonDto
        {
            MaMon = mon.MaMon,
            TenMon = mon.TenMon,
            DonGia = mon.DonGia,
            Loai = mon.Loai
        };
    }
}

