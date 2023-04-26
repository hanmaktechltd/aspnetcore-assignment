using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Queue_Management_System.Models;
using Queue_Management_System.Services;

namespace Queue_Management_System.Repositories
{
    public class CheckInRepository : ICheckInRepository
    {
        private readonly string _connectionString;
        private IConfiguration _config;
        private NpgsqlConnection _connection;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CheckInRepository(IConfiguration config, IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _hostEnvironment = hostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<FileStreamResult> CheckIn(int servicePointId, int serviceProviderId)
        {
            // Load the FastReport.Net template file dynamically
            string templatePath = $"Reports/Ticket.frx";
            HttpContext httpContext = _httpContextAccessor.HttpContext;
            var webHostEnvironment = httpContext.RequestServices.GetService<IWebHostEnvironment>();
            var physicalPath = Path.Combine(webHostEnvironment.ContentRootPath, templatePath);
            using Report report = new Report();
            report.Load(physicalPath);


            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var ticket = new CustomerTicket
            {
                ServicePointId = servicePointId,
                ServiceProviderId = serviceProviderId,
                CheckInTime = DateTime.UtcNow,
                IsCalled = false,
                NoShow = false,
                Status = "Waiting",
                Completed = false
            };

            string query = "INSERT INTO public.\"Customers\" (\"ServicePointId\", \"ServiceProviderId\", \"CheckInTime\", \"IsCalled\", \"NoShow\", \"Status\", \"Completed\")" +
                            "VALUES(@servicePointId, @serviceProviderId, @checkInTime, @isCalled, @noShow, @status, @completed) RETURNING \"Id\"";


            await using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@servicePointId", ticket.ServicePointId);
                command.Parameters.AddWithValue("@serviceProviderId", ticket.ServiceProviderId);
                command.Parameters.AddWithValue("@checkInTime", ticket.CheckInTime);
                command.Parameters.AddWithValue("@isCalled", ticket.IsCalled);
                command.Parameters.AddWithValue("@noShow", ticket.NoShow);
                command.Parameters.AddWithValue("@status", ticket.Status);
                command.Parameters.AddWithValue("@completed", ticket.Completed);

                var ticketId = (int)command.ExecuteScalar();

                TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                DateTime localCheckInTime = TimeZoneInfo.ConvertTimeFromUtc(ticket.CheckInTime, localTimeZone);

                report.SetParameterValue("TicketNumber", ticketId);
                report.SetParameterValue("CheckInTime", localCheckInTime);
                report.SetParameterValue("ServicePointName", ticket.ServicePointId);

            }


            using (var stream = new MemoryStream())
            using (var export = new PDFSimpleExport())
            {
                // Prepare the report data
                report.Prepare();
                export.Export(report, stream);
                // Set the position of the MemoryStream back to the beginning
                stream.Seek(0, SeekOrigin.Begin);
                // Create a new MemoryStream and copy the contents of the original stream to it
                var outputStream = new MemoryStream(stream.ToArray());

                return new FileStreamResult(outputStream, "application/pdf");
            }
        }

        public async Task<List<CustomerTicket>> Waiting()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var customers = new List<CustomerTicket>();
            string query = $"SELECT * FROM public.\"Customers\" WHERE \"IsCalled\" = true AND \"Status\" = 'In Progress'";
            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var customer = new CustomerTicket
                        {
                            Id = (int)reader["Id"],
                            Status = (string)reader["Status"],
                            ServicePointId = (int)reader["ServicePointId"],
                            IsCalled = (bool)reader["IsCalled"]
                        };

                        customers.Add(customer);
                    }
                }
            }
            if (customers.Count() is 0)
                return null;
            return customers;
        }
    }
}