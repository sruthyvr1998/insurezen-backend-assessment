namespace InsureZenAPI.DTO
{
    public class MakerReviewDto
    {
        public string Decision { get; set; } = string.Empty;
        public string MakerId { get; set; } = string.Empty;
        public string? Feedback { get; set; }
    }
}