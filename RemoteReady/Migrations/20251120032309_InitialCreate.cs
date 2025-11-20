using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteReady.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_GS_NET_BLOG_POST",
                columns: table => new
                {
                    ID_POST = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DS_TITULO = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    DS_DESCRICAO = table.Column<string>(type: "NVARCHAR2(600)", maxLength: 600, nullable: true),
                    DS_IMAGE_URL = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    DS_TAG = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    DT_CRIACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_GS_NET_BLOG_POST", x => x.ID_POST);
                });

            migrationBuilder.CreateTable(
                name: "TB_GS_NET_EMPRESA",
                columns: table => new
                {
                    ID_EMPRESA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_EMPRESA = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    DS_EMPRESA = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    DS_AREA = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: true),
                    FL_HIRING_NOW = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false),
                    DS_LOGO_URL = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    DS_WEBSITE = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: true),
                    DT_CRIACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_GS_NET_EMPRESA", x => x.ID_EMPRESA);
                });

            migrationBuilder.CreateTable(
                name: "TB_GS_NET_USUARIO",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_USUARIO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DS_EMAIL = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    DS_PASSWORD = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: false),
                    TP_ROLE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DT_CRIACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_GS_NET_USUARIO", x => x.ID_USUARIO);
                });

            migrationBuilder.CreateTable(
                name: "TB_GS_NET_USER_POST",
                columns: table => new
                {
                    ID_USER_POST = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_POST = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DS_STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DT_LEITURA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_GS_NET_USER_POST", x => x.ID_USER_POST);
                    table.ForeignKey(
                        name: "FK_TB_GS_NET_USER_POST_TB_GS_NET_BLOG_POST_ID_POST",
                        column: x => x.ID_POST,
                        principalTable: "TB_GS_NET_BLOG_POST",
                        principalColumn: "ID_POST",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_GS_NET_USER_POST_TB_GS_NET_USUARIO_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "TB_GS_NET_USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_GS_NET_USER_POST_ID_POST",
                table: "TB_GS_NET_USER_POST",
                column: "ID_POST");

            migrationBuilder.CreateIndex(
                name: "IX_TB_GS_NET_USER_POST_ID_USUARIO",
                table: "TB_GS_NET_USER_POST",
                column: "ID_USUARIO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_GS_NET_EMPRESA");

            migrationBuilder.DropTable(
                name: "TB_GS_NET_USER_POST");

            migrationBuilder.DropTable(
                name: "TB_GS_NET_BLOG_POST");

            migrationBuilder.DropTable(
                name: "TB_GS_NET_USUARIO");
        }
    }
}
