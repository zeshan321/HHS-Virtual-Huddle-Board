using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHSBoard.Models
{
    public class FileUploadModel
    {
        public IFormFile FormFile { get; set; }
        public string Type { get; set; }
        public int BoardId { get; set; }
        public int TableType { get; set; }
    }
}
