using HHSBoard.Helpers;

namespace HHSBoard.Models.CelebrationViewModels
{
    public class BoardTableModel
    {
        public TableType TableType { get; set; }
        public string Search { get; set; }
        public string Sort { get; set; }
        public string Order { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int BoardID { get; set; }
    }
}