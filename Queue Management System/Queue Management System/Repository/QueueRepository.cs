using FastReport.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NuGet.Common;
using Queue_Management_System.Models;
using Queue_Management_System.Models.ViewModels;
using Queue_Management_System.Services;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Queue_Management_System.Repository
{
    public class QueueRepository
    {
        private readonly string conString;

        public QueueRepository( IConfiguration configuration)
        {
            conString = configuration.GetConnectionString("DefaultConnection");
        }


        //servicePoints
        public async Task<IActionResult> CreateServicePoint(ServicePointModel servicePoint)
        {
            if (servicePoint == null)
            {
                throw new ArgumentNullException(nameof(servicePoint));
            }
           //var result= await  _context.servicePoints.AddAsync(servicePoint);
            var connection = new NpgsqlConnection(conString);

            connection.Open();

            var sql = $"INSERT INTO servicepoints (name, description, datecreated, createdby) VALUES (@name, @description, @dateCreated, @createdBy)";
            var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("name", servicePoint.name);
            command.Parameters.AddWithValue("description", servicePoint.description);
            command.Parameters.AddWithValue("dateCreated", servicePoint.datecreated);
            command.Parameters.AddWithValue("createdBy", servicePoint.createdby);

            command.ExecuteNonQuery();
            command.Dispose();
            connection.Close();
            return null;
        }

        public IEnumerable<ServicePointModel> getServicePoints()
        {
            var servicePoints = new List<ServicePointModel>();
            using (var connection = new NpgsqlConnection(conString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM servicepoints", connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var servicepoint = new ServicePointModel
                            {
                                id = (int)reader["id"],
                                name = reader["name"].ToString(),
                                description = reader["description"].ToString(),
                                datecreated = Convert.ToDateTime(reader["datecreated"]),
                                createdby = reader["createdby"].ToString()
                            };
                            servicePoints.Add(servicepoint);
                        }
                    };
                };
            };
               

            return servicePoints;
        }
        public ServicePointModel getServicePointById(int id)
        {
            var servicePoint = new ServicePointModel();
            using (var connection = new NpgsqlConnection(conString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM servicepoints where id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            servicePoint.id = (int)reader["id"];
                            servicePoint.name = reader["name"].ToString();
                            servicePoint.description = reader["description"].ToString();
                            servicePoint.datecreated = Convert.ToDateTime(reader["datecreated"]);
                            servicePoint.createdby = reader["createdby"].ToString();
                        }
                    }
                }
            }
            return servicePoint;
        }

        //customers
        public int CountCustomersinQueueByServicePoint(int servicePointId)
        {
            int customersInQueue = 0;
            var connection = new NpgsqlConnection(conString);
            connection.Open();
            using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM customers WHERE servicepointid = @servicePointId AND UPPER(status) = 'WAITING'", connection))
            {
                cmd.Parameters.AddWithValue("@servicePointId", servicePointId);
                customersInQueue = Convert.ToInt32(cmd.ExecuteScalar());
            };
            connection.Close();

            return customersInQueue;
        }

        public int CountTotalCustomersServed()
        {
            int totalCustomers = 0;
            var connection = new NpgsqlConnection(conString);
            connection.Open();
            using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM customers", connection))
            {
                totalCustomers = Convert.ToInt32(cmd.ExecuteScalar());

            }
            connection.Close();

            return totalCustomers;
        }
        public async Task<IActionResult> CreateCustomer(Customers customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            using (var conn = new NpgsqlConnection(conString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO customers (name, servicepointid, timein, timeout, status, ticketnumber, timeservicestarted) VALUES (@name,  @servicePointId,@timein, @timeout, @status, @ticketnumber, @timeservicestarted)", conn))
                {
                    cmd.Parameters.AddWithValue("@name", customer.name);
                    cmd.Parameters.AddWithValue("@servicePointId", customer.servicepoint.id);
                    cmd.Parameters.AddWithValue("@timein", customer.timein);
                    cmd.Parameters.AddWithValue("@timeout", customer.timeout);
                    cmd.Parameters.AddWithValue("@status", customer.status);
                    cmd.Parameters.AddWithValue("@ticketnumber", customer.ticketnumber);
                    cmd.Parameters.AddWithValue("@timeservicestarted", customer.timeservicestarted);
                    cmd.ExecuteNonQuery();
                };
            }
            return null;
        }
        public List<Customers> GetCustomersinQueueByServicePoint(int? servicePointId)
        {
            var customersInQueue = new List<Customers>();
            var connection = new NpgsqlConnection(conString);
            connection.Open();
            using (var cmd = new NpgsqlCommand("SELECT * FROM customers WHERE servicepointid = @servicePointId AND UPPER(status) = 'WAITING'", connection))
            {
                cmd.Parameters.AddWithValue("@servicePointId", servicePointId);
                var reader  =cmd.ExecuteReader();
                while (reader.Read())
                {
                    var customers = new Customers()
                    {
                        name = reader["name"].ToString(),
                        servicepoint = getServicePointById(Convert.ToInt32(reader["servicepointid"])),
                        timein = Convert.ToDateTime(reader["timein"]),
                        timeout = Convert.ToDateTime(reader["timeout"]),
                        status = reader["status"].ToString(),
                        ticketnumber = reader["ticketnumber"].ToString()
                    };
                    customersInQueue.Add(customers);
                };
                reader.DisposeAsync();
            }
            connection.Close();

            return customersInQueue;
        }
        public Customers GetCurrentCustomer(int? servicePointId)
        {
            var CurrentCustomer = new Customers();
            using var connection= new NpgsqlConnection(conString);
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("Select  * from customers where servicepointid=@servicepointid and UPPER(status)='WAITING' LIMIT 1", connection))
                {
                    cmd.Parameters.AddWithValue("@servicePointId", servicePointId);
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        CurrentCustomer.id = Convert.ToInt32(reader["id"]);
                        CurrentCustomer.name = reader["name"].ToString();
                        CurrentCustomer.servicepoint = getServicePointById(Convert.ToInt32(reader["servicepointid"]));
                        CurrentCustomer.timein = Convert.ToDateTime(reader["timein"]);
                        CurrentCustomer.timeout = Convert.ToDateTime(reader["timeout"]);
                        CurrentCustomer.status = reader["status"].ToString();
                        CurrentCustomer.ticketnumber = reader["ticketnumber"].ToString();
                   
                    }
                    else
                    {
                        CurrentCustomer = new Customers { name = "There are no customers waiting in the Queue" };
                    }
                    reader.DisposeAsync();
                };
            };
            return CurrentCustomer;
        }
        public Customers GetCustomerById(int Id)
        {
            var customer = new Customers();
            using (var connection = new NpgsqlConnection(conString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM customers where id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", Id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customer.id = Convert.ToInt32(reader["id"]);
                            customer.name = reader["name"].ToString();
                            customer.servicepoint = getServicePointById(Convert.ToInt32(reader["servicepointid"]));
                            customer.timein = Convert.ToDateTime(reader["timein"]);
                            customer.timeout = Convert.ToDateTime(reader["timeout"]);
                            customer.status = reader["status"].ToString();
                            customer.ticketnumber = reader["ticketnumber"].ToString();
                        }
                    }
                }
            }
            return customer;
        }
        public void updateCustomerStatus(Customers customer)
        {
            var customerToUpdate = GetCustomerById(customer.id);
            if (customerToUpdate != null)
            {
                using (var conn = new NpgsqlConnection(conString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("UPDATE  customers set status=@status, timeout=@timeout where id=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", customerToUpdate.id);
                        cmd.Parameters.AddWithValue("@timeout", DateTime.Now.ToUniversalTime());
                        cmd.Parameters.AddWithValue("@status", customer.status);

                        cmd.ExecuteNonQuery();
                    };
                }
                updateCustomerStartedService(customerToUpdate);//mark next customer as started service
            }
        }
        public void updateCustomerStartedService(Customers customer)
        {
            var customerToUpdate = GetCurrentCustomer(customer.servicepoint.id);
            if (customerToUpdate != null)
            {
                using (var conn = new NpgsqlConnection(conString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("UPDATE  customers set timeservicestarted=@timeservicestarted where id=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", customerToUpdate.id);
                        cmd.Parameters.AddWithValue("@timeservicestarted", DateTime.Now.ToUniversalTime());

                        cmd.ExecuteNonQuery();
                    };
                }
            }
        }
        public void TransferCustomerStatus(Customers customer)
        {
            var customerToUpdate = GetCustomerById(customer.id);
            if (customerToUpdate != null)
            {
                using (var conn = new NpgsqlConnection(conString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("UPDATE  customers set servicepointid=@servicepointid where id=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", customerToUpdate.id);
                        cmd.Parameters.AddWithValue("@servicepointid", customer.servicepoint.id);

                        cmd.ExecuteNonQuery();
                    };
                }
            }
        }
        public Customers RecallCustomer(string TicketNumber)
        {
            var customer = new Customers();
            using (var connection = new NpgsqlConnection(conString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM customers where ticketnumber=@ticketnumber", connection))
                {
                    cmd.Parameters.AddWithValue("@ticketnumber", TicketNumber);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customer.id = Convert.ToInt32(reader["id"]);
                            customer.name = reader["name"].ToString();
                            customer.servicepoint = getServicePointById(Convert.ToInt32(reader["servicepointid"]));
                            customer.timein = Convert.ToDateTime(reader["timein"]);
                            customer.timeout = Convert.ToDateTime(reader["timeout"]);
                            customer.status = reader["status"].ToString();
                            customer.ticketnumber = reader["ticketnumber"].ToString();
                        }
                        else
                        {
                            customer = new Customers { name = $"No customer exists with ticket number {TicketNumber}" };

                        }
                    }
                }
            }
            return customer;
        }
        public int CountServedCustomersByServicePoint(FilterModel filter)
        {
            int totalCustomersServed = 0;
            var connection = new NpgsqlConnection(conString);
            connection.Open();
            using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM customers where servicepointid=@servicepointid AND UPPER(status)<>'WAITING' AND timeout BETWEEN @startdate and @enddate", connection))
            {
                cmd.Parameters.AddWithValue("@servicepointid", filter.ServicePointId);
                cmd.Parameters.AddWithValue("@startdate", filter.StartDate);
                cmd.Parameters.AddWithValue("@enddate", filter.EndDate);
                var result = cmd.ExecuteScalar();
                totalCustomersServed = !DBNull.Value.Equals(result) ? Convert.ToInt32(result) : totalCustomersServed;

            }
            connection.Close();

            return totalCustomersServed;
        }
        public double CalculateAverageWaitTime(FilterModel filter)
        {
            double averageWaitTime = 0;
            var connection = new NpgsqlConnection(conString);
            connection.Open();
            using (var cmd = new NpgsqlCommand("SELECT AVG(EXTRACT(EPOCH FROM (timeservicestarted - timein)) / 60) FROM customers where UPPER(status)<>'WAITING' and timeout BETWEEN @startdate and @enddate and servicepointid=@servicepointid", connection))
            {
                cmd.Parameters.AddWithValue("@startdate", filter.StartDate);
                cmd.Parameters.AddWithValue("@enddate", filter.EndDate);
                cmd.Parameters.AddWithValue("@servicepointid", filter.ServicePointId);
                var result = cmd.ExecuteScalar();
                averageWaitTime = !DBNull.Value.Equals(result)?Convert.ToDouble(result): averageWaitTime;

            }
            connection.Close();

            return averageWaitTime;

        }
        public double CalculateAverageServiceTime(FilterModel filter)
        {
            double averageWaitTime = 0;
            var connection = new NpgsqlConnection(conString);
            connection.Open();
            using (var cmd = new NpgsqlCommand("SELECT AVG(EXTRACT(EPOCH FROM (timeout - timeservicestarted)) / 60) FROM customers where UPPER(status)<>'WAITING' and timeout BETWEEN @startdate and @enddate and servicepointid=@servicepointid", connection))
            {
                cmd.Parameters.AddWithValue("@startdate", filter.StartDate);
                cmd.Parameters.AddWithValue("@enddate", filter.EndDate);
                cmd.Parameters.AddWithValue("@servicepointid", filter.ServicePointId);
                var result = cmd.ExecuteScalar();
                averageWaitTime = !DBNull.Value.Equals(result) ? Convert.ToDouble(result) : averageWaitTime;

            }
            connection.Close();

            return averageWaitTime;

        }
        public async Task<bool> checkPassword(LoginViewModel loginDetails)
        {
            using (var connection = new NpgsqlConnection(conString))
            {
                await connection.OpenAsync();
                
                using (var command = new NpgsqlCommand("SELECT \"PasswordHash\" FROM \"AspNetUsers\" WHERE \"Email\" = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", loginDetails.Email);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var passwordHash = reader.GetString(0);
                            if (loginDetails.Password == passwordHash)
                            {
                                // Password matches, sign in the user
                                return true;
                            }
                        }
                        return false;
                    }
                }

            }
        }
        public async Task<bool> createUser(RegisterViewModel registerDetails)
        {
            using (var conn = new NpgsqlConnection(conString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO \"AspNetUsers\" (\"Id\", \"UserName\", \"Email\", \"PasswordHash\", \"EmailConfirmed\", \"PhoneNumberConfirmed\", \"LockoutEnabled\", \"TwoFactorEnabled\", \"AccessFailedCount\") VALUES (@Id, @username,  @email,@passwordhash, false, false, false, false,0)", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@username", registerDetails.Email);
                    cmd.Parameters.AddWithValue("@email", registerDetails.Email);
                    cmd.Parameters.AddWithValue("@passwordhash", registerDetails.Password);
                    cmd.ExecuteNonQuery();
                };
                conn.Close();
                return true;
            }
        }
        public async Task<IdentityUser> getUserByEmail(string Email)
        {
            var user = new IdentityUser();
            using (var connection = new NpgsqlConnection(conString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM \"AspNetUsers\" where \"Email\"=@email", connection))
                {
                    cmd.Parameters.AddWithValue("@email", Email);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user.UserName = reader["UserName"].ToString();
                            user.Email = reader["Email"].ToString();
                            user.Id = reader["Id"].ToString();
                        }
                       
                    }
                }
            }
            return user;
        }
        public async Task<bool> AddRole(IdentityRole role)
        {
            using (var conn = new NpgsqlConnection(conString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO \"AspNetRoles\" (\"Id\",\"Name\", \"NormalizedName\",\"ConcurrencyStamp\" ) VALUES (@Id, @name, @normalizedName, @concurrencyStamp)", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", role.Id);
                    cmd.Parameters.AddWithValue("@name", role.Name);
                    cmd.Parameters.AddWithValue("@normalizedName", role.Name.ToUpper());
                    cmd.Parameters.AddWithValue("@concurrencyStamp", role.ConcurrencyStamp);

                    cmd.ExecuteNonQuery();
                };
                conn.Close();
                return true;
            }
        }
        public async Task<List<IdentityRole>> getUserRoles(IdentityUser user)
        {
            var roles = new List<IdentityRole>();
            using (var connection = new NpgsqlConnection(conString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM \"AspNetUserRoles\" where \"UserId\"=@UserId", connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", user.Id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var role = await getRoleById(reader["RoleId"].ToString());
                            roles.Add(role);
                        }

                    }
                }
            }
            return roles;
        }
        public async Task<bool> userIsInRole(IdentityUser user, string roleId)
        {
            using (var conn = new NpgsqlConnection(conString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM   \"AspNetUserRoles\" WHERE \"UserId\"=@userid AND \"RoleId\"=@roleid", conn))
                {
                    cmd.Parameters.AddWithValue("@userid", user.Id);
                    cmd.Parameters.AddWithValue("@roleid", roleId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return true;
                        }
                        return false;
                    }
                };
            }
        }
        public async Task<bool> AddUserToRole(string Email, string roleId)
        {
            var user = await getUserByEmail(Email);
            using (var conn = new NpgsqlConnection(conString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO \"AspNetUserRoles\" (\"UserId\", \"RoleId\") VALUES (@userid, @roleid)", conn))
                {
                    cmd.Parameters.AddWithValue("@userid", user.Id);
                    cmd.Parameters.AddWithValue("@roleid", roleId);
                    cmd.ExecuteNonQuery();
                };
                conn.Close();
                return true;
            }
        }
        public async Task<List<IdentityRole>> getRoles()
        {
            var roles = new List<IdentityRole>();
            using (var conn = new NpgsqlConnection(conString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM   \"AspNetRoles\" ", conn))
                {

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var role = new IdentityRole
                            {
                                Id = reader["Id"].ToString(),
                                Name = reader["Name"].ToString(),
                            };
                            roles.Add(role);
                        }
                    }
                }
            }
            return roles;
        }
        public async Task<IdentityRole> getRoleById(string RoleId)
        {var role= new IdentityRole();
            using (var conn = new NpgsqlConnection(conString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM   \"AspNetRoles\" WHERE \"Id\"=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", RoleId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            role.Id = reader["Id"].ToString();
                            role.Name = reader["Name"].ToString();
                 
                        }
                    }
                }
            }
            return role;
        }

    }
}
