using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
