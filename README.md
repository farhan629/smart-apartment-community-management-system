# 🏢 Smart Apartment / Community Management System

## 📌 Project Overview

The **Smart Apartment / Community Management System** is a full-stack web application designed to digitally manage and automate common residential community operations.

The system provides a centralized platform for:

- 👥 Resident and flat management
- 🚪 Visitor registration and approval
- 📱 QR-based visitor passes
- 🛠️ Complaint and maintenance management
- 🏊 Amenity and facility booking
- 🔔 Notifications
- 📊 Dashboard and analytics
- 🔐 Authentication and authorization

The application supports multiple user roles including:

- Admin
- Resident
- Security
- Maintenance Staff

The project is built using **.NET 10 Web API** for the backend and **Angular 21** for the frontend, following **Clean Architecture**, **SOLID principles**, **RESTful API design**, and **Contract-First OpenAPI development**. :contentReference[oaicite:1]{index=1}

---

# 🏗️ System Architecture

The application follows a **Full-Stack Layered Architecture with Clean Architecture principles**. :contentReference[oaicite:2]{index=2}

```text
                         ┌──────────────────────┐
                         │       End User       │
                         │                      │
                         │ Admin                │
                         │ Resident             │
                         │ Security             │
                         │ Maintenance Staff    │
                         └──────────┬───────────┘
                                    │
                                    ▼
                         ┌──────────────────────┐
                         │     Angular 21       │
                         │      Frontend        │
                         └──────────┬───────────┘
                                    │
                                    ▼
                         ┌──────────────────────┐
                         │    OpenAPI Client    │
                         │   Generated Client   │
                         └──────────┬───────────┘
                                    │
                               HTTPS / REST
                                    │
                                    ▼
                  ┌──────────────────────────────────┐
                  │          .NET 10 Web API          │
                  │                                  │
                  │ Controllers / Middleware         │
                  └────────────────┬─────────────────┘
                                   │
                                   ▼
                  ┌──────────────────────────────────┐
                  │       Application Layer           │
                  │                                  │
                  │ Services / DTOs / Validators     │
                  │ Business Workflows                │
                  └────────────────┬─────────────────┘
                                   │
                                   ▼
                  ┌──────────────────────────────────┐
                  │          Domain Layer             │
                  │                                  │
                  │ Entities / Enums / Rules         │
                  │ Business Abstractions            │
                  └────────────────┬─────────────────┘
                                   │
                                   ▼
                  ┌──────────────────────────────────┐
                  │       Infrastructure Layer       │
                  │                                  │
                  │ EF Core / Repositories           │
                  │ Authentication / Email           │
                  │ Notifications / Caching          │
                  └────────────────┬─────────────────┘
                                   │
                                   ▼
                  ┌──────────────────────────────────┐
                  │       SQL Server / PostgreSQL    │
                  └──────────────────────────────────┘