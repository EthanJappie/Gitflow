using IronPdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickIndexing.Common;
using QuickIndexing.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;

namespace QuickIndexing.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment _environment;
        public HomeController(IHostingEnvironment environment)
        {
            _environment = environment;
            Configure.Config(0);
        }
        public IActionResult Index()
        {
            if (!string.IsNullOrEmpty(SessionModel.ID))
            {
                if (!string.IsNullOrEmpty(ReferenceModel.ReferenceNumber))
                {
                    GetFileCount();
                }

                return View();

            }
            return View("~/Views/Login/Login.cshtml");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Post(List<IFormFile> files)
        {
            if (!string.IsNullOrEmpty(SessionModel.ID))
            {
                UploadModel.FilesUploaded = false;
                if (files.Count > 0)
                {
                    var size = files.Sum(x => x.Length);

                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", $@"{ReferenceModel.ReferenceNumber}");
                    Directory.CreateDirectory(filePath);

                    foreach (var formFile in files)
                    {
                        if (formFile.Length > 0)
                        {
                            var fullFileName = formFile.FileName.EndsWith(FileExtensions.Pdf) ? $@"{filePath}\{Path.GetFileNameWithoutExtension(formFile.FileName)}*.{FileExtensions.Png}"
                                : $@"{filePath}\{Path.GetFileNameWithoutExtension(formFile.FileName)}{Path.GetExtension(formFile.FileName)}";

                            if (!System.IO.File.Exists(fullFileName))
                            {
                                UploadModel.FilesUploaded = true;

                                if (formFile.FileName.EndsWith(FileExtensions.Pdf))
                                {
                                    using (var ms = new MemoryStream())
                                    {
                                        formFile.CopyTo(ms);
                                        var fileBytes = ms.ToArray();
                                        var pdf = new PdfDocument(fileBytes);
                                        pdf.ToPngImages(fullFileName);
                                    }
                                }
                                else if (formFile.FileName.EndsWith(FileExtensions.Tiff))
                                {
                                    var pdfFileName = fullFileName.Replace($".{FileExtensions.Tiff}", $".{FileExtensions.Pdf}");

                                    using (var stream = new FileStream(fullFileName, FileMode.Create))
                                    {
                                        formFile.CopyTo(stream);
                                    }
                                    new FileConversion().ImageToPng(fullFileName, pdfFileName);
                                    var pdf = new PdfDocument(pdfFileName);
                                    new FileConversion().DeleteFile(pdfFileName);
                                    pdf.ToPngImages(fullFileName.Replace($".{FileExtensions.Tiff}", $".{FileExtensions.Png}"));
                                }
                                else
                                {
                                    if (!System.IO.File.Exists(fullFileName))
                                    {
                                        using (var stream = new FileStream(fullFileName.Replace('*', new char()), FileMode.Create))
                                        {
                                            formFile.CopyTo(stream);
                                        }
                                    }
                                }
                            }

                        }
                    }
                    GetFileCount();
                    GetUploadFileInfo();

                    return View("Index", UploadModel.FileDetail);
                }
                else
                {
                    return View("Index");
                }
            }
            else
            {
                return View("~/Views/Login/Login.cshtml");
            }
        }

        public IActionResult SetRef(string referenceNumber, string referenceType, string referenceAction)
        {
            if (!string.IsNullOrEmpty(SessionModel.ID))
            {
                if (referenceAction == "Apply")
                {
                    var result = false;
                    using (var client = new WebClient())
                    {
                        var url = $"{API.GetAPIUrl()}data/ValidateReference/{referenceNumber}/{referenceType}";

                        result = Convert.ToBoolean(client.DownloadString(url));

                        if (result)
                        {
                            Configure.Config(referenceNumber);
                            Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "uploads", $"{ReferenceModel.ReferenceNumber}"));
                            ReferenceModel.ReferenceError = string.Empty;
                            Configure.ConfigDealType(referenceType);
                            Configure.Config(0);
                            GetFileCount();
                            GetUploadFileInfo();
                            return View("Index", ReferenceModel.ReferenceNumber);
                        }
                        else
                        {
                            ReferenceModel.ReferenceError = "Incorrect Reference Number";
                            ReferenceModel.ReferenceNumber = null;
                            return RedirectToAction("Index");
                        }
                    }                   
                }
                else
                {
                    ReferenceModel.ReferenceNumber = null;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View("~/Views/Login/Login.cshtml");
            }

        }

        public void GetFileCount()
        {
            var files = Directory.GetFiles(Path.Combine(_environment.WebRootPath, "uploads", $"{ReferenceModel.ReferenceNumber}")).Count();

            if (files > 0)
                UploadModel.FileCount = files;
        }

        public IActionResult ClearUploads()
        {
            if (!string.IsNullOrEmpty(SessionModel.ID))
            {
                UploadModel.FilesCleared = false;
                UploadModel.FilesUploaded = false;
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", $"{ReferenceModel.ReferenceNumber}");

                var files = Directory.GetFiles(filePath).ToList();

                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }

                UploadModel.FilesCleared = true;
                return View("Index");
            }
            else
            {
                return View("~/Views/Login/Login.cshtml");
            }
        }

        public void GetUploadFileInfo()
        {
            Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "uploads", $"{ReferenceModel.ReferenceNumber}"));
            var files = Directory.GetFiles(Path.Combine(_environment.WebRootPath, "uploads", $"{ReferenceModel.ReferenceNumber}")).ToList();
            var result = new List<FileDetailModel>();
            foreach (var item in files)
            {
                result.Add(new FileDetailModel()
                {
                    Filename = Path.GetFileName(item),
                    FilePath = item
                });
            }

            UploadModel.FileDetail = result;
        }

        public void ImageRotation(string filePath, string rotationDir)
        {
            var img = Image.FromFile(filePath);

            switch (rotationDir)
            {
                case "left":
                    img.RotateFlip(RotateFlipType.Rotate90FlipXY);
                    break;
                case "right":
                    img.RotateFlip(RotateFlipType.Rotate270FlipXY);
                    break;
                case "vertical":
                    img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    break;
                case "horizontal":
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                default:
                    break;
            }
            img.Save(filePath);
        }

        public IActionResult ViewImage(string path)
        {
            UploadModel.SelectedImage = string.Empty;
            using (var img = Image.FromFile(path))
            {
                using (var ms = new MemoryStream())
                {
                    img.Save(ms, ImageFormat.Png);
                    var bytes = ms.ToArray();

                    UploadModel.SelectedImage = Convert.ToBase64String(bytes);
                }
            }
            return Json(new { success = true, data = UploadModel.SelectedImage, path });
        }

        public IActionResult RemoveFile(string filePath)
        {
            if (!string.IsNullOrEmpty(SessionModel.ID))
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    GetFileCount();
                    GetUploadFileInfo();
                }

                return View("Index");
            }
            else
            {
                return View("~/Views/Login/Login.cshtml");
            }
        }
    }
}
