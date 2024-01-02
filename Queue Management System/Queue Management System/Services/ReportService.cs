using System.Drawing;
using System.Text;
using FastReport;
using FastReport.Export.Pdf;
using FastReport.Utils;
using Npgsql;
using Queue_Management_System.Models;

public class ReportService : IReportService
{

    private readonly string _connectionString;

    private readonly ILogger<ReportService> _logger;

    public ReportService(IConfiguration configuration, ILogger<ReportService> logger)
    {

        _connectionString = configuration.GetConnectionString("DefaultConnection");

        _logger = logger;
    }

     public byte[] GenerateReport()
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

            DataBand dataBand = new DataBand();
            dataBand.Name = "Data1";
            dataBand.Height = Units.Millimeters * 10;

            page.Bands.Add(dataBand);

            int totalTicketsIssued = FindTotalTicketsIssued();

            TextObject totalTicketsLabel = new TextObject();
            totalTicketsLabel.Name = "TotalTicketsLabel";
            totalTicketsLabel.Bounds = new RectangleF(0, 0, Units.Millimeters * 50, Units.Millimeters * 5);
            totalTicketsLabel.Text = "Total Tickets Issued:";
            dataBand.Objects.Add(totalTicketsLabel);

            TextObject totalTicketsData = new TextObject();
            totalTicketsData.Name = "TotalTicketsData";
            totalTicketsData.Bounds = new RectangleF(Units.Millimeters * 50, 0, Units.Millimeters * 50, Units.Millimeters * 5);
            totalTicketsData.Text = totalTicketsIssued.ToString();
            dataBand.Objects.Add(totalTicketsData);


            var result = report.Prepare();

            _logger.LogInformation("report prepared: {result}");

            var pdfExport = new PDFExport();

            MemoryStream memoryStream = new MemoryStream();

            report.Export(pdfExport, memoryStream);

            var pdfBytes = memoryStream.ToArray();

            return pdfBytes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred in GenerateReport: {ex.Message}");
            throw;
        }
    }

     private int FindTotalTicketsIssued()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand("SELECT COUNT(*) FROM Ticket", connection))
            {
                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int count))
                {
                    return count;
                }
            }
        }
        return 0;
    }
}
