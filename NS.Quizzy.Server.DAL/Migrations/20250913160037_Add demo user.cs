using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Adddemouser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b900d543-90ab-4e7a-83ba-b961918dcc8c"),
                column: "NotificationToken",
                value: "eiP6Nt2PTK-oE-gd2hD_0a:APA91bGCpOImJoDUpEbrRBqPWqu50Z-bx90hdVLEhscx1og2FPtPt5Xn1UMT0siClSbXpAMdeie6d41FxnJtUpEPEzaU7p2tlMETwyng_YT4kEQ5sBd-0Zs");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ClassId", "Email", "FullName", "IdNumber", "NotificationToken", "Password", "Role", "TwoFactorSecretKey" },
                values: new object[] { new Guid("f8178f51-6d73-45e1-a4e7-1b01baf9884d"), null, "QuizzyDemo@ExamProduction.com", "Demo user", null, "eiP6Nt2PTK-oE-gd2hD_0a:APA91bGCpOImJoDUpEbrRBqPWqu50Z-bx90hdVLEhscx1og2FPtPt5Xn1UMT0siClSbXpAMdeie6d41FxnJtUpEPEzaU7p2tlMETwyng_YT4kEQ5sBd-0Zs", "nS+ja7PyR+pO3sNsUm4c3vleK3reskgbk++4+vlPrSK6jjQ0RCV3JJfENUr/zesg", 2, "USUYJXW3QXUHA7R53YP4QBPPXHH54KHS" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f8178f51-6d73-45e1-a4e7-1b01baf9884d"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b900d543-90ab-4e7a-83ba-b961918dcc8c"),
                column: "NotificationToken",
                value: "dxkmG0jIQyWsrrnlihsPzp:APA91bEZptXfWbtClGiGQsnoJyFlgZOvb8ezRkv0aBeUNMJv-OxAKNofrXSh3CNAV2Ix4qB5c_11LmcnFSgqH4dzZo5f-zQSIVfF3j1jvmqQfG-uRHcJ4Oc");
        }
    }
}
