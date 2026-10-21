using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddLichLamViec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaLamViec",
                columns: table => new
                {
                    MaCa = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenCa = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LoaiCa = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    LoaiNhanVien = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SoLuongNhanVienCan = table.Column<int>(type: "INTEGER", nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    NguoiTao = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    GhiChu = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaLamViec", x => x.MaCa);
                });

            migrationBuilder.CreateTable(
                name: "DangKyCa",
                columns: table => new
                {
                    MaDangKy = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaCa = table.Column<int>(type: "INTEGER", nullable: false),
                    MaNV = table.Column<int>(type: "INTEGER", nullable: false),
                    MaNguoiDung = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    ThoiGianDangKy = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    NguoiDuyet = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    ThoiGianDuyet = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyCa", x => x.MaDangKy);
                    table.ForeignKey(
                        name: "FK_DangKyCa_CaLamViec_MaCa",
                        column: x => x.MaCa,
                        principalTable: "CaLamViec",
                        principalColumn: "MaCa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DangKyCa_NhanVien_MaNV",
                        column: x => x.MaNV,
                        principalTable: "NhanVien",
                        principalColumn: "MaNV",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DangKyCa_MaCa_MaNV",
                table: "DangKyCa",
                columns: new[] { "MaCa", "MaNV" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DangKyCa_MaNV",
                table: "DangKyCa",
                column: "MaNV");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DangKyCa");

            migrationBuilder.DropTable(
                name: "CaLamViec");
        }
    }
}
