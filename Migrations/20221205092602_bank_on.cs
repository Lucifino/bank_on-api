using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bank_on_api.Migrations
{
    public partial class bank_on : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "bank_on");

            migrationBuilder.CreateTable(
                name: "FinanceProduct",
                schema: "bank_on",
                columns: table => new
                {
                    FinanceProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InterestRate = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    TermMin = table.Column<int>(type: "int", nullable: true),
                    AmountMin = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    _Deleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceProduct", x => x.FinanceProductId);
                });

            migrationBuilder.CreateTable(
                name: "FinanceRequestStatus",
                schema: "bank_on",
                columns: table => new
                {
                    FinanceRequestStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    _case = table.Column<int>(type: "int", nullable: true),
                    _Deleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceRequestStatus", x => x.FinanceRequestStatusId);
                });

            migrationBuilder.CreateTable(
                name: "FinanceRequest",
                schema: "bank_on",
                columns: table => new
                {
                    FinanceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    AmountRequired = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Term = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FinanceProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FinanceRequestStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DateOfApplication = table.Column<DateTime>(type: "date", nullable: true),
                    _Deleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceRequest", x => x.FinanceRequestId);
                    table.ForeignKey(
                        name: "fk_finance_product_finance_request_1",
                        column: x => x.FinanceProductId,
                        principalSchema: "bank_on",
                        principalTable: "FinanceProduct",
                        principalColumn: "FinanceProductId");
                    table.ForeignKey(
                        name: "fk_finance_request_status_finance_request_1",
                        column: x => x.FinanceRequestStatusId,
                        principalSchema: "bank_on",
                        principalTable: "FinanceRequestStatus",
                        principalColumn: "FinanceRequestStatusId");
                });

            migrationBuilder.CreateTable(
                name: "FinanceRequestLog",
                schema: "bank_on",
                columns: table => new
                {
                    FinanceRequestLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FinanceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    _Deleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceRequestLog", x => x.FinanceRequestLogId);
                    table.ForeignKey(
                        name: "fk_finance_request_finance_request_log_1",
                        column: x => x.FinanceRequestId,
                        principalSchema: "bank_on",
                        principalTable: "FinanceRequest",
                        principalColumn: "FinanceRequestId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinanceRequest_FinanceProductId",
                schema: "bank_on",
                table: "FinanceRequest",
                column: "FinanceProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceRequest_FinanceRequestStatusId",
                schema: "bank_on",
                table: "FinanceRequest",
                column: "FinanceRequestStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceRequestLog_FinanceRequestId",
                schema: "bank_on",
                table: "FinanceRequestLog",
                column: "FinanceRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinanceRequestLog",
                schema: "bank_on");

            migrationBuilder.DropTable(
                name: "FinanceRequest",
                schema: "bank_on");

            migrationBuilder.DropTable(
                name: "FinanceProduct",
                schema: "bank_on");

            migrationBuilder.DropTable(
                name: "FinanceRequestStatus",
                schema: "bank_on");
        }
    }
}
