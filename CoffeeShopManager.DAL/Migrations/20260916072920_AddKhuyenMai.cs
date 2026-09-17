using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddKhuyenMai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaKM",
                table: "HoaDon",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TienGiamKhuyenMai",
                table: "HoaDon",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "KhuyenMai",
                columns: table => new
                {
                    MaKM = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenKhuyenMai = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    LoaiKhuyenMai = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    GiaTriGiam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HoaDonToiThieu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaMon = table.Column<int>(type: "INTEGER", nullable: true),
                    LoaiMon = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SoLanSuDungToiDa = table.Column<int>(type: "INTEGER", nullable: true),
                    SoLanDaSuDung = table.Column<int>(type: "INTEGER", nullable: false),
                    DaKichHoat = table.Column<bool>(type: "INTEGER", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MaNguoiTao = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuyenMai", x => x.MaKM);
                    table.ForeignKey(
                        name: "FK_KhuyenMai_Mon_MaMon",
                        column: x => x.MaMon,
                        principalTable: "Mon",
                        principalColumn: "MaMon",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HoaDonKhuyenMai",
                columns: table => new
                {
                    MaHDKM = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaHD = table.Column<int>(type: "INTEGER", nullable: false),
                    MaKM = table.Column<int>(type: "INTEGER", nullable: false),
                    SoTienGiam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThoiGianApDung = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonKhuyenMai", x => x.MaHDKM);
                    table.ForeignKey(
                        name: "FK_HoaDonKhuyenMai_HoaDon_MaHD",
                        column: x => x.MaHD,
                        principalTable: "HoaDon",
                        principalColumn: "MaHD",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDonKhuyenMai_KhuyenMai_MaKM",
                        column: x => x.MaKM,
                        principalTable: "KhuyenMai",
                        principalColumn: "MaKM",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaKM",
                table: "HoaDon",
                column: "MaKM");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonKhuyenMai_MaHD",
                table: "HoaDonKhuyenMai",
                column: "MaHD");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonKhuyenMai_MaKM",
                table: "HoaDonKhuyenMai",
                column: "MaKM");

            migrationBuilder.CreateIndex(
                name: "IX_KhuyenMai_MaMon",
                table: "KhuyenMai",
                column: "MaMon");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_KhuyenMai_MaKM",
                table: "HoaDon",
                column: "MaKM",
                principalTable: "KhuyenMai",
                principalColumn: "MaKM",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_KhuyenMai_MaKM",
                table: "HoaDon");

            migrationBuilder.DropTable(
                name: "HoaDonKhuyenMai");

            migrationBuilder.DropTable(
                name: "KhuyenMai");

            migrationBuilder.DropIndex(
                name: "IX_HoaDon_MaKM",
                table: "HoaDon");

            migrationBuilder.DropColumn(
                name: "MaKM",
                table: "HoaDon");

            migrationBuilder.DropColumn(
                name: "TienGiamKhuyenMai",
                table: "HoaDon");
        }
    }
}
