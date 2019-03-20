using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Data
{
    public class Board
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int UnitID { get; set; }

        [ForeignKey("UnitID")]
        public virtual Unit Unit { get; set; }
    }
}
