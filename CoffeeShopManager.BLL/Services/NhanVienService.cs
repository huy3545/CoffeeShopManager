using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;

namespace CoffeeShopManager.BLL.Services;

public interface INhanVienService
{
    Task<IEnumerable<NhanVienDto>> GetAllNhanViensAsync();
    Task<NhanVienDto?> GetNhanVienByIdAsync(int maNV);
    Task<NhanVienDto> CreateNhanVienAsync(CreateNhanVienDto createDto);
    Task<NhanVienDto> UpdateNhanVienAsync(int maNV, UpdateNhanVienDto updateDto);
    Task DeleteNhanVienAsync(int maNV);
    Task<IEnumerable<NhanVienDto>> GetNhanViensByCaLamAsync(string caLam);
}

public class NhanVienService : INhanVienService
{
    private readonly INhanVienRepository _nhanVienRepository;

    public NhanVienService(INhanVienRepository nhanVienRepository)
    {
        _nhanVienRepository = nhanVienRepository;
    }

    public async Task<IEnumerable<NhanVienDto>> GetAllNhanViensAsync()
    {
        try
        {
            var nhanViens = await _nhanVienRepository.GetAllAsync();
            return nhanViens.Select(nv => MapToDto(nv));
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy danh sách nhân viên: {ex.Message}", ex);
        }
    }

    public async Task<NhanVienDto?> GetNhanVienByIdAsync(int maNV)
    {
        try
        {
            if (maNV <= 0)
                throw new ArgumentException("Mã nhân viên không hợp lệ");

            var nhanVien = await _nhanVienRepository.GetByIdAsync(maNV);
            return nhanVien != null ? MapToDto(nhanVien) : null;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy thông tin nhân viên: {ex.Message}", ex);
        }
    }

    public async Task<NhanVienDto> CreateNhanVienAsync(CreateNhanVienDto createDto)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(createDto.TenNV))
                throw new ArgumentException("Tên nhân viên không được để trống");

            if (string.IsNullOrWhiteSpace(createDto.CaLam))
                throw new ArgumentException("Ca làm không được để trống");

            var nhanVien = new NhanVien
            {
                TenNV = createDto.TenNV.Trim(),
                CaLam = createDto.CaLam.Trim(),
                SoDienThoai = createDto.SoDienThoai?.Trim(),
                Email = createDto.Email?.Trim()
            };

            var createdNhanVien = await _nhanVienRepository.AddAsync(nhanVien);
            return MapToDto(createdNhanVien);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi tạo nhân viên mới: {ex.Message}", ex);
        }
    }

    public async Task<NhanVienDto> UpdateNhanVienAsync(int maNV, UpdateNhanVienDto updateDto)
    {
        try
        {
            if (maNV <= 0)
                throw new ArgumentException("Mã nhân viên không hợp lệ");

            var nhanVien = await _nhanVienRepository.GetByIdAsync(maNV);
            if (nhanVien == null)
                throw new KeyNotFoundException($"Không tìm thấy nhân viên với mã {maNV}");

            // Update fields
            if (!string.IsNullOrWhiteSpace(updateDto.TenNV))
                nhanVien.TenNV = updateDto.TenNV.Trim();

            if (!string.IsNullOrWhiteSpace(updateDto.CaLam))
                nhanVien.CaLam = updateDto.CaLam.Trim();

            if (updateDto.SoDienThoai != null)
                nhanVien.SoDienThoai = updateDto.SoDienThoai.Trim();

            if (updateDto.Email != null)
                nhanVien.Email = updateDto.Email.Trim();

            await _nhanVienRepository.UpdateAsync(nhanVien);
            return MapToDto(nhanVien);
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
            throw new Exception($"Lỗi khi cập nhật nhân viên: {ex.Message}", ex);
        }
    }

    public async Task DeleteNhanVienAsync(int maNV)
    {
        try
        {
            if (maNV <= 0)
                throw new ArgumentException("Mã nhân viên không hợp lệ");

            var exists = await _nhanVienRepository.ExistsAsync(maNV);
            if (!exists)
                throw new KeyNotFoundException($"Không tìm thấy nhân viên với mã {maNV}");

            await _nhanVienRepository.DeleteAsync(maNV);
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
            throw new Exception($"Lỗi khi xóa nhân viên: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<NhanVienDto>> GetNhanViensByCaLamAsync(string caLam)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(caLam))
                throw new ArgumentException("Ca làm không được để trống");

            var nhanViens = await _nhanVienRepository.GetNhanViensByCaLamAsync(caLam);
            return nhanViens.Select(nv => MapToDto(nv));
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy danh sách nhân viên theo ca làm: {ex.Message}", ex);
        }
    }

    private static NhanVienDto MapToDto(NhanVien nhanVien)
    {
        return new NhanVienDto
        {
            MaNV = nhanVien.MaNV,
            TenNV = nhanVien.TenNV,
            CaLam = nhanVien.CaLam,
            SoDienThoai = nhanVien.SoDienThoai,
            Email = nhanVien.Email
        };
    }
}

