# BidNexus

**BidNexus** is a multi-tenant B2B procurement and auction platform designed around the complete auction lifecycle — from procurement requirements and vendor qualification to real-time competitive bidding, auction completion, and result generation.

The project is built as a production-oriented full-stack system to demonstrate how a procurement auction platform can be designed with **Clean Architecture, tenant isolation, real-time communication, transactional bid processing, and auditable auction results**.

> **Project status:** Core auction workflow, bidding engine, real-time auction experience, auction statements, dashboards, masters, authentication, and responsive UI are implemented. The current focus is deployment hardening and operational improvements.

---

## What BidNexus Solves

A procurement auction is more than a CRUD operation.

An organization needs to:

1. Define what it wants to procure.
2. Create and authorize an auction.
3. Define its requirements and commercial rules.
4. Invite or qualify vendors.
5. Schedule and open the auction.
6. Allow vendors to compete under auction-specific rules.
7. Process bids safely while multiple vendors are bidding.
8. Keep the auction state synchronized in real time.
9. Close the auction.
10. Produce a historical result showing the winning bid and ranking.

BidNexus models these responsibilities as a connected business workflow rather than treating each screen as an isolated feature.

---

# High-Level Architecture

```text
                         ┌───────────────────────────┐
                         │       React Frontend      │
                         │      TypeScript + Vite    │
                         └─────────────┬─────────────┘
                                       │
                                HTTP / JWT
                                       │
                                       ▼
                         ┌───────────────────────────┐
                         │       ASP.NET Core API    │
                         │ Controllers / Middleware  │
                         └─────────────┬─────────────┘
                                       │
                                       ▼
                         ┌───────────────────────────┐
                         │       Application Layer   │
                         │ Services / Business Flow  │
                         └─────────────┬─────────────┘
                                       │
                                       ▼
                         ┌───────────────────────────┐
                         │        Core Domain        │
                         │ Auction Engine / Bid Core │
                         │ Entities / Abstractions   │
                         └─────────────┬─────────────┘
                                       │
                                       ▼
                         ┌───────────────────────────┐
                         │       Infrastructure      │
                         │ EF Core / Repositories     │
                         │ PostgreSQL / Persistence   │
                         └─────────────┬─────────────┘
                                       │
                                       ▼
                              ┌─────────────────┐
                              │   PostgreSQL    │
                              └─────────────────┘

        Real-time path:
        Bid accepted → Commit → SignalR → Connected clients
```

The architecture deliberately keeps **business rules away from controllers and persistence concerns**.

---

# Backend Architecture

The backend is organized around Clean Architecture principles.

```text
Backend/BidNexus
│
├── BidNexus/              → ASP.NET Core API
│   ├── Controllers/
│   ├── Services/
│   ├── Middleware/
│   ├── ModelValidators/
│   └── Program.cs
│
├── Core/                  → Domain & application core
│   ├── Entities/
│   ├── Models/
│   ├── Enumeration/
│   ├── Services/
│   ├── Abstraction/
│   ├── Exceptions/
│   └── Utils/
│
└── Infrastructure/        → Persistence & external infrastructure
    ├── Repository/
    ├── DbContext/
    ├── Configurations/
    └── Data / migrations
```

### API Layer

Responsible for:

- HTTP endpoints
- Authentication and authorization boundaries
- Request/response models
- Validation integration
- Middleware
- Exception handling
- API composition

Controllers do not own the auction business rules.

### Core

Contains the business-facing behavior and abstractions.

Important auction components include:

- `AuctionEngine`
- `BidCoreService`
- `IAuctionEngine`
- `IAuctionRealtimeCoreService`
- Auction lifecycle services
- Domain entities and models
- Business exceptions and enumerations

### Infrastructure

Responsible for:

- Entity Framework Core
- PostgreSQL persistence
- Repository implementations
- Database transactions
- Data access and query composition
- Entity configurations and migrations

---

# Auction Lifecycle

The auction is treated as a stateful business process.

```text
 Draft
   │
   ▼
 Authorized
   │
   ▼
 Scheduled
   │
   ▼
 Open
   │
   ▼
 Completed
   │
   ▼
 Auction Statement
```

Depending on the auction configuration, vendor intent and qualification workflows can participate before the auction becomes available for bidding.

The lifecycle worker currently evaluates auctions that need to start or complete and invokes the corresponding business operations.

The important design decision is that the **worker is an execution mechanism**, while the lifecycle rules belong in the application/domain services. This allows the scheduling mechanism to evolve later without rewriting auction business logic.

---

# Auction Creation Flow

```text
Organization
     │
     ▼
Create Auction
     │
     ├── Auction metadata
     ├── Auction type
     ├── Category
     ├── Start / end time
     ├── Requirements
     ├── Commercial rules
     └── Vendor participation
     │
     ▼
Authorize
     │
     ▼
Schedule
     │
     ▼
Open Auction
```

An auction is therefore the root of a larger procurement workflow rather than just a timed page.

---

# Vendor Participation

Vendor access is controlled according to the auction and tenant context.

The system supports:

- Vendor registration
- Vendor tenancy
- Auction visibility
- Open-to-all auctions
- Vendor intent
- Vendor qualification
- Bid eligibility
- Vendor-specific access rules

For restricted auctions, a vendor does not automatically gain bidding access simply because the auction exists.

---

# Bid Processing Architecture

The most important backend workflow is the bid pipeline.

```text
Vendor
  │
  ▼
Bid API
  │
  ▼
Bid Service
  │
  ├── Tenant / user context
  ├── Amount construction
  ├── Tax calculation
  └── Bid model creation
  │
  ▼
Bid Core Service
  │
  ├── Vendor qualification
  ├── Auction validation
  ├── Requirement validation
  └── Basic bid rules
  │
  ▼
Auction Engine
  │
  ├── Auction state validation
  ├── Current leading bid
  ├── Forward / reverse comparison
  └── Per-auction serialization
  │
  ▼
Bidding Repository
  │
  ├── Transaction
  ├── Revision creation
  ├── Previous bid invalidation
  └── Atomic persistence
  │
  ▼
PostgreSQL
  │
  ▼
SignalR notification
  │
  ▼
Connected clients
```

This separation makes the bidding workflow independently understandable and testable.

---

# Bid Revision Model

BidNexus does not simply overwrite a vendor's previous bid.

Each bid belongs to a revision chain.

```text
Main Bid
   │
   ├── Revision 1
   ├── Revision 2
   ├── Revision 3
   └── Revision 4 ← current
```

The bid model uses:

- `Id` → individual bid identity
- `MainBidId` → identifies the bid series
- `BidRevisionNo` → ordering within the series
- `IsCurrent` → identifies the active revision

This preserves bidding history while allowing the current auction state to be determined efficiently.

---

# Auction Engine

The Auction Engine is responsible for auction-specific bid decisions.

It handles concepts such as:

- Auction state
- Current bid
- Forward auction comparison
- Reverse auction comparison
- Bid serialization
- Delegation to persistence

For a **forward auction**, the competitive direction is higher.

For a **reverse auction**, the competitive direction is lower.

The comparison is performed server-side. The browser never decides whether a bid is valid.

---

# Concurrency Strategy

Multiple vendors may submit bids at nearly the same time.

BidNexus currently serializes bid processing **per auction** rather than globally.

```text
Auction A
  ├── Bid 1
  ├── Bid 2
  └── Bid 3
      ↓
   Serialized

Auction B
  ├── Bid 1
  └── Bid 2
      ↓
   Independently processed
```

The current implementation uses a per-auction `SemaphoreSlim` inside the application process.

This is appropriate for the current single-instance deployment model.

For horizontal scaling, the architecture is intended to evolve toward database/distributed concurrency controls, with PostgreSQL being the natural place to strengthen cross-instance serialization and optimistic concurrency.

RabbitMQ is **not required merely to solve the current bid-concurrency problem**.

---

# Transactional Bid Persistence

A bid update is persisted atomically.

Conceptually:

```text
BEGIN TRANSACTION

    Validate current state

    Create new bid revision

    Mark previous revision as not current

    Persist new state

COMMIT
```

If persistence fails, the transaction is rolled back.

This prevents the auction from ending up with a partially updated revision chain.

---

# Tax & Commercial Calculation

Bid values are calculated on the server.

The calculation flow is:

```text
Tax Master
    ↓
Tax Nature
    ↓
Charge Type
    ↓
Tax Value
    ↓
Tax Amount
    ↓
Net Amount
```

Current global commercial definitions include:

### Charge Type

- Fixed
- Percentage

### Tax Nature

- Additive
- Deductive

The client provides input; the server remains authoritative for financial calculations.

---

# Real-Time Auction Architecture

Real-time communication is handled using **SignalR**.

The important rule is:

> **Persistence first, notification second.**

```text
Client submits bid
       │
       ▼
Bid validation
       │
       ▼
Database transaction
       │
       ▼
Commit succeeds
       │
       ▼
SignalR event
       │
       ├── Organization clients
       └── Authorized vendor clients
```

SignalR is therefore a synchronization/notification mechanism, not the source of truth.

The database remains authoritative.

---

# Competitive Data Protection

Auction participants should not receive information they are not entitled to see.

Vendor-facing bid responses therefore apply masking rules where required.

Depending on auction configuration:

- Competitor identity can be masked.
- Competitor prices can be hidden.
- Competitor bid details can be removed.
- A vendor's own bid remains identifiable to that vendor.
- Leaderboard information is exposed according to the auction's visibility rules.

This is implemented server-side rather than relying on the frontend to hide sensitive information.

---

# Multi-Tenant Architecture

BidNexus is designed as a multi-tenant platform.

The authenticated context carries tenant information and the backend uses that context when resolving tenant-owned data.

Conceptually:

```text
JWT
 │
 ├── User
 ├── Role
 └── Tenant
       │
       ▼
Application context
       │
       ▼
Tenant-aware queries
       │
       ▼
Tenant-owned data
```

Tenant isolation is treated as a backend security responsibility.

The frontend must never be considered a security boundary.

---

# Auction Statement

When an auction completes, the system generates an auction statement.

```text
Auction Completed
       │
       ▼
Collect current bids
       │
       ▼
Apply auction direction
       │
       ▼
Rank vendors
       │
       ▼
Determine winner
       │
       ▼
Persist statement
```

The statement records:

- Vendor ranking
- Winning bid
- Winning vendor
- Bid reference
- Auction result

For a reverse auction, the lowest eligible bid wins.

For a forward auction, the highest eligible bid wins.

The statement acts as a historical snapshot rather than recalculating the result every time it is viewed.

---

# Frontend Architecture

The frontend is a React + TypeScript application built with Vite.

The current UI is organized around product features and reusable application components.

```text
src/
├── components/
├── features/
│   ├── auctions/
│   ├── overview/
│   ├── masters/
│   ├── profile/
│   └── auth/
├── services/
├── types/
├── utils/
├── styles/
└── lib/
```

The application includes separate experiences for:

- Organization users
- Vendors
- Auction management
- Live auctions
- Bidding
- Auction statements
- Masters
- Workspace/profile areas

The shell is responsive and supports desktop and mobile navigation.

---

# Frontend → Backend Flow

```text
React UI
   │
   ├── REST API requests
   │
   ▼
ASP.NET Core API
   │
   ▼
Application / Core
   │
   ▼
PostgreSQL

For live auction updates:

PostgreSQL
   │
   ▼
SignalR
   │
   ▼
React live auction UI
```

The frontend is responsible for presentation and interaction.

Business decisions such as bid validity, auction direction, qualification and financial calculations remain server-side.

---

# Authentication

The current authentication model uses:

- JWT access tokens
- Tenant-aware identity
- Role-aware application behavior
- Token refresh lifecycle
- Protected API access

OAuth/social authentication is intentionally deferred until the core platform workflow is stable.

---

# Technology Stack

## Backend

- C#
- .NET 10
- ASP.NET Core
- Entity Framework Core 10
- PostgreSQL
- Npgsql
- JWT authentication
- SignalR
- Clean Architecture
- Repository pattern

## Frontend

- React 19
- TypeScript
- Vite
- SignalR JavaScript client
- Responsive application shell

## Development & Delivery

- Git / GitHub
- Docker
- PostgreSQL
- Swagger / OpenAPI
- Unit testing project
- EF Core migrations

---

# Repository Structure

```text
BidNexus/
│
├── Backend/
│   └── BidNexus/
│       ├── BidNexus/          # API
│       ├── Core/              # Domain/application core
│       └── Infrastructure/    # Persistence
│
├── Frontend/
│   └── BidNexus/              # React application
│
└── README.md
```

---

# Current Product Capabilities

### Implemented

- [x] Multi-tenant foundation
- [x] JWT authentication
- [x] Organization and vendor workflows
- [x] Global reference/master data
- [x] Auction CRUD
- [x] Auction requirements
- [x] Vendor participation / qualification flow
- [x] Auction lifecycle
- [x] Auction Engine
- [x] Bid Engine
- [x] Bid revision history
- [x] Forward and reverse auction behavior
- [x] Server-side tax/commercial calculation
- [x] Transactional bid persistence
- [x] Real-time SignalR updates
- [x] Auction activity / leaderboard behavior
- [x] Auction statement generation
- [x] Organization dashboard
- [x] Vendor dashboard
- [x] Responsive live auction experience
- [x] Responsive application shell
- [x] API documentation through Swagger/OpenAPI
- [x] EF Core migrations
- [x] Docker-ready backend

---

# Planned / Next Engineering Steps

The following are intentionally **not represented as completed features**.

### Near-term hardening

- Strengthen database-level concurrency for multi-instance deployments.
- Complete end-to-end and concurrency testing around bidding.
- Review all auction and bid access paths for strict tenant isolation.
- Improve post-commit real-time event reliability.

### Background processing

The current lifecycle processing uses a hosted background worker.

The next evolution is to introduce **Hangfire** as the scheduling/execution mechanism for tasks such as:

- Auction lifecycle jobs
- Scheduled processing
- Overnight notification jobs
- Email notifications for auction events

The business logic will remain in application services so that Hangfire is an execution mechanism rather than a second business layer.

### Later platform evolution

- OAuth / external identity providers
- Email verification
- Stronger distributed concurrency controls
- Advanced analytics
- Notification center
- Further operational observability

---

# Design Principles

BidNexus is being developed around a few core principles:

### 1. Server is authoritative

The browser never determines whether a bid, tax calculation or auction transition is valid.

### 2. Business logic is separated from transport

Controllers coordinate requests. Core services perform business decisions.

### 3. Persistence is transactional

Auction state and bid revisions are persisted atomically where the business operation requires it.

### 4. Real-time communication is not the source of truth

SignalR distributes state changes; PostgreSQL remains authoritative.

### 5. Tenant isolation is a security concern

Tenant boundaries are enforced in the backend rather than relying on UI behavior.

### 6. Historical results should remain reproducible

Auction statements preserve the outcome of a completed auction instead of depending entirely on mutable live state.

### 7. Scale deliberately

The current architecture avoids introducing distributed infrastructure before it is necessary. Components such as RabbitMQ or distributed locking can be introduced when actual deployment topology requires them.

---

# Running the Project

## Backend

The backend solution is located at:

```text
Backend/BidNexus/BidNexus.slnx
```

The API project is:

```text
Backend/BidNexus/BidNexus/
```

## Frontend

The React application is located at:

```text
Frontend/BidNexus/
```

Install dependencies and run the Vite development server from that directory.

## Database

BidNexus uses PostgreSQL.

EF Core migrations are maintained with the backend solution and should be applied against the configured `BidNexus` database.

---

# Why This Project Exists

BidNexus is intentionally more than a CRUD application.

The project is an exploration of how a real procurement platform can handle:

- stateful business workflows
- multi-tenancy
- authorization
- competitive bidding
- concurrency
- transactional persistence
- real-time communication
- financial calculations
- historical result generation
- responsive product design

The goal is to build the system from the business workflow downward rather than simply assembling screens around database tables.

---

## Project

**BidNexus — B2B Procurement & Real-Time Auction Platform**

Built with **.NET 10, PostgreSQL, React, TypeScript and SignalR**.

[Repository](https://github.com/Pranjal91000/BidNexus)
