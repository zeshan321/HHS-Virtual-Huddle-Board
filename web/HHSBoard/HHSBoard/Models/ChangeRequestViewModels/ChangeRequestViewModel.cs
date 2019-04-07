using HHSBoard.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models.ChangeRequestViewModels
{
    public class ChangeRequestViewModel
    {
        public int Total { get; set; }

        [JsonProperty("rows")]
        public List<ApproveViewModel> ChangeRequests { get; set; }
    }
}
