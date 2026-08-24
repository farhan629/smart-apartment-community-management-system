using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReconcileModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "date_updated",
                schema: "DB_TEAM_C_notification",
                table: "ref_terms",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "date_created",
                schema: "DB_TEAM_C_notification",
                table: "ref_terms",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "date_updated",
                schema: "DB_TEAM_C_notification",
                table: "ref_sets",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "date_created",
                schema: "DB_TEAM_C_notification",
                table: "ref_sets",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "date_updated",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "date_created",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "date_updated",
                schema: "DB_TEAM_C_notification",
                table: "notification_templates",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "date_created",
                schema: "DB_TEAM_C_notification",
                table: "notification_templates",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "date_updated",
                schema: "DB_TEAM_C_notification",
                table: "email_templates",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "date_created",
                schema: "DB_TEAM_C_notification",
                table: "email_templates",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "date_updated",
                schema: "DB_TEAM_C_notification",
                table: "email_logs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "date_created",
                schema: "DB_TEAM_C_notification",
                table: "email_logs",
                newName: "created_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "DB_TEAM_C_notification",
                table: "ref_terms",
                newName: "date_updated");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "DB_TEAM_C_notification",
                table: "ref_terms",
                newName: "date_created");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "DB_TEAM_C_notification",
                table: "ref_sets",
                newName: "date_updated");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "DB_TEAM_C_notification",
                table: "ref_sets",
                newName: "date_created");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                newName: "date_updated");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                newName: "date_created");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "DB_TEAM_C_notification",
                table: "notification_templates",
                newName: "date_updated");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "DB_TEAM_C_notification",
                table: "notification_templates",
                newName: "date_created");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "DB_TEAM_C_notification",
                table: "email_templates",
                newName: "date_updated");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "DB_TEAM_C_notification",
                table: "email_templates",
                newName: "date_created");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "DB_TEAM_C_notification",
                table: "email_logs",
                newName: "date_updated");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "DB_TEAM_C_notification",
                table: "email_logs",
                newName: "date_created");
        }
    }
}
