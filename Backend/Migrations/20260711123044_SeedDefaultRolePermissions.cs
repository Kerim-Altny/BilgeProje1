using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultRolePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CanAccessDashboard", "CanAdd", "CanDelete", "CanEdit" },
                values: new object[] { true, true, true, true });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CanAccessDashboard", "CanAdd", "CanDelete", "CanEdit" },
                values: new object[] { true, true, true, true });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CanAccessDashboard", "CanAdd", "CanEdit" },
                values: new object[] { true, true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CanAccessDashboard", "CanAdd", "CanDelete", "CanEdit" },
                values: new object[] { false, false, false, false });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CanAccessDashboard", "CanAdd", "CanDelete", "CanEdit" },
                values: new object[] { false, false, false, false });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CanAccessDashboard", "CanAdd", "CanEdit" },
                values: new object[] { false, false, false });
        }
    }
}
