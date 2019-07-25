using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace QuickIndexing.Common
{
    public class ImageHelper
    {
        public int GetNextBatchNumber()
        {
            using (var client = new WebClient())
            {
                var url = $"{API.GetAPIUrl()}document/GetNextBatchNumber";

                var result = client.DownloadString(url);

                return int.Parse(result);
            }
        }

        public int ResolveReferenceType(string referenceTypeCode)
        {
            using (var client = new WebClient())
            {
                var url = $"{API.GetAPIUrl()}document/ResolveReferenceType/{referenceTypeCode}";

                var result = client.DownloadString(url);

                return int.Parse(result);
            }
        }

        public int CreatePool(string referenceNumber, int referenceType)
        {
            using (var client = new WebClient())
            {
                var url = $"{API.GetAPIUrl()}document/CreatePool/{referenceNumber}/{referenceType}";

                var result = client.DownloadString(url);

                return int.Parse(result);
            }
        }

        public int GetRootID()
        {
            using (var client = new WebClient())
            {
                var url = $"{API.GetAPIUrl()}document/GetRootID";

                var result = client.DownloadString(url);

                return int.Parse(result);
            }
        }

        public string GetRootLocationPath(int rootId)
        {
            using (var client = new WebClient())
            {
                var url = $"{API.GetAPIUrl()}document/GetRootLocationPath/{rootId}";

                var result = client.DownloadString(url);

                return result;
            }

        }

        public DateTime SQLIntroduceDocument(int batchNumber, int docNumber, string recievedBy, int sourceFolder, int rootID, int poolNumber, string fileName
                        , int documentID, string documentDescription, string referenceNumber, int referenceType)
        {
            using (var client = new WebClient())
            {
                var url = $"{API.GetAPIUrl()}document/SQLIntroduceDocument/{batchNumber}/{docNumber}/{recievedBy}/{sourceFolder}/{rootID}" +
                    $"/{poolNumber}/{fileName}/{documentID}/{documentDescription}/{referenceNumber}/{referenceType}";

                var result = client.DownloadString(url);

                return Convert.ToDateTime(result);
            }
        }
    }
}
