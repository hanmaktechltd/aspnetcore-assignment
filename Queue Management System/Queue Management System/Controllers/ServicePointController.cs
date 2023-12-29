using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;

using Queue_Management_System.Database;
using Queue_Management_System.Models;

namespace Queue_Management_System.Controllers
{
    public class ServicePointController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseConnection _databaseConnection;

        public ServicePointController(IConfiguration configuration, DatabaseConnection databaseConnection)
        {
            _configuration = configuration;
            _databaseConnection = databaseConnection;
        }

        [HttpGet]
        public IActionResult CreateServicePoint()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateServicePoint(ServicePoint servicePoint)
        {
            if (ModelState.IsValid)
            {
                // Save the new service point to the database
                SaveServicePointToDatabase(servicePoint);

                // Redirect to the list of service points
                return RedirectToAction("ListofServicePoints");
            }

            return View(servicePoint);
        }

        [HttpGet]
        public IActionResult ListofServicePoints()
        {
            // Retrieve the list of service points from the database
            var servicePoints = GetServicePointsFromDatabase();

            return View(servicePoints);
        }


        [HttpGet]
        public IActionResult EditServicePoint(int id)
        {
            // Retrieve the service point from the database based on the id
            var servicePoint = GetServicePointById(id);

            if (servicePoint == null)
            {
                return NotFound();
            }

            return View(servicePoint);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditServicePoint(int id, ServicePoint servicePoint)
        {
            if (id != servicePoint.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Update the service point in the database
                UpdateServicePointInDatabase(servicePoint);

                return RedirectToAction("ListofServicePoints");
            }

            return View(servicePoint);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            // Retrieve the service point from the database based on the id
            var servicePoint = GetServicePointById(id);

            if (servicePoint == null)
            {
                return NotFound();
            }

            return View(servicePoint);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete the service point from the database
            DeleteServicePointFromDatabase(id);

            return RedirectToAction("ListofServicePoints");
        }






        //Methods






        private void SaveServicePointToDatabase(ServicePoint servicePoint)
        {
            using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
            {
                string insertQuery = "INSERT INTO ServicePoints (ServicePointName, ServedBy) VALUES (@ServicePointName, @ServedBy);";
                using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ServicePointName", servicePoint.ServicePointName);
                    command.Parameters.AddWithValue("@ServedBy", servicePoint.ServedBy);

                    command.ExecuteNonQuery();
                }
            }
        }

        private List<ServicePoint> GetServicePointsFromDatabase()
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
                                // ServicePointNumber = Convert.ToInt32(reader["ServicePointNumber"]),
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

        private ServicePoint GetServicePointById(int id)
        {
            using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
            {
                string selectQuery = "SELECT * FROM ServicePoints WHERE Id = @Id;";
                using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ServicePoint
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                //ServicePointNumber = Convert.ToInt32(reader["ServicePointNumber"]),
                                ServicePointName = reader["ServicePointName"].ToString(),
                                ServedBy = reader["ServedBy"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        private void UpdateServicePointInDatabase(ServicePoint servicePoint)
        {
            using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
            {
                string updateQuery = "UPDATE ServicePoints SET ServicePointName = @ServicePointName, ServedBy = @ServedBy WHERE Id = @Id;";
                using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@ServicePointName", servicePoint.ServicePointName);
                    command.Parameters.AddWithValue("@ServedBy", servicePoint.ServedBy);
                    command.Parameters.AddWithValue("@Id", servicePoint.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteServicePointFromDatabase(int id)
        {
            using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
            {
                string deleteQuery = "DELETE FROM ServicePoints WHERE Id = @Id;";
                using (NpgsqlCommand command = new NpgsqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
