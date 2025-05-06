using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PreferredCategoryId = table.Column<int>(type: "int", nullable: true),
                    EmailNotifications = table.Column<bool>(type: "bit", nullable: false),
                    CourseUpdates = table.Column<bool>(type: "bit", nullable: false),
                    QuizResults = table.Column<bool>(type: "bit", nullable: false),
                    ProgressUpdates = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserPreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPreferences_CourseCategory_PreferredCategoryId",
                        column: x => x.PreferredCategoryId,
                        principalTable: "CourseCategory",
                        principalColumn: "CourseCategoryId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_PreferredCategoryId",
                table: "UserPreferences",
                column: "PreferredCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPreferences");
        }
    }
} 