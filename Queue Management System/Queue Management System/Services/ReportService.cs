using FastReport.Web;

namespace Queue_Management_System.Services
{
    public interface IReportService
    {
        WebReport GenerateTicketReport(string ticketNumber, string serviceId, DateTime TimePrinted);
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
    }
}