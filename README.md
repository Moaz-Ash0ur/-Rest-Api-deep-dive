# 🧠 REST API Deep Dive

A practical project built with **ASP.NET Core Web API** to explore and apply real-world REST concepts — including HTTP methods, status codes, content negotiation, pagination, file handling, redirection, and custom middleware.

---

## 🚀 Project Overview

This project serves as a **learning and experimentation playground** for RESTful API design in .NET.

It simulates an in-memory `ProductRepository` with related `ProductReview` data to demonstrate key REST concepts using `ControllerBase` (non-minimal API).

---

## 🧩 Features

✅ Full CRUD for `Product`  
✅ Nested routes for `ProductReview`  
✅ Handling various HTTP methods (GET, POST, PUT, PATCH, DELETE, OPTIONS, HEAD)  
✅ Pagination and filtering  
✅ Proper use of status codes (`200`, `201`, `204`, `400`, `404`, `409`, `500`, etc.)  
✅ File upload & download (with MIME type handling)  
✅ CSV export using `File()` and `PhysicalFile()`  
✅ Temporary & permanent redirects  
✅ `Accepted (202)` async process simulation  
✅ Custom middleware integration for request logging & error handling  

---

## 📦 Technologies

- **.NET 9 / ASP.NET Core Web API**
- **C#**
- **In-memory Repository Pattern**
- **JSON Patch**
- **Swagger (OpenAPI) for testing**
- **MIME type handling**

---


## 📁 Project Structure

```text
RestApiDeepDive/
│
├── Controllers/
│   └── ProductController.cs
│
├── Middlewares/
│   └── ErrorHandlingMiddleware.cs
│
├── Models/
│   ├── Product.cs
│   ├── ProductReview.cs
│   ├── Requests/
│   │   ├── CreateProductRequest.cs
│   │   ├── UpdateProductRequest.cs
│   │   └── CreateProductReviewRequest.cs
│   └── Responses/
│       ├── ProductResponse.cs
│       ├── ProductReviewResponse.cs
│       └── PagedResult.cs
│
├── Repositories/
│   └── ProductRepository.cs
│
└── Program.cs










