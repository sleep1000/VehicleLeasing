using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleLeasing.ServerApp.Data.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "VehicleTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,] {
                                { 1, "Легковой" },
                                { 2, "Грузовой" },
                                { 3, "Болид" },
                                { 4, "Ретро" },
                                { 5, "Спортивный" },
                                { 6, "Кроссовер" },
                                { 7, "Тягач" }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "TypeId", "ImageName" },
                values: new object[,] {
                    { 1, 1, "car.jpg" },
                    { 2, 2, "cargo.jpg" },
                    { 3, 3, "bolide.jpg" },
                    { 4, 4, "retro.jpg" },
                    { 5, 5, "sport.jpg" },
                    { 6, 6, "suv.jpg" },
                    { 7, 7, "truck.jpg" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Vehicles", true);
            migrationBuilder.Sql("DELETE FROM VehicleTypes", true);
        }
    }
}
