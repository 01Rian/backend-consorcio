using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rian_p01_back.Src.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAdministradoraEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consorcios_Administradoras_AdministradoraId",
                table: "Consorcios");

            migrationBuilder.DropTable(
                name: "Administradoras");

            migrationBuilder.DropIndex(
                name: "IX_Consorcios_AdministradoraId",
                table: "Consorcios");

            migrationBuilder.DropColumn(
                name: "AdministradoraId",
                table: "Consorcios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdministradoraId",
                table: "Consorcios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Administradoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CNPJ = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Endereco_Bairro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Endereco_CEP = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Endereco_Cidade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Endereco_Complemento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Endereco_Estado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Endereco_Logradouro = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Endereco_Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradoras", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consorcios_AdministradoraId",
                table: "Consorcios",
                column: "AdministradoraId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consorcios_Administradoras_AdministradoraId",
                table: "Consorcios",
                column: "AdministradoraId",
                principalTable: "Administradoras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
