using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EF_Activity001.Migrations
{
    public partial class EncryptionMigration_Step5a_DropTargetedFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
             //   name: "AK_Employee_NationalIDNumber",
            //    schema: "HumanResources",
            //    table: "Employee");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                schema: "HumanResources",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "HumanResources",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "HireDate",
                schema: "HumanResources",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                schema: "HumanResources",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                schema: "HumanResources",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "NationalIDNumber",
                schema: "HumanResources",
                table: "Employee");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                schema: "HumanResources",
                table: "Employee",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Date of birth.");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                schema: "HumanResources",
                table: "Employee",
                type: "nchar(1)",
                fixedLength: true,
                maxLength: 1,
                nullable: false,
                defaultValue: "",
                comment: "M = Male, F = Female");

            migrationBuilder.AddColumn<DateTime>(
                name: "HireDate",
                schema: "HumanResources",
                table: "Employee",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Employee hired on this date.");

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                schema: "HumanResources",
                table: "Employee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                comment: "Work title such as Buyer or Sales Representative.");

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                schema: "HumanResources",
                table: "Employee",
                type: "nchar(1)",
                fixedLength: true,
                maxLength: 1,
                nullable: false,
                defaultValue: "",
                comment: "M = Married, S = Single");

            migrationBuilder.AddColumn<string>(
                name: "NationalIDNumber",
                schema: "HumanResources",
                table: "Employee",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                comment: "Unique national identification number such as a social security number.");

            migrationBuilder.CreateIndex(
                name: "AK_Employee_NationalIDNumber",
                schema: "HumanResources",
                table: "Employee",
                column: "NationalIDNumber",
                unique: true);
        }
    }
}
