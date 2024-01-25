# Queue Management System

Queue Management System is a web application used for efficiently managing queues using a ticket service. It also supports generation of reports for analysis.

## Dependencies
- ASP.NET Core 6+
- PostgreSQL 11+
- FastReport.Net
- Npgsql 7+

## Build and Run Instructions
Navigate to the folder containing the .csproj file and build and start the application using the following command.
```bash
dotnet run --roll-forward LatestMajor
``` 
The PostgreSQL database schema has been provided alongside the project. Use
```bash
pg_restore -U postgres Queue Management System < QueueManagementSystem.sql
```
to restore the database.

Default administrator user email "dantes@gmail.com" and password "password09" can be used to access pages requiring authentication and authorization in the application.
