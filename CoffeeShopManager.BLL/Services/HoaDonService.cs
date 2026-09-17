using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;

namespace CoffeeShopManager.BLL.Services;

public interface IHoaDonService
{
    Task<IEnumerable<HoaDonDto>> GetAllHoaDonsAsync();
    Task<HoaDonDto?> GetHoaDonByIdAsync(int maHD);
    Task<HoaDonDto> CreateHoaDonAsync(CreateHoaDonDto createDto);
    Task<HoaDonDto> GoiMonAsync(GoiMonDto goiMonDto);
    Task<HoaDonDto> ThanhToanAsync(ThanhToanDto thanhToanDto);
    Task<HoaDonDto> UpdateHoaDonAsync(int maHD, UpdateHoaDonDto updateDto);
    Task DeleteHoaDonAsync(int maHD);
    Task<IEnumerable<HoaDonDto>> GetHoaDonsByBanAsync(int maBan);
    Task<IEnumerable<HoaDonDto>> GetHoaDonsByDateRangeAsync(DateTime tuNgay, DateTime denNgay);
    Task<decimal> TinhTongTienHoaDonAsync(int maHD);
    Task<HoaDonDto?> XoaMonAsync(int maChiTiet);
}

public class HoaDonService : IHoaDonService
{
    private readonly IHoaDonRepository _hoaDonRepository;
    private readonly IBanRepository _banRepository;
    private readonly IMonRepository _monRepository;
    private readonly IChiTietHDRepository _chiTietRepository;
    private readonly IKhachHangRepository _khachHangRepository;
    private readonly IKhuyenMaiService _khuyenMaiService;
    private readonly IKhuyenMaiRepository _khuyenMaiRepository;
    private readonly IHoaDonKhuyenMaiRepository _hoaDonKhuyenMaiRepository;

    public HoaDonService(
        IHoaDonRepository hoaDonRepository,
        IBanRepository banRepository,
        IMonRepository monRepository,
        IChiTietHDRepository chiTietRepository,
        IKhachHangRepository khachHangRepository,
        IKhuyenMaiService khuyenMaiService,
        IKhuyenMaiRepository khuyenMaiRepository,
        IHoaDonKhuyenMaiRepository hoaDonKhuyenMaiRepository)
    {
        _hoaDonRepository = hoaDonRepository;
        _banRepository = banRepository;
        _monRepository = monRepository;
        _chiTietRepository = chiTietRepository;
        _khachHangRepository = khachHangRepository;
        _khuyenMaiService = khuyenMaiService;
        _khuyenMaiRepository = khuyenMaiRepository;
        _hoaDonKhuyenMaiRepository = hoaDonKhuyenMaiRepository;
    }

    public async Task<IEnumerable<HoaDonDto>> GetAllHoaDonsAsync()
    {
        try
        {
            var hoaDons = await _hoaDonRepository.GetAllAsync();
            var result = new List<HoaDonDto>();

            foreach (var hd in hoaDons)
            {
                var hdWithDetails = await _hoaDonRepository.GetHoaDonWithDetailsAsync(hd.MaHD);
                if (hdWithDetails != null)
                    result.Add(MapToDto(hdWithDetails));
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy danh sách hóa đơn: {ex.Message}", ex);
        }
    }

    public async Task<HoaDonDto?> GetHoaDonByIdAsync(int maHD)
    {
        try
        {
            if (maHD <= 0)
                throw new ArgumentException("Mã hóa đơn không hợp lệ");

            var hoaDon = await _hoaDonRepository.GetHoaDonWithDetailsAsync(maHD);
            return hoaDon != null ? MapToDto(hoaDon) : null;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy thông tin hóa đơn: {ex.Message}", ex);
        }
    }

    public async Task<HoaDonDto> CreateHoaDonAsync(CreateHoaDonDto createDto)
    {
        try
        {
            // Validation
            if (createDto.MaBan <= 0)
                throw new ArgumentException("Mã bàn không hợp lệ");

            var ban = await _banRepository.GetByIdAsync(createDto.MaBan);
            if (ban == null)
                throw new KeyNotFoundException($"Không tìm thấy bàn với mã {createDto.MaBan}");

            // Kiểm tra xem bàn đã có hóa đơn chưa thanh toán chưa
            var hoaDons = await _hoaDonRepository.GetHoaDonsByBanAsync(createDto.MaBan);
            var hoaDonChuaThanhToan = hoaDons.FirstOrDefault(h => h.TrangThai == "Chưa thanh toán");
            
            if (hoaDonChuaThanhToan != null)
            {
                // Bàn đã có hóa đơn chưa thanh toán, trả về hóa đơn đó thay vì tạo mới
                var result = await _hoaDonRepository.GetHoaDonWithDetailsAsync(hoaDonChuaThanhToan.MaHD);
                return MapToDto(result!);
            }

            // Không cho phép tạo hóa đơn mới cho bàn đang được sử dụng.
            if (ban.TrangThai == "Đang sử dụng")
                throw new InvalidOperationException("Không thể tạo hóa đơn mới cho bàn đang được sử dụng");

            var hoaDon = new HoaDon
            {
                MaBan = createDto.MaBan,
                ThoiGianTao = DateTime.Now,
                TongTien = 0,
                TrangThai = "Chưa thanh toán",
                MaKH = createDto.MaKH,
                MaNV = createDto.MaNV
            };

            var createdHoaDon = await _hoaDonRepository.AddAsync(hoaDon);

            // Cập nhật trạng thái bàn
            ban.TrangThai = "Đang sử dụng";
            await _banRepository.UpdateAsync(ban);

            var resultNew = await _hoaDonRepository.GetHoaDonWithDetailsAsync(createdHoaDon.MaHD);
            return MapToDto(resultNew!);
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
            throw new Exception($"Lỗi khi tạo hóa đơn: {ex.Message}", ex);
        }
    }

    public async Task<HoaDonDto> GoiMonAsync(GoiMonDto goiMonDto)
    {
        try
        {
            // Validation
            if (goiMonDto.MaHD <= 0)
                throw new ArgumentException("Mã hóa đơn không hợp lệ");

            if (goiMonDto.MaMon <= 0)
                throw new ArgumentException("Mã món không hợp lệ");

            if (goiMonDto.SoLuong <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0");

            var hoaDon = await _hoaDonRepository.GetHoaDonWithDetailsAsync(goiMonDto.MaHD);
            if (hoaDon == null)
                throw new KeyNotFoundException($"Không tìm thấy hóa đơn với mã {goiMonDto.MaHD}");

            if (hoaDon.TrangThai == "Đã thanh toán")
                throw new InvalidOperationException("Không thể gọi món cho hóa đơn đã thanh toán");

            var mon = await _monRepository.GetByIdAsync(goiMonDto.MaMon);
            if (mon == null)
                throw new KeyNotFoundException($"Không tìm thấy món với mã {goiMonDto.MaMon}");

            // Kiểm tra xem món đã có trong hóa đơn chưa
            var existingChiTiet = hoaDon.ChiTietHDs.FirstOrDefault(ct => ct.MaMon == goiMonDto.MaMon);

            if (existingChiTiet != null)
            {
                // Cập nhật số lượng
                existingChiTiet.SoLuong += goiMonDto.SoLuong;
                existingChiTiet.ThanhTien = existingChiTiet.SoLuong * mon.DonGia;
                await _chiTietRepository.UpdateAsync(existingChiTiet);
            }
            else
            {
                // Thêm chi tiết mới
                var chiTiet = new ChiTietHD
                {
                    MaHD = goiMonDto.MaHD,
                    MaMon = goiMonDto.MaMon,
                    SoLuong = goiMonDto.SoLuong,
                    ThanhTien = goiMonDto.SoLuong * mon.DonGia
                };
                await _chiTietRepository.AddAsync(chiTiet);
            }

            // Cập nhật tổng tiền
            var tongTien = await TinhTongTienHoaDonAsync(goiMonDto.MaHD);
            hoaDon.TongTien = tongTien;
            await _hoaDonRepository.UpdateAsync(hoaDon);

            var result = await _hoaDonRepository.GetHoaDonWithDetailsAsync(goiMonDto.MaHD);
            return MapToDto(result!);
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
            throw new Exception($"Lỗi khi gọi món: {ex.Message}", ex);
        }
    }

    public async Task<HoaDonDto> ThanhToanAsync(ThanhToanDto thanhToanDto)
    {
        try
        {
            if (thanhToanDto.MaHD <= 0)
                throw new ArgumentException("Mã hóa đơn không hợp lệ");

            var hoaDon = await _hoaDonRepository.GetHoaDonWithDetailsAsync(thanhToanDto.MaHD);
            if (hoaDon == null)
                throw new KeyNotFoundException($"Không tìm thấy hóa đơn với mã {thanhToanDto.MaHD}");

            if (hoaDon.TrangThai == "Đã thanh toán")
                throw new InvalidOperationException("Hóa đơn đã được thanh toán");

            if (hoaDon.ChiTietHDs.Count == 0)
                throw new InvalidOperationException("Không thể thanh toán hóa đơn chưa có món");

            var tongTienGoc = hoaDon.ChiTietHDs.Sum(ct => ct.ThanhTien);
            decimal tienGiamKhuyenMai = 0;
            if (thanhToanDto.MaKM.HasValue)
            {
                if (hoaDon.MaKM.HasValue)
                    throw new InvalidOperationException("Hóa đơn chỉ được áp dụng một khuyến mãi.");

                var promotionResult = await _khuyenMaiService.CalculateDiscountAsync(thanhToanDto.MaKM.Value, hoaDon.MaHD);
                tienGiamKhuyenMai = promotionResult.SoTienGiam;
            }

            // Xử lý khách hàng: chỉ tạo khách hàng nếu có số điện thoại (để tích điểm)
            int? maKH = thanhToanDto.MaKH;
            string? tenKH = null;
            
            if (!string.IsNullOrWhiteSpace(thanhToanDto.SoDienThoai))
            {
                // Có số điện thoại → tìm hoặc tạo khách hàng để tích điểm
                var khachHang = await _khachHangRepository.GetKhachHangBySoDienThoaiAsync(thanhToanDto.SoDienThoai.Trim());
                
                if (khachHang != null)
                {
                    // Khách hàng đã tồn tại, cập nhật tên nếu có
                    if (!string.IsNullOrWhiteSpace(thanhToanDto.TenKH))
                    {
                        khachHang.TenKH = thanhToanDto.TenKH.Trim();
                        await _khachHangRepository.UpdateAsync(khachHang);
                    }
                    maKH = khachHang.MaKH;
                }
                else
                {
                    // Tạo khách hàng mới với số điện thoại
                    var tenKHNew = !string.IsNullOrWhiteSpace(thanhToanDto.TenKH) 
                        ? thanhToanDto.TenKH.Trim() 
                        : $"Khách hàng_{new Random().Next(1000, 9999)}";
                    
                    var newKhachHang = new KhachHang
                    {
                        TenKH = tenKHNew,
                        SoDienThoai = thanhToanDto.SoDienThoai.Trim(),
                        DiemTichLuy = 0
                    };
                    
                    var createdKhachHang = await _khachHangRepository.AddAsync(newKhachHang);
                    maKH = createdKhachHang.MaKH;
                }
            }
            else if (!string.IsNullOrWhiteSpace(thanhToanDto.TenKH))
            {
                // Chỉ có tên, không có số điện thoại → lưu tên vào hóa đơn, KHÔNG tạo khách hàng
                tenKH = thanhToanDto.TenKH.Trim();
                maKH = null; // Không tạo khách hàng, không tích điểm
            }
            else if (!maKH.HasValue)
            {
                // Không có thông tin khách hàng → không tạo khách hàng, không tích điểm
                maKH = null;
                tenKH = null;
            }

            // Xử lý sử dụng điểm tích lũy (100 điểm = 10,000 VNĐ)
            decimal tienGiamTuDiem = 0;
            int diemSuDung = 0;
            
            if (maKH.HasValue && thanhToanDto.DiemSuDung > 0)
            {
                var khachHang = await _khachHangRepository.GetByIdAsync(maKH.Value);
                if (khachHang != null)
                {
                    // Kiểm tra số điểm khách hàng có
                    if (khachHang.DiemTichLuy < thanhToanDto.DiemSuDung)
                    {
                        throw new InvalidOperationException($"Khách hàng chỉ có {khachHang.DiemTichLuy} điểm, không đủ để sử dụng {thanhToanDto.DiemSuDung} điểm");
                    }

                    // Tính số tiền được giảm: 100 điểm = 10,000 VNĐ
                    tienGiamTuDiem = (thanhToanDto.DiemSuDung / 100m) * 10000m;
                    
                    // Đảm bảo số tiền giảm không vượt quá tổng tiền hóa đơn
                    var tongTienTruocDiem = Math.Max(0, tongTienGoc - tienGiamKhuyenMai);
                    if (tienGiamTuDiem > tongTienTruocDiem)
                    {
                        tienGiamTuDiem = tongTienTruocDiem;
                        // Tính lại số điểm cần dùng để giảm đúng bằng tổng tiền
                        diemSuDung = (int)Math.Ceiling((tongTienTruocDiem / 10000m) * 100);
                    }
                    else
                    {
                        diemSuDung = thanhToanDto.DiemSuDung;
                    }

                    // Trừ điểm từ tài khoản khách hàng
                    khachHang.DiemTichLuy -= diemSuDung;
                    await _khachHangRepository.UpdateAsync(khachHang);
                }
            }

            // Tính lại tổng tiền sau khi trừ điểm
            decimal tongTienCuoi = tongTienGoc - tienGiamKhuyenMai - tienGiamTuDiem;
            if (tongTienCuoi < 0)
                tongTienCuoi = 0;

            // Cập nhật trạng thái hóa đơn
            hoaDon.TrangThai = "Đã thanh toán";
            hoaDon.MaKH = maKH;
            hoaDon.TenKH = tenKH; // Lưu tên trực tiếp vào hóa đơn (nếu không có MaKH)
            hoaDon.PhuongThucThanhToan = thanhToanDto.PhuongThucThanhToan;
            hoaDon.TienGiamTuDiem = tienGiamTuDiem;
            hoaDon.DiemSuDung = diemSuDung;
            hoaDon.TienGiamKhuyenMai = tienGiamKhuyenMai;
            hoaDon.MaKM = thanhToanDto.MaKM;
            hoaDon.TongTien = tongTienCuoi; // Cập nhật tổng tiền sau khi trừ điểm

            // Cập nhật điểm tích lũy cho khách hàng (chỉ khi có MaKH - tức là có số điện thoại)
            // Tích điểm dựa trên tổng tiền ban đầu (trước khi trừ điểm)
            if (maKH.HasValue)
            {
                var khachHang = await _khachHangRepository.GetByIdAsync(maKH.Value);
                if (khachHang != null)
                {
                    // Tính tổng tiền ban đầu (trước khi trừ điểm) để tích điểm
                    decimal tongTienBanDau = tongTienGoc;
                    // Tích lũy 1 điểm cho mỗi 10,000 VNĐ (dựa trên tổng tiền ban đầu)
                    int diemMoi = (int)(tongTienBanDau / 10000);
                    khachHang.DiemTichLuy += diemMoi;
                    await _khachHangRepository.UpdateAsync(khachHang);
                }
            }

            await _hoaDonRepository.UpdateAsync(hoaDon);

            if (thanhToanDto.MaKM.HasValue && tienGiamKhuyenMai > 0)
            {
                var promotion = await _khuyenMaiRepository.GetByIdAsync(thanhToanDto.MaKM.Value);
                if (promotion != null)
                {
                    promotion.SoLanDaSuDung++;
                    await _khuyenMaiRepository.UpdateAsync(promotion);
                    await _hoaDonKhuyenMaiRepository.AddAsync(new HoaDonKhuyenMai
                    {
                        MaHD = hoaDon.MaHD,
                        MaKM = promotion.MaKM,
                        SoTienGiam = tienGiamKhuyenMai
                    });
                }
            }

            // Cập nhật trạng thái bàn về Trống
            var ban = await _banRepository.GetByIdAsync(hoaDon.MaBan);
            if (ban != null)
            {
                ban.TrangThai = "Trống";
                await _banRepository.UpdateAsync(ban);
            }

            var result = await _hoaDonRepository.GetHoaDonWithDetailsAsync(thanhToanDto.MaHD);
            return MapToDto(result!);
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
            throw new Exception($"Lỗi khi thanh toán: {ex.Message}", ex);
        }
    }

    public async Task<HoaDonDto> UpdateHoaDonAsync(int maHD, UpdateHoaDonDto updateDto)
    {
        try
        {
            if (maHD <= 0)
                throw new ArgumentException("Mã hóa đơn không hợp lệ");

            var hoaDon = await _hoaDonRepository.GetByIdAsync(maHD);
            if (hoaDon == null)
                throw new KeyNotFoundException($"Không tìm thấy hóa đơn với mã {maHD}");

            if (!string.IsNullOrWhiteSpace(updateDto.TrangThai))
                hoaDon.TrangThai = updateDto.TrangThai;

            if (updateDto.MaKH.HasValue)
                hoaDon.MaKH = updateDto.MaKH.Value;

            if (updateDto.MaNV.HasValue)
                hoaDon.MaNV = updateDto.MaNV.Value;

            await _hoaDonRepository.UpdateAsync(hoaDon);

            var result = await _hoaDonRepository.GetHoaDonWithDetailsAsync(maHD);
            return MapToDto(result!);
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
            throw new Exception($"Lỗi khi cập nhật hóa đơn: {ex.Message}", ex);
        }
    }

    public async Task DeleteHoaDonAsync(int maHD)
    {
        try
        {
            if (maHD <= 0)
                throw new ArgumentException("Mã hóa đơn không hợp lệ");

            var hoaDon = await _hoaDonRepository.GetByIdAsync(maHD);
            if (hoaDon == null)
                throw new KeyNotFoundException($"Không tìm thấy hóa đơn với mã {maHD}");

            // Cập nhật trạng thái bàn về Trống nếu hóa đơn chưa thanh toán
            if (hoaDon.TrangThai == "Chưa thanh toán")
            {
                var ban = await _banRepository.GetByIdAsync(hoaDon.MaBan);
                if (ban != null)
                {
                    ban.TrangThai = "Trống";
                    await _banRepository.UpdateAsync(ban);
                }
            }

            await _hoaDonRepository.DeleteAsync(maHD);
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
            throw new Exception($"Lỗi khi xóa hóa đơn: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<HoaDonDto>> GetHoaDonsByBanAsync(int maBan)
    {
        try
        {
            var hoaDons = await _hoaDonRepository.GetHoaDonsByBanAsync(maBan);
            var result = new List<HoaDonDto>();

            foreach (var hd in hoaDons)
            {
                var hdWithDetails = await _hoaDonRepository.GetHoaDonWithDetailsAsync(hd.MaHD);
                if (hdWithDetails != null)
                    result.Add(MapToDto(hdWithDetails));
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy danh sách hóa đơn theo bàn: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<HoaDonDto>> GetHoaDonsByDateRangeAsync(DateTime tuNgay, DateTime denNgay)
    {
        try
        {
            var hoaDons = await _hoaDonRepository.GetHoaDonsByDateRangeAsync(tuNgay, denNgay);
            var result = new List<HoaDonDto>();

            foreach (var hd in hoaDons)
            {
                var hdWithDetails = await _hoaDonRepository.GetHoaDonWithDetailsAsync(hd.MaHD);
                if (hdWithDetails != null)
                    result.Add(MapToDto(hdWithDetails));
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi lấy danh sách hóa đơn theo khoảng thời gian: {ex.Message}", ex);
        }
    }

    public async Task<decimal> TinhTongTienHoaDonAsync(int maHD)
    {
        try
        {
            var chiTiets = await _chiTietRepository.GetChiTietsByHoaDonAsync(maHD);
            return chiTiets.Sum(ct => ct.ThanhTien);
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi tính tổng tiền hóa đơn: {ex.Message}", ex);
        }
    }

    public async Task<HoaDonDto?> XoaMonAsync(int maChiTiet)
    {
        try
        {
            if (maChiTiet <= 0)
                throw new ArgumentException("Mã chi tiết không hợp lệ");

            var chiTiet = await _chiTietRepository.GetByIdAsync(maChiTiet);
            if (chiTiet == null)
                throw new KeyNotFoundException($"Không tìm thấy chi tiết với mã {maChiTiet}");

            var hoaDon = await _hoaDonRepository.GetHoaDonWithDetailsAsync(chiTiet.MaHD);
            if (hoaDon == null)
                throw new KeyNotFoundException($"Không tìm thấy hóa đơn với mã {chiTiet.MaHD}");

            if (hoaDon.TrangThai == "Đã thanh toán")
                throw new InvalidOperationException("Không thể xóa món khỏi hóa đơn đã thanh toán");

            // Xóa chi tiết
            await _chiTietRepository.DeleteAsync(maChiTiet);

            // Kiểm tra xem hóa đơn còn món nào không
            var remainingChiTiets = await _chiTietRepository.GetChiTietsByHoaDonAsync(chiTiet.MaHD);
            
            if (!remainingChiTiets.Any())
            {
                // Nếu không còn món nào, xóa hóa đơn và cập nhật trạng thái bàn về Trống
                var ban = await _banRepository.GetByIdAsync(hoaDon.MaBan);
                if (ban != null)
                {
                    ban.TrangThai = "Trống";
                    await _banRepository.UpdateAsync(ban);
                }
                await _hoaDonRepository.DeleteAsync(chiTiet.MaHD);
                return null; // Trả về null để báo hiệu hóa đơn đã bị xóa
            }

            // Cập nhật tổng tiền hóa đơn
            var tongTien = await TinhTongTienHoaDonAsync(chiTiet.MaHD);
            hoaDon.TongTien = tongTien;
            await _hoaDonRepository.UpdateAsync(hoaDon);

            // Lấy lại hóa đơn với chi tiết mới
            var updatedHoaDon = await _hoaDonRepository.GetHoaDonWithDetailsAsync(chiTiet.MaHD);
            if (updatedHoaDon == null)
                throw new KeyNotFoundException($"Không tìm thấy hóa đơn sau khi xóa món");

            return MapToDto(updatedHoaDon);
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
            throw new Exception($"Lỗi khi xóa món: {ex.Message}", ex);
        }
    }

    private static HoaDonDto MapToDto(HoaDon hoaDon)
    {
        return new HoaDonDto
        {
            MaHD = hoaDon.MaHD,
            MaBan = hoaDon.MaBan,
            TenBan = hoaDon.Ban?.TenBan ?? "",
            ThoiGianTao = hoaDon.ThoiGianTao,
            TongTien = hoaDon.TongTien,
            TrangThai = hoaDon.TrangThai,
            PhuongThucThanhToan = hoaDon.PhuongThucThanhToan,
            MaKH = hoaDon.MaKH,
            // Ưu tiên tên từ hóa đơn, nếu không có thì lấy từ khách hàng
            TenKH = !string.IsNullOrWhiteSpace(hoaDon.TenKH) ? hoaDon.TenKH : hoaDon.KhachHang?.TenKH,
            TenKHFromCustomer = hoaDon.KhachHang?.TenKH,
            MaNV = hoaDon.MaNV,
            TenNV = hoaDon.NhanVien?.TenNV,
            TienGiamTuDiem = hoaDon.TienGiamTuDiem,
            DiemSuDung = hoaDon.DiemSuDung,
            TienGiamKhuyenMai = hoaDon.TienGiamKhuyenMai,
            MaKM = hoaDon.MaKM,
            ChiTiets = hoaDon.ChiTietHDs.Select(ct => new ChiTietHDDto
            {
                MaChiTiet = ct.MaChiTiet,
                MaHD = ct.MaHD,
                MaMon = ct.MaMon,
                TenMon = ct.Mon?.TenMon ?? "",
                SoLuong = ct.SoLuong,
                DonGia = ct.Mon?.DonGia ?? 0,
                ThanhTien = ct.ThanhTien
            }).ToList()
        };
    }
}
