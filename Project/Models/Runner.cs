namespace Project.Models
{
    public class Runner
    {
        public int Id { get; set; }
        public int? Phone { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Comment { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
