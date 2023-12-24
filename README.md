# ASP.NET Core Queue Management System

This repository contains the code for a Queue Management System developed using ASP.NET Core.

## Libraries/Tools Used

- ASP.NET Core 5+
- PostgreSQL 11+
- FastReport.Net
- Npgsql 6.0.0+

## Pages

### Check-In Page

- Allows customers to select the kind of service.
- Supports kiosk mode.
- Uses descriptive text.
- Large buttons.
- Ability to print tickets using FastReport.Net.

### Waiting Page

- Displays the called customer's ticket number and the corresponding service point.

### Service Point

- Allows service providers to authenticate and select their service point.
- Functions:
  - Get next number.
  - Recall number.
  - Mark number as no show.
  - Mark number as finished.
  - Transfer number.
  - View their queue.

### Admin Dashboard

- Configures service points and providers.
- Generates an analytical report using FastReport.Net, displaying:
  - Number of customers served.
  - Average waiting time per service point/provider.
  - Average service time per service point/provider.

## Database Modelling

- Uses PostgreSQL as the database of choice.
- Database tables are properly mapped to the project's POCO models.
- CRUD methods are implemented without using existing ORMs (e.g., Entity Framework).

## How to Run the Solution

### Prerequisites

- [Install ASP.NET Core 5+](https://dotnet.microsoft.com/download)
- [Install PostgreSQL 11+](https://www.postgresql.org/download/)
- [Install FastReport.Net](https://www.fastreport.com/en/download/fast-report-net-download/)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/ithau10/Queue-Management-System
   cd Queue-Management-System
   
