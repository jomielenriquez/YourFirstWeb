## .NET Core with SQL

 In this guide, we are going to create a product list screen for you to learn the basics on how to implement entity framework on .NET Core app.

 1. Install entity framework core

    ```bash
    dotnet add package Microsoft.EntityFrameworkCore
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    dotnet add package Microsoft.EntityFrameworkCore.Tools
    ```

 2. Create Model

    ```cs
    // Models/Product.cs
    namespace MyApp.Models
    {
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
        }
    }
    ```

 3. 