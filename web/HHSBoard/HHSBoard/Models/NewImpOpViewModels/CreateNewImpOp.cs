using HHSBoard.Helpers;
using System;

namespace HHSBoard.Models.NewImpOpViewModels
{
    public class CreateNewImpOp : BaseCreateModel
    {
        public string Legend { get; set; }
        public string PersonIdentifyingOpportunity { get; set; }
        public DateTime? DateIdentified { get; set; }
        public string Problem { get; set; }
        public string StaffWorkingOnOpportunity { get; set; }
        public string StrategicGoals { get; set; }
        public bool IsPtFamilyInvovlmentOpportunity { get; set; }
        public PickChart? PickChart { get; set; }
        public string EightWs { get; set; }
        public string JustDoIt { get; set; }
        public int BoardID { get; set; }
    }
}
