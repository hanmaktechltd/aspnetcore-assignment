using Queue_Management_System.Models;
using Npgsql;

namespace Queue_Management_System.Repositories
{
    public interface IServicePointRepository
    {
        Task<ServicePointModel> GetServicePointById(string id);

        Task<ServicePointModel> GetServicePointByServiceProviderId(string serviceProviderId);

        Task<IEnumerable<ServicePointModel>> GetServicePoints();

        Task AddServicePoint(ServicePointModel servicePoint);

        Task UpdateServicePoint(ServicePointModel servicePoint);

        Task DeleteServicePoint(string id);
    }



    /*public class MockServicePointRepository : IServicePointRepository
    {
        public ServicePointModel GetServicePointById(string id)
        {
            return new ServicePointModel(){
                Id = id,
                Description = "Mock Service Point for GetServicePointById Method",
                ServiceProviderId = null
            };
        }

        public ServicePointModel GetServicePointByProviderId(string serviceProviderId)
        {
            return new ServicePointModel(){
                Id = "MockID_ServicePoint_009",
                Description = "Mock Service Point for GetServicePointByProviderId Method",
                ServiceProviderId = serviceProviderId
            }; 
        }

        public IEnumerable<ServicePointModel> GetServicePoints()
        {
            ServicePointModel SamplePoint1 = new ServicePointModel(){
                Id = "SP_001",
                Description = "Mock Service Point 1",
                ServiceProviderId = null
            };

            ServicePointModel SamplePoint2 = new ServicePointModel(){
                Id = "SP_002",
                Description = "Mock Service Point 2",
                ServiceProviderId = null
            };

            ServicePointModel SamplePoint3 = new ServicePointModel(){
                Id = "SP_003",
                Description = "Mock Service Point 3",
                ServiceProviderId = null
            };

            return new List<ServicePointModel> (){
                SamplePoint1, SamplePoint2, SamplePoint3
            };
        }

        public void AddServicePoint(ServicePointModel servicePoint)
        {

        }

        public void UpdateServicePoint(ServicePointModel servicePoint)
        {

        }

        public void DeleteServicePoint(string id)
        {

        }
    }*/

    public class ServicePointRepository : IServicePointRepository
    {
        private readonly IConfiguration _configuration;
        
        private readonly NpgsqlDataSource dataSource;

        public ServicePointRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            string connectionString = _configuration.GetConnectionString("qmsdb");
            dataSource = NpgsqlDataSource.Create(connectionString);
        }

        public async Task<ServicePointModel> GetServicePointById(string id)
        {
            
            await using var connection = await dataSource.OpenConnectionAsync();
            //string querystring = $"SELECT * FROM services WHERE id='{id}'";
            await using var command = new NpgsqlCommand("SELECT * FROM service_points WHERE id=$1", connection)
            {
                Parameters = 
                {
                    new() {Value = id}
                }
            };
            await using var reader = await command.ExecuteReaderAsync();

            ServicePointModel? servicePoint = null;
            while (await reader.ReadAsync())
            {
                servicePoint = new ServicePointModel
                {
                    Id = reader.GetString(0),
                    Description = reader.IsDBNull(1) ? null : reader.GetString(1), //todo change description field of spoint model to serviceid
                    ServiceProviderId = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
            }

            return servicePoint;

        }

        public async Task<ServicePointModel> GetServicePointByServiceProviderId(string serviceProviderId)
        {
            
            await using var connection = await dataSource.OpenConnectionAsync();
            //string querystring = $"SELECT * FROM services WHERE id='{id}'";
            await using var command = new NpgsqlCommand("SELECT * FROM service_points WHERE service_provider_id=$1", connection)
            {
                Parameters = 
                {
                    new() {Value = serviceProviderId}
                }
            };
            await using var reader = await command.ExecuteReaderAsync();

            ServicePointModel? servicePoint = null;// = new ServicePointModel();
            while (await reader.ReadAsync())
            {
                servicePoint = new ServicePointModel
                {
                    Id = reader.GetString(0),
                    Description = reader.GetString(1), //todo change description field of spoint model to serviceid
                    ServiceProviderId = reader.GetString(2)
                };
            }

            return servicePoint;

        }

        public async Task<IEnumerable<ServicePointModel>> GetServicePoints()
        {
            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("SELECT * FROM service_points", connection);
            await using var reader = await command.ExecuteReaderAsync();

            var servicePoints = new List<ServicePointModel>();
            while (await reader.ReadAsync())
            {
                var servicePoint = new ServicePointModel
                {
                    Id = reader.GetString(0),
                    Description = reader.IsDBNull(1) ? null : reader.GetString(1),
                    ServiceProviderId = reader.IsDBNull(2) ? null : reader.GetString(2)
                };

                servicePoints.Add(servicePoint);
            }

            return servicePoints;
        }

        public async Task AddServicePoint(ServicePointModel servicePoint)
        {
            string id = servicePoint.Id;
            string? description = servicePoint.Description;
            string? serviceProviderId = servicePoint.ServiceProviderId;

            await using var connection = await dataSource.OpenConnectionAsync();
            //string querystring = $"INSERT INTO services (id, description) VALUES ('{id}', '{description}')";
            await using var command = new NpgsqlCommand("INSERT INTO service_points (id, service_id, service_provider_id) VALUES ($1, $2, $3)", connection)
            {
                Parameters = 
                {
                    new() {Value = id},
                    new() {Value = description ?? (object)DBNull.Value},
                    new() {Value = serviceProviderId ?? (object)DBNull.Value}
                }
            };
            await command.ExecuteNonQueryAsync();

        }

        public async Task UpdateServicePoint(ServicePointModel servicePoint)
        {
            string id = servicePoint.Id;
            string description = servicePoint.Description;
            string? serviceProviderId = servicePoint.ServiceProviderId;

            await using var connection = await dataSource.OpenConnectionAsync();
            //string querystring = $"INSERT INTO services (id, description) VALUES ('{id}', '{description}')";
            await using var command = new NpgsqlCommand("UPDATE service_points SET (service_id, service_provider_id) = ($2, $3) WHERE id = $1", connection)
            {
                Parameters = 
                {
                    new() {Value = id},
                    new() {Value = description ?? (object)DBNull.Value},
                    new() {Value = serviceProviderId ?? (object)DBNull.Value}
                }
            };
            await command.ExecuteNonQueryAsync();

        }

        public async Task DeleteServicePoint(string id)
        {
            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("DELETE FROM service_points WHERE id = $1", connection)
            {
                Parameters = 
                {
                    new() {Value = id}
                }
            };
            await command.ExecuteNonQueryAsync();
        }
    
    }
}



