using Microsoft.EntityFrameworkCore.Migrations;

namespace ConsoleApp5.Migrations
{
    public partial class FK_ParendID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateIndex(
            //    name: "IX_Department_ParentID",
            //    table: "Department",
            //    column: "ParentID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Department_Department_ParentID",
            //    table: "Department",
            //    column: "ParentID",
            //    principalTable: "Department",
            //    principalColumn: "ID",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Department_Department_ParentID",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Department_ParentID",
                table: "Department");
        }
    }
}
