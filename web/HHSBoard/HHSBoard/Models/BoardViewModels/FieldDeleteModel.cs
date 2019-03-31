using HHSBoard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HHSBoard.Controllers.BoardController;

namespace HHSBoard.Models.CelebrationViewModels
{
    public class FieldDeleteModel
    {
        public List<int> Delete { get; set; }

        public TableType TableType { get; set; }
    }
}
