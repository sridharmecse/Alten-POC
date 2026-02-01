# Full Stack Microservices Web App (.NET 8 + Angular + MSSQL + Azure Blob ready)

This repository contains a working microservices setup with:

- **Product Service** (.NET 8 Web API, MSSQL, Azure Blob upload for product images)
- **Order Service** (.NET 8 Web API, MSSQL, validation against Product Service, unit tests)
- **Frontend** (Angular, basic pages: product list, create/edit with image upload, place order, order history)
- **Infrastructure**: Dockerfiles, `docker-compose.yml` (MSSQL + Azurite + services + frontend), Swagger, Serilog, centralized exception handling

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

> The solution uses **Azurite** (local Azure Storage emulator). Image uploads are sent to Azurite and the returned URLs are accessible via the Product Service. You can swap the connection string to real Azure Storage without code changes.

---

## Running Locally (without Docker)

**Prerequisites**: .NET 8 SDK, Node 18+, Angular CLI (`npm i -g @angular/cli`), SQL Server (local or remote), and Azurite (optional) or real Azure Blob Storage.

1. Create two SQL databases (or let EF create them):
   - `ProductDb`
   - `OrderDb`
2. Update connection strings in:
   - `backend/product-service/ProductService/appsettings.json`
   - `backend/order-service/OrderService/appsettings.json`
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

## Services & Endpoints (Highlights)

### Product Service
- `GET /products` – list products
- `GET /products/{id}` – get by id
- `POST /products` – create
- `PUT /products/{id}` – update
- `DELETE /products/{id}` – delete
- `POST /products/{id}/image` – **upload product image** to Azure Blob (Azurite in dev), returns `imageUrl`. Container configurable in `appsettings`.

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

README.md
```
