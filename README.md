**Summary of My Approach**

I have built the solution using a clear layered structure - Controllers, Services, and Repositories, with EF Core and SQLite handling the data layer.  
Each layer has a single responsibility, and I used interfaces and dependency injection to keep the design flexible and aligned with SOLID principles.

The API is built on .NET 9 and uses a repository pattern for data access, with a service layer handling business logic and mapping between domain models and DTOs.  
DTOs keep the API contract clean and separate from the database models.

For testing, I have added unit tests using xUnit and Moq to verify the service logic, and integration tests using WebApplicationFactory with an in‑memory SQLite database to test the API endpoints end‑to‑end.

The project supports full CRUD operations, includes automatic timestamps, enforces unique emails, and uses centralized exception handling. 
All operations are async and support cancellation tokens for better performance and responsiveness.

**How to use these in Postman**
1. Create a User (POST)
   
   curl -X POST http://localhost:5076/api/users -H "Content-Type: application/json" -d "{\"firstName\":\"Test\",\"lastName\":\"User\",\"email\":\"test.user@example.com\"}"
2. Get All Users (GET) 

   curl -X GET http://localhost:5076/api/users
3. Get User by ID (GET) 

   curl -X GET http://localhost:5076/api/users/1
4. Update a User (PUT) 

   curl -X PUT http://localhost:5076/api/users/1 -H "Content-Type: application/json" -d "{\"firstName\":\"New\",\"lastName\":\"User\",\"email\":\"new.user@example.com\"}"
5. Get Non-Existent User (404 Test) 

   curl -X GET http://localhost:5076/api/users/999999
6. Update Non-Existent User (404 Test) 

    curl -X PUT http://localhost:5076/api/users/999999 -H "Content-Type: application/json" -d "{\"firstName\":\"invalid\",\"lastName\":\"User\",\"email\":\"invalid.user@example.com\"}"
