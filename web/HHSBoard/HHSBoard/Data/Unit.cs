using System.ComponentModel.DataAnnotations;

namespace HHSBoard.Data
{
    public class Unit
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
    }
}