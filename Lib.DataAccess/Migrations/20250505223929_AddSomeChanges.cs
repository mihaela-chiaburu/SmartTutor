using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTutor.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChapterProgress_AspNetUsers_UserId",
                table: "ChapterProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_ChapterProgress_Chapters_ChapterId",
                table: "ChapterProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_ChapterProgress_UserProgresses_UserProgressId",
                table: "ChapterProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_ChapterProgress_ChapterProgressId",
                table: "QuizResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChapterProgress",
                table: "ChapterProgress");

            migrationBuilder.RenameTable(
                name: "ChapterProgress",
                newName: "ChapterProgresses");

            migrationBuilder.RenameIndex(
                name: "IX_ChapterProgress_UserProgressId",
                table: "ChapterProgresses",
                newName: "IX_ChapterProgresses_UserProgressId");

            migrationBuilder.RenameIndex(
                name: "IX_ChapterProgress_UserId",
                table: "ChapterProgresses",
                newName: "IX_ChapterProgresses_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChapterProgress_ChapterId",
                table: "ChapterProgresses",
                newName: "IX_ChapterProgresses_ChapterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChapterProgresses",
                table: "ChapterProgresses",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 5, 6, 1, 39, 28, 56, DateTimeKind.Local).AddTicks(3392));

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterProgresses_AspNetUsers_UserId",
                table: "ChapterProgresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterProgresses_Chapters_ChapterId",
                table: "ChapterProgresses",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "ChapterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterProgresses_UserProgresses_UserProgressId",
                table: "ChapterProgresses",
                column: "UserProgressId",
                principalTable: "UserProgresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_ChapterProgresses_ChapterProgressId",
                table: "QuizResults",
                column: "ChapterProgressId",
                principalTable: "ChapterProgresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChapterProgresses_AspNetUsers_UserId",
                table: "ChapterProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_ChapterProgresses_Chapters_ChapterId",
                table: "ChapterProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_ChapterProgresses_UserProgresses_UserProgressId",
                table: "ChapterProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_ChapterProgresses_ChapterProgressId",
                table: "QuizResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChapterProgresses",
                table: "ChapterProgresses");

            migrationBuilder.RenameTable(
                name: "ChapterProgresses",
                newName: "ChapterProgress");

            migrationBuilder.RenameIndex(
                name: "IX_ChapterProgresses_UserProgressId",
                table: "ChapterProgress",
                newName: "IX_ChapterProgress_UserProgressId");

            migrationBuilder.RenameIndex(
                name: "IX_ChapterProgresses_UserId",
                table: "ChapterProgress",
                newName: "IX_ChapterProgress_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChapterProgresses_ChapterId",
                table: "ChapterProgress",
                newName: "IX_ChapterProgress_ChapterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChapterProgress",
                table: "ChapterProgress",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 5, 6, 1, 20, 13, 133, DateTimeKind.Local).AddTicks(7215));

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterProgress_AspNetUsers_UserId",
                table: "ChapterProgress",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterProgress_Chapters_ChapterId",
                table: "ChapterProgress",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "ChapterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterProgress_UserProgresses_UserProgressId",
                table: "ChapterProgress",
                column: "UserProgressId",
                principalTable: "UserProgresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_ChapterProgress_ChapterProgressId",
                table: "QuizResults",
                column: "ChapterProgressId",
                principalTable: "ChapterProgress",
                principalColumn: "Id");
        }
    }
}
