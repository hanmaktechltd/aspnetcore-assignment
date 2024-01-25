using Queue_Management_System.Models;
using FastReport;
using FastReport.Web;

namespace Queue_Management_System.Services
{
    public interface IReportService
    {
        WebReport GenerateTicketReport(string ticketNumber, string serviceId, DateTime TimePrinted);

        WebReport GenerateAnalyticalReport(IEnumerable<ServicePointAnalyticModel> analytics);
    }

    public class ReportService : IReportService
    {
        public WebReport GenerateTicketReport(string ticketNumber, string serviceId, DateTime TimePrinted)
        {
            var report = new WebReport();
            report.Report.Load("Reports/Ticket.frx");
            report.Report.SetParameterValue("TicketNo", ticketNumber);
            report.Report.SetParameterValue("serviceID", serviceId);
            report.Report.SetParameterValue("printTime", TimePrinted);

            return report;
        }

        public WebReport GenerateAnalyticalReport(IEnumerable<ServicePointAnalyticModel> analytics)
        {
            var report = new WebReport();
            report.Report.Load("Reports/ServicePointsAnalytics.frx");
            report.Report.Dictionary.RegisterData(analytics.ToList(), "analytics", true);
            DataBand db1 = (DataBand)report.Report.FindObject("Data1");
            db1.DataSource = report.Report.GetDataSource("analytics");
            report.Report.Save("Reports/ServicePointsAnalytics.frx");

            return report;

        }
    }
}