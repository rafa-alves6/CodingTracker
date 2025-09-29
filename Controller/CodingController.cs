using CodingTracker.Entity;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Configuration;

namespace CodingTracker.Controller
{
    public class CodingController
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public CodingController()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var tableCmd = @"
                CREATE TABLE IF NOT EXISTS CodingSessions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StartTime TEXT NOT NULL,
                    EndTime TEXT NOT NULL
                )";
                connection.Execute(tableCmd);
            }
        }

        public void AddSession(DateTime startTime, DateTime endTime)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sql = "INSERT INTO CodingSessions (StartTime, EndTime) VALUES (@StartTime, @EndTime)";
                connection.Execute(sql, new { StartTime = startTime, EndTime = endTime });
            }
        }

        public List<CodingSession> GetAllSessions()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sql = "SELECT Id, StartTime, EndTime FROM CodingSessions";
                var sessions = connection.Query<CodingSession>(sql);
                return sessions.ToList();
            }
        }

        public CodingSession GetSessionById(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sql = "SELECT * FROM CodingSessions WHERE Id = @Id";
                return connection.QuerySingleOrDefault<CodingSession>(sql, new { Id = id })!;
            }
        }

        public void DeleteSession(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sql = "DELETE FROM CodingSessions WHERE Id = @Id";
                connection.Execute(sql, new { Id = id });
            }
        }

        public void UpdateSession(CodingSession session)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sql = @"
                    UPDATE CodingSessions 
                    SET StartTime = @StartTime, EndTime = @EndTime 
                    WHERE Id = @Id";
                connection.Execute(sql, session);
            }
        }
    }
}