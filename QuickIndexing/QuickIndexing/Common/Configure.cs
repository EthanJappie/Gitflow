using QuickIndexing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickIndexing.Enumeration;

namespace QuickIndexing.Common
{
    public class Configure
    {
        public static void Config(string referenceNumber)
        {
            ReferenceModel.ReferenceNumber = referenceNumber;
        }

        public static void Config(int fileCount)
        {
            UploadModel.FileCount = fileCount;
        }
        
        public static void ConfigDealType(string dealType)
        {
            switch (dealType)
            {
                case "1": ReferenceModel.DealType = ReferenceTypes.Credit.ToString();
                    break;
                case "Deal": ReferenceModel.DealType = ReferenceTypes.Deal.ToString();
                    break;
                case "DP": ReferenceModel.DealType = ReferenceTypes.DP.ToString();
                    break;
                default:
                    break;
            }
        }
        public static void Config(string name, string lastname, string Id)
        {
            SessionModel.Name = name;
            SessionModel.Lastname = lastname;
            SessionModel.ID = Id;
        }
    }
}
