namespace InsureZenAPI.DTO
{
    public class CheckerReviewDto
    {
        public string Decision { get; set; } = string.Empty;
        public string CheckerId { get; set; } = string.Empty;
        public string? Feedback { get; set; }
    }
}