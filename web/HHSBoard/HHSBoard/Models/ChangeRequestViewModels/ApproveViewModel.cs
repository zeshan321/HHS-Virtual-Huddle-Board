using HHSBoard.Data;
using HHSBoard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models.ChangeRequestViewModels
{
    public class ApproveViewModel
    {
        public int ID { get; set; }

        public string Username { get; set; }

        public ChangeRequestType ChangeRequestType { get; set; }

        public string TableName { get; set; }

        public int AssociatedID { get; set; }

        public string AssociatedName { get; set; }

        public string Values { get; set; }

        public string BoardName { get; set; }
        public string UnitName { get; set; }

        public string PreviousValues { get; set; }
    }
}
