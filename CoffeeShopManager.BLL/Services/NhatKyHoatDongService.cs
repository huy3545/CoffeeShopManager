using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;

namespace CoffeeShopManager.BLL.Services;

public interface INhatKyHoatDongService
{
    Task<IEnumerable<NhatKyHoatDongDto>> SearchAsync(string? searchText = null, string? action = null, DateTime? from = null, DateTime? to = null);
    Task<NhatKyHoatDongDto> GhiNhanAsync(GhiNhatKyDto dto);
}

public class NhatKyHoatDongService : INhatKyHoatDongService
{
    private readonly INhatKyHoatDongRepository _repository;

    public NhatKyHoatDongService(INhatKyHoatDongRepository repository) => _repository = repository;

    public async Task<IEnumerable<NhatKyHoatDongDto>> SearchAsync(string? searchText = null, string? action = null, DateTime? from = null, DateTime? to = null) =>
        (await _repository.SearchAsync(searchText, action, from, to)).Select(Map).ToList();

    public async Task<NhatKyHoatDongDto> GhiNhanAsync(GhiNhatKyDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.HanhDong)) throw new ArgumentException("Hành động không được để trống.");
        if (string.IsNullOrWhiteSpace(dto.LoaiDoiTuong)) throw new ArgumentException("Loại đối tượng không được để trống.");
        if (string.IsNullOrWhiteSpace(dto.MoTa)) throw new ArgumentException("Mô tả không được để trống.");

        var entity = await _repository.AddAsync(new NhatKyHoatDong
        {
            MaNguoiDung = dto.MaNguoiDung,
            TenNguoiDung = dto.TenNguoiDung,
            VaiTro = dto.VaiTro,
            HanhDong = dto.HanhDong.Trim(),
            LoaiDoiTuong = dto.LoaiDoiTuong.Trim(),
            MaDoiTuong = dto.MaDoiTuong,
            MoTa = dto.MoTa.Trim(),
            DiaChiIP = dto.DiaChiIP,
            KetQua = string.IsNullOrWhiteSpace(dto.KetQua) ? "Thành công" : dto.KetQua.Trim(),
            DuLieuCu = dto.DuLieuCu,
            DuLieuMoi = dto.DuLieuMoi,
            ThoiGianThucHien = DateTime.Now
        });
        return Map(entity);
    }

    private static NhatKyHoatDongDto Map(NhatKyHoatDong x) => new()
    {
        MaNhatKy = x.MaNhatKy,
        MaNguoiDung = x.MaNguoiDung,
        TenNguoiDung = x.TenNguoiDung,
        VaiTro = x.VaiTro,
        HanhDong = x.HanhDong,
        LoaiDoiTuong = x.LoaiDoiTuong,
        MaDoiTuong = x.MaDoiTuong,
        MoTa = x.MoTa,
        ThoiGianThucHien = x.ThoiGianThucHien,
        DiaChiIP = x.DiaChiIP,
        KetQua = x.KetQua,
        DuLieuCu = x.DuLieuCu,
        DuLieuMoi = x.DuLieuMoi
    };
}
