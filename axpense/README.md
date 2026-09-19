# Axpense

B2B SaaS foundation for fleet, asset, equipment, maintenance, expenses and budgeting.

## Stack
- ASP.NET Core / C#
- Entity Framework Core
- PostgreSQL
- React + TypeScript
- REST API
- Docker-ready

## Phase 1
Foundation, tenant-aware domain model, authentication-ready structure, dashboard shell, and development Docker setup.

## Run
Prerequisites: .NET 8 SDK, Node.js 20+, Docker Desktop.

See `docs/PHASE-1.md`.

## Phase 2
Organization/user model, role field, tenant-scoped uniqueness, vehicle CRUD API, and Vehicles UI.

## Phase 3
Maintenance scheduling, completion workflow, maintenance API, and dashboard KPIs.

## Phase 4 — Expenses, Fuel & Budgets
- Expense tracking with categories, dates, vendors and vehicle links
- Fuel transaction tracking with quantity, unit price and calculated total
- Monthly budgets by category
- Dashboard monthly expense, fuel, maintenance and total-cost KPIs
- Tenant-scoped cost indexes and validation

## Phase 5 — Drivers & Assignments
- Driver management with license/status data
- Vehicle-to-driver assignments with start/end dates
- Assignment types: Primary, Temporary, Pool
- Tenant-scoped APIs and indexes
- Drivers and Assignments UI

Equipment & Assets intentionally skipped per product direction.

## Phase 6 — Reports & Analytics
- Monthly cost summary for expenses, fuel and completed maintenance.
- Cost by vehicle.
- Expense-category analysis.
- Responsive Reports & Analytics UI with month/year filters.
