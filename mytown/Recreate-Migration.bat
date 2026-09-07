rmdir /s /q Migrations
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet ef dbcontext info

