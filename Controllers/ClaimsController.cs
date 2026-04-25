using InsureZenAPI.Data;
using InsureZenAPI.DTO;
using InsureZenAPI.DTOs;
using InsureZenAPI.Enums;
using InsureZenAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace InsureZenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        //Create claim
        [HttpPost]
        public IActionResult CreateClaim([FromBody] ClaimCreateDto request)
        {
            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than 0");

            var claim = new Claim
            {
                Id = InMemoryDb.Claims.Count + 1,
                PatientName = request.PatientName,
                InsuranceCompany = request.InsuranceCompany,
                Amount = request.Amount,
                Status = ClaimStatus.New,
                CreatedAt = DateTime.Now
            };

            InMemoryDb.Claims.Add(claim);

            AuditLogger.Add($"Claim {claim.Id} created for {claim.PatientName}");

            return Ok(claim);
        }

        // get all claims
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(InMemoryDb.Claims);
        }

        // maker review
        [HttpPost("{id}/maker-review")]
        public IActionResult MakerReview(int id, [FromBody] MakerReviewDto request)
        {
            lock (InMemoryDb.Claims)
            {
                var claim = InMemoryDb.Claims.FirstOrDefault(x => x.Id == id);

                if (claim == null)
                    return NotFound("Claim not found");

                if (claim.Status != ClaimStatus.New)
                {
                    if (claim.Status == ClaimStatus.Recommended ||
                        claim.Status == ClaimStatus.Approved ||
                        claim.Status == ClaimStatus.Rejected)
                    {
                        return Conflict("Claim has already been reviewed by Maker");
                    }

                    return Conflict("Claim is not available for Maker review");
                }

                if (!IsValidDecision(request.Decision))
                    return BadRequest("Decision must be Approved or Rejected");

                claim.MakerId = request.MakerId;
                claim.MakerDecision = ParseDecision(request.Decision);
                claim.Status = ClaimStatus.Recommended;
                claim.MakerFeedback = request.Feedback;

                AuditLogger.Add($"Maker {request.MakerId} reviewed Claim {id} as {request.Decision}");

                return Ok(claim);
            }
        }

        // checker review
        [HttpPost("{id}/checker-review")]
        public IActionResult CheckerReview(int id, [FromBody] CheckerReviewDto request)
        {
            var claim = InMemoryDb.Claims.FirstOrDefault(x => x.Id == id);

            if (claim == null)
                return NotFound("Claim not found");

            if (claim.Status == ClaimStatus.New)
                return Conflict("Claim is still pending Maker review");

            if (claim.Status == ClaimStatus.Approved || claim.Status == ClaimStatus.Rejected)
                return Conflict("Claim already finalized");

            if (claim.Status != ClaimStatus.Recommended)
                return Conflict("Claim not ready for Checker review");

            if (!IsValidDecision(request.Decision))
                return BadRequest("Decision must be Approved or Rejected");

            claim.CheckerId = request.CheckerId;
            claim.CheckerDecision = ParseDecision(request.Decision);
            claim.CheckerFeedback = request.Feedback;

            claim.Status = claim.CheckerDecision == Decision.Approved
                ? ClaimStatus.Approved
                : ClaimStatus.Rejected;

            AuditLogger.Add($"Checker {request.CheckerId} finalized Claim {id} as {request.Decision}");

            return Ok(claim);
        }

        // PAGINATED + FILTERED HISTORY 
        [HttpGet("history")]
        public IActionResult History(
            int page = 1,
            int pageSize = 5,
            string? status = null,
            string? company = null)
        {
            var query = InMemoryDb.Claims.AsQueryable();

            // FILTER BY STATUS
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<ClaimStatus>(status, true, out var parsedStatus))
                {
                    query = query.Where(x => x.Status == parsedStatus);
                }
            }

            // FILTER BY COMPANY
            if (!string.IsNullOrWhiteSpace(company))
            {
                query = query.Where(x => x.InsuranceCompany.ToLower() == company.ToLower());
            }

            // PAGINATION
            var result = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(result);
        }

        // audit logs
        [HttpGet("audit-logs")]
        public IActionResult AuditLogs()
        {
            return Ok(AuditLogger.Logs);
        }

        // helper methods
        private bool IsValidDecision(string decision)
        {
            if (string.IsNullOrWhiteSpace(decision))
                return false;

            var normalized = decision.Trim().ToLower();

            return normalized == "approved" || normalized == "rejected";
        }

        private Decision ParseDecision(string decision)
        {
            return decision.Trim().ToLower() == "approved"
                ? Decision.Approved
                : Decision.Rejected;
        }
    }
}