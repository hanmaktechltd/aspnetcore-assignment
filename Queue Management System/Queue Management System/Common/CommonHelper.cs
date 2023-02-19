using Npgsql;
using Queue_Management_System.Data;

namespace Queue_Management_System.Common
{
    public class CommonHelper
    {
        private IConfiguration _config;
        public CommonHelper(IConfiguration config)
        {
            _config = config;
        }
        public User GetUserByUserName(string query)
        {
            User user = new User();

            string connectionString = _config.GetConnectionString("DefaultConnection");

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {

                string sql = query;

                // Prep command object.
                NpgsqlCommand command = new NpgsqlCommand(sql, connection);

                connection.Open();

                // Obtain a data reader via ExecuteReader()
                using (NpgsqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        user.Id = Convert.ToInt32(dataReader["id"]);
                        user.Name = dataReader["name"].ToString();
                        user.Password =dataReader["password"].ToString();
                        user.Role = dataReader["role"].ToString();
                        user.ServicePointId = Convert.ToInt32(dataReader["servicepointid"]);
                    }
                    dataReader.Close();
                }
                connection.Close();
            }
            return user;
        }
    }
}