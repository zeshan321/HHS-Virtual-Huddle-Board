using HHSBoard.Helpers;
using System.Collections.Generic;

namespace HHSBoard.Models.CelebrationViewModels
{
    public class FieldDeleteModel
    {
        public List<int> Delete { get; set; }

        public TableType TableType { get; set; }
    }
}