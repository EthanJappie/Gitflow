using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuickIndexing.Common
{
    public class FileConversion
    {
        public void ImageToPng(string path, string fullName)
        {
            using (var _stream = new FileStream(path, FileMode.Open))
            {
                var pdf = new PdfDocument();
                var Tiff = Image.FromStream(_stream);
                var pageCount = Tiff.GetFrameCount(FrameDimension.Page);

                for (int i = 0; i < pageCount; i++)
                {
                    var pdfPage = pdf.AddPage();
                    Tiff.SelectActiveFrame(FrameDimension.Page, i);
                    var _xGraphics = XGraphics.FromPdfPage(pdfPage);
                    var stream = new Func<Stream>(() => ToStream(Tiff, ImageFormat.Png));
                    var image = XImage.FromStream(stream);
                    _xGraphics.DrawImage(image, 0, 0);
                    pdf.Save(fullName);
                }

                _stream.Close();
            }

            DeleteFile(path);
        }

        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public Stream ToStream(Image image, ImageFormat format)
        {
            var stream = new MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
    }
}
