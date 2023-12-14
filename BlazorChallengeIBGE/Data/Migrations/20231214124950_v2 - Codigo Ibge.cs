using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorChallengeIBGE.Migrations
{
    /// <inheritdoc />
    public partial class v2CodigoIbge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Ibge",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "IbgeCode",
                table: "Ibge",
                type: "INTEGER",
                maxLength: 2,
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IbgeCode",
                table: "Ibge");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Ibge",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);
        }
    }
}
