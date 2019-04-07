using HHSBoard.Helpers;
using System;
using System.Collections.Generic;

namespace HHSBoard.Models.CelebrationViewModels
{
    public class FieldDeleteModel
    {
        public List<string> Delete { get; set; }
        public TableType TableType { get; set; }
        public int BoardID { get; set; }
    }
}