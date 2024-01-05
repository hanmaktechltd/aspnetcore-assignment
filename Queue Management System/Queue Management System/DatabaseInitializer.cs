using System;
using Npgsql;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void InitializeDatabase()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;

                command.CommandText = @"
                          CREATE TABLE IF NOT EXISTS ServicePoint (
    ServicePointId SERIAL PRIMARY KEY,
    ServicePointName VARCHAR(255) NOT NULL UNIQUE,
    Description TEXT UNIQUE,
    UNIQUE (ServicePointId, ServicePointName)
);

CREATE TABLE IF NOT EXISTS Ticket (
    TicketId SERIAL PRIMARY KEY,
    IssueTime TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(255) NOT NULL DEFAULT 'Not Served',
    ServiceStartTime TIMESTAMP,
    ServiceCompleteTime TIMESTAMP,
    ServicePointId INT NOT NULL,
    ServicePoint VARCHAR(255),
    ServiceProvider VARCHAR(255),
    FOREIGN KEY (ServicePointId) REFERENCES ServicePoint(ServicePointId)
);

CREATE TABLE IF NOT EXISTS ServiceProvider (
    ServiceProviderId SERIAL PRIMARY KEY,
    Username VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Role VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS ServiceProviderServicePoint (
    ServiceProviderId INT,
    ServicePointId INT,
    PRIMARY KEY (ServiceProviderId, ServicePointId),
    FOREIGN KEY (ServiceProviderId) REFERENCES ServiceProvider(ServiceProviderId),
    FOREIGN KEY (ServicePointId) REFERENCES ServicePoint(ServicePointId)
);


INSERT INTO ServicePoint (ServicePointName, Description)
VALUES
    ('servicePointTest 1', 'service test 1'),
    ('servicePointTest 2', 'service test 2'),
    ('servicePointTest 3', 'service test 3')
ON CONFLICT (ServicePointName)
DO NOTHING;

INSERT INTO ServiceProvider (Username, PasswordHash, Role) 
VALUES 
    ('superuser', 'superuser_password_hash', 'super'),
    ('regularuser', 'regularuser_password_hash', 'regular')
ON CONFLICT (Username) -- Unique constraint for Username
DO NOTHING;


WITH inserted_service_providers AS (
    SELECT ServiceProviderId, Username
    FROM ServiceProvider
    WHERE Username IN ('superuser', 'regularuser')
)

INSERT INTO ServiceProviderServicePoint (ServiceProviderId, ServicePointId)
VALUES 
    ((SELECT ServiceProviderId FROM inserted_service_providers WHERE Username = 'superuser'), 1),
    ((SELECT ServiceProviderId FROM inserted_service_providers WHERE Username = 'superuser'), 2),
    ((SELECT ServiceProviderId FROM inserted_service_providers WHERE Username = 'regularuser'), 3)
ON CONFLICT (ServiceProviderId, ServicePointId)
DO NOTHING;


                        ";


                command.ExecuteNonQuery();
            }
        }

        Console.WriteLine("Database initialization complete.");
    }

   
}
