using System;

namespace Project.Models
{
    public class Marathon
    {
        public int Id { get; set; }
        public int? Number { get; set; }
        public DateTime? StartRegister { get; set; }
        public DateTime? EndRegister { get; set; }
        public DateTime? StartFlow { get; set; }
        public DateTime? EndFlow { get; set; }
        public DateTime? EndTask { get; set; }
    }
}
