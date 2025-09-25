using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stripe_Integration.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ServiceMainID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PlanType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ServiceMainID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    ServiceTypeID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.ServiceTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    B2CSubID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.B2CSubID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDetails",
                columns: table => new
                {
                    ServiceDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceMainID = table.Column<int>(type: "int", nullable: false),
                    DetailItemDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ServiceTypeID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MonthlyCount = table.Column<int>(type: "int", nullable: true),
                    UserDisplay = table.Column<bool>(type: "bit", nullable: false),
                    ServiceCodedDetails = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ValidForMonths = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDetails", x => x.ServiceDetailID);
                    table.ForeignKey(
                        name: "FK_ServiceDetails_Products_ServiceMainID",
                        column: x => x.ServiceMainID,
                        principalTable: "Products",
                        principalColumn: "ServiceMainID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceDetails_ServiceTypes_ServiceTypeID",
                        column: x => x.ServiceTypeID,
                        principalTable: "ServiceTypes",
                        principalColumn: "ServiceTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPlans",
                columns: table => new
                {
                    UserPlanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    B2CSubID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ServiceMainID = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StripeSubscriptionID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StripeCustomerID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StripePaymentIntentID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StripeInvoiceID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StripeStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlans", x => x.UserPlanID);
                    table.ForeignKey(
                        name: "FK_UserPlans_Products_ServiceMainID",
                        column: x => x.ServiceMainID,
                        principalTable: "Products",
                        principalColumn: "ServiceMainID");
                    table.ForeignKey(
                        name: "FK_UserPlans_Users_B2CSubID",
                        column: x => x.B2CSubID,
                        principalTable: "Users",
                        principalColumn: "B2CSubID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserServiceUsages",
                columns: table => new
                {
                    UsageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserPlanID = table.Column<int>(type: "int", nullable: false),
                    ServiceDetailID = table.Column<int>(type: "int", nullable: false),
                    AddedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUsageDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemainingBalance = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserServiceUsages", x => x.UsageID);
                    table.ForeignKey(
                        name: "FK_UserServiceUsages_ServiceDetails_ServiceDetailID",
                        column: x => x.ServiceDetailID,
                        principalTable: "ServiceDetails",
                        principalColumn: "ServiceDetailID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserServiceUsages_UserPlans_UserPlanID",
                        column: x => x.UserPlanID,
                        principalTable: "UserPlans",
                        principalColumn: "UserPlanID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDetails_ServiceMainID",
                table: "ServiceDetails",
                column: "ServiceMainID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDetails_ServiceTypeID",
                table: "ServiceDetails",
                column: "ServiceTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlans_B2CSubID",
                table: "UserPlans",
                column: "B2CSubID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlans_ServiceMainID",
                table: "UserPlans",
                column: "ServiceMainID");

            migrationBuilder.CreateIndex(
                name: "IX_UserServiceUsages_ServiceDetailID",
                table: "UserServiceUsages",
                column: "ServiceDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_UserServiceUsages_UserPlanID",
                table: "UserServiceUsages",
                column: "UserPlanID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserServiceUsages");

            migrationBuilder.DropTable(
                name: "ServiceDetails");

            migrationBuilder.DropTable(
                name: "UserPlans");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
