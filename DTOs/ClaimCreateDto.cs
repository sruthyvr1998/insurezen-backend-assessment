namespace InsureZenAPI.DTOs
{
    public class ClaimCreateDto
    {
        public string PatientName { get; set; } = string.Empty;
        public string InsuranceCompany { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}