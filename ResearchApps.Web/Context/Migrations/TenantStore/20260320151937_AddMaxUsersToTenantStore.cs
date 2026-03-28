using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResearchApps.Web.Context.Migrations.TenantStore
{
    /// <inheritdoc />
    public partial class AddMaxUsersToTenantStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF COL_LENGTH('dbo.Tenants', 'MaxUsers') IS NOT NULL
                BEGIN
                    DECLARE @ConstraintName NVARCHAR(128);
                    SELECT @ConstraintName = dc.name
                    FROM sys.default_constraints dc
                    INNER JOIN sys.columns c ON c.default_object_id = dc.object_id
                    INNER JOIN sys.tables t ON t.object_id = c.object_id
                    INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
                    WHERE s.name = 'dbo' AND t.name = 'Tenants' AND c.name = 'MaxUsers';

                    IF @ConstraintName IS NOT NULL
                        EXEC('ALTER TABLE [dbo].[Tenants] DROP CONSTRAINT [' + @ConstraintName + ']');

                    ALTER TABLE [dbo].[Tenants] DROP COLUMN [MaxUsers];
                END

                ALTER TABLE [dbo].[Tenants]
                    ADD [MaxUsers] INT NOT NULL CONSTRAINT [DF_Tenants_MaxUsers] DEFAULT (0);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxUsers",
                schema: "dbo",
                table: "Tenants");
        }
    }
}
