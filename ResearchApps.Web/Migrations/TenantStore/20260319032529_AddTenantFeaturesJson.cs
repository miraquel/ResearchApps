using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResearchApps.Web.Migrations.TenantStore
{
    /// <inheritdoc />
    public partial class AddTenantFeaturesJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeaturesJson",
                schema: "dbo",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeaturesJson",
                schema: "dbo",
                table: "Tenants");
        }
    }
}
