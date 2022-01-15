using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace APP.Auth.Model.Migrations
{
    public partial class updatedDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PacketEndTime",
                schema: "auth",
                table: "Packets");

            migrationBuilder.DropColumn(
                name: "PacketStartTime",
                schema: "auth",
                table: "Packets");

            migrationBuilder.RenameColumn(
                name: "Cost",
                schema: "auth",
                table: "Packets",
                newName: "Price");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                schema: "auth",
                table: "Packets",
                newName: "Cost");

            migrationBuilder.AddColumn<DateTime>(
                name: "PacketEndTime",
                schema: "auth",
                table: "Packets",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PacketStartTime",
                schema: "auth",
                table: "Packets",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
