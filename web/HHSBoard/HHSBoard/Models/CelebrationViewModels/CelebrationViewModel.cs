using HHSBoard.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HHSBoard.Models.CelebrationViewModels
{
    public class CelebrationViewModel
    {
        public int Total { get; set; }

        [JsonProperty("rows")]
        public List<Celebration> Celebrations { get; set; }
    }
}