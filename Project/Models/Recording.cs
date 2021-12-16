namespace Project.Models
{
    public class Recording
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int MarathonId { get; set; }
        public Marathon Marathon { get; set; }
        public int RunnerId { get; set; }
        public Runner Runner { get; set; }

    }
}
