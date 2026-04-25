#  InsureZen Backend API

##  Problem Overview

InsureZen is a backend system designed to process medical insurance claims.  
Claims are already received in structured format from an external system (OCR is not part of this scope).

The system follows a **two-step human review workflow**:
- Maker Review (initial recommendation)
- Checker Review (final decision)


##  Actors

- **Maker** → Reviews claims and provides recommendation (Approve/Reject)
- **Checker** → Reviews Maker decision and gives final decision
- **System** → Manages claims, workflow, and audit logs


##  Core Entities

### Claim
- Id
- PatientName
- InsuranceCompany
- Amount
- Status (New, Recommended, Approved, Rejected)
- MakerId
- CheckerId
- MakerDecision
- CheckerDecision
- MakerFeedback
- CheckerFeedback
- CreatedAt


##  Workflow

  New → Maker Review → Recommended → Checker Review → Approved / Rejected



##  Functional Requirements

- Create insurance claim
- Maker review with decision + feedback
- Checker final review
- Maintain audit logs
- Paginated + filtered claim history
- Retrieve audit logs


##  Non-Functional Requirements

- Handle multiple reviewers (basic concurrency handled using lock)
- Ensure correct state transitions between workflow stages
- Maintain audit traceability
- Efficient filtering and pagination for history API


##  Assumptions

- Authentication is not implemented (MakerId/CheckerId passed in request body)
- Data is stored in-memory (no database used)
- External OCR/document system already provides structured claim data
- Application is a single monolithic ASP.NET Core Web API (no microservices)


#  API Design (Task 2)

## Base URL  :  /api/claims



1️. Create Claim
**POST** `/api/claims`

Request:
```json
{
  "patientName": "John Doe",
  "insuranceCompany": "ABC Insurance",
  "amount": 5000
}


2️. Get All Claims

GET /api/claims

Returns all claims stored in the system.

3️. Maker Review

POST /api/claims/{id}/maker-review

Request
{
  "decision": "Approved",
  "makerId": "MKR001",
  "feedback": "Valid claim"
}

Rules
Allowed only when status = New
Updates status → Recommended
Stores Maker decision and feedback

4️. Checker Review

POST /api/claims/{id}/checker-review

Request
{
  "decision": "Approved",
  "checkerId": "CHK001",
  "feedback": "Verified successfully"
}

Rules

Allowed only when status = Recommended
Final status → Approved / Rejected

5️. Claim History (Pagination + Filtering)

GET

/api/claims/history?page=1&pageSize=5&status=Approved&company=ABC Insurance
Supports
Pagination
Filter by status
Filter by insurance company

6️. Audit Logs

GET /api/claims/audit-logs

Returns all system activity logs (creation, Maker review, Checker decision).

- How to Run

   Option 1 (Visual Studio)
   Open solution
   Click ▶ Run button
   Swagger opens automatically in browser

   Option 2 (CLI)
   dotnet run

   Swagger URL: https://localhost:<port>/swagger

- Status Codes Used

  200 OK → Success
  400 BadRequest → Invalid input
  404 NotFound → Claim not found
  409 Conflict → Invalid workflow state

- Design Notes
  DTOs used for request separation
  Enums used for status consistency
  In-memory database used for simplicity
  Audit logging implemented for traceability
  Basic locking used in Maker review for concurrency safety

- Future Improvements

  Add SQL Server / PostgreSQL database
  Implement JWT authentication (Maker/Checker roles)
  Add service layer for better separation of concerns
  Add unit and integration tests
  Replace in-memory storage with persistent DB

- Summary

  This project simulates a real-world insurance claim processing system with a Maker-Checker workflow.
  It focuses on clean API design, state management, audit tracking, and basic concurrency handling using ASP.NET Core   Web API.
