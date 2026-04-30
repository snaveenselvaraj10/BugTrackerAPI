using BugTrackerAPI.DTOs;
using BugTrackerAPI.Model;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;



namespace BugTrackerAPI.Services
{
    public class BugService
    {
        private readonly string _connectionString;

        public BugService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new Exception("Connection string not found");
        }

        // 🔹 Create Bug using Stored Procedure
        public void CreateBug(CreateBugDto dto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Execute(
                    "sp_CreateBug",
                    new
                    {
                        Title = dto.Title,
                        Description = dto.Description,
                        Severity = dto.Severity,
                        CreatedBy = dto.CreatedBy
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public IEnumerable<Bug> GetAllBugs()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Bug>("SELECT * FROM Bugs");
            }
        }

        public void UpdateStatus(int bugId, string status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Execute(
                    "UPDATE Bugs SET Status = @Status WHERE BugId = @BugId",
                    new { Status = status, BugId = bugId }
                );
            }
        }

        public int GetOpenBugCount(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.ExecuteScalar<int>(
                    "SELECT dbo.fn_OpenBugsByUser(@UserId)",
                    new { UserId = userId }
                );
            }
        }

        public void DeleteBug(int bugId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Execute(
                    "DELETE FROM Bugs WHERE BugId = @BugId",
                    new { BugId = bugId }
                );
            }
        }
    }
}
