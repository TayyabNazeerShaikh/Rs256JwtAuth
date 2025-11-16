# ASP.NET Core 9 — RS256 JWT Auth + Employee CRUD + Role-Based Access

A complete, lightweight demo showing how to implement **RS256 JWT authentication**, **Employee CRUD**, and **role-based authorization** in ASP.NET Core 9 using **separated files**, **clean structure**, and **RSA key pairs**.

This README is designed to be **GitHub‑ready**, clear, and beginner‑friendly.

---

## 🚀 Features

* ✔ RS256 JWT Authentication (Public/Private RSA Keys)
* ✔ Login endpoint (Admin or Employee)
* ✔ Employee CRUD (Admin‑only)
* ✔ Inventory access (Admin can Add/Remove, Employee can only View)
* ✔ Clean file-by-file architecture
* ✔ In-memory repositories for quick learning
* ✔ Secure key handling (private key never exposed)

---

## 📁 Project Structure

```
AspNetJwtRs256/
├─ Program.cs
├─ appsettings.json
├─ Keys/
│  ├─ private_key.pem   # keep secret
│  └─ public_key.pem
├─ Controllers/
│  ├─ AuthController.cs
│  ├─ EmployeeController.cs
│  └─ InventoryController.cs
├─ Models/
│  ├─ Employee.cs
│  ├─ InventoryItem.cs
│  └─ LoginRequest.cs
├─ Repositories/
│  ├─ EmployeeRepository.cs
│  └─ InventoryRepository.cs
└─ README.md
```

---

## 🔐 RSA Key Generation

Run this (Linux, macOS, WSL, Git Bash):

```bash
# Generate 2048-bit RSA private key
openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:2048 -out private_key.pem

# Extract the public key
openssl rsa -pubout -in private_key.pem -out public_key.pem
```

Place both inside the `Keys` folder.

> Never commit `private_key.pem` to GitHub.

---

## ⚙ appsettings.json

```json
{
  "Jwt": {
    "Issuer": "MyAuthServer",
    "Audience": "MyResourceApi",
    "DurationInMinutes": "60"
  }
}
```

---

## 🟦 Authentication Flow (Simplified)

1. Client sends **request** to `/api/auth/login`.
2. Server finds employee → signs JWT using **private RSA key**.
3. Client stores token → sends it in **Authorization: Bearer** header.
4. Server verifies token using **public RSA key**.

---

## 🔑 Role Logic

### **Admin** can:

* Get all employees
* Add employee
* Update employee
* Delete employee
* Add/remove inventory items

### **Employee** can:

* View inventory only

---

## 🧪 Test Login

### Example Request

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@test.com"
}
```

### Example Response

```json
{
  "token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

Use this token for all authenticated requests:

```
Authorization: Bearer <token>
```

---

## 📚 Endpoints Overview

### 🔐 Auth

| Method | Endpoint          | Description                  |
| ------ | ----------------- | ---------------------------- |
| POST   | `/api/auth/login` | Generates an RS256 JWT token |

### 👤 Employee (Admin Only)

| Method | Endpoint             | Description       |
| ------ | -------------------- | ----------------- |
| GET    | `/api/employee`      | Get all employees |
| POST   | `/api/employee`      | Create employee   |
| PUT    | `/api/employee`      | Update employee   |
| DELETE | `/api/employee/{id}` | Delete employee   |

### 📦 Inventory

| Method | Endpoint              | Role             | Description    |
| ------ | --------------------- | ---------------- | -------------- |
| GET    | `/api/inventory`      | Admin + Employee | View inventory |
| POST   | `/api/inventory`      | Admin            | Add item       |
| DELETE | `/api/inventory/{id}` | Admin            | Remove item    |

---

## ▶ Run Project

```bash
dotnet restore
dotnet run
```

API is available at:

```
https://localhost:7190
```

---

## 🧱 Next Steps (Optional Improvements)

* Add EF Core + SQL database
* Add password authentication + hashing
* Add user registration
* Implement Refresh Tokens
* Add Swagger JWT support

---

## 📄 License

MIT — free to use, modify, and learn.

---

**Happy Coding! 🚀**
