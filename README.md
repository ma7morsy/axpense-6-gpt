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

## Phase 7
Notifications & reminders: maintenance due alerts, driver license expiry reminders, unread/read notifications, and notification generation endpoint.

## Phase 8 — Authentication & SaaS Security
- JWT authentication with 12-hour sessions
- Secure PBKDF2 password hashing
- Organization registration and login
- Role claims and active-user validation
- Tenant authorization middleware
- Cross-organization query/body protection
- Protected application APIs
- React login/register experience
- Demo administrator: admin@axpense.local / Axpense123!

## Phase 9
Advanced maintenance and inspections: inspection records, checklist items, pass/fail results, failure reasons, image references, completion workflow, and maintenance workflow API foundations.


## Phase 10 — Production SaaS Layer
- Organization settings
- Audit logs
- Admin-only settings updates
- Tenant-scoped CSV vehicle export
- Production health endpoint
- Saas configuration APIs

## Phase 11 — Final SaaS UX & Administration
- Users & roles administration UI
- Admin user activation/deactivation
- Organization settings UI
- Audit log viewer
- Vehicle CSV export from settings
- Responsive administration experience
- Server-side tenant-scoped user administration


## Phase 12 — Final Integration & Production Hardening

- Global RFC 7807-style problem details for unhandled API errors
- API rate limiting (120 requests/minute per application instance)
- Response compression
- Dedicated readiness endpoint at `/ready`
- Health endpoint retained at `/health`
- Controllers protected by the API rate limiter
- Consolidated deployment/readiness notes

### Production checklist

1. Set a strong `Jwt:Key` through environment variables or a secret manager.
2. Set a production PostgreSQL connection string through environment configuration.
3. Disable Swagger outside controlled environments.
4. Put the API behind HTTPS and a reverse proxy/load balancer.
5. Configure backups and PostgreSQL point-in-time recovery.
6. Replace the demo admin credentials before production use.
7. Configure a real email/notification provider before enabling external notifications.
8. Run database migrations instead of relying on `EnsureCreatedAsync` for a production database.
