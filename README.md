

## Queue Management System

##### I received the assignment and I could not fully do it. I Have outlined my approach to handle it


1. Check-In page. This page should allow customers to select the kind of service they're receiving and should support kiosk mode with the following in mind:
   - Use descriptive text.
   - Buttons should be large enough.
   - Ability to print tickets. Design the ticket using FastReport.Net
#### My Approach
- List all services offered vertically in a button tag
- Put a click event on each button
- Click Event: when a user clicks a button, systems first get list of customer in the for that particular service.Then get estimated waited time, then register the ticket and print the ticket.

2. Waiting page. This page should simply display the called customer's ticket number and the service point they should head to.
#### My Approach 
- Fetch customer ticket ticket, filter where ticket is open and ticket type is that of the customer
- You display the data as a list since there can many service points and many customers

3. Service point. This page should allow the service provider to:
   - Authenticate and select their service point.
   - Get next number.
   - Recall number.
   - Mark number as no show.
   - Mark number as finished.
   - Transfer number.
   - View their queue.
#### My Approach
- I would use cookie based authentication, since this is a Model View Controller pattern project
- Then I will be updating particular and call the queue

4. Admin dashboard. This page should have the following:
   - Ability to configure service points.
   - Ability to configure service providers.
   - Ability to generate an analytical report, using **FastReport.Net**, displaying the following information:
     - Number of customers served.
     - Average waiting time per service point/provider.
     - Average service time per service point/provider.
#### My Approach
- I will introduce roles to distinguish normal users and admin users.
- Others requirements are easy to do.

## Database modelling

- Use PostgreSQL as the database of choice.
- Ensure that the database tables are properly mapped to your project's POCO models.
- **DO NOT** use existing ORMs i.e. Entity Framework for modelling. Instead, write your own CRUD methods.
#### My Approach
- I have set up databasecontext and registered it in dependency injection container
- In the databasecontext class, I would create methods to be executed when the application is initial ran
- This includes creation of the database and relevant tables
- I would then created all Models mapping tables in the database and their corresponding DTOs
## How to work on the assignment

- Fork this repository.
- Clone your forked repository.
- Start working on the assignment.
- Ensure to do periodic commits with meaningful commit messages.
- Once you are done, push your work to your forked repository and finally submit a pull request to the upstream repository.
- If you don't want to create a public repository please invite (@hanmaktechltd) to your working repository.
- Please include a brief description of how to run your solution and also include a copy of the database schema.
- If you have any questions contact us (<hr@hanmak.co.ke>)
