using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangescolumnorderinDevicestable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AppVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppBuildNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTV = table.Column<bool>(type: "bit", nullable: false),
                    IsTesting = table.Column<bool>(type: "bit", nullable: false),
                    IsIOS = table.Column<bool>(type: "bit", nullable: false),
                    IsAndroid = table.Column<bool>(type: "bit", nullable: false),
                    IsWindows = table.Column<bool>(type: "bit", nullable: false),
                    IsMacOS = table.Column<bool>(type: "bit", nullable: false),
                    IsWeb = table.Column<bool>(type: "bit", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    LastHeartBeat = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
