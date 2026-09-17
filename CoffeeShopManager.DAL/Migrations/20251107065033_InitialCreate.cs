using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CoffeeShopManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ban",
                columns: table => new
                {
                    MaBan = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenBan = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ban", x => x.MaBan);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenKH = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DiemTichLuy = table.Column<int>(type: "INTEGER", nullable: false),
                    SoDienThoai = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "Mon",
                columns: table => new
                {
                    MaMon = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenMon = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Loai = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mon", x => x.MaMon);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNV = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenNV = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CaLam = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.MaNV);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHD = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaBan = table.Column<int>(type: "INTEGER", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MaKH = table.Column<int>(type: "INTEGER", nullable: true),
                    MaNV = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon", x => x.MaHD);
                    table.ForeignKey(
                        name: "FK_HoaDon_Ban_MaBan",
                        column: x => x.MaBan,
                        principalTable: "Ban",
                        principalColumn: "MaBan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoaDon_KhachHang_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHang",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HoaDon_NhanVien_MaNV",
                        column: x => x.MaNV,
                        principalTable: "NhanVien",
                        principalColumn: "MaNV",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHD",
                columns: table => new
                {
                    MaChiTiet = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaHD = table.Column<int>(type: "INTEGER", nullable: false),
                    MaMon = table.Column<int>(type: "INTEGER", nullable: false),
                    SoLuong = table.Column<int>(type: "INTEGER", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietHD", x => x.MaChiTiet);
                    table.ForeignKey(
                        name: "FK_ChiTietHD_HoaDon_MaHD",
                        column: x => x.MaHD,
                        principalTable: "HoaDon",
                        principalColumn: "MaHD",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietHD_Mon_MaMon",
                        column: x => x.MaMon,
                        principalTable: "Mon",
                        principalColumn: "MaMon",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Ban",
                columns: new[] { "MaBan", "TenBan", "TrangThai" },
                values: new object[,]
                {
                    { 1, "Bàn 1", "Trống" },
                    { 2, "Bàn 2", "Trống" },
                    { 3, "Bàn 3", "Trống" },
                    { 4, "Bàn 4", "Trống" },
                    { 5, "Bàn 5", "Trống" },
                    { 6, "Bàn VIP", "Trống" }
                });

            migrationBuilder.InsertData(
                table: "KhachHang",
                columns: new[] { "MaKH", "DiemTichLuy", "Email", "SoDienThoai", "TenKH" },
                values: new object[,]
                {
                    { 1, 0, "", "", "Khách lẻ" },
                    { 2, 100, "pvd@gmail.com", "0904567890", "Phạm Văn D" },
                    { 3, 250, "hte@gmail.com", "0905678901", "Hoàng Thị E" }
                });

            migrationBuilder.InsertData(
                table: "Mon",
                columns: new[] { "MaMon", "DonGia", "Loai", "TenMon" },
                values: new object[,]
                {
                    { 1, 25000m, "Cà phê", "Cà phê đen" },
                    { 2, 30000m, "Cà phê", "Cà phê sữa" },
                    { 3, 30000m, "Cà phê", "Bạc xỉu" },
                    { 4, 45000m, "Cà phê", "Cappuccino" },
                    { 5, 35000m, "Trà", "Trà đào cam sả" },
                    { 6, 40000m, "Trà", "Trà sữa trân châu" },
                    { 7, 35000m, "Nước ép", "Nước cam ép" },
                    { 8, 40000m, "Nước ép", "Sinh tố bơ" },
                    { 9, 20000m, "Đồ ăn nhẹ", "Bánh mì" },
                    { 10, 25000m, "Đồ ăn nhẹ", "Bánh ngọt" }
                });

            migrationBuilder.InsertData(
                table: "NhanVien",
                columns: new[] { "MaNV", "CaLam", "Email", "SoDienThoai", "TenNV" },
                values: new object[,]
                {
                    { 1, "Sáng", "nva@coffee.com", "0901234567", "Nguyễn Văn A" },
                    { 2, "Chiều", "ttb@coffee.com", "0902345678", "Trần Thị B" },
                    { 3, "Tối", "lvc@coffee.com", "0903456789", "Lê Văn C" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHD_MaHD",
                table: "ChiTietHD",
                column: "MaHD");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHD_MaMon",
                table: "ChiTietHD",
                column: "MaMon");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaBan",
                table: "HoaDon",
                column: "MaBan");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaKH",
                table: "HoaDon",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaNV",
                table: "HoaDon",
                column: "MaNV");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietHD");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "Mon");

            migrationBuilder.DropTable(
                name: "Ban");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "NhanVien");
        }
    }
}
