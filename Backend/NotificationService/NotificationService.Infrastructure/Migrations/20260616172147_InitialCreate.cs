using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DB_TEAM_C_notification");

            migrationBuilder.CreateTable(
                name: "email_templates",
                schema: "DB_TEAM_C_notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    body_template = table.Column<string>(type: "text", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_email_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_templates",
                schema: "DB_TEAM_C_notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    notification_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    message_template = table.Column<string>(type: "text", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_notification_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ref_sets",
                schema: "DB_TEAM_C_notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_ref_sets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "email_logs",
                schema: "DB_TEAM_C_notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email_address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    error_message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_email_logs", x => x.id);
                    table.ForeignKey(
                        name: "f_k_email_logs_email_templates_template_id",
                        column: x => x.template_id,
                        principalSchema: "DB_TEAM_C_notification",
                        principalTable: "email_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                schema: "DB_TEAM_C_notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    visit_id = table.Column<Guid>(type: "uuid", nullable: true),
                    complaint_id = table.Column<Guid>(type: "uuid", nullable: true),
                    amenity_booking_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    notification_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    scheduled_for = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_reminder_sent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_notifications", x => x.id);
                    table.ForeignKey(
                        name: "f_k_notifications_notification_templates_template_id",
                        column: x => x.template_id,
                        principalSchema: "DB_TEAM_C_notification",
                        principalTable: "notification_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ref_terms",
                schema: "DB_TEAM_C_notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ref_set_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_ref_terms", x => x.id);
                    table.ForeignKey(
                        name: "f_k_ref_terms_ref_sets_ref_set_id",
                        column: x => x.ref_set_id,
                        principalSchema: "DB_TEAM_C_notification",
                        principalTable: "ref_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_email_logs_email_address",
                schema: "DB_TEAM_C_notification",
                table: "email_logs",
                column: "email_address");

            migrationBuilder.CreateIndex(
                name: "i_x_email_logs_sent_at",
                schema: "DB_TEAM_C_notification",
                table: "email_logs",
                column: "sent_at");

            migrationBuilder.CreateIndex(
                name: "i_x_email_logs_status",
                schema: "DB_TEAM_C_notification",
                table: "email_logs",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "i_x_email_logs_template_id",
                schema: "DB_TEAM_C_notification",
                table: "email_logs",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "i_x_email_logs_user_id",
                schema: "DB_TEAM_C_notification",
                table: "email_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_email_templates_email_type",
                schema: "DB_TEAM_C_notification",
                table: "email_templates",
                column: "email_type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_notification_templates_notification_type",
                schema: "DB_TEAM_C_notification",
                table: "notification_templates",
                column: "notification_type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_amenity_booking_id",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                column: "amenity_booking_id");

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_complaint_id",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                column: "complaint_id");

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_is_read",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                column: "is_read");

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_notification_type",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                column: "notification_type");

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_scheduled_for",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                column: "scheduled_for");

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_sent_at",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                column: "sent_at");

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_status",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_template_id",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_user_id",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_visit_id",
                schema: "DB_TEAM_C_notification",
                table: "notifications",
                column: "visit_id");

            migrationBuilder.CreateIndex(
                name: "i_x_ref_sets_code",
                schema: "DB_TEAM_C_notification",
                table: "ref_sets",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_ref_terms_ref_set_id_code",
                schema: "DB_TEAM_C_notification",
                table: "ref_terms",
                columns: new[] { "ref_set_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "email_logs",
                schema: "DB_TEAM_C_notification");

            migrationBuilder.DropTable(
                name: "notifications",
                schema: "DB_TEAM_C_notification");

            migrationBuilder.DropTable(
                name: "ref_terms",
                schema: "DB_TEAM_C_notification");

            migrationBuilder.DropTable(
                name: "email_templates",
                schema: "DB_TEAM_C_notification");

            migrationBuilder.DropTable(
                name: "notification_templates",
                schema: "DB_TEAM_C_notification");

            migrationBuilder.DropTable(
                name: "ref_sets",
                schema: "DB_TEAM_C_notification");
        }
    }
}
