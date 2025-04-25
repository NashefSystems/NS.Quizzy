using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationtokensforinitusers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2325b8ae-f12a-43d8-be46-7041e57c9283"),
                column: "NotificationToken",
                value: "eeCTLExNRcGRRTmz1yXhue:APA91bHtF4PgtFbLsqAJHDw8-koBVRjZBCy6A_15OI_NfavWtvQwtytXsGQDoVa9Y_w__nap_jgQkOoA_YYAVM6UyKpeONooPgfCd4y5CItcIfUOizkmvHU");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b900d543-90ab-4e7a-83ba-b961918dcc8c"),
                column: "NotificationToken",
                value: "dxkmG0jIQyWsrrnlihsPzp:APA91bEZptXfWbtClGiGQsnoJyFlgZOvb8ezRkv0aBeUNMJv-OxAKNofrXSh3CNAV2Ix4qB5c_11LmcnFSgqH4dzZo5f-zQSIVfF3j1jvmqQfG-uRHcJ4Oc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2325b8ae-f12a-43d8-be46-7041e57c9283"),
                column: "NotificationToken",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b900d543-90ab-4e7a-83ba-b961918dcc8c"),
                column: "NotificationToken",
                value: null);
        }
    }
}
