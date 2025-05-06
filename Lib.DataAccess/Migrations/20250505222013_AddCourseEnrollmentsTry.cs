using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTutor.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseEnrollmentsTry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 5, 6, 1, 20, 13, 133, DateTimeKind.Local).AddTicks(7215));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 5, 6, 1, 7, 3, 996, DateTimeKind.Local).AddTicks(3765));
        }
    }
}
