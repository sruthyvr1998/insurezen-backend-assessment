using InsureZenAPI.Enums;

namespace InsureZenAPI.Models
{
    public class Claim
    {
        public int Id { get; set; }

        public string PatientName { get; set; } = string.Empty;
        public string InsuranceCompany { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.New;

        public string? MakerId { get; set; }
        public string? CheckerId { get; set; }

        public Decision? MakerDecision { get; set; }
        public Decision? CheckerDecision { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? MakerFeedback { get; set; }
        public string? CheckerFeedback { get; set; }
    }
}