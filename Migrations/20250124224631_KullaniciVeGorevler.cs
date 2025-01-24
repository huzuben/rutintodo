using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todolist.Migrations
{
    /// <inheritdoc />
    public partial class KullaniciVeGorevler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bildirimler_Gorevler_GorevId",
                table: "Bildirimler");

            migrationBuilder.AlterColumn<string>(
                name: "Baslik",
                table: "Gorevler",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "KullaniciId",
                table: "Gorevler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KullaniciId1",
                table: "Gorevler",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Eposta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gorevler_KullaniciId",
                table: "Gorevler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Gorevler_KullaniciId1",
                table: "Gorevler",
                column: "KullaniciId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Bildirimler_Gorevler_GorevId",
                table: "Bildirimler",
                column: "GorevId",
                principalTable: "Gorevler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Gorevler_Kullanicilar_KullaniciId",
                table: "Gorevler",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gorevler_Kullanicilar_KullaniciId1",
                table: "Gorevler",
                column: "KullaniciId1",
                principalTable: "Kullanicilar",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bildirimler_Gorevler_GorevId",
                table: "Bildirimler");

            migrationBuilder.DropForeignKey(
                name: "FK_Gorevler_Kullanicilar_KullaniciId",
                table: "Gorevler");

            migrationBuilder.DropForeignKey(
                name: "FK_Gorevler_Kullanicilar_KullaniciId1",
                table: "Gorevler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropIndex(
                name: "IX_Gorevler_KullaniciId",
                table: "Gorevler");

            migrationBuilder.DropIndex(
                name: "IX_Gorevler_KullaniciId1",
                table: "Gorevler");

            migrationBuilder.DropColumn(
                name: "KullaniciId",
                table: "Gorevler");

            migrationBuilder.DropColumn(
                name: "KullaniciId1",
                table: "Gorevler");

            migrationBuilder.AlterColumn<string>(
                name: "Baslik",
                table: "Gorevler",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Bildirimler_Gorevler_GorevId",
                table: "Bildirimler",
                column: "GorevId",
                principalTable: "Gorevler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
