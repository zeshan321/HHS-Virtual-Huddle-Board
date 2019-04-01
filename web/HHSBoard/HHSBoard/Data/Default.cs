using System.ComponentModel.DataAnnotations;

namespace HHSBoard.Data
{
    public class Default
    {
        [Key]
        public int ID { get; set; }

        public string Field { get; set; }

        public string Value { get; set; }
    }
}