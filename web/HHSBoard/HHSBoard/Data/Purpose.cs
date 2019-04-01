using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHSBoard.Data
{
    public class Purpose
    {
        [Key]
        public int ID { get; set; }

        public string Text { get; set; }

        [Required]
        public int BoardID { get; set; }

        [ForeignKey("BoardID")]
        public virtual Board Board { get; set; }
    }
}