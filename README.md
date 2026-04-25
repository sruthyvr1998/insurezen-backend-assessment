InsureZen Backend API
Problem Understanding (Task 1 - Requirements Analysis)

InsureZen is a backend system for processing medical insurance claims. Claims come from an external system already in structured format (no OCR needed). The system supports a two-step human review workflow.

1. Actors
   - Maker → First-level reviewer who evaluates claim and gives recommendation
   - Checker → Final reviewer who validates Maker’s decision
   - System → Stores claims, handles workflow, maintains logs
2. Core Entities
   Claim
      - Id
      - PatientName
      - InsuranceCompany
      - Amount
      - Status (New, Recommended, Approved, Rejected)
      - MakerId / CheckerId
      - MakerDecision / CheckerDecision
      - Feedback
      - CreatedAt
3. Workflow
   - Claim is created → Status = New
   - Maker reviews claim → Status = Recommended
   - Checker reviews Maker decision → Final status: Approved or Rejected
4. Functional Requirements
   - Create claim
   - Maker review with decision + feedback
   - Checker final review
   - Store audit logs
   - Paginated + filtered claim history
   - Fetch all audit logs
5. Non-Functional Requirements
   - Handle multiple users reviewing claims (basic concurrency handled using lock)
   - Maintain data consistency during status transitions
   - Fast retrieval of claim history (pagination used)
   - Auditability of all actions
6. Assumptions
   - No authentication system is implemented (MakerId/CheckerId passed in request body)
   - Data is stored in-memory (no database used)
   - external OCR/document processing system already extracts claim data and sends structured JSON input to this API.
   - Single service application (no microservices required)

API Design (Task 2)
Base URL: /api/claims

1. Create Claim

POST /api/claims

Request:

{
  "patientName": "John Doe",
  "insuranceCompany": "ABC Insurance",
  "amount": 5000
}

Response: Created claim object

2. Get All Claims

GET /api/claims

Returns all claims in system

3. Maker Review

POST /api/claims/{id}/maker-review

Request:

{
  "decision": "Approved",
  "makerId": "MKR001",
  "feedback": "Looks valid"
}

Behavior:

Only allowed if status = New
Updates status → Recommended
Stores Maker decision + feedback

4. Checker Review

POST /api/claims/{id}/checker-review

Request:

{
  "decision": "Approved",
  "checkerId": "CHK001",
  "feedback": "Verified successfully"
}

Behavior:

Only allowed if status = Recommended
Final status becomes Approved or Rejected

5. Claim History (Pagination + Filtering)

GET /api/claims/history?page=1&pageSize=5&status=Approved&company=ABC

Supports:

Pagination
Filter by status
Filter by insurance company

6. Audit Logs

GET /api/claims/audit-logs

Returns system activity logs

- Status Codes Used 
  200 OK → Success
  400 BadRequest → Invalid input
  404 NotFound → Claim not found
  409 Conflict → Invalid workflow state
  Design Notes (short explanation)
  Used DTOs for request separation
  Used Enums for status consistency
  Used in-memory DB for simplicity
  Added Audit Logger for traceability
  Basic locking used in Maker review for concurrency safety

- How to Run
  Clone the repository
  Open the solution in Visual Studio
  Run the project using: dotnet run
  Or simply press the Run (▶) button in Visual Studio.
  Swagger will open automatically in the browser.
  
- Summary
  This project simulates a real-world insurance claim workflow with Maker-Checker approval system, focusing on:

  - Clean API design
  - State transitions
  - Basic concurrency handling
  - Audit tracking
  - Filtering & pagination support
