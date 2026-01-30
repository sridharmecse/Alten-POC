# Full Stack Microservices Web App (.NET 8 + Angular + MSSQL + Azure Blob ready)

This repository contains a working microservices setup with:

- **Product Service** (.NET 8 Web API, MSSQL, Azure Blob upload for product images)
- **Order Service** (.NET 8 Web API, MSSQL, validation against Product Service, unit tests)
- **Frontend** (Angular, basic pages: product list, create/edit with image upload, place order, order history)
- **Infrastructure**: Dockerfiles, `docker-compose.yml` (MSSQL + Azurite + services + frontend), Swagger, Serilog, centralized exception handling

> Built to match the attached assignment requirements (two microservices, Azure Blob image upload, EF Core, Docker, Swagger, tests, GitHub push). citeturn1search1

---

## Quick Start (Docker)

> Prerequisites: Docker Desktop (or Docker Engine + Docker Compose). Ports used: `8080` (Product API), `8081` (Order API), `4200` (Frontend), `14333` (SQL), `10000-10002` (Azurite).

```bash
# 1) Clone (or extract this folder) and open a terminal at repo root
# 2) Build & run all services
docker compose up -d --build

# 3) Open UIs
# Product API Swagger
http://localhost:8080/swagger
# Order API Swagger
http://localhost:8081/swagger
# Frontend app
http://localhost:4200/
```

**Default credentials** for SQL Server (local containers only):
- `SA_PASSWORD`: `Your_strong_password123!`

> The solution uses **Azurite** (local Azure Storage emulator). Image uploads are sent to Azurite and the returned URLs are accessible via the Product Service. You can swap the connection string to real Azure Storage without code changes. citeturn1search1

---

## Running Locally (without Docker)

**Prerequisites**: .NET 8 SDK, Node 18+, Angular CLI (`npm i -g @angular/cli`), SQL Server (local or remote), and Azurite (optional) or real Azure Blob Storage.

1. Create two SQL databases (or let EF create them):
   - `ProductDb`
   - `OrderDb`
2. Update connection strings in:
   - `backend/product-service/ProductService/appsettings.Development.json`
   - `backend/order-service/OrderService/appsettings.Development.json`
3. (Optional) Start **Azurite** locally:
   ```bash
   docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
   ```
4. From each API folder, run:
   ```bash
   dotnet restore
   dotnet build
   dotnet ef database update
   dotnet run
   ```
   APIs will run on `http://localhost:8080` (Product) and `http://localhost:8081` (Order).
5. Frontend:
   ```bash
   cd frontend
   npm install
   npm start
   ```
   Open `http://localhost:4200`.

---

## Database & EF Core

- Each service has its **own DbContext** and database.
- **Migrations** are enabled and **applied at startup** (`Database.Migrate()`). Initial seed data is added via `OnModelCreating` for quick testing. You can also add and update migrations via CLI:

```bash
# Product Service
cd backend/product-service/ProductService
 dotnet ef migrations add InitialCreate
 dotnet ef database update

# Order Service
cd ../../order-service/OrderService
 dotnet ef migrations add InitialCreate
 dotnet ef database update
```

---

## Unit Tests (Order Service)

```bash
cd backend/order-service/tests/OrderService.Tests
 dotnet test
```

Tests cover:
- Valid order flow (sufficient stock)
- Insufficient stock validation
- Product not found handling

---

## GitHub – Push Instructions

```bash
# From repo root
 git init
 git add .
 git commit -m "feat: initial microservices setup (.NET 8 + Angular + MSSQL + Azurite)"
 git branch -M main
 git remote add origin https://github.com/<YOUR_GITHUB_USERNAME>/<YOUR_REPO_NAME>.git
 git push -u origin main
```

> Replace `<YOUR_GITHUB_USERNAME>` and `<YOUR_REPO_NAME>` with your actual GitHub values. Enable GitHub Actions (optional) for CI.

---

## Services & Endpoints (Highlights)

### Product Service
- `GET /products` – list products
- `GET /products/{id}` – get by id
- `POST /products` – create
- `PUT /products/{id}` – update
- `DELETE /products/{id}` – delete
- `POST /products/{id}/image` – **upload product image** to Azure Blob (Azurite in dev), returns `imageUrl`. Container configurable in `appsettings`. citeturn1search1

### Order Service
- `POST /orders` – place order (validates product stock via Product Service)
- `GET /orders/{id}` – order by id
- `GET /orders` – list orders

---

## Configuration

### Environment Variables (examples)

- **Product Service**
  - `ConnectionStrings__DefaultConnection` – SQL connection string
  - `AzureBlob__ConnectionString` – Azure Storage/Azurite connection
  - `AzureBlob__ContainerName` – container for images (default: `product-images`)
- **Order Service**
  - `ConnectionStrings__DefaultConnection` – SQL connection string
  - `ProductService__BaseUrl` – URL for Product Service (e.g., `http://product-service:8080` in Docker)

---

## Assumptions & Tradeoffs

- Blob deletes & SAS URLs are not enabled by default (kept simple), but code is structured for easy extension.
- Concurrency on stock is simplistic for demo purposes.
- Angular UI is intentionally basic to focus on correctness over styling. citeturn1search1

---

## Project Structure

```
backend/
  product-service/
    ProductService/
      Controllers/
      Data/
      Models/
      Services/
      Migrations/ (generated via CLI)
      ProductService.csproj
      Program.cs
      appsettings.json
      appsettings.Development.json
      Dockerfile
  order-service/
    OrderService/
      Controllers/
      Data/
      Models/
      Services/
      Migrations/ (generated via CLI)
      OrderService.csproj
      Program.cs
      appsettings.json
      appsettings.Development.json
      Dockerfile
    tests/
      OrderService.Tests/
        OrderService.Tests.csproj
        OrderServiceTests.cs
frontend/
  src/ (... Angular app ...)
  angular.json
  package.json
  tsconfig.json
  Dockerfile
  nginx.conf

docker-compose.yml
README.md
```
