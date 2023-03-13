using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Queue_Management_System.Data;
using Queue_Management_System.Migrations;
using Queue_Management_System.Models;
using System.Data.Entity.Core.Objects;
using System.Net;

namespace Queue_Management_System.Repository
{
    public class QueueRepository
    {
        private readonly QueueDBContext _context;

        public QueueRepository(QueueDBContext context)
        {
            _context = context;
        }

        //savechanges

        public async Task<int> SaveChanges()
        {
            return (await _context.SaveChangesAsync());
        }

        //servicePoints
        public async Task<IActionResult> CreateServicePoint(ServicePointModel servicePoint)
        {
            if (servicePoint == null)
            {
                throw new ArgumentNullException(nameof(servicePoint));
            }
           var result= await  _context.servicePoints.AddAsync(servicePoint);
            return null;
        }

        public IEnumerable<ServicePointModel> getServicePoints()
        {
            var servicePoints= _context.servicePoints.ToList();
            return servicePoints;
        }
        public ServicePointModel getServicePointById(int id)
        {
            return _context.servicePoints.FirstOrDefault(s => s.Id == id);
        }

        //customers
        public int CountCustomersinQueueByServicePoint(int servicePointId)
        {
            return _context.customers.Count(c => c.ServicePoint.Id == servicePointId && c.Status.ToUpper()=="WAITING");
        }

        public int CountTotalCustomersServed()
        {
            return _context.customers.Count();
        }
        public async Task<IActionResult> CreateCustomer(Customers customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            var result = await _context.customers.AddAsync(customer);
            return null;
        }
        public List<Customers> GetCustomersinQueueByServicePoint(int? servicePointId)
        {
            return _context.customers.Where(c => c.ServicePoint.Id == servicePointId && c.Status.ToUpper()=="WAITING").ToList();
        }
        public Customers GetCurrentCustomer(int? servicePointId)
        {
            var CurrentCustomer = _context.customers.FirstOrDefault(c => c.ServicePoint.Id == servicePointId && c.Status.ToUpper() == "WAITING");
            if (CurrentCustomer == null)
            {
                CurrentCustomer = new Customers { Name = "There are no customers waiting in the Queue" };
            }
            return CurrentCustomer;
        }
        public Customers GetCustomerById(int Id)
        {
            return _context.customers.Where(c => c.Id == Id).FirstOrDefault();
        }
        public void updateCustomerStatus(Customers customer)
        {
            var customerToUpdate = _context.customers.Find(customer.Id);
            if (customerToUpdate != null)
            {
                customerToUpdate.Status = customer.Status;
                customerToUpdate.TimeOut = DateTime.Now.ToUniversalTime();
                _context.SaveChanges();
            }
        }
        public void TransferCustomerStatus(Customers customer)
        {
            var customerToUpdate = _context.customers.Find(customer.Id);
            if (customerToUpdate != null)
            {
                customerToUpdate.ServicePoint = customer.ServicePoint;
                _context.SaveChanges();
            }
        }
        public Customers RecallCustomer(string TicketNumber)
        {
            var CurrentCustomer = _context.customers.FirstOrDefault(c => c.TicketNumber == TicketNumber);
            if (CurrentCustomer == null)
            {
                CurrentCustomer = new Customers { Name = "There are no customers waiting in the Queue" };
            }
            return CurrentCustomer;
        }
        public int CountServedCustomersByServicePoint(int servicePointId)
        {
            return _context.customers.Count(c => c.ServicePoint.Id == servicePointId && c.Status.ToUpper() != "WAITING");
        }
        public double CalculateAverageWaitTime(FilterModel filter)
        {
            return _context.customers.Where(c=>c.Status.ToUpper()!="WAITING" && c.ServicePoint.Id==filter.ServicePointId && (c.TimeIn>filter.StartDate.ToUniversalTime() && c.TimeIn<filter.EndDate.ToUniversalTime()))
        .Sum(c => (c.TimeOut - c.TimeIn).TotalMinutes);
        }
    }
}
