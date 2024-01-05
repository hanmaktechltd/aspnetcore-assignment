# How to Run the Application
* Please create a database (myDatabase) on your psql client. You can find the connection string [Queue Management System/Queue Management System/appsettings.json](here)
* In the [Queue Management System/Queue Management System/DatabaseInitializer.cs](DatabaseInitializer) I have included scripts to initalize all tables, as well as seed the ServicePoint and ServiceProvider with test Data.

* Below are user profiles and their passwords which you can use to get started, though you can find them in the DatabaseInitializer. 

# User Profiles
* superuser => superuser_password_hash
* regularuser=> regularuser_password_hash
* superuser has a super ROLE and can access the Dashboard to configure ServiceProviders and ServicePoints
  
