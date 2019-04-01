using HHSBoard.Data;
using System.Collections.Generic;

namespace HHSBoard.Models.HomeViewModels
{
    public class HomeIndexViewModel
    {
        public List<Unit> Units { get; set; }

        public List<Board> Boards { get; set; }
    }
}