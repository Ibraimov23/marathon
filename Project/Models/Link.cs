using System;

namespace Project.Models
{
    public class Link
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool Active { get; set; }
        public DateTime? ShelfLife { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
