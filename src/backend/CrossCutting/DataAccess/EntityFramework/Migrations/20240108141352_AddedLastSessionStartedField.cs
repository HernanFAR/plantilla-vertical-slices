using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrossCutting.DataAccess.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddedLastSessionStartedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastSessionStarted",
                table: "AspNetUsers",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSessionStarted",
                table: "AspNetUsers");
        }
    }
}
