using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorChallengeIBGE.Migrations
{
    /// <inheritdoc />
    public partial class v1SchemaIBGE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ibge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    State = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ibge", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ibge_State_City",
                table: "Ibge",
                columns: new[] { "State", "City" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ibge");
        }
    }
}
