using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickIndexing.Models
{
    public class SubmitModel
    {
        public string Username { get; set; }
        public string ReferenceNumber { get; set; }
        public int ReferenceType { get; set; }
        public int DocumentID { get; set; }
        public int FolderID { get; set; }
        public string DocumentDescription { get; set; }
        public string Extension { get; set; }
        public byte[] File { get; set; }
    }
}
