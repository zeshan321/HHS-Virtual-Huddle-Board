using HHSBoard.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models.BoardViewModels
{
    public class CelebrationViewModel
    {
        public int Total { get; set; }

        [JsonProperty("rows")]
        public List<Celebration> Celebrations { get; set; }
    }
}
