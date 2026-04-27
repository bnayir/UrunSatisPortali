#  Product Sales & E-Commerce Portal

> **A comprehensive E-Commerce Solution** built with **.NET Core MVC**, featuring a secure management panel, real-time notifications, and a scalable repository-based architecture.

---

##  Overview
This project is a full-stack web application designed for seamless product management and sales. It provides two distinct interfaces:
* **User-facing Portal:** For customers to browse and interact with products.
* **Robust Admin Panel:** A secure management dashboard for store administrators to control inventory.

The system is built on **.NET 8.0** using the **Model-View-Controller (MVC)** design pattern, focusing on industry-standard clean code practices.

##  Tech Stack & Patterns
* **Framework:** .NET Core MVC
* **Database:** MSSQL Server (Code-First approach)
* **Frontend:** Bootstrap 4+, JQuery, AJAX
* **Real-Time:**  **SignalR** (Integrated for live updates and notifications)
* **Security:** * **Cookie-based Authentication** & **ASP.NET Core Identity**
    * **Role-Based Access Control (RBAC)** (Admin/User permissions)
* **Design Pattern:** **Repository Pattern** for a decoupled and testable data layer.

##  Key Features
* ** Secure Membership:** Full implementation of ASP.NET Core Identity for secure registration and login.
* ** Advanced Admin Dashboard:** Specialized interface for handling products, categories, and user management.
* ** AJAX Integration:** Optimized user experience with asynchronous data operations (no page refreshes).
* ** Real-time Notifications:** Integrated **SignalR** in the admin panel for live monitoring.
* ** Responsive Design:** Fully compatible with mobile and desktop devices thanks to **Bootstrap**.

##  Technical Highlights
* **Separation of Concerns:** Utilized **ViewModels** to keep UI logic separate from database models, enhancing security.
* **Scalable Architecture:** Implemented the **Repository Pattern** to ensure the codebase remains maintainable as the project grows.
* **Database Versioning:** Managed all schema changes through **EF Core Migrations** for a seamless development workflow.

---

