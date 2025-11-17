using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamFeedBackPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedbacks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "users",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "teams",
                newName: "teams",
                newSchema: "public");

            migrationBuilder.CreateTable(
                name: "feedbacks",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    is_anonymous = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    reviewed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    review_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedbacks", x => x.id);
                    table.ForeignKey(
                        name: "FK_feedbacks_users_author_id",
                        column: x => x.author_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_feedbacks_users_recipient_id",
                        column: x => x.recipient_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_feedbacks_users_reviewed_by",
                        column: x => x.reviewed_by,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_feedbacks_author_id",
                schema: "public",
                table: "feedbacks",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_feedbacks_created_at",
                schema: "public",
                table: "feedbacks",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_feedbacks_recipient_id",
                schema: "public",
                table: "feedbacks",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_reviewed_by",
                schema: "public",
                table: "feedbacks",
                column: "reviewed_by");

            migrationBuilder.CreateIndex(
                name: "ix_feedbacks_status",
                schema: "public",
                table: "feedbacks",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "feedbacks",
                schema: "public");

            migrationBuilder.RenameTable(
                name: "users",
                schema: "public",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "teams",
                schema: "public",
                newName: "teams");
        }
    }
}
