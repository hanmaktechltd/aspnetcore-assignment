using Npgsql;
using NuGet.Protocol.Plugins;
using Queue_Management_System.Contracts;
using Queue_Management_System.Models;

namespace Queue_Management_System.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private const string CONNECTION_STRING = "Host=localhost:5432;" +
                          "Username=postgres;" +
                          "Password=*mikemathu;" +
                          "Database=QMS";

        private const string TABLE_NAME = "users";
        private const string TABLE_NAME2 = "servicepoints";

        private NpgsqlConnection connection;

        public AdminRepository()
        {
            connection = new NpgsqlConnection(CONNECTION_STRING);
            connection.Open();
        }
        public Task Add(AdminVM game)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<AdminVM> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ServiceProviderVM>> GetServiceProviders()
        {
            List<ServiceProviderVM> games = new List<ServiceProviderVM>();

            string commandText = $"SELECT * FROM {TABLE_NAME}";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, connection))
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    ServiceProviderVM game = ReadBoardGame(reader);
                    games.Add(game);
                }

            return games;
        }

        public async Task<IEnumerable<ServicePointVM>> GetServicePoints()
        {
            List<ServicePointVM> games = new List<ServicePointVM>();

            string commandText = $"SELECT * FROM {TABLE_NAME2}";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, connection))
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    ServicePointVM game = ReadServicePoints(reader);
                    games.Add(game);
                }

            return games;
        }

        public Task Update(int id, AdminVM game)
        {
            throw new NotImplementedException();
        }

        private static ServiceProviderVM ReadBoardGame(NpgsqlDataReader reader)
        {
            int? id = reader["id"] as int?;
            string name = reader["name"] as string;
            string role = reader["role"] as string;
            ServiceProviderVM game = new ServiceProviderVM
            {
                Id = (int)id,
                Name = name,
                Role = role
            };
            return game;
        }

        private static ServicePointVM ReadServicePoints(NpgsqlDataReader reader)
        {
            int? id = reader["id"] as int?;
            string name = reader["name"] as string;
            int? serviceproviderId = reader["serviceproviderId"] as int?;
            ServicePointVM game = new ServicePointVM
            {
                Id = (int)id,
                Name = name,
                ServiceProviderId = (int)serviceproviderId
            };
            return game;
        }
    }
}
