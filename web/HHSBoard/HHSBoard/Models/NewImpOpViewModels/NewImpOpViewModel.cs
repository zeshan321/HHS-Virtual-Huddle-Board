using HHSBoard.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HHSBoard.Models.NewImpOpViewModels
{
    public class NewImpOpViewModel
    {
        public int Total { get; set; }

        [JsonProperty("rows")]
        public List<NewImpOp> NewImpOps { get; set; }
    }
}
