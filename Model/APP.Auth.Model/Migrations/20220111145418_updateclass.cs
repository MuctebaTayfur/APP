using Microsoft.EntityFrameworkCore.Migrations;

namespace APP.Auth.Model.Migrations
{
    public partial class updateclass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizedFolders",
                schema: "auth",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorizedFolders",
                schema: "auth",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }
    }
}
