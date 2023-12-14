using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorChallengeIBGE.Migrations
{
    /// <inheritdoc />
    public partial class v3CodigoIbgeintparastring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IbgeCode",
                table: "Ibge",
                type: "TEXT",
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldMaxLength: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IbgeCode",
                table: "Ibge",
                type: "INTEGER",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 7);
        }
    }
}
