using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;

namespace CoffeeShopManager.BLL.Services;

public interface ILichLamViecService
{
    Task<IEnumerable<CaLamViecDto>> GetShiftsAsync(DateTime? from = null, DateTime? to = null);
    Task<IEnumerable<DangKyCaDto>> GetRegistrationsAsync(int? maCa = null, int? maNV = null);
    Task<CaLamViecDto> CreateShiftAsync(CreateCaLamViecDto dto);
    Task<CaLamViecDto> UpdateShiftAsync(int id, UpdateCaLamViecDto dto);
    Task DeleteShiftAsync(int id);
    Task<DangKyCaDto> RegisterAsync(DangKyCaRequestDto dto);
    Task<DangKyCaDto> AssignAsync(DangKyCaRequestDto dto, string? reviewer);
    Task CancelRegistrationAsync(int id, int maNV);
    Task ReviewRegistrationAsync(int id, string status, string? reviewer);
}

public class LichLamViecService : ILichLamViecService
{
    public const string ChoDuyet = "Chờ duyệt";
    public const string DaDuyet = "Đã duyệt";
    public const string TuChoi = "Từ chối";
    public const string DaHuy = "Đã hủy";
    public const string DangMo = "Đang mở";
    public const string DaDuNguoi = "Đã đủ người";
    public const string DaDong = "Đã đóng";
    public const string DaHuyCa = "Đã hủy";

    private readonly ILichLamViecRepository _shiftRepository;
    private readonly IDangKyCaRepository _registrationRepository;

    public LichLamViecService(ILichLamViecRepository shiftRepository, IDangKyCaRepository registrationRepository)
    {
        _shiftRepository = shiftRepository;
        _registrationRepository = registrationRepository;
    }

    public async Task<IEnumerable<CaLamViecDto>> GetShiftsAsync(DateTime? from = null, DateTime? to = null) =>
        (await _shiftRepository.GetShiftsAsync(from, to)).Select(MapShift).ToList();

    public async Task<IEnumerable<DangKyCaDto>> GetRegistrationsAsync(int? maCa = null, int? maNV = null) =>
        (await _registrationRepository.GetRegistrationsAsync(maCa, maNV)).Select(MapRegistration).ToList();

    public async Task<CaLamViecDto> CreateShiftAsync(CreateCaLamViecDto dto)
    {
        var times = NormalizeAndValidateShift(dto);
        var entity = await _shiftRepository.AddAsync(new CaLamViec
        {
            TenCa = dto.TenCa.Trim(),
            ThoiGianBatDau = times.start,
            ThoiGianKetThuc = times.end,
            LoaiCa = dto.LoaiCa.Trim(),
            LoaiNhanVien = dto.LoaiNhanVien.Trim(),
            SoLuongNhanVienCan = dto.SoLuongNhanVienCan,
            NguoiTao = dto.NguoiTao?.Trim(),
            GhiChu = dto.GhiChu?.Trim()
        });
        return MapShift(entity);
    }

    public async Task<CaLamViecDto> UpdateShiftAsync(int id, UpdateCaLamViecDto dto)
    {
        var times = NormalizeAndValidateShift(dto);
        var entity = await _shiftRepository.GetShiftWithRegistrationsAsync(id) ?? throw new KeyNotFoundException("Không tìm thấy ca làm.");
        EnsureShiftEditable(entity, times.start);
        entity.TenCa = dto.TenCa.Trim();
        entity.ThoiGianBatDau = times.start;
        entity.ThoiGianKetThuc = times.end;
        entity.LoaiCa = dto.LoaiCa.Trim();
        entity.LoaiNhanVien = dto.LoaiNhanVien.Trim();
        entity.SoLuongNhanVienCan = dto.SoLuongNhanVienCan;
        entity.GhiChu = dto.GhiChu?.Trim();
        await _shiftRepository.UpdateAsync(entity);
        return MapShift(entity);
    }

    public async Task DeleteShiftAsync(int id)
    {
        var entity = await _shiftRepository.GetShiftWithRegistrationsAsync(id) ?? throw new KeyNotFoundException("Không tìm thấy ca làm.");
        if (entity.ThoiGianBatDau.Date < DateTime.Today)
            throw new InvalidOperationException("Không thể xóa hoặc hủy lịch của ngày đã qua.");

        if (entity.DangKyCas.Count > 0 || entity.ThoiGianBatDau <= DateTime.Now)
        {
            // Giữ lịch sử phân ca: chuyển ca và các đăng ký sang Đã hủy thay vì xóa vật lý.
            entity.TrangThai = DaHuyCa;
            foreach (var registration in entity.DangKyCas.Where(x => x.TrangThai != DaHuy))
            {
                registration.TrangThai = DaHuy;
                registration.ThoiGianDuyet = DateTime.Now;
            }
            await _shiftRepository.UpdateAsync(entity);
            return;
        }

        await _shiftRepository.DeleteAsync(id);
    }

    public async Task<DangKyCaDto> RegisterAsync(DangKyCaRequestDto dto)
    {
        if (dto.MaCa <= 0 || dto.MaNV <= 0) throw new ArgumentException("Ca làm hoặc nhân viên không hợp lệ.");
        var shift = await _shiftRepository.GetShiftWithRegistrationsAsync(dto.MaCa) ?? throw new KeyNotFoundException("Không tìm thấy ca làm.");
        var employee = await _shiftRepository.GetEmployeeAsync(dto.MaNV) ?? throw new KeyNotFoundException("Không tìm thấy nhân viên.");
        if (!string.Equals(employee.LoaiNhanVien, shift.LoaiNhanVien, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Ca này chỉ dành cho nhân viên {shift.LoaiNhanVien}.");
        if (shift.TrangThai is DaDong or DaHuyCa || shift.ThoiGianBatDau <= DateTime.Now)
            throw new InvalidOperationException("Ca làm đã đóng, đã hủy hoặc đã bắt đầu.");
        if (await _shiftRepository.HasRegistrationAsync(dto.MaCa, dto.MaNV))
            throw new InvalidOperationException("Nhân viên đã đăng ký ca này.");
        var approved = await _shiftRepository.CountApprovedAsync(dto.MaCa);
        if (approved >= shift.SoLuongNhanVienCan)
            throw new InvalidOperationException("Ca làm đã đủ số lượng nhân viên.");
        if (await _shiftRepository.HasStaffOverlapAsync(dto.MaNV, shift.ThoiGianBatDau, shift.ThoiGianKetThuc))
            throw new InvalidOperationException("Nhân viên đã có ca được duyệt bị trùng thời gian.");

        var entity = await _registrationRepository.AddAsync(new DangKyCa
        {
            MaCa = dto.MaCa,
            MaNV = dto.MaNV,
            MaNguoiDung = dto.MaNguoiDung,
            GhiChu = dto.GhiChu?.Trim(),
            TrangThai = ChoDuyet
        });
        return MapRegistration(entity);
    }

    public async Task<DangKyCaDto> AssignAsync(DangKyCaRequestDto dto, string? reviewer)
    {
        if (dto.MaCa <= 0 || dto.MaNV <= 0) throw new ArgumentException("Ca làm hoặc nhân viên không hợp lệ.");
        var shift = await _shiftRepository.GetShiftWithRegistrationsAsync(dto.MaCa) ?? throw new KeyNotFoundException("Không tìm thấy ca làm.");
        var employee = await _shiftRepository.GetEmployeeAsync(dto.MaNV) ?? throw new KeyNotFoundException("Không tìm thấy nhân viên.");
        if (shift.TrangThai is DaDong or DaHuyCa || shift.ThoiGianBatDau.Date < DateTime.Today)
            throw new InvalidOperationException("Không thể phân công ca đã đóng, đã hủy hoặc thuộc ngày đã qua.");
        if (!string.Equals(employee.LoaiNhanVien, shift.LoaiNhanVien, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Ca này chỉ dành cho nhân viên {shift.LoaiNhanVien}.");
        if (await _shiftRepository.HasRegistrationAsync(dto.MaCa, dto.MaNV))
            throw new InvalidOperationException("Nhân viên đã có đăng ký trong ca này.");
        if (await _shiftRepository.CountApprovedAsync(dto.MaCa) >= shift.SoLuongNhanVienCan)
            throw new InvalidOperationException("Ca làm đã đủ số lượng nhân viên.");
        if (await _shiftRepository.HasStaffOverlapAsync(dto.MaNV, shift.ThoiGianBatDau, shift.ThoiGianKetThuc))
            throw new InvalidOperationException("Nhân viên đã có ca được duyệt bị trùng thời gian.");

        var entity = await _registrationRepository.AddAsync(new DangKyCa
        {
            MaCa = dto.MaCa,
            MaNV = dto.MaNV,
            MaNguoiDung = dto.MaNguoiDung,
            GhiChu = dto.GhiChu?.Trim(),
            TrangThai = DaDuyet,
            NguoiDuyet = reviewer,
            ThoiGianDuyet = DateTime.Now
        });
        return MapRegistration(entity);
    }

    public async Task CancelRegistrationAsync(int id, int maNV)
    {
        var entity = await _registrationRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Không tìm thấy đăng ký ca.");
        if (entity.MaNV != maNV) throw new UnauthorizedAccessException("Bạn không được hủy đăng ký của người khác.");
        if (entity.TrangThai != ChoDuyet) throw new InvalidOperationException("Chỉ được hủy đăng ký đang chờ duyệt.");
        entity.TrangThai = DaHuy;
        await _registrationRepository.UpdateAsync(entity);
    }

    public async Task ReviewRegistrationAsync(int id, string status, string? reviewer)
    {
        if (status is not (DaDuyet or TuChoi)) throw new ArgumentException("Trạng thái duyệt không hợp lệ.");
        var entity = await _registrationRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Không tìm thấy đăng ký ca.");
        if (entity.TrangThai is DaHuy or TuChoi) throw new InvalidOperationException("Đăng ký đã kết thúc.");
        if (status == DaDuyet)
        {
            var shift = await _shiftRepository.GetShiftWithRegistrationsAsync(entity.MaCa) ?? throw new KeyNotFoundException("Không tìm thấy ca làm.");
            var employee = await _shiftRepository.GetEmployeeAsync(entity.MaNV) ?? throw new KeyNotFoundException("Không tìm thấy nhân viên.");
            if (!string.Equals(employee.LoaiNhanVien, shift.LoaiNhanVien, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Nhân viên không phù hợp với ca {shift.LoaiNhanVien}.");
            if (shift.ThoiGianBatDau <= DateTime.Now) throw new InvalidOperationException("Không thể duyệt ca đã bắt đầu.");
            if (await _shiftRepository.CountApprovedAsync(entity.MaCa) >= shift.SoLuongNhanVienCan)
                throw new InvalidOperationException("Ca làm đã đủ số lượng nhân viên.");
            if (await _shiftRepository.HasStaffOverlapAsync(entity.MaNV, shift.ThoiGianBatDau, shift.ThoiGianKetThuc, entity.MaDangKy))
                throw new InvalidOperationException("Nhân viên đã có ca khác bị trùng thời gian.");
        }
        entity.TrangThai = status;
        entity.NguoiDuyet = reviewer;
        entity.ThoiGianDuyet = DateTime.Now;
        await _registrationRepository.UpdateAsync(entity);
    }

    private static (DateTime start, DateTime end) NormalizeAndValidateShift(CreateCaLamViecDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.TenCa)) throw new ArgumentException("Tên ca không được để trống.");
        var start = dto.ThoiGianBatDau;
        var end = dto.ThoiGianKetThuc;
        // Ca tối có thể kết thúc vào ngày hôm sau, ví dụ 22:00 - 06:00.
        if (dto.LoaiCa == "Tối" && end <= start && end.Date == start.Date)
            end = end.AddDays(1);
        if (end <= start) throw new ArgumentException("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
        // Cho phép Manager/Admin tạo ca bổ sung trong ngày hôm nay khi nhân viên nghỉ đột xuất.
        // Chỉ từ chối các ca thuộc ngày đã qua; việc đăng ký vẫn kiểm tra ca đã bắt đầu hay chưa.
        if (start.Date < DateTime.Today) throw new ArgumentException("Không thể tạo ca cho ngày đã qua.");
        if (dto.SoLuongNhanVienCan <= 0) throw new ArgumentException("Số lượng nhân viên phải lớn hơn 0.");
        if (dto.LoaiNhanVien is not ("Part-time" or "Full-time")) throw new ArgumentException("Loại nhân viên không hợp lệ.");
        return (start, end);
    }

    private static void EnsureShiftEditable(CaLamViec entity, DateTime newStart)
    {
        if (entity.TrangThai is DaDong or DaHuyCa)
            throw new InvalidOperationException("Không thể thay đổi ca đã bắt đầu, đã đóng hoặc đã hủy.");

        // Cho phép dời ca chưa có đăng ký sang thời gian tương lai,
        // kể cả khi giờ bắt đầu cũ vừa trôi qua do nhân viên nghỉ đột xuất.
        if (entity.ThoiGianBatDau <= DateTime.Now &&
            (newStart <= DateTime.Now || entity.DangKyCas.Any(x => x.TrangThai != DaHuy)))
            throw new InvalidOperationException("Ca đã bắt đầu. Chỉ được dời sang thời gian tương lai khi ca chưa có đăng ký.");
    }

    private static CaLamViecDto MapShift(CaLamViec x) => new()
    {
        MaCa = x.MaCa, TenCa = x.TenCa, ThoiGianBatDau = x.ThoiGianBatDau, ThoiGianKetThuc = x.ThoiGianKetThuc,
        LoaiCa = x.LoaiCa, LoaiNhanVien = x.LoaiNhanVien, SoLuongNhanVienCan = x.SoLuongNhanVienCan,
        SoLuongDaDuyet = x.DangKyCas.Count(d => d.TrangThai == DaDuyet), TrangThai = x.TrangThai,
        NguoiTao = x.NguoiTao, GhiChu = x.GhiChu,
        DangKyCas = x.DangKyCas.Select(MapRegistration).ToList()
    };

    private static DangKyCaDto MapRegistration(DangKyCa x) => new()
    {
        MaDangKy = x.MaDangKy, MaCa = x.MaCa, MaNV = x.MaNV, TenNhanVien = x.NhanVien?.TenNV ?? string.Empty,
        ThoiGianDangKy = x.ThoiGianDangKy, TrangThai = x.TrangThai, GhiChu = x.GhiChu, NguoiDuyet = x.NguoiDuyet
    };
}
