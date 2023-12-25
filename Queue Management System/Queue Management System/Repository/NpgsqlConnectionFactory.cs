using Npgsql;
using System;
using System.Data;

namespace Queue_Management_System.Repository
{
    public class NpgsqlConnectionFactory : IDisposable
    {
        private readonly string _connectionString;
        private NpgsqlConnection _connection;

        public NpgsqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public NpgsqlConnection CreateConnection()
        {
            _connection = new NpgsqlConnection(_connectionString);
            return _connection;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }

        // Other methods for executing queries can be added here
        public DataTable ExecuteQuery(string query)
        {
            using var connection = CreateConnection();
            DataTable dataTable = new DataTable();
            try
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(cmd))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing query: " + ex.Message);
            }
            return dataTable;
        }
    }
}
