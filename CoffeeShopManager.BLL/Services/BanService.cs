using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;

namespace CoffeeShopManager.BLL.Services;

public interface IBanService
{
    Task<IEnumerable<BanDto>> GetAllBansAsync();
    Task<BanDto?> GetBanByIdAsync(int maBan);
    Task<BanDto> CreateBanAsync(CreateBanDto createDto);
    Task<BanDto> UpdateBanAsync(int maBan, UpdateBanDto updateDto);
    Task DeleteBanAsync(int maBan);
    Task<IEnumerable<BanDto>> GetBansByTrangThaiAsync(string trangThai);
}

public class BanService : IBanService
{
    private readonly IBanRepository _banRepository;
    private readonly IHoaDonRepository _hoaDonRepository;

    public BanService(IBanRepository banRepository, IHoaDonRepository hoaDonRepository)
    {
        _banRepository = banRepository;
        _hoaDonRepository = hoaDonRepository;
    }

    public async Task<IEnumerable<BanDto>> GetAllBansAsync()
    {
        try
        {
            var bans = await _banRepository.GetAllAsync();
            return bans.Select(b => MapToDto(b));
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy danh sách bàn: {ex.Message}", ex);
        }
    }

    public async Task<BanDto?> GetBanByIdAsync(int maBan)
    {
        try
        {
            if (maBan <= 0)
                throw new ArgumentException("Mã bàn không hợp lệ");

            var ban = await _banRepository.GetByIdAsync(maBan);
            return ban != null ? MapToDto(ban) : null;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy thông tin bàn: {ex.Message}", ex);
        }
    }

    public async Task<BanDto> CreateBanAsync(CreateBanDto createDto)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(createDto.TenBan))
                throw new ArgumentException("Tên bàn không được để trống");

            if (!IsValidTrangThai(createDto.TrangThai))
                throw new ArgumentException("Trạng thái không hợp lệ");

            var ban = new Ban
            {
                TenBan = createDto.TenBan.Trim(),
                TrangThai = createDto.TrangThai
            };

            var createdBan = await _banRepository.AddAsync(ban);
            return MapToDto(createdBan);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi tạo bàn mới: {ex.Message}", ex);
        }
    }

    public async Task<BanDto> UpdateBanAsync(int maBan, UpdateBanDto updateDto)
    {
        try
        {
            if (maBan <= 0)
                throw new ArgumentException("Mã bàn không hợp lệ");

            var ban = await _banRepository.GetByIdAsync(maBan);
            if (ban == null)
                throw new KeyNotFoundException($"Không tìm thấy bàn với mã {maBan}");

            // Update fields
            if (!string.IsNullOrWhiteSpace(updateDto.TenBan))
                ban.TenBan = updateDto.TenBan.Trim();

            if (!string.IsNullOrWhiteSpace(updateDto.TrangThai))
            {
                if (!IsValidTrangThai(updateDto.TrangThai))
                    throw new ArgumentException("Trạng thái không hợp lệ");
                ban.TrangThai = updateDto.TrangThai;
            }

            await _banRepository.UpdateAsync(ban);
            return MapToDto(ban);
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
            throw new Exception($"Lỗi khi cập nhật bàn: {ex.Message}", ex);
        }
    }

    public async Task DeleteBanAsync(int maBan)
    {
        try
        {
            if (maBan <= 0)
                throw new ArgumentException("Mã bàn không hợp lệ");

            var ban = await _banRepository.GetByIdAsync(maBan);
            if (ban == null)
                throw new KeyNotFoundException($"Không tìm thấy bàn với mã {maBan}");

            // Kiểm tra xem bàn có hóa đơn đã thanh toán không
            var hoaDons = await _hoaDonRepository.GetHoaDonsByBanAsync(maBan);
            var hoaDonDaThanhToan = hoaDons.Any(h => h.TrangThai == "Đã thanh toán");
            
            if (hoaDonDaThanhToan)
            {
                throw new InvalidOperationException("Không thể xóa bàn đã có hóa đơn đã thanh toán. Vui lòng giữ lại bàn để lưu lịch sử.");
            }

            // Xóa các hóa đơn chưa thanh toán (nếu có)
            var hoaDonsChuaThanhToan = hoaDons.Where(h => h.TrangThai == "Chưa thanh toán").ToList();
            foreach (var hoaDon in hoaDonsChuaThanhToan)
            {
                await _hoaDonRepository.DeleteAsync(hoaDon.MaHD);
            }

            // Xóa bàn
            await _banRepository.DeleteAsync(maBan);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi xóa bàn: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<BanDto>> GetBansByTrangThaiAsync(string trangThai)
    {
        try
        {
            if (!IsValidTrangThai(trangThai))
                throw new ArgumentException("Trạng thái không hợp lệ");

            var bans = await _banRepository.GetBansByTrangThaiAsync(trangThai);
            return bans.Select(b => MapToDto(b));
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy danh sách bàn theo trạng thái: {ex.Message}", ex);
        }
    }

    private static bool IsValidTrangThai(string trangThai)
    {
        var validStates = new[] { "Trống", "Đang sử dụng", "Đã đặt" };
        return validStates.Contains(trangThai);
    }

    private static BanDto MapToDto(Ban ban)
    {
        return new BanDto
        {
            MaBan = ban.MaBan,
            TenBan = ban.TenBan,
            TrangThai = ban.TrangThai
        };
    }
}

