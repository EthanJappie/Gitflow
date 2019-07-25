using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickIndexing.Models
{
    public class DocumentTypeModel
    {
        public int DocumentID { get; set; }
        public string Description { get; set; }
        public int FolderID { get; set; }
        public string DocumentGroup { get; set; }
        public int ColourBits { get; set; }
        public string ConditionCode { get; set; }
        public int ConditionID { get; set; }
        public int OrderID { get; set; }
        public string BucketColour { get; set; }
        public bool Show_DatePicker { get; set; }
        public bool Attest { get; set; }
    }

    public class DocumentTypeViewModel
    {
        public int id { get; set; }
        public string text { get; set; } 
        public int GroupID { get; set; }
        public int DocumentID { get; set; }
        public string Description { get; set; }
        public int FolderID { get; set; }
        public string DocumentGroup { get; set; }
        public int ColourBits { get; set; }
        public string ConditionCode { get; set; }
        public int ConditionID { get; set; }
        public int OrderID { get; set; }
        public string BucketColour { get; set; }
        public bool Show_DatePicker { get; set; }
        public bool Attest { get; set; }
        public List<DocumentTypeViewModel> Children { get; set; }
    }
}
