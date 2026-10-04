using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPerUnitChargeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM ""GlobalData"".""ChargeType"" 
                        WHERE ""Code"" = 'PER_UNIT' OR ""Code"" = 'PERUNIT' OR ""Code"" = 'UNIT' OR ""Name"" ILIKE '%per unit%'
                    ) THEN
                        INSERT INTO ""GlobalData"".""ChargeType"" (""Name"", ""Code"", ""IsActive"")
                        VALUES ('Per Unit', 'PER_UNIT', true);
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ""GlobalData"".""ChargeType"" WHERE ""Code"" = 'PER_UNIT';
            ");
        }
    }
}
