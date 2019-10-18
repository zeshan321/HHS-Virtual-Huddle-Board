using HHSBoard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models.AdminViewModels
{
    public class UserViewModel
    {
        public string ID { get; set; }

        public string Username { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsStaff { get; set; }

        public List<AdminUnitViewModel> AdminUnitViewModels { get; set; }
    }
}
