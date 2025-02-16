# Product Inventory Management System (PIMS)

## Overview
The **Product Inventory Management System (PIMS)** is a backend application built using **ASP.NET Core** to manage products, inventory, and user roles efficiently. It provides a **secure, versioned RESTful Web API** for interacting with the system.  

The application integrates **SQL Server** for persistent storage, **Entity Framework Core** for database management, and **JWT authentication** for secure access. It also supports **role-based access control (RBAC)**, **global error handling**, and **logging using NLog**.

---

## Features

### 🔹 Product Management
- Categorization of products (e.g., electronics, clothing, groceries).  
- Price adjustment logic for individual or bulk updates.  
- SKU uniqueness validation for products.  

### 🔹 Inventory Management
- Inventory adjustment transactions with timestamps and reasons.  
- Low inventory alerts based on predefined thresholds.  
- Inventory auditing with manual adjustments and audit trails.  

### 🔹 User Authentication & Authorization
- JWT-based authentication.  
- **Role-based access control (RBAC)** with two roles:  
  - **Administrator**: Can manage products and inventory.  
  - **User**: Can view products and inventory levels.  
- Secure credential storage using hashing and salting.  

### 🔹 Global Error Handling & Logging
- Centralized error handling for user-friendly responses.  
- Logging of API requests, responses, and application errors using **NLog**.  

### 🔹 Testing & Documentation
- **Comprehensive unit and integration tests** using `xUnit`.  
- **API documentation** via **Swagger/OpenAPI**.  

---

## 🛠 Technologies Used
| Component       | Technology |
|----------------|------------|
| Backend Framework | .NET Core 6/7/8 |
| Database | SQL Server |
| ORM | Entity Framework Core (Code-First) |
| Authentication | JWT (JSON Web Tokens) |
| Logging | NLog |
| API Documentation | Swagger/OpenAPI |
| Testing | xUnit |

---

## 📌 Requirements
- .NET Core SDK (6/7/8)
- SQL Server
- Visual Studio / Visual Studio Code

---

## 🚀 Getting Started

### 1️⃣ Clone the Repository
```bash
git clone https://github.com/your-username/PIMS.git
cd PIMS
```
### 2️⃣ Clean and Build sln
### 3️⃣ Configure the Database
```bash
- Update the connection string in appsettings.json i.e Add your server name and DBname.
- Run migrations to create the database: dotnet ef database update / update-database
```
### 4️⃣ Run the Application
```bash
- dotnet run
- Open **SwaggerUI**
  - 👉 http://localhost:5077/index.html
```
### 5️⃣ Admin/User Credential
```bash
- (Admin)
 - Email : admin@pims.com
 - Password : Admin@123
- (User)
 - Email : user@pims.com
 - Password : User@123
```
## 📡 API Endpoints
### 🔐 Authentication
 - **POST /api/auth/login** - Authenticate and receive a JWT token.
 - **POST /api/auth/register** - Register a new user.
 ### 🔐 Products
 - **GET /api/v1/products** - Get all products.
 - **GET /api/v1/products/{id}** - Get a product by ID.
 - **POST /api/v1/products** - Create a new product.
 - **PUT /api/v1/products/{id}** - Update a product.
 - **DELETE /api/v1/products/{id}** - Delete a product.
 ### 📦 Inventory
 - **GET /api/v1/inventory** - Get all inventory records.
 - **POST /api/v1/inventory** - Add an inventory transaction.
 - **PUT /api/v1/inventory/{id}** - Update inventory.
 ## 🧪 Testing
 ### Run unit tests:
 ```bash
 dotnet test
 ```
 ### 📖 Documentation
 [API documentation is available Here](https://localhost:5077/swagger)

 ### 👤 Author
 - Deepak Singh
 - [Email](151deepaksss@gmail.com)
 - [Github](https://github.com/deepaksinghh13/)

### 🤝 Contributing
Contributions are welcome!
Please open an issue or submit a pull request.
