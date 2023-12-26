using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

using Queue_Management_System.Database;
using Queue_Management_System.Models;

public class ServiceProviderTaskController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseConnection _databaseConnection;

    public ServiceProviderTaskController(IConfiguration configuration, DatabaseConnection databaseConnection)
    {
        _configuration = configuration;
        _databaseConnection = databaseConnection;
    }

    [HttpGet]
    public IActionResult GetNextCustomerOnQueue()
    {
        // Get the last customer in the queue and assign a service point
        var lastCustomer = GetLastCustomerInQueue();
        if (lastCustomer == null)
        {
            // No customers in the queue
            ViewBag.Message = "No customers in the queue.";
            return View();
        }

        // Assign a service point
        var servicePoints = GetServicePoints();

        // Convert the list of ServicePoint to a list of SelectListItem
        var servicePointItems = servicePoints.Select(sp => new SelectListItem
        {
            Value = sp.Id.ToString(),
            Text = sp.ServicePointName
        }).ToList();

        ViewBag.ServicePoints = servicePointItems;

        return View(lastCustomer);
    }


    [HttpPost]
    public IActionResult AssignServicePoint(Customer customer)
    {
        if (customer.ServicePointId.HasValue)
        {
            UpdateCustomerServicePoint(customer.Id, customer.ServicePointId.Value);
        }

        return RedirectToAction("GetNextCustomerOnQueue");
    }


    [HttpGet]
    public IActionResult ViewLatestCustomerQueueInfo()
    {
        var lastUpdatedCustomer = GetLastUpdatedCustomer();
        return View(lastUpdatedCustomer);
    }









    // Methods










    private Customer GetLastCustomerInQueue()
    {
        using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
        {
            string selectQuery = "SELECT * FROM Customers ORDER BY Id DESC LIMIT 1;";
            using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Customer
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            QueueNumber = Convert.ToInt32(reader["QueueNumber"]),
                            ServicePointId = Convert.ToInt32(reader["ServicePointId"])
                        };
                    }
                }
            }
        }

        return null;
    }

    private List<ServicePoint> GetServicePoints()
    {
        List<ServicePoint> servicePoints = new List<ServicePoint>();

        using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
        {
            string selectQuery = "SELECT * FROM ServicePoints;";
            using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ServicePoint servicePoint = new ServicePoint
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            ServicePointName = reader["ServicePointName"].ToString(),
                            ServedBy = reader["ServedBy"].ToString()
                        };

                        servicePoints.Add(servicePoint);
                    }
                }
            }
        }

        return servicePoints;
    }

    private void IncrementQueueNumber()
    {
        using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
        {
            string updateQuery = "UPDATE Customers SET QueueNumber = QueueNumber + 1 WHERE QueueNumber = (SELECT MIN(QueueNumber) FROM Customers);";
            using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    private Customer GetCustomerById(int customerId)
    {
        using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
        {
            string selectQuery = "SELECT c.*, s.ServicePointName FROM Customers c LEFT JOIN ServicePoints s ON c.ServicePointId = s.Id WHERE c.Id = @CustomerId;";
            using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue("@CustomerId", customerId);

                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Customer
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            QueueNumber = Convert.ToInt32(reader["QueueNumber"]),
                            ServicePointId = Convert.ToInt32(reader["ServicePointId"]),
                            ServicePoint = new ServicePoint
                            {
                                Id = Convert.ToInt32(reader["ServicePointId"]),
                                ServicePointName = reader["ServicePointName"].ToString(),
                                ServedBy = reader["ServedBy"].ToString()
                            }
                        };
                    }
                }
            }
        }

        return null;
    }

    private void UpdateCustomerServicePoint(int customerId, int servicePointId)
    {
        using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
        {
            string updateQuery = "UPDATE Customers SET ServicePointId = @ServicePointId WHERE Id = @CustomerId;";
            using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@ServicePointId", servicePointId);
                command.Parameters.AddWithValue("@CustomerId", customerId);

                command.ExecuteNonQuery();
            }
        }
    }

    private Customer GetLastUpdatedCustomer()
    {
        using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
        {
            string selectQuery = "SELECT * FROM Customers ORDER BY Id DESC LIMIT 1;";
            using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Customer
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            QueueNumber = Convert.ToInt32(reader["QueueNumber"]),
                            ServicePointId = Convert.ToInt32(reader["ServicePointId"])
                        };
                    }
                }
            }
        }

        return null;
    }

}




