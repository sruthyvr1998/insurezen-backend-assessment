TASK 1 — Requirements Analysis
1. Overview

InsureZen processes medical insurance claims submitted by multiple insurance companies. Each claim goes through a two-stage review workflow involving a Maker and a Checker before being finalized and forwarded.

2. Entities
Claim

Represents an insurance claim in the system.

Fields:

   - Id (unique identifier)
   - PatientName
   - InsuranceCompany
   - Hospital
   - Amount
   - Status (Pending, MakerReviewed, CheckerReviewed, Forwarded)
   - CreatedAt
   - Review Data (embedded within Claim)

Instead of separate entities, review information is stored within the Claim for simplicity.

   - MakerDecision (Approve / Reject)
   - MakerComment
   - CheckerDecision
   - CheckerComment
3. Actors
   Maker
   - Reviews claims first
   - Adds recommendation (Approve / Reject)
   - Provides comments
   Checker
   - Reviews Maker’s decision
   - Provides final decision
   - Adds comments
4. Functional Requirements
   - The system must allow creation of new claims
   - The system must allow Makers to review claims and provide recommendations
   - The system must allow Checkers to review Maker decisions and provide final decisions
   - The system must store all claim data including review history
   - The system must provide APIs to retrieve claims
   - The system must support pagination and filtering of claims
5. Non-Functional Requirements
   - Concurrency: Multiple users may access the system simultaneously; a claim should not be reviewed by multiple          Makers at the same time
   - Data Integrity: Claim state transitions must follow defined workflow rules
   - Auditability: All decisions and comments must be stored for traceability
   - Scalability: System should handle large volumes of claims
6. Claim Status Workflow
   Pending → MakerReviewed → CheckerReviewed → Forwarded
7. Edge Cases
   - Maker tries to review a claim that is already reviewed
   - Checker tries to review before Maker completes review
   - Multiple Makers attempt to review the same claim simultaneously
   - Invalid input data (missing fields, incorrect values)
   - Attempt to modify claim after final decision
8. Assumptions
   - Claim data is already extracted and provided as structured input
   - Each claim is reviewed by only one Maker and one Checker
   - Once Checker decision is made, the claim cannot be modified
   - No actual integration with insurance company is required (simulation only)
   - Claim assignment is handled implicitly via claim status

TASK 2 — API Design
1. Base URL
/api/claims
2. Endpoints
   1. Create Claim
      POST /api/claims

      Description: Create a new insurance claim

      Request Body:

      {
        "patientName": "John",
        "insuranceCompany": "ABC Insurance",
        "hospital": "XYZ Hospital",
        "amount": 5000
      }

      Response:

      {
        "id": 1,
        "status": "Pending"
      }
2. Get All Claims (with pagination & filtering)
   GET /api/claims?page=1&pageSize=10&status=Pending

   Description: Retrieve paginated list of claims

   Supports filtering by:

      - status
      - insurance company
      - date range (optional)
3. Get Claim by ID
   GET /api/claims/{id}

   Description: Retrieve details of a specific claim

4. Maker Review
   POST /api/claims/{id}/maker-review

   Description: Maker reviews claim and provides recommendation

   Request Body:

   {
     "decision": "Approve",
     "comment": "Documents are valid"
   }

   Behavior:

   - Allowed only when status = Pending
   - Updates claim status to MakerReviewed
5. Checker Review
   POST /api/claims/{id}/checker-review

   Description: Checker reviews Maker decision and gives final decision

   Request Body:

   {
     "decision": "Approved",
     "comment": "Verified and approved"
   }   

   Behavior:

   - Allowed only when status = MakerReviewed
   - Updates claim status to CheckerReviewed
6. Forward Claim (Simulation)
   POST /api/claims/{id}/forward

   Description: Simulates forwarding claim to insurance company

   Behavior:

   - Allowed only when status = CheckerReviewed
   - Updates status to Forwarded
   - Logs or returns confirmation message
3. Status Codes
   - 200 OK → Successful operation
   - 400 Bad Request → Invalid input or invalid workflow state
   - 404 Not Found → Claim not found
4. Workflow Handling
   - Claims start in Pending
   - Maker updates claim → MakerReviewed
   - Checker updates claim → CheckerReviewed
   - Final step → Forwarded
5. Concurrency Handling
   - Claim state is validated before any update
   - A claim cannot be reviewed by multiple users simultaneously
   - Status checks prevent invalid transitions
