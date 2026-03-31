using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeManagerWebApi.Data.DatabaseChanges
{
    /// <inheritdoc />
    public partial class DeleteUniqueConstrainUserDocumentFileName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserDocument_FileName",
                table: "UserDocument");

            migrationBuilder.CreateIndex(
                name: "IX_UserDocument_FileName",
                table: "UserDocument",
                column: "FileName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserDocument_FileName",
                table: "UserDocument");

            migrationBuilder.CreateIndex(
                name: "IX_UserDocument_FileName",
                table: "UserDocument",
                column: "FileName",
                unique: true);
        }
    }
}
