using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApprovalWorkflow.Migrations
{
    /// <inheritdoc />
    public partial class InitialBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Approval.Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval.Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approval.Users_Approval.Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.Users_Approval.Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Approval.ApprovalHashes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval.ApprovalHashes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approval.ApprovalHashes_Approval.Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.ApprovalHashes_Approval.Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Approval.ApprovalItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    AuthoringUserId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval.ApprovalItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approval.ApprovalItems_Approval.Users_AuthoringUserId",
                        column: x => x.AuthoringUserId,
                        principalTable: "Approval.Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Approval.ApprovalItems_Approval.Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.ApprovalItems_Approval.Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Approval.ApprovalTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval.ApprovalTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approval.ApprovalTypes_Approval.Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.ApprovalTypes_Approval.Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Approval.UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval.UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approval.UserClaims_Approval.Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.UserClaims_Approval.Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.UserClaims_Approval.Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Approval.Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Approval.UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval.UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_Approval.UserLogins_Approval.Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.UserLogins_Approval.Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.UserLogins_Approval.Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Approval.Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Approval.UserTokens",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval.UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_Approval.UserTokens_Approval.Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.UserTokens_Approval.Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.UserTokens_Approval.Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Approval.Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Approval.Roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShadowId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovalHashId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval.Roles", x => x.Id);
                    table.CheckConstraint("CK_Role_ApprovalRequired", "(ApprovalStatus = '1' AND (ApprovalHashId IS NULL OR ApprovalHashId = '')) OR (ApprovalStatus <> '1' AND (ApprovalHashId IS NOT NULL AND ApprovalHashId <> ''))");
                    table.ForeignKey(
                        name: "FK_Approval.Roles_Approval.ApprovalHashes_ApprovalHashId",
                        column: x => x.ApprovalHashId,
                        principalTable: "Approval.ApprovalHashes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.Roles_Approval.Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.Roles_Approval.Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Approval.ApprovalRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalTypeId = table.Column<long>(type: "bigint", nullable: false),
                    CurrentStep = table.Column<byte>(type: "tinyint", nullable: false),
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    RequestAction = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval.ApprovalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approval.ApprovalRequests_Approval.ApprovalTypes_ApprovalTypeId",
                        column: x => x.ApprovalTypeId,
                        principalTable: "Approval.ApprovalTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Approval.ApprovalRequests_Approval.Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.ApprovalRequests_Approval.Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Approval.ApprovalSteps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovalTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<byte>(type: "tinyint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    Rule = table.Column<int>(type: "int", nullable: false),
                    NumberOfUsers = table.Column<byte>(type: "tinyint", nullable: false),
                    ExtensionProperty = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval.ApprovalSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approval.ApprovalSteps_Approval.Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Approval.Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Approval.ApprovalSteps_Approval.Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.ApprovalSteps_Approval.Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Approval.RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval.RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approval.RoleClaims_Approval.Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Approval.Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Approval.RoleClaims_Approval.Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.RoleClaims_Approval.Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Approval.UserRoles",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval.UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_Approval.UserRoles_Approval.Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Approval.Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Approval.UserRoles_Approval.Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.UserRoles_Approval.Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Approval.Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approval.UserRoles_Approval.Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Approval.Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalHashes_CreatedById",
                table: "Approval.ApprovalHashes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalHashes_UpdatedById",
                table: "Approval.ApprovalHashes",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalItems_AuthoringUserId",
                table: "Approval.ApprovalItems",
                column: "AuthoringUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalItems_CreatedById",
                table: "Approval.ApprovalItems",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalItems_UpdatedById",
                table: "Approval.ApprovalItems",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalRequests_ApprovalTypeId",
                table: "Approval.ApprovalRequests",
                column: "ApprovalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalRequests_CreatedById",
                table: "Approval.ApprovalRequests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalRequests_UpdatedById",
                table: "Approval.ApprovalRequests",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalSteps_ApprovalTypeId",
                table: "Approval.ApprovalSteps",
                column: "ApprovalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalSteps_CreatedById",
                table: "Approval.ApprovalSteps",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalSteps_RoleId",
                table: "Approval.ApprovalSteps",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalSteps_UpdatedById",
                table: "Approval.ApprovalSteps",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalTypes_CreatedById",
                table: "Approval.ApprovalTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.ApprovalTypes_UpdatedById",
                table: "Approval.ApprovalTypes",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.RoleClaims_CreatedById",
                table: "Approval.RoleClaims",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.RoleClaims_RoleId",
                table: "Approval.RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.RoleClaims_UpdatedById",
                table: "Approval.RoleClaims",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.Roles_ApprovalHashId",
                table: "Approval.Roles",
                column: "ApprovalHashId",
                unique: true,
                filter: "[ApprovalHashId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.Roles_CreatedById",
                table: "Approval.Roles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.Roles_UpdatedById",
                table: "Approval.Roles",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Approval.Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.UserClaims_CreatedById",
                table: "Approval.UserClaims",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.UserClaims_UpdatedById",
                table: "Approval.UserClaims",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.UserClaims_UserId",
                table: "Approval.UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.UserLogins_CreatedById",
                table: "Approval.UserLogins",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.UserLogins_UpdatedById",
                table: "Approval.UserLogins",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.UserLogins_UserId",
                table: "Approval.UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.UserRoles_CreatedById",
                table: "Approval.UserRoles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.UserRoles_RoleId",
                table: "Approval.UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.UserRoles_UpdatedById",
                table: "Approval.UserRoles",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Approval.Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.Users_CreatedById",
                table: "Approval.Users",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.Users_UpdatedById",
                table: "Approval.Users",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Approval.Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.UserTokens_CreatedById",
                table: "Approval.UserTokens",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Approval.UserTokens_UpdatedById",
                table: "Approval.UserTokens",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Approval.ApprovalItems");

            migrationBuilder.DropTable(
                name: "Approval.ApprovalRequests");

            migrationBuilder.DropTable(
                name: "Approval.ApprovalSteps");

            migrationBuilder.DropTable(
                name: "Approval.RoleClaims");

            migrationBuilder.DropTable(
                name: "Approval.UserClaims");

            migrationBuilder.DropTable(
                name: "Approval.UserLogins");

            migrationBuilder.DropTable(
                name: "Approval.UserRoles");

            migrationBuilder.DropTable(
                name: "Approval.UserTokens");

            migrationBuilder.DropTable(
                name: "Approval.ApprovalTypes");

            migrationBuilder.DropTable(
                name: "Approval.Roles");

            migrationBuilder.DropTable(
                name: "Approval.ApprovalHashes");

            migrationBuilder.DropTable(
                name: "Approval.Users");
        }
    }
}
