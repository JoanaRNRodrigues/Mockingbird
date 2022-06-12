namespace MockingBird.Models
{
    public class Chirp
    {
        //A Chirp is a post, with Id, body of text, time of creation and the user that creates the chirp
        public int Id { get; set; }
        public string Text { get; set; }
        public User User { get; set; }

        public DateTime Date;
        public string? ImagePath { get; set; }
        public Chirp()
        {
            LikeList = new List<User>();
        }
        public List<User> LikeList { get; set; }

        public int Likes { get; set; }

    }
}
