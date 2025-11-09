using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCard.Migrations
{
    /// <inheritdoc />
    public partial class AddCardPairTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackFlash",
                table: "Flashcards");

            migrationBuilder.RenameColumn(
                name: "FrontFlash",
                table: "Flashcards",
                newName: "CardTitle");

            migrationBuilder.AddColumn<string>(
                name: "CardDescription",
                table: "Flashcards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Flashcards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CardPairs",
                columns: table => new
                {
                    PairId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FrontCard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackCard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlashcardId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardPairs", x => x.PairId);
                    table.ForeignKey(
                        name: "FK_CardPairs_Flashcards_FlashcardId",
                        column: x => x.FlashcardId,
                        principalTable: "Flashcards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardPairs_FlashcardId",
                table: "CardPairs",
                column: "FlashcardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardPairs");

            migrationBuilder.DropColumn(
                name: "CardDescription",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Flashcards");

            migrationBuilder.RenameColumn(
                name: "CardTitle",
                table: "Flashcards",
                newName: "FrontFlash");

            migrationBuilder.AddColumn<string>(
                name: "BackFlash",
                table: "Flashcards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
