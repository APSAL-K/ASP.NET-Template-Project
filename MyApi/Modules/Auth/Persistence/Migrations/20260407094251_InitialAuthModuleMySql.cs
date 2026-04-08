using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyApi.Modules.Auth.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAuthModuleMySql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuthPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthPermissions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuthRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuthUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordSalt = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUsers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuthRolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PermissionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthRolePermissions_AuthPermissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "AuthPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthRolePermissions_AuthRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AuthRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuthRefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Token = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IssuedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsRevoked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthRefreshTokens_AuthUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AuthUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuthUserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AssignedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthUserRoles_AuthRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AuthRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthUserRoles_AuthUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AuthUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AuthPermissions",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("11a7d0ca-a7aa-49d2-81c2-eb09fbf67e95"), "Update the authenticated user profile.", "auth.profile.update" },
                    { new Guid("9007725f-8a40-4a4c-9cdb-01e983f7f98d"), "Manage users, roles, and permissions.", "auth.users.manage" },
                    { new Guid("b1daf2a5-3508-4b35-a449-38590f266f59"), "Refresh access tokens.", "auth.tokens.refresh" },
                    { new Guid("ccf425d8-c99e-4d54-b5f0-1949920727f0"), "Read the authenticated user profile.", "auth.profile.read" }
                });

            migrationBuilder.InsertData(
                table: "AuthRoles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("0c6b3d90-5513-4c67-9d7f-5bc2b4e2d9c1"), "Default application user role.", "User" },
                    { new Guid("0e2714f8-a22d-4ec3-a730-0710bf3efc4c"), "Administrator role with access to auth management operations.", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "AuthRolePermissions",
                columns: new[] { "Id", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("21a9dc48-7e7b-4af5-89b8-c792d2113348"), new Guid("ccf425d8-c99e-4d54-b5f0-1949920727f0"), new Guid("0c6b3d90-5513-4c67-9d7f-5bc2b4e2d9c1") },
                    { new Guid("5ba40513-f48e-4671-9327-23344dacac51"), new Guid("ccf425d8-c99e-4d54-b5f0-1949920727f0"), new Guid("0e2714f8-a22d-4ec3-a730-0710bf3efc4c") },
                    { new Guid("6851ee5d-703e-41ef-aaad-071761761ce2"), new Guid("11a7d0ca-a7aa-49d2-81c2-eb09fbf67e95"), new Guid("0c6b3d90-5513-4c67-9d7f-5bc2b4e2d9c1") },
                    { new Guid("77832da3-a93e-4d8b-b18b-31de7476c959"), new Guid("9007725f-8a40-4a4c-9cdb-01e983f7f98d"), new Guid("0e2714f8-a22d-4ec3-a730-0710bf3efc4c") },
                    { new Guid("9d23d6d5-b013-41a4-b9bb-430f8dd2e74d"), new Guid("b1daf2a5-3508-4b35-a449-38590f266f59"), new Guid("0e2714f8-a22d-4ec3-a730-0710bf3efc4c") },
                    { new Guid("f477c9b1-ac9a-4a1d-ab88-bd9302c970f1"), new Guid("b1daf2a5-3508-4b35-a449-38590f266f59"), new Guid("0c6b3d90-5513-4c67-9d7f-5bc2b4e2d9c1") },
                    { new Guid("f6560fd2-81b7-4673-ad0d-fc82ef3222db"), new Guid("11a7d0ca-a7aa-49d2-81c2-eb09fbf67e95"), new Guid("0e2714f8-a22d-4ec3-a730-0710bf3efc4c") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthPermissions_Name",
                table: "AuthPermissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthRefreshTokens_Token",
                table: "AuthRefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthRefreshTokens_UserId",
                table: "AuthRefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthRolePermissions_PermissionId",
                table: "AuthRolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthRolePermissions_RoleId_PermissionId",
                table: "AuthRolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthRoles_Name",
                table: "AuthRoles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserRoles_RoleId",
                table: "AuthUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserRoles_UserId_RoleId",
                table: "AuthUserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthUsers_Email",
                table: "AuthUsers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthRefreshTokens");

            migrationBuilder.DropTable(
                name: "AuthRolePermissions");

            migrationBuilder.DropTable(
                name: "AuthUserRoles");

            migrationBuilder.DropTable(
                name: "AuthPermissions");

            migrationBuilder.DropTable(
                name: "AuthRoles");

            migrationBuilder.DropTable(
                name: "AuthUsers");
        }
    }
}
