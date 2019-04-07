using HHSBoard.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Data
{
    public class ChangeRequest
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public ChangeRequestType ChangeRequestType { get; set; }

        [Required]
        public TableType TableName { get; set; }
        
        public int AssociatedID { get; set; }

        public string AssociatedName { get; set; }

        [Required]
        public string Values { get; set; }

        [Required]
        public int BoardID { get; set; }

        [ForeignKey("BoardID")]
        public virtual Board Board { get; set; }
    }
}
