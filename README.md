# BidNexus

**BidNexus** is a multi-tenant B2B procurement and real-time auction platform built around the complete auction workflow — from creating procurement requirements and managing vendors to live bidding and auction results.

It is designed as a production-oriented full-stack application, with a focus on clean architecture, tenant isolation, transactional bidding, real-time updates, and reliable auction outcomes.

> **Project status:** Core auction workflow, bidding, real-time updates, auction statements, dashboards, masters, authentication, and responsive UI are implemented. Current work is focused on deployment hardening and operational improvements.

## How It Works

```text
Organization
     │
     ▼
Create & Schedule Auction
     │
     ▼
Vendor Participation
     │
     ▼
Live Bidding
     │
     ▼
Auction Completion
     │
     ▼
Auction Statement
```

The platform supports both **forward and reverse auctions**, with bidding rules and winner selection handled on the server.

## High-Level Architecture

```text
React + TypeScript
        │
        ▼
ASP.NET Core API
        │
        ▼
Clean Architecture
        │
        ├── Auction & Bid Services
        ├── Auction Engine
        └── Business Rules
        │
        ▼
Entity Framework Core
        │
        ▼
PostgreSQL

Live updates:
PostgreSQL → SignalR → Connected Clients
```

The backend remains authoritative for auction state, bidding, financial calculations, authorization, and tenant isolation.

## Key Capabilities

- Multi-tenant organization and vendor workflows
- Auction creation, authorization and scheduling
- Vendor participation and qualification
- Forward and reverse auctions
- Real-time bidding with SignalR
- Bid revision history
- Server-side commercial and tax calculations
- Auction statements and winner determination
- Organization and vendor dashboards
- Responsive web application
- JWT authentication

## Technology Stack

**Backend:** C#, .NET 10, ASP.NET Core, EF Core, PostgreSQL, SignalR

**Frontend:** React 19, TypeScript, Vite

**Architecture:** Clean Architecture, REST APIs, Repository pattern

**Development:** Git, GitHub, Docker, Swagger/OpenAPI

## Repository Structure

```text
BidNexus/
├── Backend/
│   └── BidNexus/
│       ├── BidNexus/          # API
│       ├── Core/              # Business logic
│       └── Infrastructure/    # Persistence
│
├── Frontend/
│   └── BidNexus/              # React application
│
└── README.md
```

## Next Steps

The next phase focuses on:

- Deployment and security hardening
- Concurrency and integration testing
- Hangfire-based background processing
- Auction event email notifications
- Further operational improvements

## Why BidNexus

BidNexus is built to demonstrate how a real business workflow can be translated into a working full-stack system — combining **stateful workflows, multi-tenancy, authorization, concurrency, transactional persistence, real-time communication, and responsive product design**.

## Project

**BidNexus — B2B Procurement & Real-Time Auction Platform**

Built with **.NET 10, PostgreSQL, React, TypeScript and SignalR**.

[Repository](https://github.com/Pranjal91000/BidNexus)
