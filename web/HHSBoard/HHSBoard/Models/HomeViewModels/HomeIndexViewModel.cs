using HHSBoard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models.HomeViewModels
{
    public class HomeIndexViewModel
    {
        public List<Unit> Units { get; set; }

        public List<Board> Boards { get; set; }
    }
}
