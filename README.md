# NFL Depth Charts API Dev Guide
A simple API solution using .NET Core 6 along with EF Core 6. This application currently uses an in-memory database, but can be extended to use SQL server or SQL lite as needed. API enables Players and their Positions to be added to a depth chart via API endpoints according to the spec. 

## Setup
Run in VS or Rider; or alternatively use following commands:
```
dotnet build
cd .\NFLDepthcharts.API\
dotnet run
```

then head to https://localhost:7196/swagger/index.html

## Working and Building in Project
The project is organised into several folders:

- `Controllers`: Contains the API controllers responsible for handling HTTP requests and returning responses.
- `Data`: Contains the data access layer, including the `DepthChartDbContext` class for interacting with the database.
- `Exceptions`: Contains custom exception classes used for specific error scenarios. This approach was used over DataAnnotation validation for a bit more control.
- `Middleware`: Contains the custom middleware classes, such as the `ApiExceptionMiddleware` for handling exceptions and returning appropriate responses globally.
- `Validators`: Contains validator classes, such as the `PlayerValidator` for validating Entity models with business logic.
- `Models`: Contains the model classes representing entities in the application, such as the `Player` class.
- `Repositories`: Contains the repository classes responsible for data persistence and retrieval.
- `Services`: Contains the service classes that encapsulate the business logic and interact with the repositories.

Note that AutoMapper is used in various locations, if a new Entity is ever added or new mapping is required remember to add this to AutoMapperProfile.cs

Also DI is used for so remember to add any new services to the DI container in the `Program.cs` file.

Generally do any filtering in the data/repository layer to limit the amount of data being pulled over the network. `.AsNoTracking` has been used in most calls as most of the current functionality is all Read only.

Right now because the solution is using in-memory DB, seeding is occurring in `Program.cs`. If we ever move to SQL, this should be updated and a EF Migration should be created

## Approach

The following patterns or approaches were used to ensure adhereing to SOLID principles.

1. **Dependency Injection**: The project uses DI to ensure loose coupling and facilitate testability (At least as much as EF allows). Interfaces are utilised for this purpose.

2. **Repository Pattern**: The repository pattern is utilised to separate data access logic from the service layer, this was to adhere to single responsibility so that the Entity classes could remain free from Business validation, instead ocurring in the service layer.

3. **Validation**: Data validation is performed in the Service layer, using `{Entity}Validator.cs` class which will throw a custom `ValidationException` if any business constraints are broken. These rules are as follows:
    - Position is required (and must exist in our DB) 
    - Player Number must be between 1 and 99 inclusive
    - Player Name is required
    - Player Name cannot be more than 100 chars
    - Chart Depth cannot be negative

4. **Error Handling**: Used global error handling middleware (`ApiExceptionMiddleware`) to catch and handle exceptions thrown during request processing. The middleware returns appropriate error responses with meaningful error messages.

5. **Logging**: Logging is implemented using the Microsoft.Extensions.Logging framework. Loggers are injected into the relevant classes, such as controllers and repositories, to record important events and exceptions. Unfortunately didn't have much time to actually add many logs.

6. **Unit Testing**: Unit tests are written using NUnit and Moq frameworks, covering everything in the primary solution as TDD was used.

## Testing
- CD into `NFLDepthCharts.Tests` and run the following command: `dotnet test` to run test suite
- Or use VS Test Runner

## Assumptions
- That an API would suffice over the CLI/scripting approach shown in examples in the spec
- Querying by Player Number for Player rather than Name 
- Positions are generally FIXED, so pre-seeded these into the DB rather than creating them dynamically whenever the user provides them
- Once a player is REMOVED from the depth chart, all others below the depth will shift higher to make up the gap
- Similarly when a player is ADDED, anyone below in the depth chart or equal will get shifted down further by one
- Teams/Sport functionality not required yet, however would be doable using the Entities I have left in the solution

## Potential Improvements
- ADD MEANINGFUL LOGS!! Didn't get a chance to add any meaningful ones but in a prod environment this would be very important.
- Add a migration script to add some dummy data for DB if SQL used (right now)
- Move validation/exceptions to a seperate Exception classes rather than use generic ValidationException for them all
- Add StatusCode attribute to ValidationException, which could be used to determine which StatusCode the ApiExceptionMiddleware returns
- API versioning for backwards compatibility (e.g when Teams/Sports functionality is added)
- Once shifted to SQL, create a EF migration using `dotnet ef migrations add InitialSeedPositions` and then add in the data as required; then run `dotnet ef database update` with the relevant connection strings
- I would generally use the **Builder Pattern** in tests for creating our Entity objects using `Test{Entity}Builder.cs` where object instantiation occurs often, to improve readability and reusable code. However in the interest of time I did a lot of copy and pasting and thought this would be a good future improvement.
- Use AutoFac over Microsoft DI, especially for unit tests to clean up the setup a bit by being able to resolve through interfaces rather than the concrete implementation (for example in my validator classes, I'm instantiating concrete validators. I'd prefer to do something like...)
    ```
    private IPersonValidator _validator;

    [SetUp]
    public void Setup()
    {
        builder.RegisterModule(new AutofacModule());
        var container = builder.Build();

        _validator = container.Resolve<IPersonValidator>();
    }
    ```
    I've done a temporary solution for `DepthchartServiceTests.cs` as it was getting a bit messy but it's still not as great as it could be with AutoFac.
- Rate limiting for scalability
- More API documentation/comments for Swagger to display.
- Utilise Generics to have a `IRepository<T>`, `Repository<T>`, this way if we ever introduce other entities we can have shared Repository functions such as get, search, delete, and adding. 
  
  The way this would be implemented would be then our PlayerRepository would be a `Repository<Player>`, and any other entity we introduced would easily be setup as `Repository<Entity>` without having to implement the shared functions.
  
  Unfortunately this can be a bit time consuming to configure and get working.
  It also tends to introduce a level of coupling, however, this trade off will make working with Repositories in EF easier.