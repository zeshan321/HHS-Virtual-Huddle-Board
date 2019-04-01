using HHSBoard.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHSBoard.Data
{
    public class WIP : BaseEntity
    {
        [Key]
        public int ID { get; set; }

        public string Saftey { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Problem { get; set; }
        public string EightWs { get; set; }
        public string StrategicGoals { get; set; }
        public bool IsPtFamilyInvovlmentOpportunity { get; set; }
        public PickChart? PickChart { get; set; }
        public DateTime? DateAssigned { get; set; }
        public string StaffWorkingOnOpportunity { get; set; }
        public string Why { get; set; }
        public string JustDoIt { get; set; }
        public string Updates { get; set; }

        [Required]
        public int BoardID { get; set; }

        [ForeignKey("BoardID")]
        public virtual Board Board { get; set; }
    }
}