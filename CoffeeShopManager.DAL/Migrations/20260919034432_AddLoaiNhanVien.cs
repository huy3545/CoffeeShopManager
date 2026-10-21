using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddLoaiNhanVien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoaiNhanVien",
                table: "NhanVien",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "Part-time");

            migrationBuilder.UpdateData(
                table: "NhanVien",
                keyColumn: "MaNV",
                keyValue: 1,
                column: "LoaiNhanVien",
                value: "Part-time");

            migrationBuilder.UpdateData(
                table: "NhanVien",
                keyColumn: "MaNV",
                keyValue: 2,
                column: "LoaiNhanVien",
                value: "Part-time");

            migrationBuilder.UpdateData(
                table: "NhanVien",
                keyColumn: "MaNV",
                keyValue: 3,
                column: "LoaiNhanVien",
                value: "Part-time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiNhanVien",
                table: "NhanVien");
        }
    }
}
