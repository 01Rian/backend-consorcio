using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rian_p01_back.Src.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administradoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CNPJ = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Endereco_Logradouro = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Endereco_Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Endereco_Complemento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Endereco_Bairro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Endereco_Cidade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Endereco_Estado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Endereco_CEP = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradoras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Celular = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Endereco_Logradouro = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Endereco_Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Endereco_Complemento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Endereco_Bairro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Endereco_Cidade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Endereco_Estado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Endereco_CEP = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Consorcios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ValorBem = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantidadeCotas = table.Column<int>(type: "int", nullable: false),
                    PrazoMeses = table.Column<int>(type: "int", nullable: false),
                    TaxaAdministracao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FundoReserva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataTermino = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdministradoraId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consorcios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consorcios_Administradoras_AdministradoraId",
                        column: x => x.AdministradoraId,
                        principalTable: "Administradoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cotas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroCota = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ValorParcela = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorPago = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ParcelasPagas = table.Column<int>(type: "int", nullable: false),
                    Contemplada = table.Column<bool>(type: "bit", nullable: false),
                    DataContemplacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConsorcioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cotas_Consorcios_ConsorcioId",
                        column: x => x.ConsorcioId,
                        principalTable: "Consorcios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cotas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consorcios_AdministradoraId",
                table: "Consorcios",
                column: "AdministradoraId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotas_ConsorcioId",
                table: "Cotas",
                column: "ConsorcioId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotas_UsuarioId",
                table: "Cotas",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cotas");

            migrationBuilder.DropTable(
                name: "Consorcios");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Administradoras");
        }
    }
}
