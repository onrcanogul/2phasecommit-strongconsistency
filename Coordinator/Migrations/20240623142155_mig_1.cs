using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coordinator.Migrations
{
    /// <inheritdoc />
    public partial class mig_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NodeStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionState = table.Column<int>(type: "int", nullable: false),
                    IsReady = table.Column<int>(type: "int", nullable: false),
                    NodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NodeStates_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Nodes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("8cf249d2-dfcd-4f20-acd0-c66cdbef1b83"), "Stock.API" },
                    { new Guid("9e4c6a33-3fad-4f3b-b687-ff441b326187"), "Order.API" },
                    { new Guid("cf7954b9-aed2-4311-84ed-ad71f0c0b573"), "Payment.API" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_NodeStates_NodeId",
                table: "NodeStates",
                column: "NodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NodeStates");

            migrationBuilder.DropTable(
                name: "Nodes");
        }
    }
}
