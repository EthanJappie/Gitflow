using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuickIndexing.Common;
using QuickIndexing.Models;

namespace QuickIndexing.Controllers
{
    public class TreeController : Controller
    {
        [HttpGet]
        public JsonResult GetDocumentTree(string referenceType, string referenceNumber)
        {
            var documentList = new List<DocumentTypeModel>();
            var documentGroups = new List<string>();

            var tree = new List<DocumentTypeViewModel>();
            try
            {
                using (var client = new WebClient())
                {
                    var url = string.Empty;

                    if (!string.IsNullOrEmpty(referenceNumber))
                    {
                        url = $"{API.GetAPIUrl()}data/GetDocumentTypes/{referenceType}/{referenceNumber}";
                    }
                    else
                    {
                        url = $"{API.GetAPIUrl()}data/GetDocumentTypes";
                    }                       
                    var result = client.DownloadString(url);

                    documentList = JsonConvert.DeserializeObject<List<DocumentTypeModel>>(result);
                    documentGroups = GetDocumentGroups(documentList);
                    int groupId = 1;

                    foreach (var item in documentGroups)
                    {
                        tree.Add(new DocumentTypeViewModel()
                        {
                            id = groupId,
                            text = item,
                            Description = item,
                            GroupID = groupId,
                            Children = GetChildren(documentList, item)
                        });
                        groupId++;
                    }
                }
                return this.Json(tree);
            }
            catch (Exception)
            {
                throw;
            }            
        }

        public List<string> GetDocumentGroups(List<DocumentTypeModel> model)
        {
            var result = new List<string>();

            foreach (var item in model.Select(x => x.DocumentGroup).Distinct())
            {
                result.Add(item);
            }
            return result;
        }

        public List<DocumentTypeViewModel> GetChildren(List<DocumentTypeModel> model, string parentGroup)
        {
            var result = new List<DocumentTypeViewModel>();

            try
            {
                result = model.Where(x => x.DocumentGroup == parentGroup).OrderBy(x => x.OrderID)
                .Select(x => new DocumentTypeViewModel
                {
                    id = x.DocumentID,
                    text = x.Description,
                    DocumentID = x.DocumentID,
                    Description = x.Description,
                    FolderID = x.FolderID,
                    DocumentGroup = x.DocumentGroup,
                    ColourBits = x.ColourBits,
                    ConditionCode = x.ConditionCode,
                    ConditionID = x.ConditionID,
                    OrderID = x.OrderID,
                    BucketColour = x.BucketColour,
                    Show_DatePicker = x.Show_DatePicker,
                    Attest = x.Attest
                }).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
    }
}