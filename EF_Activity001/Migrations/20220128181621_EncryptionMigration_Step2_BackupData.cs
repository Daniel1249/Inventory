using Microsoft.EntityFrameworkCore.Migrations;

namespace EF_Activity001.Migrations
{
    public partial class EncryptionMigration_Step2_BackupData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE [HumanResources].[Employee]
                                SET [NationalIDNumberBackup] = [NationalIDNumber]
                                ,[JobTitleBackup] = [JobTitle]
                                ,[BirthDateBackup] = [BirthDate]
                                ,[MaritalStatusBackup] = [MaritalStatus]
                                ,[GenderBackup] = [Gender]
                                ,[HireDateBackup] = [HireDate]"
                                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
