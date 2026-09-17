using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;

namespace CoffeeShopManager.BLL.Services;

public interface IKhuyenMaiService
{
    Task<IEnumerable<KhuyenMaiDto>> GetAllAsync();
    Task<IEnumerable<KhuyenMaiDto>> GetActiveAsync();
    Task<KhuyenMaiDto?> GetByIdAsync(int maKM);
    Task<KhuyenMaiDto> CreateAsync(CreateKhuyenMaiDto dto);
    Task<KhuyenMaiDto> UpdateAsync(int maKM, UpdateKhuyenMaiDto dto);
    Task DeleteAsync(int maKM);
    Task<KhuyenMaiDto> ToggleAsync(int maKM);
    Task<ApDungKhuyenMaiDto> CalculateDiscountAsync(int maKM, int maHD);
}

public class KhuyenMaiService : IKhuyenMaiService
{
    public const string PhanTram = "Phần trăm";
    public const string SoTien = "Số tiền cố định";
    public const string HoaDonToiThieu = "Theo hóa đơn tối thiểu";
    public const string Mon = "Theo món";
    public const string LoaiMon = "Theo loại món";

    private static readonly string[] ValidTypes = { PhanTram, SoTien, HoaDonToiThieu, Mon, LoaiMon };
    private readonly IKhuyenMaiRepository _repository;
    private readonly IHoaDonRepository _hoaDonRepository;

    public KhuyenMaiService(IKhuyenMaiRepository repository, IHoaDonRepository hoaDonRepository)
    {
        _repository = repository;
        _hoaDonRepository = hoaDonRepository;
    }

    public async Task<IEnumerable<KhuyenMaiDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<KhuyenMaiDto>> GetActiveAsync()
    {
        var now = DateTime.Now;
        var items = await _repository.GetAllAsync();
        return items.Where(km => IsActive(km, now)).Select(MapToDto).ToList();
    }

    public async Task<KhuyenMaiDto?> GetByIdAsync(int maKM)
    {
        if (maKM <= 0) throw new ArgumentException("Mã khuyến mãi không hợp lệ");
        var item = await _repository.GetByIdAsync(maKM);
        return item is null ? null : MapToDto(item);
    }

    public async Task<KhuyenMaiDto> CreateAsync(CreateKhuyenMaiDto dto)
    {
        Validate(dto);
        var duplicate = (await _repository.FindAsync(km => km.TenKhuyenMai == dto.TenKhuyenMai.Trim()))
            .Any(km => DatesOverlap(km.NgayBatDau, km.NgayKetThuc, dto.NgayBatDau, dto.NgayKetThuc));
        if (duplicate) throw new InvalidOperationException("Tên khuyến mãi đã tồn tại trong cùng thời gian.");

        var entity = new KhuyenMai
        {
            TenKhuyenMai = dto.TenKhuyenMai.Trim(),
            MoTa = dto.MoTa?.Trim(),
            LoaiKhuyenMai = dto.LoaiKhuyenMai,
            GiaTriGiam = dto.GiaTriGiam,
            NgayBatDau = dto.NgayBatDau,
            NgayKetThuc = dto.NgayKetThuc,
            HoaDonToiThieu = dto.HoaDonToiThieu,
            MaMon = dto.MaMon,
            LoaiMon = dto.LoaiMon?.Trim(),
            SoLanSuDungToiDa = dto.SoLanSuDungToiDa,
            DaKichHoat = dto.DaKichHoat,
            MaNguoiTao = dto.MaNguoiTao
        };
        return MapToDto(await _repository.AddAsync(entity));
    }

    public async Task<KhuyenMaiDto> UpdateAsync(int maKM, UpdateKhuyenMaiDto dto)
    {
        Validate(dto);
        var entity = await _repository.GetByIdAsync(maKM) ?? throw new KeyNotFoundException("Không tìm thấy khuyến mãi.");
        var duplicate = (await _repository.FindAsync(km => km.TenKhuyenMai == dto.TenKhuyenMai.Trim() && km.MaKM != maKM))
            .Any(km => DatesOverlap(km.NgayBatDau, km.NgayKetThuc, dto.NgayBatDau, dto.NgayKetThuc));
        if (duplicate) throw new InvalidOperationException("Tên khuyến mãi đã tồn tại trong cùng thời gian.");

        entity.TenKhuyenMai = dto.TenKhuyenMai.Trim();
        entity.MoTa = dto.MoTa?.Trim();
        entity.LoaiKhuyenMai = dto.LoaiKhuyenMai;
        entity.GiaTriGiam = dto.GiaTriGiam;
        entity.NgayBatDau = dto.NgayBatDau;
        entity.NgayKetThuc = dto.NgayKetThuc;
        entity.HoaDonToiThieu = dto.HoaDonToiThieu;
        entity.MaMon = dto.MaMon;
        entity.LoaiMon = dto.LoaiMon?.Trim();
        entity.SoLanSuDungToiDa = dto.SoLanSuDungToiDa;
        entity.DaKichHoat = dto.DaKichHoat;
        await _repository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    public async Task DeleteAsync(int maKM)
    {
        var entity = await _repository.GetByIdAsync(maKM) ?? throw new KeyNotFoundException("Không tìm thấy khuyến mãi.");
        if (await _repository.HasBeenUsedAsync(maKM))
            throw new InvalidOperationException("Không thể xóa khuyến mãi đã được sử dụng.");
        await _repository.DeleteAsync(entity.MaKM);
    }

    public async Task<KhuyenMaiDto> ToggleAsync(int maKM)
    {
        var entity = await _repository.GetByIdAsync(maKM) ?? throw new KeyNotFoundException("Không tìm thấy khuyến mãi.");
        entity.DaKichHoat = !entity.DaKichHoat;
        await _repository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    public async Task<ApDungKhuyenMaiDto> CalculateDiscountAsync(int maKM, int maHD)
    {
        var promotion = await _repository.GetByIdAsync(maKM) ?? throw new KeyNotFoundException("Không tìm thấy khuyến mãi.");
        var invoice = await _hoaDonRepository.GetHoaDonWithDetailsAsync(maHD) ?? throw new KeyNotFoundException("Không tìm thấy hóa đơn.");
        if (invoice.TrangThai == "Đã thanh toán") throw new InvalidOperationException("Không thể áp dụng khuyến mãi cho hóa đơn đã thanh toán.");
        if (invoice.ChiTietHDs.Count == 0) throw new InvalidOperationException("Hóa đơn chưa có món.");
        if (!IsActive(promotion, DateTime.Now)) throw new InvalidOperationException("Khuyến mãi chưa bắt đầu, đã hết hạn hoặc đã ngừng sử dụng.");
        if (promotion.SoLanSuDungToiDa.HasValue && promotion.SoLanDaSuDung >= promotion.SoLanSuDungToiDa.Value)
            throw new InvalidOperationException("Khuyến mãi đã hết số lần sử dụng.");

        var originalTotal = invoice.ChiTietHDs.Sum(ct => ct.ThanhTien);
        if (originalTotal < promotion.HoaDonToiThieu)
            throw new InvalidOperationException($"Hóa đơn phải đạt tối thiểu {promotion.HoaDonToiThieu:N0} VNĐ.");

        var eligibleTotal = promotion.LoaiKhuyenMai switch
        {
            Mon => invoice.ChiTietHDs.Where(ct => ct.MaMon == promotion.MaMon).Sum(ct => ct.ThanhTien),
            LoaiMon => invoice.ChiTietHDs.Where(ct => ct.Mon?.Loai == promotion.LoaiMon).Sum(ct => ct.ThanhTien),
            _ => originalTotal
        };
        if ((promotion.LoaiKhuyenMai is Mon or LoaiMon) && eligibleTotal <= 0)
            throw new InvalidOperationException("Hóa đơn không có món phù hợp với khuyến mãi.");

        var discount = promotion.LoaiKhuyenMai == PhanTram
            ? eligibleTotal * promotion.GiaTriGiam / 100m
            : promotion.GiaTriGiam;
        discount = Math.Clamp(discount, 0m, eligibleTotal);
        return new ApDungKhuyenMaiDto
        {
            MaKM = promotion.MaKM,
            TenKhuyenMai = promotion.TenKhuyenMai,
            TongTienTruocGiam = originalTotal,
            SoTienGiam = discount,
            TongTienSauGiam = Math.Max(0, originalTotal - discount)
        };
    }

    private static bool IsActive(KhuyenMai km, DateTime now) =>
        km.DaKichHoat && now >= km.NgayBatDau && now <= km.NgayKetThuc;

    private static string GetStatus(KhuyenMai km)
    {
        if (!km.DaKichHoat) return "Đã ngừng sử dụng";
        if (DateTime.Now < km.NgayBatDau) return "Chưa bắt đầu";
        if (DateTime.Now > km.NgayKetThuc) return "Đã hết hạn";
        return "Đang hoạt động";
    }

    private static void Validate(CreateKhuyenMaiDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.TenKhuyenMai)) throw new ArgumentException("Tên khuyến mãi không được để trống.");
        if (!ValidTypes.Contains(dto.LoaiKhuyenMai)) throw new ArgumentException("Loại khuyến mãi không hợp lệ.");
        if (dto.GiaTriGiam <= 0) throw new ArgumentException("Giá trị giảm phải lớn hơn 0.");
        if (dto.LoaiKhuyenMai == PhanTram && dto.GiaTriGiam > 100) throw new ArgumentException("Phần trăm giảm không được lớn hơn 100%.");
        if (dto.NgayKetThuc < dto.NgayBatDau) throw new ArgumentException("Ngày kết thúc không được trước ngày bắt đầu.");
        if (dto.HoaDonToiThieu < 0) throw new ArgumentException("Mức hóa đơn tối thiểu không được âm.");
        if (dto.SoLanSuDungToiDa.HasValue && dto.SoLanSuDungToiDa <= 0) throw new ArgumentException("Số lần sử dụng tối đa phải lớn hơn 0.");
        if (dto.LoaiKhuyenMai == Mon && !dto.MaMon.HasValue) throw new ArgumentException("Vui lòng chọn món áp dụng.");
        if (dto.LoaiKhuyenMai == LoaiMon && string.IsNullOrWhiteSpace(dto.LoaiMon)) throw new ArgumentException("Vui lòng chọn loại món áp dụng.");
    }

    private static bool DatesOverlap(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd) => aStart <= bEnd && bStart <= aEnd;

    private static KhuyenMaiDto MapToDto(KhuyenMai km) => new()
    {
        MaKM = km.MaKM,
        TenKhuyenMai = km.TenKhuyenMai,
        MoTa = km.MoTa,
        LoaiKhuyenMai = km.LoaiKhuyenMai,
        GiaTriGiam = km.GiaTriGiam,
        NgayBatDau = km.NgayBatDau,
        NgayKetThuc = km.NgayKetThuc,
        HoaDonToiThieu = km.HoaDonToiThieu,
        MaMon = km.MaMon,
        TenMon = km.Mon?.TenMon,
        LoaiMon = km.LoaiMon,
        SoLanSuDungToiDa = km.SoLanSuDungToiDa,
        SoLanDaSuDung = km.SoLanDaSuDung,
        DaKichHoat = km.DaKichHoat,
        NgayTao = km.NgayTao,
        TrangThai = GetStatus(km)
    };
}
