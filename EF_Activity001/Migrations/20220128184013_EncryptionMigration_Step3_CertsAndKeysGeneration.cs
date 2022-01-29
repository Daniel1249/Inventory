using Microsoft.EntityFrameworkCore.Migrations;

namespace EF_Activity001.Migrations
{
    public partial class EncryptionMigration_Step3_CertsAndKeysGeneration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT *
                                FROM sys.symmetric_keys WHERE symmetric_key_id = 101)
                                BEGIN
                                CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'Password#123'
                                END");
                                            migrationBuilder.Sql(@"CREATE CERTIFICATE AW_tdeCert
                                WITH SUBJECT = 'AdventureWorks TDE Certificate'");
                                            migrationBuilder.Sql(@"BACKUP CERTIFICATE AW_tdeCert TO
                                FILE = 'C:\SourceCode\Data\DatabaseKeys\AW_tdeCert.crt'
                                WITH PRIVATE KEY
                                (
                                FILE = 'C:\SourceCode\Data\DatabaseKeys\AW_tdeCert_PrivateKey.crt',
                                ENCRYPTION BY PASSWORD = 'Password#123'
                                )");
                                            migrationBuilder.Sql(@"CREATE SYMMETRIC KEY AW_ColumnKey
                                WITH ALGORITHM = AES_256
                                ENCRYPTION BY CERTIFICATE AW_tdeCert;
                                ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
