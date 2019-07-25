using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuickIndexing.Common;
using Newtonsoft.Json;
using QuickIndexing.Models;
using QuickIndexing.Enumeration;
using System.Net.Http;
using System.IO;
using System.Text;

namespace QuickIndexing.Controllers
{
    public class DocumentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SubmitDocs(string selectedFiles, int documentId, int folderId, string documentDescription)
        {
            if (!string.IsNullOrEmpty(SessionModel.ID))
            {
                var selectedDocs = JsonConvert.DeserializeObject<List<string>>(selectedFiles);
                var username = SessionModel.ID;
                var referenceNumber = ReferenceModel.ReferenceNumber;
                Enum.TryParse(ReferenceModel.DealType, out ReferenceTypes types);

                try
                {
                    if (selectedDocs.Count > 0)
                    {
                        foreach (var item in selectedDocs)
                        {
                            using (var client = new HttpClient())
                            {
                                byte[] bytes;
                                using (var stream = new FileStream(item, FileMode.Open))
                                {
                                    using (var ms = new MemoryStream())
                                    {
                                        stream.CopyTo(ms);
                                        bytes = ms.ToArray();
                                    }
                                }
                                var model = new SubmitModel()
                                {
                                    Extension = Path.GetExtension(item),
                                    DocumentID = documentId,
                                    DocumentDescription = documentDescription,
                                    Username = username,
                                    ReferenceNumber = referenceNumber,
                                    ReferenceType = (int)types,
                                    FolderID = folderId,
                                    File = bytes
                                };

                                var url = $"{API.GetAPIUrl()}document/SubmitDocument";
                                var json = await client.PostAsJsonAsync(url, model);

                                if (json.IsSuccessStatusCode)
                                {
                                    DocumentModel.SubmissionStatus = json.IsSuccessStatusCode;
                                    DocumentModel.SubmissionFailure = false;

                                    new FileConversion().DeleteFile(item);
                                }
                                else
                                {
                                    DocumentModel.SubmissionFailure = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    DocumentModel.SubmissionFailure = true;
                }

                return View("~/Views/Home/Index.cshtml");
            }
            else
            {
                return View("~/Views/Login/Login.cshtml");
            }
        }
    }
}