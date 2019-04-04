using HHSBoard.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HHSBoard.Models.ImpIdeasImplementedViewModels
{
    public class ImpIdeasImplementedViewModel
    {
        public int Total { get; set; }

        [JsonProperty("rows")]
        public List<ImpIdeasImplemented> ImpIdeasImplementeds { get; set; }
    }
}
