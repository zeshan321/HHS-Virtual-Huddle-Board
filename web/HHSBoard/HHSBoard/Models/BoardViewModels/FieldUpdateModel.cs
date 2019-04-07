using HHSBoard.Helpers;

namespace HHSBoard.Models.CelebrationViewModels
{
    public class FieldUpdateModel
    {
        public int Pk { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public TableType TableType { get; set; }
        public int BoardID { get; set; }
    }
}