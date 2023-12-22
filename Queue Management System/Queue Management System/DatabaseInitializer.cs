using System;
using Npgsql;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void InitializeDatabase()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;

                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Ticket (
                        TicketId SERIAL PRIMARY KEY,
                        ServiceType VARCHAR(255) NOT NULL,
                        IssueTime TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                    );
                ";

                command.ExecuteNonQuery();
            }
        }

        Console.WriteLine("Database initialization complete.");
    }
}
