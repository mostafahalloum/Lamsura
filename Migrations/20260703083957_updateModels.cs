using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lamsura.Migrations
{
    
    / <inheritdoc />
    public partial class updateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MediaUrl",
                table: "Projects",
                newName: "app_VedioUrl");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Projects",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VedioUrl",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "app_Technologies",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "VedioUrl",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "app_Technologies",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "app_VedioUrl",
                table: "Projects",
                newName: "MediaUrl");
        }
    }
}
