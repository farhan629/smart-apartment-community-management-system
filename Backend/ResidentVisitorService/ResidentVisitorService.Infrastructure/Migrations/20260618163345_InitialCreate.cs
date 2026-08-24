using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidentVisitorService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DB_TEAM_C_resident_visitor");

            migrationBuilder.CreateTable(
                name: "ref_sets",
                schema: "DB_TEAM_C_resident_visitor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_ref_sets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ref_terms",
                schema: "DB_TEAM_C_resident_visitor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ref_set_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                        principalSchema: "DB_TEAM_C_resident_visitor",
                        principalTable: "ref_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "visitors",
                schema: "DB_TEAM_C_resident_visitor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    visitor_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    photo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_visitors", x => x.id);
                    table.ForeignKey(
                        name: "f_k_visitors_ref_terms_visitor_type_id",
                        column: x => x.visitor_type_id,
                        principalSchema: "DB_TEAM_C_resident_visitor",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "visits",
                schema: "DB_TEAM_C_resident_visitor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    visitor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    host_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    flat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purpose_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    check_in_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    check_out_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    approved_by = table.Column<Guid>(type: "uuid", nullable: true),
                    approved_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_visits", x => x.id);
                    table.ForeignKey(
                        name: "f_k_visits_ref_terms_purpose_type_id",
                        column: x => x.purpose_type_id,
                        principalSchema: "DB_TEAM_C_resident_visitor",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_visits_ref_terms_status_id",
                        column: x => x.status_id,
                        principalSchema: "DB_TEAM_C_resident_visitor",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_visits_visitors_visitor_id",
                        column: x => x.visitor_id,
                        principalSchema: "DB_TEAM_C_resident_visitor",
                        principalTable: "visitors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "visit_qr_tokens",
                schema: "DB_TEAM_C_resident_visitor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_visit_qr_tokens", x => x.id);
                    table.ForeignKey(
                        name: "f_k_visit_qr_tokens_visits_visit_id",
                        column: x => x.visit_id,
                        principalSchema: "DB_TEAM_C_resident_visitor",
                        principalTable: "visits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_ref_sets_code",
                schema: "DB_TEAM_C_resident_visitor",
                table: "ref_sets",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_ref_terms_ref_set_id_code",
                schema: "DB_TEAM_C_resident_visitor",
                table: "ref_terms",
                columns: new[] { "ref_set_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_visit_qr_tokens_token",
                schema: "DB_TEAM_C_resident_visitor",
                table: "visit_qr_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_visit_qr_tokens_visit_id",
                schema: "DB_TEAM_C_resident_visitor",
                table: "visit_qr_tokens",
                column: "visit_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_visitors_phone_number",
                schema: "DB_TEAM_C_resident_visitor",
                table: "visitors",
                column: "phone_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_visitors_visitor_type_id",
                schema: "DB_TEAM_C_resident_visitor",
                table: "visitors",
                column: "visitor_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_visits_approved_by",
                schema: "DB_TEAM_C_resident_visitor",
                table: "visits",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "i_x_visits_end_date",
                schema: "DB_TEAM_C_resident_visitor",
                table: "visits",
                column: "end_date");

            migrationBuilder.CreateIndex(
                name: "i_x_visits_flat_id",
                schema: "DB_TEAM_C_resident_visitor",
                table: "visits",
                column: "flat_id");

            migrationBuilder.CreateIndex(
                name: "i_x_visits_host_user_id",
                schema: "DB_TEAM_C_resident_visitor",
                table: "visits",
                column: "host_user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_visits_purpose_type_id",
                schema: "DB_TEAM_C_resident_visitor",
                table: "visits",
                column: "purpose_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_visits_start_date",
                schema: "DB_TEAM_C_resident_visitor",
                table: "visits",
                column: "start_date");

            migrationBuilder.CreateIndex(
                name: "i_x_visits_status_id",
                schema: "DB_TEAM_C_resident_visitor",
                table: "visits",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "i_x_visits_visitor_id",
                schema: "DB_TEAM_C_resident_visitor",
                table: "visits",
                column: "visitor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "visit_qr_tokens",
                schema: "DB_TEAM_C_resident_visitor");

            migrationBuilder.DropTable(
                name: "visits",
                schema: "DB_TEAM_C_resident_visitor");

            migrationBuilder.DropTable(
                name: "visitors",
                schema: "DB_TEAM_C_resident_visitor");

            migrationBuilder.DropTable(
                name: "ref_terms",
                schema: "DB_TEAM_C_resident_visitor");

            migrationBuilder.DropTable(
                name: "ref_sets",
                schema: "DB_TEAM_C_resident_visitor");
        }
    }
}
