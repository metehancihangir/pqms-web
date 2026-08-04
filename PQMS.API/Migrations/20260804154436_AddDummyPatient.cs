using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PQMS.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDummyPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "CreatedAt", "DateOfBirth", "FullName", "PhoneNumber" },
                values: new object[] { 1, new DateTime(2026, 8, 4, 15, 44, 36, 220, DateTimeKind.Utc).AddTicks(272), new DateTime(2000, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "James Kamanga", "0888876600" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
