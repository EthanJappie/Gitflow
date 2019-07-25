using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuickIndexing.Common;
using QuickIndexing.Models;

namespace QuickIndexing.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index(AuthModel model)
        {
            if (ModelState.IsValid)
            {
                var username = Security.SimpleEncrypt(model.Username);
                var password = Security.SimpleEncrypt(model.Password);

                using (var client = new WebClient())
                {
                    var result = client.DownloadString($"{API.GetAPIUrl()}auth/Authenticate/{username}/{password}");
                    var json = JsonConvert.DeserializeObject<AuthenticateModel>(result);
                }
            }
            return View(model);
        }

        public IActionResult Login(AuthModel model)
        {
            if (!string.IsNullOrEmpty(model.Username) && !string.IsNullOrEmpty(model.Password))
            {
                if (ModelState.IsValid)
                {
                    var username = Security.SimpleEncrypt(model.Username);
                    var password = Security.SimpleEncrypt(model.Password);

                    var auth = new AuthModel()
                    {
                        Username = username,
                        Password = password
                    };

                    using (var client = new WebClient())
                    {
                        var url = $"{API.GetAPIUrl()}auth/Authenticate";
                        var obj = JsonConvert.SerializeObject(auth);
                        client.Headers["Content-Type"] = ContentTypes.json;
                        var result = client.UploadString(url, obj);

                        var json = JsonConvert.DeserializeObject<AuthenticateModel>(result);

                        if (!json.AuthenticateSuccess)
                        {
                            SessionModel.ErrorMessage = json.ErrorMessage;
                            return View(model);
                        }
                        else
                        {
                            SessionModel.ID = json.USER_NAME;
                            SessionModel.Name = json.FIRST_NAME;
                            SessionModel.Lastname = json.LAST_NAME;

                            return RedirectToAction("Index", "Home", json);
                        }                        
                    }
                }
            }
            return View(model);  
        }

        public IActionResult Logout()
        {
            SessionModel.ErrorMessage = null;
            SessionModel.ID = null;
            SessionModel.Name = null;
            SessionModel.Lastname = null;

            return View("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}