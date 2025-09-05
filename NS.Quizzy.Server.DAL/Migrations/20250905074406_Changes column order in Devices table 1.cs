using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangescolumnorderinDevicestable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AppBuildNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    IsAndroid = table.Column<bool>(type: "bit", nullable: false),
                    IsIOS = table.Column<bool>(type: "bit", nullable: false),
                    IsMacOS = table.Column<bool>(type: "bit", nullable: false),
                    IsTV = table.Column<bool>(type: "bit", nullable: false),
                    IsTesting = table.Column<bool>(type: "bit", nullable: false),
                    IsWeb = table.Column<bool>(type: "bit", nullable: false),
                    IsWindows = table.Column<bool>(type: "bit", nullable: false),
                    LastHeartBeat = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    OS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.ID);
                });
        }
    }
}
