# BidNexus — Project Status Dashboard

> Living engineering dashboard for BidNexus.
> **Notion manages individual work items; this file captures the actual engineering state, progression, decisions, risks, and next steps.**
>
> **Last updated:** 2026-09-19
> **Branch:** \`docs/project-status-dashboard\`

---

## 1. Executive Snapshot

| Area | Status | Current State |
|---|---|---|
| Domain & Entities | 🟢 Complete | Core domain/entity definitions have been completed and reviewed. |
| Clean Architecture / Project Structure | 🟢 Established | API, Core, Infrastructure and UnitTests structure is in place. |
| EF Core / PostgreSQL | 🟡 In Progress | AppDbContext/configuration exists; migration/database verification is the current foundation task. |
| GlobalData APIs | 🔴 Not Started | GET APIs are the next repetitive implementation slice. |
| Master APIs | 🔴 Not Started | Follows GlobalData and authentication conventions. |
| Registration / Tenant Creation | 🟡 In Progress | Tenant model and registration approach have been designed; implementation is a current priority. |
| Login / Authentication | 🔴 Planned | LoginAttempt and credential validation need implementation. |
| JWT | 🔴 Planned | Token generation follows login design. |
| Tenant Isolation | 🔴 Planned | JWT tenant context + EF Core query isolation is a critical security milestone. |
| Auction | 🔴 Not Started | Domain exists; API/business workflow remains. |
| Bidding | 🔴 Not Started | Depends on auction and participation flow. |
| Evaluation / Award | 🔴 Not Started | Depends on bidding. |
| Rating | 🔴 Not Started | Later domain workflow. |
| Frontend | 🔴 Not Started | Backend-first development strategy. |
| Deployment | 🔴 Not Started | Docker/CI/CD/production readiness comes after core functionality. |

### Current objective

**Reach a secure tenant-aware backend foundation:**

\`Database → GlobalData → Registration → Tenant → Login → JWT → Tenant Context → EF Isolation → Masters\`

---

## 2. Current Sprint

### 🎯 Authentication + Tenant Foundation

The most important user-owned engineering work is:

1. Finalize registration flow.
2. Create Vendor/Organization and Tenant correctly.
3. Establish user/credential relationship.
4. Implement login and LoginAttempt tracking.
5. Generate JWT access tokens.
6. Resolve the current tenant from authenticated context.
7. Enforce tenant isolation through EF Core.
8. Add integration tests proving cross-tenant data cannot leak.

### Sprint exit condition

> An authenticated user can make tenant-scoped requests and the backend determines the tenant from the authenticated security context rather than trusting a tenant ID supplied by the client.

---

## 3. Critical Path

\`\`\`text
┌──────────────────────┐
│ EF Core Migrations   │
└──────────┬───────────┘
           ↓
┌──────────────────────┐
│ GlobalData GET APIs  │
└──────────┬───────────┘
           ↓
┌──────────────────────────┐
│ Registration             │
│ Vendor / Organization    │
└──────────┬───────────────┘
           ↓
┌──────────────────────┐
│ Tenant Creation      │
└──────────┬───────────┘
           ↓
┌──────────────────────┐
│ Login + Credentials  │
└──────────┬───────────┘
           ↓
┌──────────────────────┐
│ JWT Access Token     │
└──────────┬───────────┘
           ↓
┌──────────────────────┐
│ Current Tenant       │
│ Context              │
└──────────┬───────────┘
           ↓
┌──────────────────────┐
│ EF Core Isolation    │
└──────────┬───────────┘
           ↓
┌──────────────────────┐
│ Master APIs          │
└──────────────────────┘
\`\`\`

---

## 4. Progress Visualization

These bars are intentionally **milestone-based**, not claims about exact code-completion percentages.

\`\`\`text
Domain / Entities       ████████████████████  Complete
Architecture            ████████████████░░░░  Established
Database Foundation     ████████░░░░░░░░░░░░  In Progress
GlobalData APIs         ░░░░░░░░░░░░░░░░░░░░  Pending
Master APIs             ░░░░░░░░░░░░░░░░░░░░  Pending
Auth / Tenant           ███████░░░░░░░░░░░░░  In Progress
Auction                 ░░░░░░░░░░░░░░░░░░░░  Pending
Bidding                 ░░░░░░░░░░░░░░░░░░░░  Pending
Evaluation              ░░░░░░░░░░░░░░░░░░░░  Pending
Rating                  ░░░░░░░░░░░░░░░░░░░░  Pending
Frontend                ░░░░░░░░░░░░░░░░░░░░  Pending
Deployment              ░░░░░░░░░░░░░░░░░░░░  Pending
\`\`\`

### Project-state distribution

Current board-level planning contains **37 implementation cards**:

\`\`\`text
Done          █
In Progress   ██████
Not Started   ██████████████████████████████
\`\`\`

> The exact task count is maintained in Notion. This dashboard intentionally does not duplicate the complete task board.

---

## 5. Architecture Snapshot

\`\`\`text
                         ┌─────────────────┐
                         │ React + TS/Vite │
                         └────────┬────────┘
                                  │
                                  ▼
                    ┌─────────────────────────┐
                    │ ASP.NET Core / .NET 10  │
                    │          API            │
                    └────────────┬────────────┘
                                 │
                 ┌───────────────┴───────────────┐
                 ▼                               ▼
       ┌──────────────────┐            ┌──────────────────┐
       │ Authentication   │            │ Domain / Use     │
       │ + Tenant Context │            │ Cases / Services │
       └────────┬─────────┘            └────────┬─────────┘
                │                               │
                └───────────────┬───────────────┘
                                ▼
                     ┌────────────────────┐
                     │ EF Core / Infra    │
                     └─────────┬──────────┘
                               ▼
                     ┌────────────────────┐
                     │    PostgreSQL      │
                     └────────────────────┘
\`\`\`

---

## 6. Domain Snapshot

### Global Data

- Category
- RatingFor
- RatingParameter
- Status

These are application-wide/reference entities and do **not** require tenant isolation.

### Master Data

- Unit
- Item
- ItemUnitMapping

These will become the first major tenant-aware/reference-data API layer after authentication and isolation conventions are established.

### Tenant / Identity

- Tenant
- Vendor
- Organization
- User / authentication-related entities
- LoginAttempt

### Auction Domain

- Auction
- AuctionRequirement
- AuctionStatement
- VendorIntent
- Bid
- BidDetail

### Rating

- Rating
- RatingValue

---

## 7. Tenant Architecture Decision

### Current model

A Vendor or Organization becomes associated with the platform through a Tenant.

\`\`\`text
Vendor / Organization
          │
          ▼
       Tenant
          │
          ▼
      User / Owner
\`\`\`

The Tenant is intended to be the **security and data-isolation boundary**.

### Important rule

Tenant-scoped requests should obtain tenant identity from authenticated context:

\`\`\`text
JWT
 ↓
Authentication
 ↓
Tenant Claim
 ↓
CurrentTenantContext
 ↓
EF Core Query Filters
 ↓
Tenant-scoped Data
\`\`\`

The client should not be trusted to decide which tenant's data is returned.

---

## 8. Authentication Target Design

JWT should contain only the identity/security information required by the backend, such as:

- User identifier
- Tenant identifier
- Role/authorization context
- Issuer
- Audience
- Expiration
- Optional token/JTI identifier where required

Target flow:

\`\`\`text
Registration
    ↓
Vendor / Organization
    ↓
Tenant creation
    ↓
User credentials
    ↓
Login
    ↓
Credential validation
    ↓
JWT generation
    ↓
Authenticated request
    ↓
Tenant context
    ↓
Tenant-isolated EF Core query
\`\`\`

---

## 9. Auction Lifecycle

\`\`\`text
                 ┌─────────────┐
                 │   Auction   │
                 └──────┬──────┘
                        │
             ┌──────────┴──────────┐
             ▼                     ▼
      Requirements          Vendor Intent
                                   │
                                   ▼
                                  Bids
                                   │
                                   ▼
                              Bid Details
                                   │
                                   ▼
                              Evaluation
                                   │
                                   ▼
                            Result / Award
                                   │
                                   ▼
                                Rating
\`\`\`

This is the central business journey that the backend will progressively expose through APIs.

---

## 10. Workstream Details

### Foundation

**Completed / established**
- API startup and dependency injection.
- Clean Architecture project organization.
- Core entity definitions.
- EF Core/AppDbContext foundation.
- PostgreSQL direction.

**Next**
- Create/verify migration baseline.
- Confirm database creation/update path.
- Establish common API/error conventions.
- Verify Swagger/OpenAPI conventions.

### GlobalData

Planned GET APIs:

- Category — all / by ID
- RatingFor — all / by ID
- RatingParameter — all / by ID
- Status — all / by ID

Each should eventually include appropriate DTO/read model, not-found handling, Swagger documentation and tests.

### Master

Initial APIs:

- Unit — all / by ID
- Item — all / by ID
- Item search/filter/pagination
- ItemUnitMapping — relevant lookup APIs

### Authentication & Tenant

Required implementation sequence:

1. Registration design
2. Vendor registration
3. Organization registration
4. Tenant creation/reference
5. Password handling
6. Login
7. LoginAttempt
8. JWT generation
9. Current-user context
10. Tenant context
11. EF Core tenant isolation
12. Integration/security tests

### Auction

Planned:

- Auction CRUD
- Auction lifecycle
- AuctionRequirement
- AuctionStatement

### Bidding

Planned:

- VendorIntent
- Bid placement/revision/withdrawal
- BidDetail
- Bid validation
- Eligibility rules

### Evaluation

Planned:

- Bid evaluation
- Result persistence
- Winner/award handling
- Result APIs

### Rating

Planned:

- Rating creation
- RatingValue
- Target validation
- Rating retrieval/aggregation

---

## 11. Risks & Things to Watch

### 🔴 Tenant isolation

This is the most important security concern in the current architecture.

**Required verification:**
- Tenant comes from authenticated context.
- Tenant ID is not trusted from request payloads.
- Query filters or repository constraints consistently enforce tenant scope.
- Create/update operations cannot cross tenant boundaries.
- Tests explicitly attempt cross-tenant access.

### 🟡 Registration complexity

Vendor and Organization registration have different business data but converge on Tenant-based access. Keep the distinction explicit without allowing duplicated authentication/security logic.

### 🟡 Migration discipline

Once migrations start, migration history should become part of the normal development workflow. Avoid manually changing the database without corresponding migration changes unless there is a documented reason.

### 🟡 API consistency

Before many APIs are implemented, settle conventions for:

- Response shape
- Errors
- Validation
- Pagination
- Filtering
- HTTP status codes
- Swagger documentation

This prevents repetitive cleanup later.

---

## 12. Definition of Done

A feature should not be marked complete merely because the endpoint exists.

### API feature

- [ ] Entity/domain logic correct
- [ ] DTO/request model defined
- [ ] Validation implemented
- [ ] Service/use-case implemented
- [ ] Repository/query implemented where appropriate
- [ ] Controller endpoint implemented
- [ ] Authorization/tenant scope verified
- [ ] Error handling verified
- [ ] Swagger documented
- [ ] Tests added
- [ ] Database behavior/migration verified

### Security-sensitive feature

Additionally:

- [ ] Authentication behavior verified
- [ ] Authorization verified
- [ ] Tenant isolation verified
- [ ] Negative/cross-tenant tests added
- [ ] Sensitive information not exposed

---

## 13. Current Blockers

**No confirmed hard blocker currently recorded.**

Potential blockers to monitor:

- Migration/database configuration issues.
- Ambiguity in registration ownership relationships.
- JWT/tenant-context integration details.
- EF Core global query-filter behavior with the chosen entity model.

---

## 14. Next Milestone

### Milestone: Secure Tenant-Aware Backend Foundation

**Definition of complete:**

\`\`\`text
Database
   ✓
GlobalData
   ✓
Registration
   ✓
Tenant Creation
   ✓
Login
   ✓
JWT
   ✓
Tenant Context
   ✓
EF Core Isolation
   ✓
        ↓
Masters
\`\`\`

Only after this foundation is stable should the project move aggressively into auction/bidding APIs.

---

## 15. Working Relationship With Notion

**Notion = operational task board**

Use Notion for:

- Kanban
- Individual tasks
- Status
- Priority
- Phase
- Due dates
- Day-to-day task management

**PROJECT_STATUS.md = engineering dashboard**

Use this file for:

- Current project state
- Critical path
- Architecture snapshot
- Security model
- Milestones
- Risks
- Decisions
- Development history
- Next-session context

Do **not** duplicate every Notion task here.

---

## 16. Development Log

### 2026-09-19 — Dashboard Baseline

**Current state**
- Core entity definitions are considered complete.
- Tenant architecture has been explicitly decided.
- Clean Architecture foundation exists.
- PostgreSQL/EF Core foundation is in place.
- Migration setup is the immediate database task.
- GlobalData APIs are the next repetitive API slice.
- Registration + tenant creation + authentication/JWT are the critical security work.
- Notion Delivery Board contains the detailed implementation cards.

**Key decision**
- Tenant is the security boundary.
- Tenant identity should flow from authentication into the backend's current tenant context.
- Tenant-scoped data should not depend on client-supplied tenant IDs.

**Next working sequence**
1. Finish migrations/database foundation.
2. Implement GlobalData GET APIs.
3. Implement registration and tenant creation.
4. Implement login.
5. Implement JWT.
6. Implement tenant context.
7. Implement EF Core tenant isolation.
8. Verify with integration/security tests.
9. Begin Master APIs.

---

## 17. Update Protocol

When asked to **"update BidNexus status"**, update this file using the latest project state and preserve historical information.

The update should:

1. Reassess the executive snapshot.
2. Update progress visualizations.
3. Move the current sprint forward.
4. Update the critical path.
5. Record important technical decisions.
6. Record blockers/risks.
7. Add a dated development-log entry.
8. Remove obsolete assumptions.
9. Keep the dashboard concise enough to scan quickly.
10. Never claim a feature is complete unless the implementation and relevant verification are actually complete.
