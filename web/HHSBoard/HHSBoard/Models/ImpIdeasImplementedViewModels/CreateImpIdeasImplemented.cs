using HHSBoard.Helpers;
using System;

namespace HHSBoard.Models.ImpIdeasImplementedViewModels
{
    public class CreateImpIdeasImplemented
    {
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public string Problem { get; set; }
        public string Owner { get; set; }
        public string Pillar { get; set; }
        public bool IsPtFamilyInvovlmentOpportunity { get; set; }
        public string EightWs { get; set; }
        public PickChart? PickChart { get; set; }
        public string JustDoIt { get; set; }
        public string Solution { get; set; }
        public DateTime? DateComplete { get; set; }
        public bool WorkCreated { get; set; }
        public bool ProcessObservationCreated { get; set; }
        public string DateEnterIntoDatabase { get; set; }
        public int BoardID { get; set; }
    }
}
