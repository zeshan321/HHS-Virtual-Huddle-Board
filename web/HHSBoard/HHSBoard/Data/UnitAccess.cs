using HHSBoard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Data
{
    public class UnitAccess
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public int UnitID { get; set; }

        [ForeignKey("UnitID")]
        public virtual Unit Unit { get; set; }
    }
}
