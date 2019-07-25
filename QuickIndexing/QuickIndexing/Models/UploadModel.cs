using Microsoft.AspNetCore.Hosting;
using QuickIndexing.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuickIndexing.Models
{
    public static class UploadModel
    {
        public static bool Cleared { get; set; }
        public static int FileCount { get; set; }
        public static List<FileDetailModel> FileDetail { get; set; }

        public static string SelectedImage { get; set; }

        public static bool FilesUploaded { get; set; }
        public static bool FilesCleared { get; set; }
    }
    public class FileDetailModel
    {
        public string Filename { get; set; }
        public string FilePath { get; set; }
    }
}
