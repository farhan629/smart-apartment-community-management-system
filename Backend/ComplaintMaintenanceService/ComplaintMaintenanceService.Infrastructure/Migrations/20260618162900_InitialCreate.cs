using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplaintMaintenanceService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DB_TEAM_C_complaint");

            migrationBuilder.CreateTable(
                name: "categories",
                schema: "DB_TEAM_C_complaint",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    img = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ref_sets",
                schema: "DB_TEAM_C_complaint",
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
                name: "staff",
                schema: "DB_TEAM_C_complaint",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    details = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_staff", x => x.id);
                    table.ForeignKey(
                        name: "f_k_staff_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ref_terms",
                schema: "DB_TEAM_C_complaint",
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
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "ref_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "auto_assignment_rules",
                schema: "DB_TEAM_C_complaint",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority_id = table.Column<Guid>(type: "uuid", nullable: false),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fallback_staff_id = table.Column<Guid>(type: "uuid", nullable: true),
                    service_duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    allow_resident_time_pick = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    time_window_start = table.Column<TimeSpan>(type: "interval", nullable: true),
                    time_window_end = table.Column<TimeSpan>(type: "interval", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_auto_assignment_rules", x => x.id);
                    table.ForeignKey(
                        name: "f_k_auto_assignment_rules_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_auto_assignment_rules_ref_terms_priority_id",
                        column: x => x.priority_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_auto_assignment_rules_staff_fallback_staff_id",
                        column: x => x.fallback_staff_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "f_k_auto_assignment_rules_staff_staff_id",
                        column: x => x.staff_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "complaint_assignments",
                schema: "DB_TEAM_C_complaint",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    complaint_id = table.Column<Guid>(type: "uuid", nullable: false),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_by = table.Column<Guid>(type: "uuid", nullable: false),
                    status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    accepted_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    denied_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    denial_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_complaint_assignments", x => x.id);
                    table.ForeignKey(
                        name: "f_k_complaint_assignments_ref_terms_status_id",
                        column: x => x.status_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_complaint_assignments_staff_staff_id",
                        column: x => x.staff_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "complaint_comments",
                schema: "DB_TEAM_C_complaint",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    complaint_id = table.Column<Guid>(type: "uuid", nullable: false),
                    commented_by = table.Column<Guid>(type: "uuid", nullable: false),
                    comment_text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    staff_rating = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_complaint_comments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "complaint_escalations",
                schema: "DB_TEAM_C_complaint",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    complaint_id = table.Column<Guid>(type: "uuid", nullable: false),
                    escalated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    escalated_to = table.Column<Guid>(type: "uuid", nullable: false),
                    escalation_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    escalation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resolved_after_escalation = table.Column<bool>(type: "boolean", nullable: false),
                    resolution_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_complaint_escalations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "complaint_progress_logs",
                schema: "DB_TEAM_C_complaint",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    complaint_id = table.Column<Guid>(type: "uuid", nullable: false),
                    changed_by = table.Column<Guid>(type: "uuid", nullable: false),
                    status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    changed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_complaint_progress_logs", x => x.id);
                    table.ForeignKey(
                        name: "f_k_complaint_progress_logs_ref_terms_status_id",
                        column: x => x.status_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "complaints",
                schema: "DB_TEAM_C_complaint",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    resident_id = table.Column<Guid>(type: "uuid", nullable: false),
                    complaint_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scheduled_slot_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    scheduled_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    scheduled_time = table.Column<TimeSpan>(type: "interval", nullable: true),
                    cancelled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_complaints", x => x.id);
                    table.ForeignKey(
                        name: "f_k_complaints_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_complaints_ref_terms_complaint_type_id",
                        column: x => x.complaint_type_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_complaints_ref_terms_priority_id",
                        column: x => x.priority_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_complaints_ref_terms_status_id",
                        column: x => x.status_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "staff_availabilities",
                schema: "DB_TEAM_C_complaint",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    complaint_id = table.Column<Guid>(type: "uuid", nullable: true),
                    available_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    slot_start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    slot_end_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    is_booked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_cancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_staff_availabilities", x => x.id);
                    table.ForeignKey(
                        name: "f_k_staff_availabilities_complaints_complaint_id",
                        column: x => x.complaint_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "complaints",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "f_k_staff_availabilities_staff_staff_id",
                        column: x => x.staff_id,
                        principalSchema: "DB_TEAM_C_complaint",
                        principalTable: "staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_auto_assignment_rules_category_id_priority_id",
                schema: "DB_TEAM_C_complaint",
                table: "auto_assignment_rules",
                columns: new[] { "category_id", "priority_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_auto_assignment_rules_fallback_staff_id",
                schema: "DB_TEAM_C_complaint",
                table: "auto_assignment_rules",
                column: "fallback_staff_id");

            migrationBuilder.CreateIndex(
                name: "i_x_auto_assignment_rules_priority_id",
                schema: "DB_TEAM_C_complaint",
                table: "auto_assignment_rules",
                column: "priority_id");

            migrationBuilder.CreateIndex(
                name: "i_x_auto_assignment_rules_staff_id",
                schema: "DB_TEAM_C_complaint",
                table: "auto_assignment_rules",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "i_x_categories_name",
                schema: "DB_TEAM_C_complaint",
                table: "categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_complaint_assignments_complaint_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaint_assignments",
                column: "complaint_id");

            migrationBuilder.CreateIndex(
                name: "i_x_complaint_assignments_staff_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaint_assignments",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "i_x_complaint_assignments_status_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaint_assignments",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "i_x_complaint_comments_complaint_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaint_comments",
                column: "complaint_id");

            migrationBuilder.CreateIndex(
                name: "i_x_complaint_escalations_complaint_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaint_escalations",
                column: "complaint_id");

            migrationBuilder.CreateIndex(
                name: "i_x_complaint_progress_logs_complaint_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaint_progress_logs",
                column: "complaint_id");

            migrationBuilder.CreateIndex(
                name: "i_x_complaint_progress_logs_status_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaint_progress_logs",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "i_x_complaints_category_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaints",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "i_x_complaints_complaint_type_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaints",
                column: "complaint_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_complaints_priority_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaints",
                column: "priority_id");

            migrationBuilder.CreateIndex(
                name: "i_x_complaints_scheduled_slot_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaints",
                column: "scheduled_slot_id");

            migrationBuilder.CreateIndex(
                name: "i_x_complaints_status_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaints",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "i_x_ref_sets_code",
                schema: "DB_TEAM_C_complaint",
                table: "ref_sets",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_ref_terms_ref_set_id_code",
                schema: "DB_TEAM_C_complaint",
                table: "ref_terms",
                columns: new[] { "ref_set_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_staff_category_id",
                schema: "DB_TEAM_C_complaint",
                table: "staff",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "i_x_staff_user_id",
                schema: "DB_TEAM_C_complaint",
                table: "staff",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_staff_availabilities_complaint_id",
                schema: "DB_TEAM_C_complaint",
                table: "staff_availabilities",
                column: "complaint_id");

            migrationBuilder.CreateIndex(
                name: "i_x_staff_availabilities_staff_id",
                schema: "DB_TEAM_C_complaint",
                table: "staff_availabilities",
                column: "staff_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_complaint_assignments_complaints_complaint_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaint_assignments",
                column: "complaint_id",
                principalSchema: "DB_TEAM_C_complaint",
                principalTable: "complaints",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_complaint_comments_complaints_complaint_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaint_comments",
                column: "complaint_id",
                principalSchema: "DB_TEAM_C_complaint",
                principalTable: "complaints",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_complaint_escalations_complaints_complaint_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaint_escalations",
                column: "complaint_id",
                principalSchema: "DB_TEAM_C_complaint",
                principalTable: "complaints",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_complaint_progress_logs_complaints_complaint_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaint_progress_logs",
                column: "complaint_id",
                principalSchema: "DB_TEAM_C_complaint",
                principalTable: "complaints",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_complaints_staff_availabilities_scheduled_slot_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaints",
                column: "scheduled_slot_id",
                principalSchema: "DB_TEAM_C_complaint",
                principalTable: "staff_availabilities",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_complaints_categories_category_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaints");

            migrationBuilder.DropForeignKey(
                name: "f_k_staff_categories_category_id",
                schema: "DB_TEAM_C_complaint",
                table: "staff");

            migrationBuilder.DropForeignKey(
                name: "f_k_complaints_ref_terms_complaint_type_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaints");

            migrationBuilder.DropForeignKey(
                name: "f_k_complaints_ref_terms_priority_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaints");

            migrationBuilder.DropForeignKey(
                name: "f_k_complaints_ref_terms_status_id",
                schema: "DB_TEAM_C_complaint",
                table: "complaints");

            migrationBuilder.DropForeignKey(
                name: "f_k_staff_availabilities_staff_staff_id",
                schema: "DB_TEAM_C_complaint",
                table: "staff_availabilities");

            migrationBuilder.DropForeignKey(
                name: "f_k_staff_availabilities_complaints_complaint_id",
                schema: "DB_TEAM_C_complaint",
                table: "staff_availabilities");

            migrationBuilder.DropTable(
                name: "auto_assignment_rules",
                schema: "DB_TEAM_C_complaint");

            migrationBuilder.DropTable(
                name: "complaint_assignments",
                schema: "DB_TEAM_C_complaint");

            migrationBuilder.DropTable(
                name: "complaint_comments",
                schema: "DB_TEAM_C_complaint");

            migrationBuilder.DropTable(
                name: "complaint_escalations",
                schema: "DB_TEAM_C_complaint");

            migrationBuilder.DropTable(
                name: "complaint_progress_logs",
                schema: "DB_TEAM_C_complaint");

            migrationBuilder.DropTable(
                name: "categories",
                schema: "DB_TEAM_C_complaint");

            migrationBuilder.DropTable(
                name: "ref_terms",
                schema: "DB_TEAM_C_complaint");

            migrationBuilder.DropTable(
                name: "ref_sets",
                schema: "DB_TEAM_C_complaint");

            migrationBuilder.DropTable(
                name: "staff",
                schema: "DB_TEAM_C_complaint");

            migrationBuilder.DropTable(
                name: "complaints",
                schema: "DB_TEAM_C_complaint");

            migrationBuilder.DropTable(
                name: "staff_availabilities",
                schema: "DB_TEAM_C_complaint");
        }
    }
}
