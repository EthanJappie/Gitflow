using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickIndexing.Models
{
    public class AuthenticateModel
    {
        public string USER_NAME { get; set; }
        public bool AuthenticateSuccess { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string DEALER { get; set; }
        public string BRAND { get; set; }
        public string ErrorMessage { get; set; }
        public Guid Token { get; set; }
    }
}
