using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;

namespace CoffeeShopManager.BLL.Services;

public interface IDatBanService
{
    Task<IEnumerable<DatBanDto>> GetAllAsync();
    Task<DatBanDto> CreateAsync(CreateDatBanDto dto);
    Task<DatBanDto> UpdateAsync(int id, UpdateDatBanDto dto);
    Task<DatBanDto> UpdateStatusAsync(int id, string status);
    Task CancelAsync(int id);
}

public class DatBanService : IDatBanService
{
    public const string ChoXacNhan = "Chờ xác nhận";
    public const string DaXacNhan = "Đã xác nhận";
    public const string DaNhanBan = "Đã nhận bàn";
    public const string DaHuy = "Đã hủy";
    public const string DaHoanThanh = "Đã hoàn thành";

    private static readonly string[] ValidStatuses = { ChoXacNhan, DaXacNhan, DaNhanBan, DaHuy, DaHoanThanh };
    private readonly IDatBanRepository _repository;
    private readonly IBanRepository _banRepository;
    private readonly IKhachHangRepository _khachHangRepository;

    public DatBanService(IDatBanRepository repository, IBanRepository banRepository, IKhachHangRepository khachHangRepository)
    {
        _repository = repository;
        _banRepository = banRepository;
        _khachHangRepository = khachHangRepository;
    }

    public async Task<IEnumerable<DatBanDto>> GetAllAsync() =>
        (await _repository.GetHistoryAsync()).Select(Map).ToList();

    public async Task<DatBanDto> CreateAsync(CreateDatBanDto dto)
    {
        Validate(dto.MaBan, dto.MaKH, dto.ThoiGianBatDau, dto.ThoiGianKetThuc, dto.SoLuongKhach);
        var ban = await _banRepository.GetByIdAsync(dto.MaBan) ?? throw new KeyNotFoundException("Không tìm thấy bàn.");
        var khachHang = await _khachHangRepository.GetByIdAsync(dto.MaKH) ?? throw new KeyNotFoundException("Không tìm thấy khách hàng.");
        if (ban.TenBan == "Mang về") throw new InvalidOperationException("Không thể đặt bàn Mang về.");
        if (await _repository.HasOverlapAsync(dto.MaBan, dto.ThoiGianBatDau, dto.ThoiGianKetThuc))
            throw new InvalidOperationException("Bàn đã có lịch đặt trong khoảng thời gian này.");

        var entity = await _repository.AddAsync(new DatBan
        {
            MaBan = dto.MaBan,
            MaKH = dto.MaKH,
            TenKhachHang = khachHang.TenKH,
            SoDienThoai = khachHang.SoDienThoai,
            ThoiGianBatDau = dto.ThoiGianBatDau,
            ThoiGianKetThuc = dto.ThoiGianKetThuc,
            SoLuongKhach = dto.SoLuongKhach,
            TrangThai = ChoXacNhan,
            GhiChu = dto.GhiChu?.Trim(),
            NguoiTao = dto.NguoiTao?.Trim(),
            Ban = ban,
            KhachHang = khachHang
        });
        return Map(entity);
    }

    public async Task<DatBanDto> UpdateAsync(int id, UpdateDatBanDto dto)
    {
        Validate(dto.MaBan, dto.MaKH, dto.ThoiGianBatDau, dto.ThoiGianKetThuc, dto.SoLuongKhach);
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Không tìm thấy đặt bàn.");
        if (entity.TrangThai is DaHuy or DaHoanThanh or DaNhanBan)
            throw new InvalidOperationException("Không thể sửa đặt bàn ở trạng thái hiện tại.");
        _ = await _banRepository.GetByIdAsync(dto.MaBan) ?? throw new KeyNotFoundException("Không tìm thấy bàn.");
        var khachHang = await _khachHangRepository.GetByIdAsync(dto.MaKH) ?? throw new KeyNotFoundException("Không tìm thấy khách hàng.");
        if (await _repository.HasOverlapAsync(dto.MaBan, dto.ThoiGianBatDau, dto.ThoiGianKetThuc, id))
            throw new InvalidOperationException("Bàn đã có lịch đặt trong khoảng thời gian này.");

        entity.MaBan = dto.MaBan;
        entity.MaKH = dto.MaKH;
        entity.TenKhachHang = khachHang.TenKH;
        entity.SoDienThoai = khachHang.SoDienThoai;
        entity.ThoiGianBatDau = dto.ThoiGianBatDau;
        entity.ThoiGianKetThuc = dto.ThoiGianKetThuc;
        entity.SoLuongKhach = dto.SoLuongKhach;
        entity.GhiChu = dto.GhiChu?.Trim();
        await _repository.UpdateAsync(entity);
        return Map(entity);
    }

    public async Task<DatBanDto> UpdateStatusAsync(int id, string status)
    {
        if (!ValidStatuses.Contains(status)) throw new ArgumentException("Trạng thái đặt bàn không hợp lệ.");
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Không tìm thấy đặt bàn.");
        if (entity.TrangThai == DaHoanThanh || entity.TrangThai == DaHuy)
            throw new InvalidOperationException("Đặt bàn đã kết thúc và không thể cập nhật.");

        entity.TrangThai = status;
        var ban = await _banRepository.GetByIdAsync(entity.MaBan);
        if (ban != null)
        {
            if (status == DaXacNhan && ban.TrangThai == "Trống") ban.TrangThai = "Đã đặt";
            if (status == DaNhanBan) ban.TrangThai = "Đang sử dụng";
            if (status is DaHuy or DaHoanThanh) ban.TrangThai = "Trống";
            await _banRepository.UpdateAsync(ban);
        }
        await _repository.UpdateAsync(entity);
        return Map(entity);
    }

    public Task CancelAsync(int id) => UpdateStatusAsync(id, DaHuy);

    private static void Validate(int maBan, int maKH, DateTime start, DateTime end, int guests)
    {
        if (maBan <= 0) throw new ArgumentException("Bàn không hợp lệ.");
        if (maKH <= 0) throw new ArgumentException("Khách hàng không hợp lệ.");
        if (start < DateTime.Now) throw new ArgumentException("Thời gian đặt bàn không được ở trong quá khứ.");
        if (end <= start) throw new ArgumentException("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
        if (guests <= 0) throw new ArgumentException("Số lượng khách phải lớn hơn 0.");
    }

    private static DatBanDto Map(DatBan d) => new()
    {
        MaDatBan = d.MaDatBan,
        MaBan = d.MaBan,
        TenBan = d.Ban?.TenBan ?? string.Empty,
        MaKH = d.MaKH,
        TenKhachHang = d.TenKhachHang,
        SoDienThoai = d.SoDienThoai,
        ThoiGianBatDau = d.ThoiGianBatDau,
        ThoiGianKetThuc = d.ThoiGianKetThuc,
        SoLuongKhach = d.SoLuongKhach,
        TrangThai = d.TrangThai,
        GhiChu = d.GhiChu,
        NgayTao = d.NgayTao
    };
}
