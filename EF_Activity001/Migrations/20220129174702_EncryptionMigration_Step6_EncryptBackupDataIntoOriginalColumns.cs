using Microsoft.EntityFrameworkCore.Migrations;

namespace EF_Activity001.Migrations
{
    public partial class EncryptionMigration_Step6_EncryptBackupDataIntoOriginalColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"OPEN SYMMETRIC KEY AW_ColumnKey
                                    DECRYPTION BY CERTIFICATE AW_tdeCert;
                                    UPDATE [HumanResources].[Employee]
                                    SET [NationalIDNumber] = encryptByKey(Key_GUID('AW_ColumnKey'),
                                    CONVERT(varbinary(max), [NationalIDNumberBackup]))
                                    ,[JobTitle] = encryptByKey(Key_GUID('AW_ColumnKey'),
                                    CONVERT(varbinary(max), [JobTitleBackup]))
                                    ,[BirthDate] = encryptByKey(Key_GUID('AW_ColumnKey'),
                                    CONVERT(varbinary(max), [BirthDateBackup]))
                                    ,[MaritalStatus] = encryptByKey(Key_GUID('AW_ColumnKey'),
                                    CONVERT(varbinary(max), [MaritalStatusBackup]))
                                    ,[Gender] = encryptByKey(Key_GUID('AW_ColumnKey'),
                                    CONVERT(varbinary(max), [GenderBackup]))
                                    ,[HireDate] = encryptByKey(Key_GUID('AW_ColumnKey'),
                                    CONVERT(varbinary(max), [HireDateBackup]))
                                    CLOSE ALL SYMMETRIC KEYS; "
                                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
