using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Review
    {
        public int Id { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Patronymic { get; set; }
        [Required]
        public int? Phone { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Comment { get; set; }
    }
}
