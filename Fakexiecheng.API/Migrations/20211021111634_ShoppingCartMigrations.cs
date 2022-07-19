using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fakexiecheng.API.Migrations
{
    public partial class ShoppingCartMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "60a637ae-5d03-405e-984a-035935c3a99e", "7fb0fb95-2c3d-4a80-b971-32147c20ed16" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7fb0fb95-2c3d-4a80-b971-32147c20ed16");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "60a637ae-5d03-405e-984a-035935c3a99e");

            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TouristRouteId = table.Column<Guid>(nullable: false),
                    ShoppingCartId = table.Column<Guid>(nullable: true),
                    Originalprice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPresent = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineItems_ShoppingCarts_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LineItems_TouristRoutes_TouristRouteId",
                        column: x => x.TouristRouteId,
                        principalTable: "TouristRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "39764d82-e969-4e35-8a6c-746fa3f1ea11", "e113bdd5-1655-4fea-b578-daaadd6fa25d", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "a4abb531-a84b-4d68-8b68-6abb7262969b", 0, null, "a28b83fb-3f74-424a-88db-96dc036e0ccb", "13011@qq.com", true, false, null, "13011@QQ.COM", "13011@QQ.COM", "AQAAAAEAACcQAAAAEKzOidh9hs0/xFXj7oUkb7Pu1sPFFAFSGjP8G+Pg3DEfZfy8wzkp/HIwrZwgtj2yHg==", "1234567891", false, "cd94fc0d-7971-41d4-8f9d-9cb5cb81bcbc", false, "13011@qq.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId", "ApplicationUserId" },
                values: new object[] { "a4abb531-a84b-4d68-8b68-6abb7262969b", "39764d82-e969-4e35-8a6c-746fa3f1ea11", null });

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_ShoppingCartId",
                table: "LineItems",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_TouristRouteId",
                table: "LineItems",
                column: "TouristRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_UserId",
                table: "ShoppingCarts",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineItems");

            migrationBuilder.DropTable(
                name: "ShoppingCarts");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "a4abb531-a84b-4d68-8b68-6abb7262969b", "39764d82-e969-4e35-8a6c-746fa3f1ea11" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "39764d82-e969-4e35-8a6c-746fa3f1ea11");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a4abb531-a84b-4d68-8b68-6abb7262969b");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7fb0fb95-2c3d-4a80-b971-32147c20ed16", "1953206f-bc17-4902-8375-2a365119c5c5", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "60a637ae-5d03-405e-984a-035935c3a99e", 0, null, "ce81a4ad-4aae-4411-a48c-bd0d09e03f7e", "13011@qq.com", true, false, null, "13011@QQ.COM", "13011@QQ.COM", "AQAAAAEAACcQAAAAEGNVgrKmVtVJasiDdfgLpyNBHEpqJB8vLoXKJpHwaIi24CnPHj+IG0H4JCzovFrVpg==", "1234567891", false, "a087431c-d6fc-4b34-994e-8f3c876de3f1", false, "13011@qq.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId", "ApplicationUserId" },
                values: new object[] { "60a637ae-5d03-405e-984a-035935c3a99e", "7fb0fb95-2c3d-4a80-b971-32147c20ed16", null });
        }
    }
}
