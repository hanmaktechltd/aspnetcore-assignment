# Queue Management System
> This is a system which is used to manage queues in a busy environment. It is developed in ASP.NET Core MVC (.NET 6)

## Table of Contents
* [General Info](#general-information)
* [Technologies Used](#technologies-used)
* [Features Workings and About the project](#features-workings-and-about-the-project)
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

## Features Workings and About the project
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

<img width="960" alt="1) Homepage" src="https://github.com/GeoffreyOmollo/aspnetcore-assignment/assets/120243097/69a0e567-2b45-4d28-bf3a-8fd3486e2803">
_______
<img width="955" alt="2) Customer Checkin" src="https://github.com/GeoffreyOmollo/aspnetcore-assignment/assets/120243097/6974fb36-6c40-4576-9b98-cc30386f4515">
_______
<img width="960" alt="3) Customer added ro Queue" src="https://github.com/GeoffreyOmollo/aspnetcore-assignment/assets/120243097/cb187565-3cd8-4f8f-91ca-eb2e6474202c">
_______
<img width="960" alt="4) Customer Ticket Details" src="https://github.com/GeoffreyOmollo/aspnetcore-assignment/assets/120243097/86fcd197-866e-40ec-8b43-667363ea0d0e">
_______
<img width="957" alt="5) Service Provider Login" src="https://github.com/GeoffreyOmollo/aspnetcore-assignment/assets/120243097/d9a622dd-0a54-462d-ae8f-e16620f113cf">
_______
<img width="960" alt="6) Logged-in Service Provider" src="https://github.com/GeoffreyOmollo/aspnetcore-assignment/assets/120243097/007e9ca1-e966-4867-836b-85a3d7a19f3e">
_______
<img width="960" alt="7) Service Provider Assigning a Service Point" src="https://github.com/GeoffreyOmollo/aspnetcore-assignment/assets/120243097/d7bf5170-118d-497a-93e9-a0db41bf168e">
_______
<img width="959" alt="8) Admin Login" src="https://github.com/GeoffreyOmollo/aspnetcore-assignment/assets/120243097/9c4d9516-3131-44b7-8f4d-4107fcb4071d">
_______
<img width="957" alt="9) Logged-in Admin" src="https://github.com/GeoffreyOmollo/aspnetcore-assignment/assets/120243097/f7aba035-5236-4c8c-ac8b-0ff2e4698a44">
_______
<img width="954" alt="10) Configuring Service Point" src="https://github.com/GeoffreyOmollo/aspnetcore-assignment/assets/120243097/7ecf9667-6e1e-46a0-80da-6357547f556f">
_______
<img width="960" alt="11) Configuring Service Providers" src="https://github.com/GeoffreyOmollo/aspnetcore-assignment/assets/120243097/6b520e99-9e3e-4f83-b44c-9d92124fb163">

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
