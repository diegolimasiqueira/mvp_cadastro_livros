using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assunto",
                columns: table => new
                {
                    CodAs = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descricao = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assunto", x => x.CodAs);
                });

            migrationBuilder.CreateTable(
                name: "Autor",
                columns: table => new
                {
                    CodAu = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autor", x => x.CodAu);
                });

            migrationBuilder.CreateTable(
                name: "FormaCompra",
                columns: table => new
                {
                    CodFc = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descricao = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormaCompra", x => x.CodFc);
                });

            migrationBuilder.CreateTable(
                name: "Livro",
                columns: table => new
                {
                    CodI = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Editora = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Edicao = table.Column<int>(type: "integer", nullable: false),
                    AnoPublicacao = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livro", x => x.CodI);
                });

            migrationBuilder.CreateTable(
                name: "Livro_Assunto",
                columns: table => new
                {
                    Livro_CodI = table.Column<int>(type: "integer", nullable: false),
                    Assunto_CodAs = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livro_Assunto", x => new { x.Livro_CodI, x.Assunto_CodAs });
                    table.ForeignKey(
                        name: "FK_Livro_Assunto_Assunto",
                        column: x => x.Assunto_CodAs,
                        principalTable: "Assunto",
                        principalColumn: "CodAs",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Livro_Assunto_Livro",
                        column: x => x.Livro_CodI,
                        principalTable: "Livro",
                        principalColumn: "CodI",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Livro_Autor",
                columns: table => new
                {
                    Livro_CodI = table.Column<int>(type: "integer", nullable: false),
                    Autor_CodAu = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livro_Autor", x => new { x.Livro_CodI, x.Autor_CodAu });
                    table.ForeignKey(
                        name: "FK_Livro_Autor_Autor",
                        column: x => x.Autor_CodAu,
                        principalTable: "Autor",
                        principalColumn: "CodAu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Livro_Autor_Livro",
                        column: x => x.Livro_CodI,
                        principalTable: "Livro",
                        principalColumn: "CodI",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LivroPreco",
                columns: table => new
                {
                    CodLp = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Livro_CodI = table.Column<int>(type: "integer", nullable: false),
                    FormaCompra_CodFc = table.Column<int>(type: "integer", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivroPreco", x => x.CodLp);
                    table.ForeignKey(
                        name: "FK_LivroPreco_FormaCompra",
                        column: x => x.FormaCompra_CodFc,
                        principalTable: "FormaCompra",
                        principalColumn: "CodFc",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LivroPreco_Livro",
                        column: x => x.Livro_CodI,
                        principalTable: "Livro",
                        principalColumn: "CodI",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assunto_Descricao",
                table: "Assunto",
                column: "Descricao");

            migrationBuilder.CreateIndex(
                name: "IX_Autor_Nome",
                table: "Autor",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_FormaCompra_Descricao",
                table: "FormaCompra",
                column: "Descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Livro_Titulo",
                table: "Livro",
                column: "Titulo");

            migrationBuilder.CreateIndex(
                name: "Livro_Assunto_FKIndex1",
                table: "Livro_Assunto",
                column: "Livro_CodI");

            migrationBuilder.CreateIndex(
                name: "Livro_Assunto_FKIndex2",
                table: "Livro_Assunto",
                column: "Assunto_CodAs");

            migrationBuilder.CreateIndex(
                name: "Livro_Autor_FKIndex1",
                table: "Livro_Autor",
                column: "Livro_CodI");

            migrationBuilder.CreateIndex(
                name: "Livro_Autor_FKIndex2",
                table: "Livro_Autor",
                column: "Autor_CodAu");

            migrationBuilder.CreateIndex(
                name: "IX_LivroPreco_FormaCompra",
                table: "LivroPreco",
                column: "FormaCompra_CodFc");

            migrationBuilder.CreateIndex(
                name: "IX_LivroPreco_Livro",
                table: "LivroPreco",
                column: "Livro_CodI");

            migrationBuilder.CreateIndex(
                name: "IX_LivroPreco_Livro_FormaCompra",
                table: "LivroPreco",
                columns: new[] { "Livro_CodI", "FormaCompra_CodFc" },
                unique: true);

            // Criar VIEW para relatórios após todas as tabelas estarem criadas
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW vw_livros_por_autor AS
                SELECT 
                    l.""CodI"" AS ""LivroCodigo"",
                    l.""Titulo"" AS ""LivroTitulo"",
                    l.""Editora"" AS ""Editora"",
                    l.""Edicao"" AS ""Edicao"",
                    l.""AnoPublicacao"" AS ""AnoPublicacao"",
                    a.""CodAu"" AS ""AutorCodigo"",
                    a.""Nome"" AS ""AutorNome"",
                    STRING_AGG(DISTINCT ass.""Descricao"", ', ' ORDER BY ass.""Descricao"") AS ""Assuntos"",
                    STRING_AGG(DISTINCT 
                        fc.""Descricao"" || ': R$ ' || CAST(lp.""Valor"" AS VARCHAR), 
                        '; ' 
                        ORDER BY fc.""Descricao"" || ': R$ ' || CAST(lp.""Valor"" AS VARCHAR)
                    ) AS ""FormasCompra""
                FROM ""Livro"" l
                INNER JOIN ""Livro_Autor"" la ON l.""CodI"" = la.""Livro_CodI""
                INNER JOIN ""Autor"" a ON la.""Autor_CodAu"" = a.""CodAu""
                LEFT JOIN ""Livro_Assunto"" las ON l.""CodI"" = las.""Livro_CodI""
                LEFT JOIN ""Assunto"" ass ON las.""Assunto_CodAs"" = ass.""CodAs""
                LEFT JOIN ""LivroPreco"" lp ON l.""CodI"" = lp.""Livro_CodI""
                LEFT JOIN ""FormaCompra"" fc ON lp.""FormaCompra_CodFc"" = fc.""CodFc""
                GROUP BY 
                    l.""CodI"", 
                    l.""Titulo"", 
                    l.""Editora"", 
                    l.""Edicao"", 
                    l.""AnoPublicacao"",
                    a.""CodAu"",
                    a.""Nome""
                ORDER BY 
                    a.""Nome"", 
                    l.""Titulo"";
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remover VIEW primeiro
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_livros_por_autor;");

            migrationBuilder.DropTable(
                name: "Livro_Assunto");

            migrationBuilder.DropTable(
                name: "Livro_Autor");

            migrationBuilder.DropTable(
                name: "LivroPreco");

            migrationBuilder.DropTable(
                name: "Assunto");

            migrationBuilder.DropTable(
                name: "Autor");

            migrationBuilder.DropTable(
                name: "FormaCompra");

            migrationBuilder.DropTable(
                name: "Livro");
        }
    }
}
