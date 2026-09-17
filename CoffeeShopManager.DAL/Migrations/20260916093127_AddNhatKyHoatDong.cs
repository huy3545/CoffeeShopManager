using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddNhatKyHoatDong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NhatKyHoatDong",
                columns: table => new
                {
                    MaNhatKy = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaNguoiDung = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    TenNguoiDung = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    VaiTro = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    HanhDong = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LoaiDoiTuong = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MaDoiTuong = table.Column<string>(type: "TEXT", nullable: true),
                    MoTa = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ThoiGianThucHien = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DiaChiIP = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    KetQua = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    DuLieuCu = table.Column<string>(type: "TEXT", nullable: true),
                    DuLieuMoi = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhatKyHoatDong", x => x.MaNhatKy);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NhatKyHoatDong");
        }
    }
}
