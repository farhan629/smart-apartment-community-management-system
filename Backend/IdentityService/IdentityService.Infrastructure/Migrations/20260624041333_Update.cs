using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DB_TEAM_C_identity");

            migrationBuilder.CreateTable(
                name: "flats",
                schema: "DB_TEAM_C_identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    block = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    floor = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_flats", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ref_sets",
                schema: "DB_TEAM_C_identity",
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
                schema: "DB_TEAM_C_identity",
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
                        principalSchema: "DB_TEAM_C_identity",
                        principalTable: "ref_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_policies",
                schema: "DB_TEAM_C_identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_allowed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_role_policies", x => x.id);
                    table.ForeignKey(
                        name: "f_k_role_policies_ref_terms_role_id",
                        column: x => x.role_id,
                        principalSchema: "DB_TEAM_C_identity",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "DB_TEAM_C_identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone_no = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    photo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                    table.ForeignKey(
                        name: "f_k_users_ref_terms_role_id",
                        column: x => x.role_id,
                        principalSchema: "DB_TEAM_C_identity",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "flat_occupancies",
                schema: "DB_TEAM_C_identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    flat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    resident_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_approved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_flat_occupancies", x => x.id);
                    table.ForeignKey(
                        name: "f_k_flat_occupancies_flats_flat_id",
                        column: x => x.flat_id,
                        principalSchema: "DB_TEAM_C_identity",
                        principalTable: "flats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_flat_occupancies_ref_terms_resident_type_id",
                        column: x => x.resident_type_id,
                        principalSchema: "DB_TEAM_C_identity",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_flat_occupancies_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "DB_TEAM_C_identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                schema: "DB_TEAM_C_identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "f_k_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "DB_TEAM_C_identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_password_securities",
                schema: "DB_TEAM_C_identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_password_securities", x => x.id);
                    table.ForeignKey(
                        name: "f_k_user_password_securities_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "DB_TEAM_C_identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_policies",
                schema: "DB_TEAM_C_identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_allowed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_policies", x => x.id);
                    table.ForeignKey(
                        name: "f_k_user_policies_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "DB_TEAM_C_identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_flat_occupancies_flat_id",
                schema: "DB_TEAM_C_identity",
                table: "flat_occupancies",
                column: "flat_id");

            migrationBuilder.CreateIndex(
                name: "i_x_flat_occupancies_resident_type_id",
                schema: "DB_TEAM_C_identity",
                table: "flat_occupancies",
                column: "resident_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_flat_occupancies_user_id_flat_id",
                schema: "DB_TEAM_C_identity",
                table: "flat_occupancies",
                columns: new[] { "user_id", "flat_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_flats_block_number",
                schema: "DB_TEAM_C_identity",
                table: "flats",
                columns: new[] { "block", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_ref_sets_code",
                schema: "DB_TEAM_C_identity",
                table: "ref_sets",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_ref_terms_ref_set_id_code",
                schema: "DB_TEAM_C_identity",
                table: "ref_terms",
                columns: new[] { "ref_set_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_refresh_tokens_user_id",
                schema: "DB_TEAM_C_identity",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_role_policies_role_id_permission_code",
                schema: "DB_TEAM_C_identity",
                table: "role_policies",
                columns: new[] { "role_id", "permission_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_user_password_securities_user_id",
                schema: "DB_TEAM_C_identity",
                table: "user_password_securities",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_user_policies_user_id_permission_code",
                schema: "DB_TEAM_C_identity",
                table: "user_policies",
                columns: new[] { "user_id", "permission_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_users_email",
                schema: "DB_TEAM_C_identity",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_users_phone_no",
                schema: "DB_TEAM_C_identity",
                table: "users",
                column: "phone_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_users_role_id",
                schema: "DB_TEAM_C_identity",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "flat_occupancies",
                schema: "DB_TEAM_C_identity");

            migrationBuilder.DropTable(
                name: "refresh_tokens",
                schema: "DB_TEAM_C_identity");

            migrationBuilder.DropTable(
                name: "role_policies",
                schema: "DB_TEAM_C_identity");

            migrationBuilder.DropTable(
                name: "user_password_securities",
                schema: "DB_TEAM_C_identity");

            migrationBuilder.DropTable(
                name: "user_policies",
                schema: "DB_TEAM_C_identity");

            migrationBuilder.DropTable(
                name: "flats",
                schema: "DB_TEAM_C_identity");

            migrationBuilder.DropTable(
                name: "users",
                schema: "DB_TEAM_C_identity");

            migrationBuilder.DropTable(
                name: "ref_terms",
                schema: "DB_TEAM_C_identity");

            migrationBuilder.DropTable(
                name: "ref_sets",
                schema: "DB_TEAM_C_identity");
        }
    }
}
