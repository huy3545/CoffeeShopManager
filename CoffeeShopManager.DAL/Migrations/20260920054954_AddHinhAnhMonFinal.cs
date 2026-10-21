using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddHinhAnhMonFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "Mon",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "Mon");
        }
    }
}
