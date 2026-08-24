using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmenityBookingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "DB_TEAM_C_amenity");

            migrationBuilder.CreateTable(
                name: "ref_sets",
                schema: "DB_TEAM_C_amenity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_ref_sets", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ref_terms",
                schema: "DB_TEAM_C_amenity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ref_set_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    display_name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_ref_terms", x => x.id);
                    table.ForeignKey(
                        name: "f_k_ref_terms_ref_sets_ref_set_id",
                        column: x => x.ref_set_id,
                        principalSchema: "DB_TEAM_C_amenity",
                        principalTable: "ref_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "amenities",
                schema: "DB_TEAM_C_amenity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    slot_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location = table.Column<string>(type: "text", nullable: false),
                    rules = table.Column<string>(type: "text", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_amenities", x => x.id);
                    table.ForeignKey(
                        name: "f_k_amenities__ref_terms_slot_type_id",
                        column: x => x.slot_type_id,
                        principalSchema: "DB_TEAM_C_amenity",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "f_k_amenities__ref_terms_status_id",
                        column: x => x.status_id,
                        principalSchema: "DB_TEAM_C_amenity",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "amenity_slots",
                schema: "DB_TEAM_C_amenity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    amenity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    slot_label = table.Column<string>(type: "text", nullable: false),
                    slot_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    max_capacity = table.Column<int>(type: "integer", nullable: false),
                    current_booking_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_amenity_slots", x => x.id);
                    table.ForeignKey(
                        name: "f_k_amenity_slots_amenities_amenity_id",
                        column: x => x.amenity_id,
                        principalSchema: "DB_TEAM_C_amenity",
                        principalTable: "amenities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "amenity_bookings",
                schema: "DB_TEAM_C_amenity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    amenity_slot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    booking_status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    people_count = table.Column<int>(type: "integer", nullable: false),
                    cancelled_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    cancellation_reason = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_amenity_bookings", x => x.id);
                    table.ForeignKey(
                        name: "f_k_amenity_bookings__amenity_slots_amenity_slot_id",
                        column: x => x.amenity_slot_id,
                        principalSchema: "DB_TEAM_C_amenity",
                        principalTable: "amenity_slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "f_k_amenity_bookings__ref_terms_booking_status_id",
                        column: x => x.booking_status_id,
                        principalSchema: "DB_TEAM_C_amenity",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "i_x_amenities_slot_type_id",
                schema: "DB_TEAM_C_amenity",
                table: "amenities",
                column: "slot_type_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_amenities_status_id",
                schema: "DB_TEAM_C_amenity",
                table: "amenities",
                column: "status_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_amenity_bookings_amenity_slot_id",
                schema: "DB_TEAM_C_amenity",
                table: "amenity_bookings",
                column: "amenity_slot_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_amenity_bookings_booking_status_id",
                schema: "DB_TEAM_C_amenity",
                table: "amenity_bookings",
                column: "booking_status_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_amenity_bookings_user_id_amenity_slot_id",
                schema: "DB_TEAM_C_amenity",
                table: "amenity_bookings",
                columns: new[] { "user_id", "amenity_slot_id" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "i_x_amenity_slots_amenity_id_slot_date_start_time",
                schema: "DB_TEAM_C_amenity",
                table: "amenity_slots",
                columns: new[] { "amenity_id", "slot_date", "start_time" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "i_x_ref_terms_ref_set_id_code",
                schema: "DB_TEAM_C_amenity",
                table: "ref_terms",
                columns: new[] { "ref_set_id", "code" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "amenity_bookings", schema: "DB_TEAM_C_amenity");

            migrationBuilder.DropTable(name: "amenity_slots", schema: "DB_TEAM_C_amenity");

            migrationBuilder.DropTable(name: "amenities", schema: "DB_TEAM_C_amenity");

            migrationBuilder.DropTable(name: "ref_terms", schema: "DB_TEAM_C_amenity");

            migrationBuilder.DropTable(name: "ref_sets", schema: "DB_TEAM_C_amenity");
        }
    }
}
