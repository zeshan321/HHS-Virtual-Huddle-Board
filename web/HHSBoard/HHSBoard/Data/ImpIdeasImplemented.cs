using HHSBoard.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHSBoard.Data
{
    public class ImpIdeasImplemented
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Problem { get; set; }
        public string Owner { get; set; }
        public string Pillar { get; set; }
        public bool IsPtFamilyInvovlmentOpportunity { get; set; }
        public string EightWs { get; set; }
        public PickChart? PickChart { get; set; }
        public string JustDoIt { get; set; }
        public string Solution { get; set; }
        public DateTime DateComplete { get; set; }
        public bool WorkCreated { get; set; }
        public bool ProcessObservationCreated { get; set; }
        public string DateEnterIntoDatabase { get; set; }

        [Required]
        public int BoardID { get; set; }

        [ForeignKey("BoardID")]
        public virtual Board Board { get; set; }
    }
}
