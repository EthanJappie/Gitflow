using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace QuickIndexing.Common
{
    public class Constants
    {
        public static string G_ProxyName;
        public enum G_ImageEdit { RotateLeft, RotateRight, Rotate180, Flip, Trim, Deskew };
        public const string G_KEY_DOCUMENT = "HbQR9NSXQ3";
        public const string G_KEY_GIFLZW = "sg8Z2XkjL";
        public const string G_KEY_TIFLZW = "gZWEhj9ZX2j";
        public const string G_KEY_FPXEXTENSIONS = "";
        public const string G_KEY_OCR_PLUS = "rvdKxn8Zr4";
        public const string G_KEY_MULTIMEDIA = "";
        public const string G_KEY_MEDICAL = "";
        public const string G_KEY_MEDICAG_NET = "";
        public const string G_KEY_JBIG = "";
        public const string G_KEY_VECTOR = "";
        public const string G_KEY_BARCODES_1D = G_KEY_TIFLZW;
        public const string G_KEY_BARCODES_2D_READ = "";
        public const string G_KEY_BARCODES_2D_WRITE = "";
        public const string G_KEY_BARCODES_PDF_READ = "";
        public const string G_KEY_BARCODES_PDF_WRITE = "";
        public const string G_KEY_INTERNET = "";
        public const string G_KEY_VECTOR_DWG = "";
        public const string G_KEY_VECTOR_DXF = "";
        public const string G_KEY_VECTOR_MISC = "";
        public const string G_KEY_VECTOR_ALL = "";
        public const string G_KEY_VECTOR_DWF = "";
        public const string G_KEY_RESERVED1 = "";
        public const string G_KEY_PDF = "Wvuz2WC3rX";
        public const string G_KEY_VECTOR_VIEW = "";
        public enum G_REFERENCETYPE { Credit = 1, Deal = 2, DP = 3, None = 4, HA = 5, HD = 6, CF = 7 }
        public enum G_OPERATION { NEW = 1, UPDATE = 2, DELETE = 3 }
        public enum G_DEALERTYPE { BMW = 1, ALPHERA = 2 }
        public enum G_FILECONSTATS { FILE_RASTER_DUMP }
        public const string G_CreditFolderString = "Credit";
        public const string G_DealFolderString = "Deal";
        public const string G_DealerPointFolderString = "DP";
        public const string G_HomeLoneAppFolderString = "HA";
        public const string G_HomeLoneDealFolderString = "HD";
        public const string G_CommercialFinanceFolderString = "CF";
        public const string G_EmailFolderString = "Email";
        public const string G_FAXFolderString = "FaxAtt";
        public const string G_NOTSETDESCRIPTION = "Not set";
        public const string G_DYNAMIC_CONDITION = "DC";
        public const string G_STATIC_CONDITION = "ST";
        public const string G_DYNAMIC_CONDITION_CREDIT = "Credit Conditions";
        public const string G_DYNAMIC_CONDITION_PAYOUT = "Payout Conditions";
        public const string G_DEFAULT_DRAWER = "Unfiled documents";
        public const string G_DEFAULT_FOLDER = "New documents";
        public const string G_DEFAULT_USER_ADDED = "Added by user";
        public const string G_DEFAULT_ALL_DOCUMENTS_CODE = "SALL";
        public const string G_DEFAULT_ALL_DOCUMENTS_TEXT = "All Documents";
        public const string G_DEFAULT_DRAWER_CODE = "S0";
        public const string G_DEFAULT_FOLDER_CODE = "F0";
        public const string G_DEFAULT_USER_ADDED_CODE = "F-1";
        public const int G_DEFAULT_CONDITION_ID = -999;
        public const int G_WebServiceTimeOut = 60000;
        private const string _userName = "";
        private const string _password = "";
        private const string _domain = "";
        public const int G_DefaultConnectionLimit = 20;
        public const string G_EmailRecipient_BMW = "workflow.za@bmwfinance.co.za";
        public const string G_EmailRecipient_Alphera = "workflow.za@alpherafinance.co.za";
        public static string G_AllowedExtensions = string.Empty;
        public static bool G_AllowOptionToChooseFileSubmission = true;
        public static string G_DefaultFileSubmission = "Email";

        public static DataSet Deserialize(byte[] array)
        {
            var ds = new DataSet();
            try
            {
                MemoryStream stream = new MemoryStream(array);
                XmlSerializer serializer = new XmlSerializer(typeof(DataSet));

                ds = (DataSet)serializer.Deserialize(stream);
                return ds;
            }
            catch
            {
                throw;
            }
        }
    }
}
