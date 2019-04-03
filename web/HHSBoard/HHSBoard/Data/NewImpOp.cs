using HHSBoard.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHSBoard.Data
{
    public class NewImpOp
    {
        [Key]
        public int ID { get; set; }
        public string Legend { get; set; }
        public string PersonIdentifyingOpportunity { get; set; }
        public DateTime DateIdentified { get; set; }
        public string Problem { get; set; }
        public string StaffWorkingOnOpportunity { get; set; }
        public string StrategicGoals { get; set; }
        public bool IsPtFamilyInvovlmentOpportunity { get; set; }
        public string EightWs { get; set; }
        public PickChart? PickChart { get; set; }
        public string JustDoIt { get; set; }

        [Required]
        public int BoardID { get; set; }
        
        [ForeignKey("BoardID")]
        public virtual Board Board { get; set; }
    }
}
