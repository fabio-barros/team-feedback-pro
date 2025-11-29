using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamFeedBackPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class teamIdOnFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "team_id",
                schema: "public",
                table: "feedbacks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_team_id",
                schema: "public",
                table: "feedbacks",
                column: "team_id");

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_teams_team_id",
                schema: "public",
                table: "feedbacks",
                column: "team_id",
                principalSchema: "public",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_teams_team_id",
                schema: "public",
                table: "feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_feedbacks_team_id",
                schema: "public",
                table: "feedbacks");

            migrationBuilder.DropColumn(
                name: "team_id",
                schema: "public",
                table: "feedbacks");
        }
    }
}
