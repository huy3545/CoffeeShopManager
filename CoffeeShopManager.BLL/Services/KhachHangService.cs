using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;

namespace CoffeeShopManager.BLL.Services;

public interface IKhachHangService
{
    Task<IEnumerable<KhachHangDto>> GetAllKhachHangsAsync();
    Task<KhachHangDto?> GetKhachHangByIdAsync(int maKH);
    Task<KhachHangDto?> GetKhachHangBySoDienThoaiAsync(string soDienThoai);
    Task<KhachHangDto> CreateKhachHangAsync(CreateKhachHangDto createDto);
    Task<KhachHangDto> UpdateKhachHangAsync(int maKH, UpdateKhachHangDto updateDto);
    Task DeleteKhachHangAsync(int maKH);
    Task<IEnumerable<KhachHangDto>> GetTopKhachHangsAsync(int top);
}

public class KhachHangService : IKhachHangService
{
    private readonly IKhachHangRepository _khachHangRepository;

    public KhachHangService(IKhachHangRepository khachHangRepository)
    {
        _khachHangRepository = khachHangRepository;
    }

    public async Task<IEnumerable<KhachHangDto>> GetAllKhachHangsAsync()
    {
        try
        {
            var khachHangs = await _khachHangRepository.GetAllAsync();
            return khachHangs.Select(kh => MapToDto(kh));
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy danh sách khách hàng: {ex.Message}", ex);
        }
    }

    public async Task<KhachHangDto?> GetKhachHangByIdAsync(int maKH)
    {
        try
        {
            if (maKH <= 0)
                throw new ArgumentException("Mã khách hàng không hợp lệ");

            var khachHang = await _khachHangRepository.GetByIdAsync(maKH);
            return khachHang != null ? MapToDto(khachHang) : null;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy thông tin khách hàng: {ex.Message}", ex);
        }
    }

    public async Task<KhachHangDto?> GetKhachHangBySoDienThoaiAsync(string soDienThoai)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(soDienThoai))
                throw new ArgumentException("Số điện thoại không được để trống");

            var khachHang = await _khachHangRepository.GetKhachHangBySoDienThoaiAsync(soDienThoai);
            return khachHang != null ? MapToDto(khachHang) : null;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi tìm khách hàng theo số điện thoại: {ex.Message}", ex);
        }
    }

    public async Task<KhachHangDto> CreateKhachHangAsync(CreateKhachHangDto createDto)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(createDto.TenKH))
                throw new ArgumentException("Tên khách hàng không được để trống");

            var khachHang = new KhachHang
            {
                TenKH = createDto.TenKH.Trim(),
                DiemTichLuy = 0,
                SoDienThoai = createDto.SoDienThoai?.Trim(),
                Email = createDto.Email?.Trim()
            };

            var createdKhachHang = await _khachHangRepository.AddAsync(khachHang);
            return MapToDto(createdKhachHang);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi tạo khách hàng mới: {ex.Message}", ex);
        }
    }

    public async Task<KhachHangDto> UpdateKhachHangAsync(int maKH, UpdateKhachHangDto updateDto)
    {
        try
        {
            if (maKH <= 0)
                throw new ArgumentException("Mã khách hàng không hợp lệ");

            var khachHang = await _khachHangRepository.GetByIdAsync(maKH);
            if (khachHang == null)
                throw new KeyNotFoundException($"Không tìm thấy khách hàng với mã {maKH}");

            // Update fields
            if (!string.IsNullOrWhiteSpace(updateDto.TenKH))
                khachHang.TenKH = updateDto.TenKH.Trim();

            if (updateDto.DiemTichLuy.HasValue)
            {
                if (updateDto.DiemTichLuy.Value < 0)
                    throw new ArgumentException("Điểm tích lũy không được âm");
                khachHang.DiemTichLuy = updateDto.DiemTichLuy.Value;
            }

            if (updateDto.SoDienThoai != null)
                khachHang.SoDienThoai = updateDto.SoDienThoai.Trim();

            if (updateDto.Email != null)
                khachHang.Email = updateDto.Email.Trim();

            await _khachHangRepository.UpdateAsync(khachHang);
            return MapToDto(khachHang);
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
            throw new Exception($"Lỗi khi cập nhật khách hàng: {ex.Message}", ex);
        }
    }

    public async Task DeleteKhachHangAsync(int maKH)
    {
        try
        {
            if (maKH <= 0)
                throw new ArgumentException("Mã khách hàng không hợp lệ");

            var exists = await _khachHangRepository.ExistsAsync(maKH);
            if (!exists)
                throw new KeyNotFoundException($"Không tìm thấy khách hàng với mã {maKH}");

            await _khachHangRepository.DeleteAsync(maKH);
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
            throw new Exception($"Lỗi khi xóa khách hàng: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<KhachHangDto>> GetTopKhachHangsAsync(int top)
    {
        try
        {
            if (top <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0");

            var khachHangs = await _khachHangRepository.GetTopKhachHangsByDiemAsync(top);
            return khachHangs.Select(kh => MapToDto(kh));
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy danh sách khách hàng top: {ex.Message}", ex);
        }
    }

    private static KhachHangDto MapToDto(KhachHang khachHang)
    {
        return new KhachHangDto
        {
            MaKH = khachHang.MaKH,
            TenKH = khachHang.TenKH,
            DiemTichLuy = khachHang.DiemTichLuy,
            SoDienThoai = khachHang.SoDienThoai,
            Email = khachHang.Email
        };
    }
}

