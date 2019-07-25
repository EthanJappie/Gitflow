using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using WebSupergoo.ABCpdf7;

namespace BMW.FS.Indexing.WebServices
{
    public static class BarcodeDetection
    {
        static string _EDSDirectory = "";
        static string _connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
        public static string Logfile = @"c:\QIWSLogging.txt";
        static bool tryMultipleBarcodes = true;

        public static FSBarcodeRW.Result[] TryReadBarcode(string fileName, string newDirectory, string referenceNumber, string AttestUserID, string AttestUserName
            , int imageNumber, int batchNumber, int sourceFolder, int rootID, int poolNumber, int documentID, string IDorDL)
        {
            AddLog_KYC("Input", "QIWS.ID_BarcodeDetection.TryReadBarcode", fileName, "Input TXT", "ref:" + referenceNumber + " /Uid:" + AttestUserID);

            string DocType = "";
            FSBarcodeRW.Result[] results = null;
            _EDSDirectory = newDirectory;
            string BarcodeDetected = "";
            string debugMark = "01";
            try
            {
                if (!File.Exists(fileName))
                {
                    AddLog_KYC("ERROR", "QIWS.ID_BarcodeDetection.TryReadBarcode", "File does not exists - " + fileName, "Output TXT", "Ref: " + referenceNumber);
                    return null;
                }

                FileInfo info = new FileInfo(fileName);
                string fileNameOnly = info.Name.Replace("_", ""); //remove undescore added
                debugMark = "02";
                // find barcodes

                var bitmap1 = (Bitmap)Bitmap.FromFile(fileName);
                if (bitmap1 == null)
                {
                    AddLog_KYC("Error", "QIWS.ID_BarcodeDetection.TryReadBarcode", "Cannot create bitmap from file", "Output TXT", fileName);
                    return null;
                }


                tryMultipleBarcodes = false;
                using (var bitmap = (Bitmap)Bitmap.FromFile(fileName))
                {
                    debugMark = "03";
                    System.Collections.Generic.List<FSBarcodeRW.BarcodeFormat> formats = new System.Collections.Generic.List<FSBarcodeRW.BarcodeFormat>();
                    if (IDorDL == "DL")
                    {
                        DocType = "KYCDecodeDLBarcode";
                        formats.Add(FSBarcodeRW.BarcodeFormat.PDF_417);
                    }
                    else
                    {
                        DocType = "KYCDecodeIDBarcode";
                        formats.Add(FSBarcodeRW.BarcodeFormat.CODE_93);
                        formats.Add(FSBarcodeRW.BarcodeFormat.CODE_39);
                    }

                    FSBarcodeRW.BarcodeReader barcodeReader = new FSBarcodeRW.BarcodeReader();
                    barcodeReader.AutoRotate = true;
                    barcodeReader.TryInverted = true;

                    FSBarcodeRW.Common.DecodingOptions options = new FSBarcodeRW.Common.DecodingOptions();
                    options.TryHarder = true;
                    options.PossibleFormats = formats;
                    barcodeReader.Options = options;

                    debugMark = "04";
                    if (tryMultipleBarcodes)
                    {
                        results = barcodeReader.DecodeMultiple(bitmap);

                        debugMark = "05";
                        if (results != null)
                        {
                            foreach (FSBarcodeRW.Result res in results)
                            {
                                if (res.Text.Length == 13)
                                {
                                    BarcodeDetected = res.Text;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            BarcodeDetected = "No barcode recognized";
                        }
                    }
                    else
                    {
                        var result = barcodeReader.Decode(bitmap);
                        if (result != null)
                        {
                            BarcodeDetected = result.Text;
                        }
                        else
                        {
                            BarcodeDetected = "No barcode recognized";
                        }
                    }

                    debugMark = "06";
                    // save any found barcodes seperately
                    try
                    {
                        if (results != null)
                        {
                            foreach (FSBarcodeRW.Result res in results)
                            {
                                string BarcodeType = res.BarcodeFormat.ToString();
                                string Barcode = res.Text;

                                int RowsAffected = SQLInsertBarcode(imageNumber, batchNumber, sourceFolder, rootID, 0, documentID, fileNameOnly, referenceNumber, BarcodeType, Barcode);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        AddLog_KYC("ERROR", "QIWS.ID_BarcodeDetection.TryReadBarcode_b", e.Message, "Output TXT", "DocumentIndexingWS File: " + fileNameOnly);
                    }

                    debugMark = "06";
                    SaveBarcodeToImaging(referenceNumber, BarcodeDetected, fileName, results, AttestUserID, AttestUserName, DocType);

                    return results;
                }
            }
            catch (Exception e)
            {
                AddLog_KYC("Error", "QIWS.ID_BarcodeDetection.Debug:" + debugMark, e.Message, "Output TXT", referenceNumber);
                return results;
            }
        }//TryReadBarcode

        private static void SaveBarcodeToImaging(string referenceNumber, string BarcodeDetected, string ImageFilename, FSBarcodeRW.Result[] results, string AttestUserID, string AttestUserName, string DocType)
        {
            AddLog_KYC("Input", "QIWS.ID_BarcodeDetection.SaveBarcodeToImaging", BarcodeDetected, "Input TXT", referenceNumber);

            string DataString = "Application Reference number: " + referenceNumber;
            DataString += "\r\n\r\n";
            try
            {

                if (results != null)
                {
                    foreach (FSBarcodeRW.Result res in results)
                    {
                        string BarcodeType = res.BarcodeFormat.ToString();
                        string Barcode = res.Text;

                        if (res.BarcodeFormat.ToString() == "PDF_417")
                        {
                            //DataString += "Barcode Format: " + res.BarcodeFormat.ToString() + "\r\n";
                            DataString += "\r\nBarcode Data: " + "\r\n\r\n";
                            char[] splitChars = { '|' };
                            string[] StringArr = Barcode.Split(splitChars);
                            foreach (string item in StringArr)
                            {
                                if (item.Contains("123456789"))
                                    break;
                                DataString += "" + item + "\r\n";
                            }
                            DataString += "\r\n\r\n";
                        }
                        else
                        {
                            //DataString += "Barcode Format: " + res.BarcodeFormat.ToString() + "\r\n";
                            DataString += "\r\nBarcode Data: \r\n"
                                + "\r\n" + Barcode + "\r\n\r\n";
                        }
                    }
                }
                else
                {
                    DataString += "Barcode could not be decoded or barcode not detected.\r\n";
                }

                string retImageAdd = "";
                Image img = Image.FromFile(ImageFilename);

                //string TempFileName = OutputToPdf(referenceNumber, DataString);
                string TempFileName = "";//ABCpdfHandler.OutputToPdf(referenceNumber, DataString, @"ID_Dec_", img, AttestUserID, AttestUserName);
                TempFileName = CreatePDF(referenceNumber, DataString, AttestUserID, AttestUserName);

                if (TempFileName.Length > 0)
                    retImageAdd = AddToImaging(DocType, TempFileName, referenceNumber);

                if (!retImageAdd.StartsWith("NOK"))
                {
                    File.Delete(TempFileName);
                }

                AddLog_KYC("Output", "QIWS.ID_BarcodeDetection.SaveBarcodeToImaging", DataString, "Output TXT", referenceNumber);
            }
            catch (Exception e)
            {
                AddLog_KYC("Error", "QIWS.ID_BarcodeDetection.SaveBarcodeToImaging", e.Message, "Output TXT", referenceNumber);
            }

        }

        private static string CreatePDF(string referenceNumber, string ReturnString, string AttestUserID, string AttestUserName)
        {
            AddLog_KYC("Input", "QIWS.ID_BarcodeDetection.CreatePDF", ReturnString, "Input TXT", referenceNumber);

            //Dictionary<String, Object> dict_licenceinfo = new Dictionary<String, Object>();
            //dict_licenceinfo = Generics.ConvertClassToDictionary_Fields<KYC_LicenceInfo>(_licenceInfo);

            string Filename = "";
            Bitmap imageLogo = new Bitmap(BMW.FS.Indexing.WebServices.Properties.Resources.BMWMINI);
            try
            {
                using (KYC_PDF.ABCpdfHandler PDF = new KYC_PDF.ABCpdfHandler())
                {
                    PDF.IDNumber = "";
                    PDF.Reference = referenceNumber;
                    PDF.ReturnString = ReturnString;
                    PDF.FilePrefix = "ID_Dec_";
                    PDF.imgLogo = imageLogo;
                    PDF.imgPhoto = null;
                    PDF.FIUserID = AttestUserID;
                    PDF.FIName = AttestUserName;
                    PDF.DoPdfOnly = true;   // handle the saving to Imaging here
                    PDF.ConnString = "";    // if DoPdfOnly=false then set the connection string
                    Filename = PDF.OutputToPdf_IDdecode();
                }

                AddLog_KYC("Output", "QIWS.ID_BarcodeDetection.CreatePDF", Filename, "Output TXT", referenceNumber);
                return Filename;
            }
            catch (Exception e)
            {
                AddLog_KYC("Error", "QIWS.ID_BarcodeDetection.CreatePDF", e.Message, "Output TXT", referenceNumber);
            }
            return Filename;
        }

        private static string GetCurrentAssemblyVersion(string assembly)
        {
            try
            {
                string assemblyFolder = assemblyFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string AssFileName = AssFileName = Path.Combine(assemblyFolder, assembly);
                System.Reflection.AssemblyName myAssemblyName = System.Reflection.AssemblyName.GetAssemblyName(AssFileName);
                return myAssemblyName.Version.ToString();
            }
            catch (Exception ex)
            {
                //AddLog_KYC("ERR", "QIWS.ID_BarcodeDetection_GetCurrentAssemblyVersion", ex.Message, "Output TXT", "");
                return "ver?";
            }
        }

        public static void AddLog_KYC(string logType, string interfaceName, object myClass, string message, string referenceNumber)
        {
            string ver = "";
            string host = Environment.MachineName;
            try
            {
                try
                {
                    ver = GetCurrentAssemblyVersion("BMW.FS.Indexing.WebServices.dll");
                    ver = " [" + host + "~" + ver + "~BMW.FS.Indexing.WebServices.dll]";
                }
                catch
                {
                    ver = " [" + host + "~ver?~BMW.FS.Indexing.WebServices.dll]";
                }
                string xmlString = "";

                if (message.Contains("XML"))
                {
                    StringWriter XML = new StringWriter();
                    XmlSerializer x = new XmlSerializer(myClass.GetType());
                    x.Serialize(XML, myClass);

                    xmlString = XML.ToString();
                    xmlString = xmlString.Replace("utf-16", "utf-8"); // JB - otherwise can't CAST as xml in SQL
                }
                else
                {
                    xmlString = (string)myClass;
                }
                //truncate to db field sizes 
                if (interfaceName.Length > 50) interfaceName = interfaceName.Substring(0, 50);
                if (message.Length > 200) message = message.Substring(0, 200);
                if (referenceNumber.Length > 50) referenceNumber = referenceNumber.Substring(0, 50);

                DataSet dataSet = new DataSet();
                SqlDataAdapter sqlDataAdapter;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlParameter[] sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter("@LogType", logType + ver);
                    sqlParameters[1] = new SqlParameter("@Interface", interfaceName);
                    sqlParameters[2] = new SqlParameter("@XML", xmlString);
                    sqlParameters[3] = new SqlParameter("@Message", message);
                    sqlParameters[4] = new SqlParameter("@ReferenceNumber", referenceNumber);

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "Documents.dbo.spKYC_AddLog";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(sqlParameters);

                        sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(dataSet);
                    }
                }

            }
            catch (Exception e)
            {
                AddTextLog(e.Message, Logfile);
            }


        }

        public static void AddTextLog(string log, string logFile)
        {
            try
            {
                using (TextWriter tw = new StreamWriter(logFile, true))
                {
                    tw.Write(DateTime.Now.ToString());
                    tw.WriteLine(" - " + log);
                }
            }
            catch (DirectoryNotFoundException)
            {
                // Create the directory if it is not found
                int pos = Logfile.LastIndexOf("\\");
                string path = logFile.Substring(0, pos);
                DirectoryInfo di = Directory.CreateDirectory(path);
                //..then retry
                AddTextLog(log, logFile);
            }
            catch
            {
            }
        }

        public static string AddToImaging(string DocType, string DocPath, string referenceNumber)
        {
            string Msg = "";

            if (DocPath.Length == 0)
            {
                Msg = "NOK " + Environment.NewLine + "FileName is missing";
                AddLog_KYC("ERROR", "IndexingingWS.BarcodeDetection.AddToImaging", "File not found", "Output TXT", DocPath);

                return Msg;
            }

            try
            {
                AddLog_KYC("Input", "IndexingingWS.BarcodeDetection.AddToImaging", DocType + " - " + DocPath, "Output TXT", referenceNumber);
                ////****  copy doc to imaging location
                FileInfo FI = new FileInfo(DocPath);

                string newFilPath = _EDSDirectory + "\\" + FI.Name;

                File.Copy(DocPath, newFilPath, true);

                if (File.Exists(newFilPath))
                {
                    ////****  create document entry in DB
                    int inserted = 0;
                    inserted = SaveDocToImaging("KYCDecodeIDBarcode", referenceNumber, FI.Name);

                    if (inserted > 0)
                    {
                        return "OK";
                    }
                    else
                    {
                        Msg = "NOK " + Environment.NewLine + "Document not introduced to DB";
                        AddLog_KYC("Error", "AddToImaging 1", "Output TXT", Msg, referenceNumber);
                        return Msg;
                    }
                }
                else
                {
                    Msg = "NOK " + Environment.NewLine + "File not saved to imaging";
                    AddLog_KYC("Error", "AddToImaging 2", "Output TXT", Msg, referenceNumber);
                    return Msg;
                }
            }
            catch (Exception e)
            {
                AddLog_KYC("Error", "AddToImaging", e.Message, "Output TXT", referenceNumber);
                Msg = "NOK " + Environment.NewLine + e.Message;
            }
            return Msg;

        }

        public static int SaveDocToImaging(string Type, string Dealref, string fileName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@Type", Type);
                    sqlParameters[1] = new SqlParameter("@Reference", Dealref);
                    sqlParameters[2] = new SqlParameter("@FileName", fileName);

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "Documents.dbo.spKYC_IntroduceDoc";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(sqlParameters);

                        int ret = command.ExecuteNonQuery();
                        return 1;
                    }
                }
            }
            catch (Exception e)
            {
                string Msg = "NOK " + Environment.NewLine + e.Message;
                AddLog_KYC("Error", "KYC SaveDocToImaging", "", Msg, Dealref);
                return 0;
            }
        }

        private static int SQLInsertBarcode(int imageNumber, int batchNumber, int sourceFolder, int rootID, int poolNumber
            , int documentID, string fileName, string referenceNumber, string BarcodeType, string BarcodeDetected)
        {

            DataSet dataSet = new DataSet();
            SqlDataAdapter sqlDataAdapter;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlParameter[] sqlParameters = new SqlParameter[10];
                sqlParameters[0] = new SqlParameter("@ImageNumber", imageNumber);
                sqlParameters[1] = new SqlParameter("@BatchNumber", batchNumber);
                sqlParameters[2] = new SqlParameter("@SourceFolder", sourceFolder);
                sqlParameters[3] = new SqlParameter("@RootID", rootID);
                sqlParameters[4] = new SqlParameter("@PoolNumber", poolNumber);
                sqlParameters[5] = new SqlParameter("@DocumentID", documentID);
                sqlParameters[6] = new SqlParameter("@ReferenceNumber", referenceNumber);
                sqlParameters[7] = new SqlParameter("@FileNameOnly", fileName);
                sqlParameters[8] = new SqlParameter("@BarcodeType", BarcodeType);
                sqlParameters[9] = new SqlParameter("@BarcodeDetected", BarcodeDetected);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = "spKYC_InsertBarcode";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(sqlParameters);

                    sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataSet);
                }
            }

            int RowsAffected = 0;
            if (dataSet.Tables.Count == 0)
                return RowsAffected;
            else
            {
                foreach (DataRow dr in dataSet.Tables[0].Rows)
                {
                    RowsAffected = (int)dr["RowsAffected"];
                    break;
                }
                return RowsAffected;
            }
        }
    }
}