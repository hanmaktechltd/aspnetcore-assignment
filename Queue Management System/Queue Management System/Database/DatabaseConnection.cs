using System;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Queue_Management_System.Database
{
    public class DatabaseConnection
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DatabaseConnection(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public NpgsqlConnection OpenConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public void CloseConnection(NpgsqlConnection connection)
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }

}
