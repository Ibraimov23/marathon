using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Project.Models
{
    public class User : IdentityUser
    {
        public int? Phone { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
    }
}
