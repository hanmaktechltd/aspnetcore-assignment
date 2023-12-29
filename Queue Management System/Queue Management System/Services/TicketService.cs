using System.Drawing;
using System.Text;
using FastReport;
using FastReport.Export.Pdf;
using FastReport.Utils;
using Npgsql;
using Queue_Management_System.Models;

public class TicketService : ITicketService
{

    private readonly string _connectionString;

    private readonly ILogger<TicketService> _logger;

    public TicketService(IConfiguration configuration, ILogger<TicketService> logger)
    {

        _connectionString = configuration.GetConnectionString("DefaultConnection");

        _logger = logger;
    }

    public byte[] GenerateTicket(WaitingPageViewModel waitingPageData)
    {

        try
        {

            Report report = new Report();

            ReportPage page = new ReportPage();
            page.Name = "ReportPage1";
            page.CreateUniqueName();
            page.PaperWidth = 100;
            page.PaperHeight = 100;

            report.Pages.Add(page);

            ReportTitleBand reportTitle = new ReportTitleBand();
            reportTitle.Name = "ReportTitle1";
            reportTitle.Height = Units.Millimeters * 10;

            TextObject titleText = new TextObject();
            titleText.Name = "Text1";
            titleText.Bounds = new RectangleF(0, 0, Units.Millimeters * 100, Units.Millimeters * 5);
            titleText.Text = "Ticket";

            reportTitle.Objects.Add(titleText);
            page.ReportTitle = reportTitle;

            DataBand dataBand = new DataBand();
            dataBand.Name = "Data1";
            dataBand.Height = Units.Millimeters * 10;

            page.Bands.Add(dataBand);

            TextObject ticketNumberLabel = new TextObject();
            ticketNumberLabel.Name = "TicketNumberLabel";
            ticketNumberLabel.Bounds = new RectangleF(0, 0, Units.Millimeters * 50, Units.Millimeters * 5);
            ticketNumberLabel.Text = "Ticket Number:";
            dataBand.Objects.Add(ticketNumberLabel);

            TextObject ticketNumberData = new TextObject();
            ticketNumberData.Name = "TicketNumber";
            ticketNumberData.Bounds = new RectangleF(Units.Millimeters * 50, 0, Units.Millimeters * 50, Units.Millimeters * 5);
            ticketNumberData.Text = waitingPageData.TicketNumber.ToString();
            dataBand.Objects.Add(ticketNumberData);

            TextObject servicePointLabel = new TextObject();
            servicePointLabel.Name = "ServicePointLabel";
            servicePointLabel.Bounds = new RectangleF(0, Units.Millimeters * 5, Units.Millimeters * 50, Units.Millimeters * 5);
            servicePointLabel.Text = "Service Point:";
            dataBand.Objects.Add(servicePointLabel);

            TextObject servicePointData = new TextObject();
            servicePointData.Name = "ServicePointName";
            servicePointData.Bounds = new RectangleF(Units.Millimeters * 50, Units.Millimeters * 5, Units.Millimeters * 50, Units.Millimeters * 5);
            servicePointData.Text = waitingPageData.ServicePointName;
            dataBand.Objects.Add(servicePointData);

            TextObject issueTimeLabel = new TextObject();
            issueTimeLabel.Name = "IssueTimeLabel";
            issueTimeLabel.Bounds = new RectangleF(0, Units.Millimeters * 10, Units.Millimeters * 50, Units.Millimeters * 5);
            issueTimeLabel.Text = "Issue Time:";
            dataBand.Objects.Add(issueTimeLabel);

            TextObject issueTimeData = new TextObject();
            issueTimeData.Name = "IssueTime";
            issueTimeData.Bounds = new RectangleF(Units.Millimeters * 50, Units.Millimeters * 10, Units.Millimeters * 50, Units.Millimeters * 5);
            issueTimeData.Text = waitingPageData.IssueTime.ToString();
            dataBand.Objects.Add(issueTimeData);


            var result = report.Prepare();

            _logger.LogInformation("report prepared: {result}", result);

            var pdfExport = new PDFExport();

            MemoryStream memoryStream = new MemoryStream();

            report.Export(pdfExport, memoryStream);

            string memoryStreamContent = Encoding.UTF8.GetString(memoryStream.ToArray());

            var pdfBytes = memoryStream.ToArray();

            return pdfBytes;

        }
        catch (Exception ex)
        {

            Console.WriteLine($"Exception occurred in GenerateTicket: {ex.Message}");

            throw;

        }

    }


    public Ticket SaveTicketToDatabase(CheckinViewModel checkinData)
    {
        var ticket = new Ticket();

        using (var connection = new NpgsqlConnection(_connectionString))
        {

            connection.Open();

            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;

                command.CommandText = @"
                INSERT INTO Ticket (IssueTime, ServicePointId)
                VALUES (CURRENT_TIMESTAMP, @ServicePointId)
                RETURNING TicketId, IssueTime, ServicePointId;
            ";

                command.Parameters.AddWithValue("@ServicePointId", checkinData.SelectedServicePointId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ticket.TicketId = reader.GetInt32(0);
                        //ticket.TicketNumber = reader.GetInt32(0);
                        //ticket.ServicePointName = reader.GetString(1);
                        ticket.IssueTime = reader.GetDateTime(1);
                        ticket.ServicePointId = reader.GetInt32(reader.GetOrdinal("ServicePointId"));
                    }
                }
            }
        }


        return ticket;
    }




    public Ticket RetrieveTicketFromDatabase()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;

                command.CommandText = @"
                SELECT * FROM Ticket ORDER BY TicketId DESC LIMIT 1;
            ";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Ticket
                        {
                            TicketId = reader.GetInt32(reader.GetOrdinal("TicketId")),
                            //TicketNumber = reader.GetInt32(reader.GetOrdinal("TicketId")),
                            //ServicePoint = reader.GetString(reader.GetOrdinal("ServiceType"))
                        };
                    }
                }
            }
        }

        return null;
    }


     public void MarkAsNoShow(int ticketId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            Ticket ticket = GetTicketById(ticketId);

    
            if (ticket != null)
            {
                
                UpdateTicketStatus(ticket.TicketId, "No Show");
            }

              _logger.LogInformation("Ticket {TicketId} marked as No Show.", ticketId);
        
        }
    }

    public void MarkAsFinished(int ticketId)
{
    using (var connection = new NpgsqlConnection(_connectionString))
    {
        connection.Open();

        Ticket ticket = GetTicketById(ticketId);
        if (ticket != null)
        {
            UpdateTicketStatus(ticket.TicketId, "Finished");
            UpdateServiceTime(connection, ticket.TicketId, DateTime.Now);
        }

          _logger.LogInformation("Ticket {TicketId} marked as finished.", ticketId);

    }
}
    private Ticket GetTicketById(int ticketId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {

            connection.Open();
        
        using (var command = new NpgsqlCommand("SELECT * FROM Ticket WHERE TicketId = @TicketId", connection))
        {
            command.Parameters.AddWithValue("@TicketId", ticketId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Ticket
                    {
                        TicketId = (int)reader["TicketId"],
                        IssueTime = (DateTime)reader["IssueTime"],
            
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                         ServicePointId = reader.GetInt32(reader.GetOrdinal("ServicePointId"))
                    };
                }

                return null;
            }
        }
        }
    }



    public void UpdateTicketStatus(int ticketId, string status)
    {
        using (var connection = new NpgsqlConnection(_connectionString))

        {

             connection.Open();
        
        using (var command = new NpgsqlCommand("UPDATE Ticket SET Status = @Status WHERE TicketId = @TicketId", connection))
        {
            command.Parameters.AddWithValue("@TicketId", ticketId);
            command.Parameters.AddWithValue("@Status", status);

            command.ExecuteNonQuery();
        }

         _logger.LogInformation("Ticket {TicketId} marked as Not Served.", ticketId);
        }
    }


    private void UpdateServiceTime(NpgsqlConnection connection, int ticketId, DateTime serviceTime)
{
    using (var command = new NpgsqlCommand("UPDATE Ticket SET ServiceTime = @ServiceTime WHERE TicketId = @TicketId", connection))
    {
        command.Parameters.AddWithValue("@TicketId", ticketId);
        command.Parameters.AddWithValue("@ServiceTime", serviceTime);

        command.ExecuteNonQuery();
    }
}

    public void TransferTicket(int ticketId, int newServicePointId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;

                command.CommandText = "UPDATE Ticket SET ServicePointId = @newServicePointId WHERE TicketId = @ticketId";
                command.Parameters.AddWithValue("newServicePointId", newServicePointId);
                command.Parameters.AddWithValue("ticketId", ticketId);

                command.ExecuteNonQuery();
            }

             _logger.LogInformation("Ticket {ticketId} transferred to {newServicePointId}.", ticketId, newServicePointId);
        }
    }
}
