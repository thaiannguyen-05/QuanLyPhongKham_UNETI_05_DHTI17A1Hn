using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data.Migrations
{
    /// <inheritdoc />
    public partial class BookingUniquePerSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_PatientId",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PatientId_ScheduleId",
                table: "Bookings",
                columns: new[] { "PatientId", "ScheduleId" },
                unique: true,
                filter: "[Status] <> 3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_PatientId_ScheduleId",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PatientId",
                table: "Bookings",
                column: "PatientId");
        }
    }
}
