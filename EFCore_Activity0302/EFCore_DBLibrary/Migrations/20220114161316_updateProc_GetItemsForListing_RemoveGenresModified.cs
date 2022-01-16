using EFCore_DBLibrary.Scripts;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore_DBLibrary.Migrations
{
    public partial class updateProc_GetItemsForListing_RemoveGenresModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SqlResource("EFCore_DBLibrary.Scripts.Procedures.GetItemsForListing.GetItemsForListing.v2.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SqlResource("EFCore_DBLibrary.Scripts.Procedures.GetItemsForListing.GetItemsForListing.v0.sql");

        }
    }
}
