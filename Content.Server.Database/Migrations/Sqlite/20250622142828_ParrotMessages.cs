using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class ParrotMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "parrot_messages",
                columns: table => new
                {
                    parrot_messages_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    message_text = table.Column<string>(type: "TEXT", nullable: false),
                    source_player = table.Column<Guid>(type: "TEXT", nullable: false),
                    round = table.Column<int>(type: "INTEGER", nullable: false),
                    block = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parrot_messages", x => x.parrot_messages_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parrot_messages");
        }
    }
}
