using HHSBoard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models.BoardViewModels
{
    public class GetChangeRequestModel
    {
        public int BoardID { get; set; }

        public TableType TableType { get; set; }
    }
}
