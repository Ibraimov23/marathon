using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.Migrations
{
    public partial class Procedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp = @"
            IF (OBJECT_ID('DeleteRows') IS NULL)
            Begin
			Exec('
            CREATE PROCEDURE [dbo].[DeleteRows]
            AS
            BEGIN
                DELETE FROM Links
                WHERE ShelfLife = DATEFROMPARTS(DATEPART(year, GETDATE()), DATEPART(month, GETDATE()),DATEPART(day, GETDATE()));
            END')
            END";
            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
