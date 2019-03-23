using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HHSBoard.Controllers.BoardController;

namespace HHSBoard.Models.BoardViewModels
{
    public class FieldUpdateModel
    {
        public int Pk { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public TableType TableType { get; set; }
     }
}
