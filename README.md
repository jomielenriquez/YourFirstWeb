## .NET Core with SQL

 In this guide, we are going to create a product list screen for you to learn the basics on how to implement entity framework on .NET Core app.

 1. Before proceeding, change directory to the `ProductsSolution` first and try running it using `dotnet run`. Proceed to step 2 if successful.

    This solutions should be running smoothly before proceeding to the next step.

 2. Install entity framework core

    ```bash
    dotnet add package Microsoft.EntityFrameworkCore
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    dotnet add package Microsoft.EntityFrameworkCore.Tools
    ```

 3. Create `Models` folder and create `Products.cs` inside.

    ```cs
    // Models/Product.cs
    namespace ProductsSolution.Models
    {
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
        }
    }
    ```
 4. Create database context
    
    The DbContext class manages the entity objects during runtime, which includes populating objects with data from a database, change tracking, and persisting data to the database. Create a `Data` folder and add an `AppDbContext.cs` class:

    ```cs
    // Data/AppDbContext.cs
    using Microsoft.EntityFrameworkCore;
    using ProductsSolution.Models;

    namespace ProductsSolution.Data
    {
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            public DbSet<Product> Products { get; set; }
        }
    }
    ```

 5. Create Repository Pattern

    Create an `Interfaces` folder and add `IProductRepository.cs` file:

    ```cs
    // Interfaces/IProductRepository.cs
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ProductsSolution.Models;

    namespace ProductsSolution.Interfaces
    {
        public interface IProductRepository
        {
            // this is an interface to get all products
            Task<IEnumerable<Product>> GetAllProducts();
        }
    }
    ```

    Implement the interface in the Repository. Create a `Repositories` folder and add `ProductRepository.cs`:

    ```cs
    // Repositories/ProductRepository.cs
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using ProductsSolution.Data;
    using ProductsSolution.Interfaces;
    using ProductsSolution.Models;

    namespace ProductsSolution.Repositories
    {
        public class ProductRepository : IProductRepository
        {
            private readonly AppDbContext _context;

            public ProductRepository(AppDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Product>> GetAllProducts()
            {
                return await _context.Products.ToListAsync();
            }
        }
    }

    ```

 6. Configure the Database Context

    In your `Program.cs` file, configure the database context in the `ConfigureServices` method:

    ```cs
    // Include the necessary package and code
    using Microsoft.EntityFrameworkCore;
    using ProductsSolution.Data;
    using ProductsSolution.Interfaces;
    using ProductsSolution.Repositories;

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // Add DbContext with SQL Server provider
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Add repository dependency
    builder.Services.AddScoped<IProductRepository, ProductRepository>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
    ```

    Also, add your connection string in `appsettings.json`:

    NOTE: Change the `server_name` with your server name and the `database_name` with the database that you created

    ```json
    {
        "Logging": {
            "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
            }
        },
        "AllowedHosts": "*",
        "ConnectionStrings": {
            "DefaultConnection": "Server=server_name;Database=database_name;Trusted_Connection=True;TrustServerCertificate=True"
        }
    }
    ```
 7. Create a list screen

    Step 1: Create `AppModel` folder and add ``App.cs`:

    ```cs
    // AppModel/App.cs
    using ProductsSolution.Models;

    namespace ProductsSolution.AppModels
    {
        public class App
        {
            public IEnumerable<Product> Products { get; set; }
        }
    }
    ```

    Step 2: Update your `HomeController.cs`

    ```cs
    using Microsoft.AspNetCore.Mvc;
    using ProductsSolution.Interfaces;
    using ProductsSolution.AppModels;

    namespace ProductsSolution.Controllers;

    public class HomeController : Controller
    {
        // Declare IProductrepository
        private readonly IProductRepository productRepository;

        // Injection
        public HomeController(IProductRepository productRepository){
            this.productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            App app = new App();
            app.Products = await this.productRepository.GetAllProducts();
            return View(app);
        }
    }
    ```

    Step3: Update Index.cshtml
    ```cshtml
    @model ProductsSolution.AppModels.App;
    @{
        ViewData["Title"] = "Home Page";
    }

    @foreach(ProductsSolution.Models.Product product in Model.Products){
        <h1>@product.Name</h1>
    }
    ```

 8. Apply Migration and Update Database

    install EF Core tools

    ```bash
    dotnet tool install --global dotnet-ef
    ```

    Run the following commands to create and apply the initial migration, and then update the database:

    ```bash
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    ```