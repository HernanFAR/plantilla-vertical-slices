using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrossCutting.DataAccess.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPropertyNamesOfAppUserNameFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "AspNetUsers",
                newName: "MotherLastName");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "AspNetUsers",
                newName: "FatherLastName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MotherLastName",
                table: "AspNetUsers",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "FatherLastName",
                table: "AspNetUsers",
                newName: "FirstName");
        }
    }
}
