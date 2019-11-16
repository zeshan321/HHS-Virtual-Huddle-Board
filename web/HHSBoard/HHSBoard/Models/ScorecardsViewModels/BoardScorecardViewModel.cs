using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models.ScorecardsViewModels
{
    public class BoardScorecardViewModel
    {
        public string BoardName { get; set; }
        public int BoardId { get; set; }
        public List<string> FileNames { get; set; }
    }
}
