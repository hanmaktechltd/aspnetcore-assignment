using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;

using Queue_Management_System.Database;
using Queue_Management_System.Models;

namespace Queue_Management_System.Controllers
{
    public class ServiceProviderConfigurationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseConnection _databaseConnection;

        public ServiceProviderConfigurationController(IConfiguration configuration, DatabaseConnection databaseConnection)
        {
            _configuration = configuration;
            _databaseConnection = databaseConnection;
        }

        [HttpGet]
        public IActionResult CreateServiceProvider()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateServiceProvider(ServiceProviderConfiguration serviceProvider)
        {
            if (ModelState.IsValid)
            {
                // Save the new service provider to the database
                SaveServiceProviderToDatabase(serviceProvider);

                // Redirect to the list of service providers
                return RedirectToAction("ListofServiceProviders");
            }

            return View(serviceProvider);
        }

        [HttpGet]
        public IActionResult ListofServiceProviders()
        {
            // Retrieve the list of service providers from the database
            var serviceProviders = GetServiceProvidersFromDatabase();

            return View(serviceProviders);
        }


        [HttpGet]
        public IActionResult EditServiceProvider(int id)
        {
            // Retrieve the service provider from the database based on the id
            var serviceProvider = GetServiceProviderById(id);

            if (serviceProvider == null)
            {
                return NotFound();
            }

            return View(serviceProvider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditServiceProvider(int id, ServiceProviderConfiguration serviceProvider)
        {
            if (id != serviceProvider.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Update the service provider in the database
                UpdateServiceProviderInDatabase(serviceProvider);

                return RedirectToAction("ListofServiceProviders");
            }

            return View(serviceProvider);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            // Retrieve the service provider from the database based on the id
            var serviceProvider = GetServiceProviderById(id);

            if (serviceProvider == null)
            {
                return NotFound();
            }

            return View(serviceProvider);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete the service provider from the database
            DeleteServiceProviderFromDatabase(id);

            return RedirectToAction("ListofServiceProviders");
        }






        //Methods






        private void SaveServiceProviderToDatabase(ServiceProviderConfiguration serviceProvider)
        {
            using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
            {
                string insertQuery = "INSERT INTO ServiceProvider (UserName, Password) VALUES (@UserName, @Password);";
                using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserName", serviceProvider.UserName);
                    command.Parameters.AddWithValue("@Password", serviceProvider.Password);

                    command.ExecuteNonQuery();
                }
            }
        }

        private List<ServiceProviderConfiguration> GetServiceProvidersFromDatabase()
        {
            List<ServiceProviderConfiguration> serviceProviders = new List<ServiceProviderConfiguration>();

            using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
            {
                string selectQuery = "SELECT * FROM ServiceProvider;";
                using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ServiceProviderConfiguration serviceProvider = new ServiceProviderConfiguration
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                UserName = reader["UserName"].ToString(),
                                Password = reader["Password"].ToString()
                            };

                            serviceProviders.Add(serviceProvider);
                        }
                    }
                }
            }

            return serviceProviders;
        }

        private ServiceProviderConfiguration GetServiceProviderById(int id)
        {
            using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
            {
                string selectQuery = "SELECT * FROM ServiceProvider WHERE Id = @Id;";
                using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ServiceProviderConfiguration
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                UserName = reader["UserName"].ToString(),
                                Password = reader["Password"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        private void UpdateServiceProviderInDatabase(ServiceProviderConfiguration serviceProvider)
        {
            using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
            {
                string updateQuery = "UPDATE ServiceProvider SET UserName = @UserName, Password = @Password WHERE Id = @Id;";
                using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserName", serviceProvider.UserName);
                    command.Parameters.AddWithValue("@Password", serviceProvider.Password);
                    command.Parameters.AddWithValue("@Id", serviceProvider.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteServiceProviderFromDatabase(int id)
        {
            using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
            {
                string deleteQuery = "DELETE FROM ServiceProvider WHERE Id = @Id;";
                using (NpgsqlCommand command = new NpgsqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
