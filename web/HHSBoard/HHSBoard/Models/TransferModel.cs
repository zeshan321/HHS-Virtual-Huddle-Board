using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models
{
    public class TransferModel
    {
        public int BoardID { get; set; }
        public int RowId { get; set; }
        public bool IsNewImp { get; set; }
    }
}
