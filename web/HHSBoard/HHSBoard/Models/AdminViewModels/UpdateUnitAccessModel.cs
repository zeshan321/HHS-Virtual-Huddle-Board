using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models.AdminViewModels
{
    public class UpdateUnitAccessModel
    {
        public int UnitID { get; set; }

        public string UserID { get; set; }

        public bool Adding { get; set; }
    }
}
