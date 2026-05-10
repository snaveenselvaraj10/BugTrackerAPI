using BugTrackerAPI.DTOs;
using BugTrackerAPI.Model;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BugTrackerAPI.Services
{
    public class CommentService
    {
        private readonly string _connectionString;

        public CommentService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new Exception("Connection string not found");
        }

        public Comment CreateComment(CreateCommentDto dto, int createdBy)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
INSERT INTO Comments (BugId, CommentText, CreatedBy, CreatedDate, ParentCommentId, IsDeleted, Edited)
OUTPUT INSERTED.*
VALUES (@BugId, @CommentText, @CreatedBy, GETUTCDATE(), @ParentCommentId, 0, 0);
";

            var created = connection.QuerySingle<Comment>(sql, new
            {
                BugId = dto.BugId,
                CommentText = dto.CommentText,
                CreatedBy = createdBy,
                ParentCommentId = dto.ParentCommentId
            });

            return created;
        }

        public (IEnumerable<Comment> items, int total) GetCommentsByBug(int bugId, int page = 1, int pageSize = 20)
        {
            using var connection = new SqlConnection(_connectionString);
            var offset = (page - 1) * pageSize;

            var sql = @"
SELECT * FROM Comments WHERE BugId = @BugId AND IsDeleted = 0 ORDER BY CreatedDate DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
SELECT COUNT(1) FROM Comments WHERE BugId = @BugId AND IsDeleted = 0;
";

            using var multi = connection.QueryMultiple(sql, new { BugId = bugId, Offset = offset, PageSize = pageSize });
            var items = multi.Read<Comment>();
            var total = multi.ReadFirst<int>();
            return (items, total);
        }

        public void UpdateComment(int commentId, string newText, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            // ensure only author can edit - simple check
            var owner = connection.QuerySingleOrDefault<int?>("SELECT CreatedBy FROM Comments WHERE CommentId = @Id", new { Id = commentId });
            if (owner == null) throw new Exception("Comment not found");
            if (owner != userId) throw new Exception("Not authorized to edit this comment");

            connection.Execute(
                "UPDATE Comments SET CommentText = @Text, Edited = 1, EditedDate = GETUTCDATE() WHERE CommentId = @Id",
                new { Text = newText, Id = commentId }
            );
        }

        public void DeleteComment(int commentId, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            var owner = connection.QuerySingleOrDefault<int?>("SELECT CreatedBy FROM Comments WHERE CommentId = @Id", new { Id = commentId });
            if (owner == null) throw new Exception("Comment not found");
            if (owner != userId) throw new Exception("Not authorized to delete this comment");

            connection.Execute(
                "UPDATE Comments SET IsDeleted = 1 WHERE CommentId = @Id",
                new { Id = commentId }
            );
        }
    }
}
