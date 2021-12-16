using System.ComponentModel.DataAnnotations;

namespace Project.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Ваш номер телефона с кодом страны," +
        " к которому прикреплён Телеграм или ваш ник в телеграмм: без номера телефона вы не участвуете," +
        " мы вас не найдём и не сможем добавить в чат *")]
        public int? Phone { get; set; }
        [Required]
        [Display(Name = "Ваша фамилия")]
        public string Surname { get; set; }
        [Required]
        [Display(Name = "Ваше имя")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Ваше отчество")]
        public string Patronymic { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [Display(Name = "Что Вы хотите получить от марафона? *")]
        public string Comment { get; set; }
    }
}
