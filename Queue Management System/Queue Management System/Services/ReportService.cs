using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using FastReport;
using FastReport.Export.Pdf;
using FastReport.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
            DisplayTextObject(dataBand, "TotalTicketsLabel", 0, 0, Units.Millimeters * 50, Units.Millimeters * 5, "Total Tickets Issued:");
            DisplayTextObject(dataBand, "TotalTicketsData", Units.Millimeters * 50, 0, Units.Millimeters * 50, Units.Millimeters * 5, totalTicketsIssued.ToString());

            DisplayTextObject(dataBand, "AverageTimesHeadline", 0, dataBand.Height, Units.Millimeters * 100, Units.Millimeters * 5, "AVG Waiting Times:", 8, FontStyle.Bold);
            dataBand.Height += Units.Millimeters * 5;

            Dictionary<string, double> averageWaitingTimes = FindAverageWaitingTimePerServicePoint();
            foreach (var kvp in averageWaitingTimes)
            {
                string servicePointName = kvp.Key;
                double avgWaitTimeSeconds = kvp.Value;

                int minutes = (int)avgWaitTimeSeconds / 60;
                int seconds = (int)avgWaitTimeSeconds % 60;

                string formattedAvgWaitTime = $"{minutes} min {seconds} sec";

                DisplayTextObject(dataBand, $"{servicePointName}Name", 0, dataBand.Height, Units.Millimeters * 50, Units.Millimeters * 5, servicePointName);
                DisplayTextObject(dataBand, $"{servicePointName}AvgWaitTime", Units.Millimeters * 50, dataBand.Height, Units.Millimeters * 50, Units.Millimeters * 5, formattedAvgWaitTime);

                dataBand.Height += Units.Millimeters * 10;
            }

            var result = report.Prepare();
            _logger.LogInformation("Report prepared: {result}");

            var pdfExport = new PDFExport();
            MemoryStream memoryStream = new MemoryStream();
            report.Export(pdfExport, memoryStream);

            var pdfBytes = memoryStream.ToArray();
            return pdfBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception occurred in GenerateReport: {ex.Message}");
            throw;
        }
    }

    private void DisplayTextObject(DataBand dataBand, string name, float x, float y, float width, float height, string text, float fontSize = 0, FontStyle fontStyle = FontStyle.Regular)
    {
        TextObject textObject = new TextObject();
        textObject.Name = name;
        textObject.Bounds = new RectangleF(x, y, width, height);
        textObject.Text = text;
        if (fontSize > 0)
        {
            textObject.Font = new Font("Arial", fontSize, fontStyle);
        }
        dataBand.Objects.Add(textObject);
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

    private Dictionary<string, double> FindAverageWaitingTimePerServicePoint()
    {
        Dictionary<string, double> averageWaitingTimes = new Dictionary<string, double>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand("SELECT ServicePoint, AVG(EXTRACT(EPOCH FROM (ServiceTime - IssueTime))) AS AvgWaitTime " +
                                                   "FROM Ticket " +
                                                   "WHERE Status = 'Finished' " +
                                                   "GROUP BY ServicePoint", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string servicePoint = reader["ServicePoint"].ToString();
                        double avgWaitTime = Convert.ToDouble(reader["AvgWaitTime"]);

                        averageWaitingTimes.Add(servicePoint, avgWaitTime);
                    }
                }
            }
        }

        return averageWaitingTimes;
    }
}
