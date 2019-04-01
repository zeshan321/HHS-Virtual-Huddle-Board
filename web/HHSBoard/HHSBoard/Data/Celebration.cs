using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHSBoard.Data
{
    public class Celebration
    {
        [Key]
        public int ID { get; set; }

        public string Who { get; set; }

        public string What { get; set; }

        public string Why { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public int BoardID { get; set; }

        [JsonIgnore]
        [ForeignKey("BoardID")]
        public virtual Board Board { get; set; }
    }
}