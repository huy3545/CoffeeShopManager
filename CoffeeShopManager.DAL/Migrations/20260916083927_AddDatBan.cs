using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDatBan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DatBan",
                columns: table => new
                {
                    MaDatBan = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaBan = table.Column<int>(type: "INTEGER", nullable: false),
                    MaKH = table.Column<int>(type: "INTEGER", nullable: false),
                    TenKhachHang = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SoDienThoai = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    ThoiGianBatDau = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SoLuongKhach = table.Column<int>(type: "INTEGER", nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NguoiTao = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatBan", x => x.MaDatBan);
                    table.ForeignKey(
                        name: "FK_DatBan_Ban_MaBan",
                        column: x => x.MaBan,
                        principalTable: "Ban",
                        principalColumn: "MaBan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DatBan_KhachHang_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHang",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatBan_MaBan",
                table: "DatBan",
                column: "MaBan");

            migrationBuilder.CreateIndex(
                name: "IX_DatBan_MaKH",
                table: "DatBan",
                column: "MaKH");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatBan");
        }
    }
}
