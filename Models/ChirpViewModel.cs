namespace MockingBird.Models
{
    public class ChirpViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        
        public DateTime Date;
        public string? ImagePath { get; set; }
    }
}
