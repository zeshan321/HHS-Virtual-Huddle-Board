using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HHSBoard.Controllers.BoardController;

namespace HHSBoard.Models.BoardViewModels
{
    public class BoardTableModel
    {
        public TableType TableType { get; set; }
        public string Search { get; set; }
        public string Order { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int BoardID { get; set; }
    }
}
