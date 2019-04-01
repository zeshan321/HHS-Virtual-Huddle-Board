using System;

namespace HHSBoard.Models.CelebrationViewModels
{
    public class CreateCelebrationModel
    {
        public int BoardID { get; set; }

        public string Who { get; set; }

        public string What { get; set; }

        public string Why { get; set; }

        public DateTime? Date { get; set; }
    }
}