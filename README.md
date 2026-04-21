# insurezen-backend-assessment
Backend assessment project

Requirement Analysis:

1. Entities
   - claim
   - User (Maker, Checker)
   - Review
3. Actors
   - Maker: Reviews claim and gives recommendation
   - Checker: Gives final decision
4. Functional Requirements
   - create and store claims
   - maker reviews claims
   - checker final decision
   - store all history
5. Non-Functional Requirements
   - Handle multiple users
   - maintain data consistency
   - system should be scalable
6. Edge cases
   - Two users editing same claim
   - checker without maker review
   - invalid data
7. Assumptions
   - claim data already provided
   - no real insurance api needed
   - only backend system required
  
Task 2 - API Design

Base URL
/api/claims

1. Claim management APIs
   - create claim
   POST/api/claims
   Description: Create a new insurance claim in the system.

Request:

{
   "PatientName": "John",
   "amount": 5000,
   "insuranceCompany": "ABC Insurance",
   "hospital": "XYZ Hospital"
}

Response: 

{
   "claimId": 1,
   "status": "pending"
}

- Get all claims
  GET/api/claims

  Description: Retrieve list of all claims.
  Response:
  [
     {
        "claimId": 1,
        "status": "pending"
     }
  ]
- Get claim by ID
  GET/api/claims/{id}
  Description: Retrieve specific claim details by Id.

2. Maker flow APIs
   
   - Assign claim to maker
     POST/api/claims/{id}/maker/assign
     
     Description:
     Assign the claim to a maker for review. Prevents multiple makers from reviewing the same claim.

   - Maker review
     POST/api/claims/{id}/maker-review

     Description: maker reviews the claim and provides recommendation.

     Request:
     {
        "recommendation": "Approve",
        "comments": "documents are valid"
     }
     
   3. Checker flow APIs

      Checker review
      POST/api/claims/{id}/checker-review

      Description: checker reviews maker decision and provides the final result.

      Request:
         {
            "finalDecision": "Approved",
            "comments": "Verified and approved"
         }
   4. Claim History Api

      GET/api/claims/history?page=1&status=Approved

      Description: Returns a paginated list of claims with optional filters:
      - status
      - Insurance Company
      - Date Range

   5. Forward to insurance (simulation)

      POST/api/claims/{id}/forward

      Description: Simulates sending final claim decision to insurance company (no real external API integration).

      - claim status flow:
        Pending → In Review (Maker) → Reviewed → Checked (Checker) → Forwarded

Notes
- Frontend system is assumed to exist already and will consume these APIs.
- No OCR or document extraction is implemented; claim data is assumed to be pre-processed.
      
  
     


   



   

