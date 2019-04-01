using HHSBoard.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models.AuditViewModels
{
    public class AuditViewModel
    {
        public int Total { get; set; }

        [JsonProperty("rows")]
        public List<Audit> Audits { get; set; }
    }
}
