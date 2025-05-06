using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTutor.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEnrollmentProgressRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, create a temporary table to store the data
            migrationBuilder.CreateTable(
                name: "TempUserProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    ProgressPercentage = table.Column<double>(type: "float", nullable: false),
                    LastAccessed = table.Column<DateTime>(type: "datetime2", nullable: false)
                });

            // Copy data to temporary table
            migrationBuilder.Sql("INSERT INTO TempUserProgresses SELECT Id, UserId, CourseId, ProgressPercentage, LastAccessed FROM UserProgresses");

            // Drop existing foreign key constraints
            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_AspNetUsers_UserId",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProgresses_AspNetUsers_UserId",
                table: "UserProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProgresses_CourseEnrollments_Id",
                table: "UserProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProgresses_Courses_CourseId",
                table: "UserProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_ChapterProgresses_UserProgresses_UserProgressId",
                table: "ChapterProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_UserProgresses_UserProgressId",
                table: "QuizResults");

            // Drop the existing table
            migrationBuilder.DropTable(name: "UserProgresses");

            // Recreate the table with IDENTITY
            migrationBuilder.CreateTable(
                name: "UserProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    ProgressPercentage = table.Column<double>(type: "float", nullable: false),
                    LastAccessed = table.Column<DateTime>(type: "datetime2", nullable: false)
                });

            // Copy data back from temporary table
            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT UserProgresses ON;
                INSERT INTO UserProgresses (Id, UserId, CourseId, ProgressPercentage, LastAccessed)
                SELECT Id, UserId, CourseId, ProgressPercentage, LastAccessed FROM TempUserProgresses;
                SET IDENTITY_INSERT UserProgresses OFF;
            ");

            // Drop temporary table
            migrationBuilder.DropTable(name: "TempUserProgresses");

            // Add ProgressId column to CourseEnrollments
            migrationBuilder.AddColumn<int>(
                name: "ProgressId",
                table: "CourseEnrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Add primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProgresses",
                table: "UserProgresses",
                column: "Id");

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_UserProgresses_CourseId",
                table: "UserProgresses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgresses_UserId",
                table: "UserProgresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_ProgressId",
                table: "CourseEnrollments",
                column: "ProgressId",
                unique: true);

            // Add foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_AspNetUsers_UserId",
                table: "CourseEnrollments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                table: "CourseEnrollments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_UserProgresses_ProgressId",
                table: "CourseEnrollments",
                column: "ProgressId",
                principalTable: "UserProgresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProgresses_AspNetUsers_UserId",
                table: "UserProgresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProgresses_Courses_CourseId",
                table: "UserProgresses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterProgresses_UserProgresses_UserProgressId",
                table: "ChapterProgresses",
                column: "UserProgressId",
                principalTable: "UserProgresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_UserProgresses_UserProgressId",
                table: "QuizResults",
                column: "UserProgressId",
                principalTable: "UserProgresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Create temporary table
            migrationBuilder.CreateTable(
                name: "TempUserProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    ProgressPercentage = table.Column<double>(type: "float", nullable: false),
                    LastAccessed = table.Column<DateTime>(type: "datetime2", nullable: false)
                });

            // Copy data to temporary table
            migrationBuilder.Sql("INSERT INTO TempUserProgresses SELECT Id, UserId, CourseId, ProgressPercentage, LastAccessed FROM UserProgresses");

            // Drop existing foreign key constraints
            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_AspNetUsers_UserId",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_UserProgresses_ProgressId",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProgresses_AspNetUsers_UserId",
                table: "UserProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProgresses_Courses_CourseId",
                table: "UserProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_ChapterProgresses_UserProgresses_UserProgressId",
                table: "ChapterProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_UserProgresses_UserProgressId",
                table: "QuizResults");

            // Drop the existing table
            migrationBuilder.DropTable(name: "UserProgresses");

            // Recreate the table without IDENTITY
            migrationBuilder.CreateTable(
                name: "UserProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    ProgressPercentage = table.Column<double>(type: "float", nullable: false),
                    LastAccessed = table.Column<DateTime>(type: "datetime2", nullable: false)
                });

            // Copy data back from temporary table
            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT UserProgresses ON;
                INSERT INTO UserProgresses (Id, UserId, CourseId, ProgressPercentage, LastAccessed)
                SELECT Id, UserId, CourseId, ProgressPercentage, LastAccessed FROM TempUserProgresses;
                SET IDENTITY_INSERT UserProgresses OFF;
            ");

            // Drop temporary table
            migrationBuilder.DropTable(name: "TempUserProgresses");

            // Drop ProgressId column from CourseEnrollments
            migrationBuilder.DropColumn(
                name: "ProgressId",
                table: "CourseEnrollments");

            // Add primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProgresses",
                table: "UserProgresses",
                column: "Id");

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_UserProgresses_CourseId",
                table: "UserProgresses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgresses_UserId",
                table: "UserProgresses",
                column: "UserId");

            // Add foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_AspNetUsers_UserId",
                table: "CourseEnrollments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                table: "CourseEnrollments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProgresses_AspNetUsers_UserId",
                table: "UserProgresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProgresses_CourseEnrollments_Id",
                table: "UserProgresses",
                column: "Id",
                principalTable: "CourseEnrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProgresses_Courses_CourseId",
                table: "UserProgresses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterProgresses_UserProgresses_UserProgressId",
                table: "ChapterProgresses",
                column: "UserProgressId",
                principalTable: "UserProgresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_UserProgresses_UserProgressId",
                table: "QuizResults",
                column: "UserProgressId",
                principalTable: "UserProgresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
} 