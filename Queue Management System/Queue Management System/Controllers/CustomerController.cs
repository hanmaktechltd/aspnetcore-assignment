using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

using Queue_Management_System.Database;
using Queue_Management_System.Models;

public class CustomerController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseConnection _databaseConnection;

    public CustomerController(IConfiguration configuration, DatabaseConnection databaseConnection)
    {
        _configuration = configuration;
        _databaseConnection = databaseConnection;
    }

    [HttpGet]
    public IActionResult CustomerOnTheQueue()
    {
        // Add a new customer to the queue with a default service point
        var customer = AddCustomerToQueue();

        // Redirect to the view page
        return View(customer);
    }

    [HttpGet]
    public IActionResult ViewTicketInfo(int customerId)
    {
        // Retrieve customer information for display
        var customer = GetCustomerById(customerId);

        // Retrieve default service point information
        var defaultServicePoint = GetDefaultServicePoint();

        // Set the default service point for display
        customer.ServicePoint = defaultServicePoint;

        return View(customer);
    }






    // Methods








    private Customer AddCustomerToQueue()
    {
        using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
        {
            // Retrieve default service point information
            var defaultServicePoint = GetDefaultServicePoint();

            // Insert the new customer into the queue with the default service point
            string insertQuery = "INSERT INTO Customers (QueueNumber, ServicePointId) VALUES ((SELECT COALESCE(MAX(QueueNumber), 0) + 1 FROM Customers), @ServicePointId) RETURNING *;";
            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@ServicePointId", defaultServicePoint.Id);

                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Customer
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            QueueNumber = Convert.ToInt32(reader["QueueNumber"]),
                            ServicePointId = defaultServicePoint.Id,
                            ServicePoint = defaultServicePoint
                        };
                    }
                }
            }
        }

        return null;
    }

    private ServicePoint GetDefaultServicePoint()
    {
        using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
        {
            string selectQuery = "SELECT * FROM ServicePoints ORDER BY Id LIMIT 1;";
            using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ServicePoint
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            ServicePointName = reader["ServicePointName"].ToString(),
                            ServedBy = reader["ServedBy"].ToString()
                        };
                    }
                }
            }
        }

        return null;
    }

    private Customer GetCustomerById(int customerId)
    {
        using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
        {
            string selectQuery = "SELECT * FROM Customers WHERE Id = @CustomerId;";
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
                            // CustomerName = reader["CustomerName"].ToString(),
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



