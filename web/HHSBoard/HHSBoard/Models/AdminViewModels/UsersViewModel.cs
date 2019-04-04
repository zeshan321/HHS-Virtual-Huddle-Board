using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models.AdminViewModels
{
    public class UsersViewModel
    {
        public int Total { get; set; }

        [JsonProperty("rows")]
        public List<UserViewModel> UserViewModels { get; set; }
    }
}
