using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models.AdminViewModels
{
    public class AdminUnitViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public bool HasAccess { get; set; }
    }
}
