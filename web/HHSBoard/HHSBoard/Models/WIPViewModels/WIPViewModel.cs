using HHSBoard.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models.WipViewModels
{
    public class WIPViewModel
    {
        public int Total { get; set; }

        [JsonProperty("rows")]
        public List<WIP> WIPs { get; set; }
    }
}
