using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EF_Activity001.Migrations
{
    public partial class EncryptionMigration_step5b_RestoreTargetColumnsAsVarBinaryMax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "BirthDate",
                schema: "HumanResources",
                table: "Employee",
                nullable: true,
                comment: "Date of birth.");

            migrationBuilder.AddColumn<byte[]>(
                name: "Gender",
                schema: "HumanResources",
                table: "Employee",
                nullable: false,
                defaultValue: new byte[] {  },
                comment: "M = Male, F = Female");

            migrationBuilder.AddColumn<byte[]>(
                name: "HireDate",
                schema: "HumanResources",
                table: "Employee",
                nullable: true,
                comment: "Employee hired on this date.");

            migrationBuilder.AddColumn<byte[]>(
                name: "JobTitle",
                schema: "HumanResources",
                table: "Employee",
                nullable: false,
                defaultValue: new byte[] {  },
                comment: "Work title such as Buyer or Sales Representative.");

            migrationBuilder.AddColumn<byte[]>(
                name: "MaritalStatus",
                schema: "HumanResources",
                table: "Employee",
                nullable: false,
                defaultValue: new byte[] {  },
                comment: "M = Married, S = Single");

            migrationBuilder.AddColumn<byte[]>(
                name: "NationalIDNumber",
                schema: "HumanResources",
                table: "Employee",
                nullable: false,
                defaultValue: new byte[] {  },
                comment: "Unique national identification number such as a social security number.");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
