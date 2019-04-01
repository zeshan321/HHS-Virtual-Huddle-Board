using HHSBoard.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HHSBoard.Models.WipViewModels
{
    public class WIPViewModel
    {
        public int Total { get; set; }

        [JsonProperty("rows")]
        public List<WIP> WIPs { get; set; }
    }
}