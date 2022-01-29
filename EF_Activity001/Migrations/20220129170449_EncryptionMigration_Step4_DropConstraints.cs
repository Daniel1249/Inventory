using Microsoft.EntityFrameworkCore.Migrations;

namespace EF_Activity001.Migrations
{
    public partial class EncryptionMigration_Step4_DropConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                            migrationBuilder.Sql(@"ALTER TABLE[HumanResources].[Employee]
                DROP CONSTRAINT[CK_Employee_MaritalStatus]");
                            migrationBuilder.Sql(@"ALTER TABLE[HumanResources].[Employee]
                DROP CONSTRAINT[CK_Employee_HireDate]");
                            migrationBuilder.Sql(@"ALTER TABLE[HumanResources].[Employee]
                DROP CONSTRAINT[CK_Employee_Gender]");
                            migrationBuilder.Sql(@"ALTER TABLE[HumanResources].[Employee]
                DROP CONSTRAINT[CK_Employee_BirthDate]");
                            migrationBuilder.DropIndex(
                            name: "AK_Employee_NationalIDNumber",
                            schema: "HumanResources",
                            table: "Employee");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
