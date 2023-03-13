# ASP.Net Core Developer Assignment: Queue Management System


## Queue Management System

## Database installation

open appsettings.json and edit the DefaultConnection connection string to point to the Postgres database you want to use.

open the project on visual studio and run the command update-database.

Database called QueueDB should now be available on your PGAdmin. 


## Add Roles
Click on Add Roles from the nav bar. (Although this Action should require authorization, it allows anonymous access for this case to enable testers to create the Admin role)

Ensure you create these two Roles:

   i. Admin
   
   ii. ServiceProvider
   
 ##  Register Admin Account 
 Click on Self-register 
 
 Enter the Admin email, password
 
 On Role, Enter Admin.
 
 ## Register Service Providers
 
 Login in using Admin account
 
 Select Register User
 
 Enter Email and Password
 
 select ServiceProvider Role from the dropdown of roles
 
 ## Add service Points
 Logged in as admin, Click add service point from the nav Menu
 
 enter the name and the other details as required. 
 
 ## Log in as a service Provider
 Use the email and password provided by the Admin. 
 
 select the service point you will operate from the drop down of service points
 
 ## Mark as no show/ finished
 select the customer you are current serving. 
 
 select the edit status button.
 
 Update to No show/ finished respectively.
 
 
 ## Transfer client
 select Transfer. 
 
 select the service point to transfer to
 
 ## customer check in 
 
 enter the customer name
 
 select service point that you want. 
 
 on the waiting page, click on print Ticket to generate PDF ticket.


