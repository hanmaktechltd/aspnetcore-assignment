# Queue Management System
> This is a system which is used to manage queues in a busy environment. It is developed in ASP.NET Core MVC (.NET 6)

## Table of Contents
* [General Info](#general-information)
* [Technologies Used](#technologies-used)
* [Features Workings and About the project](#features,-workings-and-about-the-project)
* [Screenshots](#screenshots)
* [Setup](#setup)
* [Project Status](#project-status)
* [Room for Improvement](#room-for-improvement)
* [Acknowledgements](#acknowledgements)
* [Contact](#contact)
<!-- * [License](#license) -->

## General Information
- This is a Queue Management System. It can help to manage queues in a busy environment

## Technologies Used
- ASP.NET Core MVC (.NET 6)
- PostgreSQL & Npgsql 
- FastReport.Net
- Bootstrap

## Features, Workings and About the project
The Queue Management system involve these parts: 

- A client/customer - this is the person to be queued.
- A service point - these are the various places that customers can be served
- A service provider - this is the person who queues the customers and assigns them the service points they should head to and also gives customers their queue numbers
- Admin - creates, edits, updates, deletes (configures) the service points and the service providers. The admin will also be able to see the total number of customers served among other analysis and print the report

Working of the system
- A client/customer clicks a button to be queued.
- The customer is then given a queue number and a default service point, before they are assigned a service point by the service provider.
- The service provider then assigns the correct service point
- The customer will then print the ticket.

## Screenshots

Screenshots


## Setup

To run the project you'll need PostgreSQL database and establish a connection

## Project Status
The project : _will continue to receive future updates and bug fixes_.

## Room for Improvement
This is a simple project but a lot more could be added e.g.:
- Notification
- Conversion to a single page application

## Acknowledgements
I would like to thank my family members for the motivation they gave me. Honestly it wasn't easy with sleepless nights.

## Contact
Created by Geoffrey Omollo - feel free to contact me!

<!-- Optional -->
<!-- ## License -->
<!-- This project is open source and available under the [... License](). -->

<!-- You don't have to include all sections - just the one's relevant to your project -->
