using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Data.SqlClient;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Web.Hosting;
using System.Diagnostics;
using System.Drawing;

namespace BMW.FS.Indexing.WebServices
{
    /// <summary>
    /// Summary description for BMWFSIndexingService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]

    public class BMWFSIndexingService : System.Web.Services.WebService
    {

        #region TestWebService
        [WebMethod]
        public bool TestWebService()
        {
            try
            {
                return true;
            }
            catch
            {
                throw;
            }
        }
        #endregion


        #region GetEncryptionPublicKey
        [WebMethod]
        public string GetEncryptionPublicKey()
        {
            string xmlString = "";
            try
            {
                string fileName = HostingEnvironment.ApplicationPhysicalPath + System.Configuration.ConfigurationManager.AppSettings["EncryptionPublicKeyFile"].ToString();
                StreamReader sr = new StreamReader(fileName);
                xmlString = sr.ReadToEnd();
                sr.Close();
            }
            catch (System.IO.FileNotFoundException e)
            {
                xmlString = "No Publicly encryption file found on BMW.FS.Indexing.WebService";
                //throw;
            }
            return xmlString;
        }
        #endregion


        #region GetDocumentTypes
        [WebMethod]
        public byte[] GetDocumentTypes(string referenceType, string referenceNumber)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[2];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@ReferenceType";
                    prms[0].Value = referenceType;

                    prms[1] = new SqlParameter();
                    prms[1].ParameterName = "@ReferenceNumber";
                    prms[1].Value = referenceNumber;

                    using (SqlCommand command = new SqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandText = "SPRcnGetDocumentTypes";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);

                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetDocumentDynamicConditions
        [WebMethod]
        public byte[] GetDocumentDynamicConditions(string referenceNo, string ReferenceType)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[2];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@RefNo";
                    prms[0].Value = referenceNo;

                    prms[1] = new SqlParameter();
                    prms[1].ParameterName = "@ReferenceType";
                    prms[1].Value = ReferenceType;

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPRcnGetDocumentDynamicConditions";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetDocumentDescriptionCodes
        [WebMethod]
        public byte[] GetDocumentDescriptionCodes()
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPRcnGetDocumentDescriptionCodes";
                        command.CommandType = CommandType.StoredProcedure;

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetDefaultValues
        [WebMethod]
        public byte[] GetDefaultValues()
        {
            MemoryStream memoryStream = new MemoryStream();
            Hashtable hashList;
            string WorkFlowAddress, EmailSize, AllowPartialEmail, faxMailBox, groupName, section, keyName, allowedExtensions;
            string allowOptionToChooseFileSubmission, defaultFileSubmission;
            DataColumn column;
            DataRow row;
            DataSet ds = new DataSet();
            string imageFolderPath = string.Empty;
            string indexerScanFolder = string.Empty;
            string quickIndexerPath = string.Empty;
            try
            {
                hashList = new Hashtable();
                DataTable dtDefaults = new DataTable("DefaultData");

                WorkFlowAddress = ConfigurationManager.AppSettings.Get("WorkFlowAddress");
                EmailSize = ConfigurationManager.AppSettings.Get("EmailSize");
                AllowPartialEmail = ConfigurationManager.AppSettings.Get("AllowPartialEmail");
                faxMailBox = ConfigurationManager.AppSettings.Get("FaxMailBox");
                groupName = ConfigurationManager.AppSettings.Get("GroupName");
                section = ConfigurationManager.AppSettings.Get("Section");
                keyName = ConfigurationManager.AppSettings.Get("KeyName");
                allowedExtensions = ConfigurationManager.AppSettings.Get("AllowedExtensions");
                allowOptionToChooseFileSubmission = ConfigurationManager.AppSettings.Get("AllowOptionToChooseFileSubmission");
                defaultFileSubmission = ConfigurationManager.AppSettings.Get("DefaultFileSubmission");

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Name";
                // Add the Column to the DataColumnCollection.
                dtDefaults.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Value";
                // Add the Column to the DataColumnCollection.
                dtDefaults.Columns.Add(column);

                row = dtDefaults.NewRow();
                row["Name"] = "WorkFlowAddress";
                row["Value"] = WorkFlowAddress;
                dtDefaults.Rows.Add(row);

                row = dtDefaults.NewRow();
                row["Name"] = "EmailSize";
                row["Value"] = EmailSize;
                dtDefaults.Rows.Add(row);

                row = dtDefaults.NewRow();
                row["Name"] = "AllowPartialEmail";
                row["Value"] = AllowPartialEmail;
                dtDefaults.Rows.Add(row);

                row = dtDefaults.NewRow();
                row["Name"] = "FaxMailBox";
                row["Value"] = faxMailBox;
                dtDefaults.Rows.Add(row);

                row = dtDefaults.NewRow();
                row["Name"] = "GroupName";
                row["Value"] = groupName;
                dtDefaults.Rows.Add(row);

                row = dtDefaults.NewRow();
                row["Name"] = "Section";
                row["Value"] = section;
                dtDefaults.Rows.Add(row);

                row = dtDefaults.NewRow();
                row["Name"] = "KeyName";
                row["Value"] = keyName;
                dtDefaults.Rows.Add(row);

                row = dtDefaults.NewRow();
                row["Name"] = "AllowedExtensions";
                row["Value"] = allowedExtensions;
                dtDefaults.Rows.Add(row);

                row = dtDefaults.NewRow();
                row["Name"] = "AllowOptionToChooseFileSubmission";
                row["Value"] = allowOptionToChooseFileSubmission;
                dtDefaults.Rows.Add(row);

                row = dtDefaults.NewRow();
                row["Name"] = "DefaultFileSubmission";
                row["Value"] = defaultFileSubmission;
                dtDefaults.Rows.Add(row);

                ds.Tables.Add(dtDefaults);

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);

                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetDocumentImageFolder
        [WebMethod]
        public byte[] GetDocumentImageFolder()
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPRcnGetDocumentImageFolder";
                        command.CommandType = CommandType.StoredProcedure;

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetReferenceStructure
        [WebMethod]
        public byte[] GetReferenceStructure(int securityLevel, int ReferenceType, char admin_User)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[3];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@UserSecurityLevel";
                    prms[0].Value = securityLevel;

                    prms[1] = new SqlParameter();
                    prms[1].ParameterName = "@ReferenceType";
                    prms[1].Value = ReferenceType;

                    prms[2] = new SqlParameter();
                    prms[2].ParameterName = "@Admin_User";
                    prms[2].Value = admin_User;

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPGetStructure";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetExtractParameters
        [WebMethod]
        public byte[] GetExtractParameters(int referenceType)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[1];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@ReferenceType";
                    prms[0].Value = referenceType;

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPGetExtractParameters";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetReferences
        [WebMethod]
        public byte[] GetReferences(string searchString, int ReferenceType, int parameterNumber)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[3];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@Search";
                    prms[0].Value = searchString;

                    prms[1] = new SqlParameter();
                    prms[1].ParameterName = "@RefType";
                    prms[1].Value = ReferenceType;

                    prms[2] = new SqlParameter();
                    prms[2].ParameterName = "@ParameterNumber";
                    prms[2].Value = parameterNumber;

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPGetReferences";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetDocuments
        [WebMethod]
        public byte[] GetDocuments(string referenceNumber, int ReferenceType, char getDeleted)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[3];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@ReferenceNumber";
                    prms[0].Value = referenceNumber;

                    prms[1] = new SqlParameter();
                    prms[1].ParameterName = "@ReferenceType";
                    prms[1].Value = ReferenceType;

                    prms[2] = new SqlParameter();
                    prms[2].ParameterName = "@GetDeleted";
                    prms[2].Value = getDeleted;

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPGetDocuments";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetNextNum
        [WebMethod]
        public byte[] GetNextNum(string NumberType, string userID, string workstation, char selectOut)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;
            float nextNum = 0;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[5];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@NumberType";
                    prms[0].Value = NumberType;

                    prms[1] = new SqlParameter();
                    prms[1].Direction = ParameterDirection.ReturnValue;
                    prms[1].ParameterName = "@NextNum";
                    prms[1].Value = nextNum;

                    prms[2] = new SqlParameter();
                    prms[2].ParameterName = "@UID";
                    prms[2].Value = userID;

                    prms[3] = new SqlParameter();
                    prms[3].ParameterName = "@WorkStat";
                    prms[3].Value = workstation;

                    prms[4] = new SqlParameter();
                    prms[4].ParameterName = "@SelectOUT";
                    prms[4].Value = selectOut;

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPGetNextNum";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetReferenceType
        [WebMethod]
        public byte[] GetReferenceType()
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPGetReferenceType";
                        command.CommandType = CommandType.StoredProcedure;

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region ValidateReference
        [WebMethod]
        public bool ValidateReference(string referenceNo, string referenceType)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            bool isValid = false;
            SqlParameter[] prms;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                prms = new SqlParameter[2];

                prms[0] = new SqlParameter();
                prms[0].ParameterName = "@ReferenceNo";
                prms[0].Value = referenceNo;

                prms[1] = new SqlParameter();
                prms[1].ParameterName = "@ReferenceType";
                prms[1].Value = referenceType;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPValidateReference";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);

                        if (ds != null)
                            if (ds.Tables[0].Rows.Count == 1)
                                bool.TryParse(ds.Tables[0].Rows[0]["IsValid"].ToString(), out isValid);
                    }
                }

                return isValid;
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region GetReferenceTypeDetails
        [WebMethod]
        public byte[] GetReferenceTypeDetails()
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPGetReferenceTypeDetails";
                        command.CommandType = CommandType.StoredProcedure;

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetDocumentPathDetails
        [WebMethod]
        public byte[] GetDocumentPathDetails()
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPGetDocumentPathDetails";
                        command.CommandType = CommandType.StoredProcedure;

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetScanFolderID
        [WebMethod]
        public int GetScanFolderID()
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            int scanFolderID = -1;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPGetScanFolderID";
                        command.CommandType = CommandType.StoredProcedure;

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);

                        if (ds != null)
                        {
                            if (ds.Tables[0].Rows.Count == 1)
                                int.TryParse(ds.Tables[0].Rows[0][0].ToString(), out scanFolderID);
                        }
                    }
                }
                return scanFolderID;
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region GetApplicationVersion
        [WebMethod]
        public string GetApplicationVersion()
        {
            string appVersion = string.Empty;

            try
            {
                appVersion = ConfigurationManager.AppSettings.Get("Version");
                return appVersion;
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region GetSystemIniDetails
        [WebMethod]
        public byte[] GetSystemIniDetails()
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            string groupName, section, keyName;
            SqlParameter[] prms;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                groupName = ConfigurationManager.AppSettings.Get("GroupName");
                section = ConfigurationManager.AppSettings.Get("Section");
                keyName = ConfigurationManager.AppSettings.Get("KeyName");

                prms = new SqlParameter[3];

                prms[0] = new SqlParameter();
                prms[0].ParameterName = "@GroupName";
                prms[0].Value = groupName;

                prms[1] = new SqlParameter();
                prms[1].ParameterName = "@Section";
                prms[1].Value = section;

                prms[2] = new SqlParameter();
                prms[2].ParameterName = "@KeyName";
                prms[2].Value = keyName;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPGetSystemIniDetails";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetAccessDetail
        [WebMethod]
        public byte[] GetAccessDetail(string userName, string appName)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["SysAdminConn"].ToString();

                prms = new SqlParameter[2];

                prms[0] = new SqlParameter();
                prms[0].ParameterName = "@UID";
                prms[0].Value = userName;

                prms[1] = new SqlParameter();
                prms[1].ParameterName = "@APPNAME";
                prms[1].Value = appName;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPGetAccessDetail";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region SearchCoverPage
        [WebMethod]
        public byte[] SearchCoverPage(int referenceType, string searchReference, string description, char unMatchOnly)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[4];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@ReferenceType";
                    prms[0].Value = referenceType;

                    prms[1] = new SqlParameter();
                    prms[1].ParameterName = "@SearchRef";
                    prms[1].Value = searchReference;

                    prms[2] = new SqlParameter();
                    prms[2].ParameterName = "@SearchDesc";
                    prms[2].Value = description;

                    prms[3] = new SqlParameter();
                    prms[3].ParameterName = "@UnMatchOnly";
                    prms[3].Value = unMatchOnly;

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPSearchCoverPage";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        #region GetReferenceDescriptionDetails
        [WebMethod]
        public byte[] GetReferenceDescriptionDetails()
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPGetReferenceDescriptionDetails";
                        command.CommandType = CommandType.StoredProcedure;

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }
        #endregion
        //#region DeleteDocument
        //[WebMethod]
        //public void DeleteDocument(long imageNumber, int copy)
        //{
        //    string connectionString = string.Empty;            
        //    SqlParameter[] prms;
        //    try
        //    {
        //        connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            prms = new SqlParameter[2];

        //            prms[0] = new SqlParameter();
        //            prms[0].ParameterName = "@ImageNumber";
        //            prms[0].Value = imageNumber;

        //            prms[1] = new SqlParameter();
        //            prms[1].ParameterName = "@Copy";
        //            prms[1].Value = copy;                   

        //            using (SqlCommand command = new SqlCommand())
        //            {
        //                command.Connection = conn;
        //                command.CommandText = "SPDeleteDoc";
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddRange(prms);
        //                command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }           
        //}
        //#endregion
        #region UpdateDocumentPool
        [WebMethod]
        public void UpdateDocumentPool(long imageNumber, int copy)
        {
            string connectionString = string.Empty;
            SqlParameter[] prms;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[2];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@ImageNumber";
                    prms[0].Value = imageNumber;

                    prms[1] = new SqlParameter();
                    prms[1].ParameterName = "@Copy";
                    prms[1].Value = copy;

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SPUpdateDocumentPool";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region IntroduceDocument
        [WebMethod]
        public bool IntroduceDocument(int batchNumber, int docNumber, string receivedBy, int sourceFolder, int rootID, int poolNumber, string fileName, string documentDescr, ref DateTime dtDate, ref int imageNumber)
        {
            string connectionString = string.Empty;
            SqlParameter[] prms;
            bool isTransactionSuccess = true;
            SqlTransaction transaction = null;
            DataSet dsResults;
            SqlDataAdapter sqlAdapt;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[8];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@BatchNumber";
                    prms[0].Value = batchNumber;

                    prms[1] = new SqlParameter();
                    prms[1].ParameterName = "@DocNumber";
                    prms[1].Value = docNumber;

                    prms[2] = new SqlParameter();
                    prms[2].ParameterName = "@RecievedBy";
                    prms[2].Value = receivedBy;

                    prms[3] = new SqlParameter();
                    prms[3].ParameterName = "@SourceFolder";
                    prms[3].Value = sourceFolder;

                    prms[4] = new SqlParameter();
                    prms[4].ParameterName = "@RootID";
                    prms[4].Value = rootID;

                    prms[5] = new SqlParameter();
                    prms[5].ParameterName = "@PoolNumber";
                    prms[5].Value = poolNumber;

                    prms[6] = new SqlParameter();
                    prms[6].ParameterName = "@FileName";
                    prms[6].Value = fileName;

                    prms[7] = new SqlParameter();
                    prms[7].ParameterName = "@DocumentDescr";
                    prms[7].Value = documentDescr;

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        transaction = conn.BeginTransaction("Main");

                        command.CommandText = "SPIntroduceDocument";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        dsResults = new DataSet();
                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(dsResults);

                        // Attempt to commit the transaction.
                        transaction.Commit();
                        isTransactionSuccess = true;
                    }

                    if (dsResults != null)
                    {
                        if (dsResults.Tables[0].Rows.Count == 1)
                        {
                            DateTime.TryParse(dsResults.Tables[0].Rows[0][0].ToString(), out dtDate);
                            int.TryParse(dsResults.Tables[0].Rows[0][1].ToString(), out imageNumber);
                        }
                        else
                            isTransactionSuccess = false;
                    }
                    else
                        isTransactionSuccess = false;
                }
            }
            catch
            {
                try
                {
                    isTransactionSuccess = false;

                    if (transaction != null)
                        transaction.Rollback("Main");
                }
                catch { }
                throw; // still throw an exception.
            }
            return isTransactionSuccess;
        }
        #endregion
        //#region MoveDocument
        //[WebMethod]
        //public bool MoveDocument(int poolID, long imageNumber, int copy, int newFolder, char UpdateTimeStamp)
        //{
        //    string connectionString = string.Empty;
        //    SqlParameter[] prms;
        //    bool isTransactionSuccess = true;
        //    SqlTransaction transaction = null;
        //    try
        //    {
        //        connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            prms = new SqlParameter[5];

        //            prms[0] = new SqlParameter();
        //            prms[0].ParameterName = "@PoolID";
        //            prms[0].Value = poolID;

        //            prms[1] = new SqlParameter();
        //            prms[1].ParameterName = "@ImageNumber";
        //            prms[1].Value = imageNumber;

        //            prms[2] = new SqlParameter();
        //            prms[2].ParameterName = "@Copy";
        //            prms[2].Value = copy;

        //            prms[3] = new SqlParameter();
        //            prms[3].ParameterName = "@NewFolder";
        //            prms[3].Value = newFolder;

        //            prms[4] = new SqlParameter();
        //            prms[4].ParameterName = "@UpdateTimeStamp";
        //            prms[4].Value = UpdateTimeStamp;                   

        //            using (SqlCommand command = new SqlCommand())
        //            {
        //                command.Connection = conn;
        //                transaction = conn.BeginTransaction("Main");

        //                command.CommandText = "SPMoveDocument";
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddRange(prms);

        //                command.ExecuteNonQuery();

        //                // Attempt to commit the transaction.
        //                transaction.Commit();
        //                isTransactionSuccess = true;
        //            }
        //        }

        //    }
        //    catch
        //    {
        //        try
        //        {
        //            isTransactionSuccess = false;
        //            if (transaction != null)
        //                transaction.Rollback("Main");
        //        }
        //        catch { }
        //        throw; // still throw an exception.
        //    }

        //    return isTransactionSuccess;
        //}
        //#endregion        
        #region GetUserName
        [WebMethod]
        public string GetUserName()
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings["UserName"].ToString();
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region GetPassword
        [WebMethod]
        public string GetPassword()
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings["Password"].ToString();
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region GetExecutableFileName
        [WebMethod]
        public string GetExecutableFileName()
        {
            string exeFileName = string.Empty;
            try
            {
                exeFileName = System.Configuration.ConfigurationManager.AppSettings["ExecutableFileName"].ToString();
                return exeFileName;
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region GetInstallationLocation
        [WebMethod]
        public string GetInstallationLocation()
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings["InstallSharedLocation"].ToString();
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region GetHttpInstallLocation
        [WebMethod]
        public string GetHttpInstallLocation()
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings["HttpInstallLocation"].ToString();
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region GetHttpInstallFile
        [WebMethod]
        public string GetHttpInstallFile()
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings["HttpInstallFile"].ToString();
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region DealerDetails

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DealerCode"></param>
        /// <returns></returns>
        [WebMethod]
        public byte[] GetDealerDetails(string DealerCode)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = string.Empty;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[1];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@DealerCode";
                    prms[0].Value = DealerCode;

                    using (SqlCommand command = new SqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandText = "spGetDealerDetails";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);

                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }

        #endregion
        #region InsertMailGUID
        [WebMethod]
        public int InsertMailGUID(string GuidEmail, string ApplicationVersion, string UserName, int EmailNr)
        {
            int iExec = 0;
            string connectionString = string.Empty;

            connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    System.Guid guidBatch = Guid.NewGuid();

                    cmd.CommandText = "spInsertDocumentEmail";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@BatchID", guidBatch));
                    cmd.Parameters.Add(new SqlParameter("@EmailID", GuidEmail));
                    cmd.Parameters.Add(new SqlParameter("@UserName", UserName));
                    cmd.Parameters.Add(new SqlParameter("@ProcessDate", DateTime.Now));
                    cmd.Parameters.Add(new SqlParameter("@Version", ApplicationVersion));
                    cmd.Parameters.Add(new SqlParameter("@EmailNr", EmailNr));

                    cmd.Connection = cn;

                    iExec = cmd.ExecuteNonQuery();

                }
            }

            return iExec;

        }
        #endregion
        #region GetServerFileList
        [WebMethod]
        public string[] GetServerFileList()
        {

            FileVersionInfo info = null;
            /// <summary>System.Collections.ArrayList</summary>
            ArrayList listOfFiles = new ArrayList();

            string filePath = Server.MapPath("~" + "/Installation/AutoUpdate/");

            string[] serverFileList = Directory.GetFiles(filePath);

            foreach (string fileItem in serverFileList)
            {
                info = FileVersionInfo.GetVersionInfo(fileItem);
                FileInfo inf = new FileInfo(fileItem);
                listOfFiles.Add(info.FileName + "|" + info.FileVersion + "|" + inf.Length);
            }

            string[] rolelistarray = (string[])listOfFiles.ToArray(typeof(string));

            return rolelistarray;
        }
        #endregion

        #region GetServerFile
        [WebMethod]
        public byte[] GetServerFile(string sFilename)
        {


            string filePath = Server.MapPath("~" + "/Installation/AutoUpdate/");

            byte[] bytesFile = File.ReadAllBytes(filePath + sFilename);

            return bytesFile;

        }
        #endregion


        #region GetAllDocumentChecks
        [WebMethod]
        public DataSet GetAllDocumentChecks()
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            //XmlDocument xmlDoc = new XmlDocument();
            //string xmlString = string.Empty;
            //MemoryStream memoryStream = new MemoryStream();
            //SqlParameter[] prms;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    //prms = new SqlParameter[2];

                    //prms[0] = new SqlParameter();
                    //prms[0].ParameterName = "@ReferenceType";
                    //prms[0].Value = referenceType;

                    //prms[1] = new SqlParameter();
                    //prms[1].ParameterName = "@ReferenceNumber";
                    //prms[1].Value = referenceNumber;

                    using (SqlCommand command = new SqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandText = "spGetAllDocumentChecks";
                        command.CommandType = CommandType.StoredProcedure;
                        //command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                //System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                //serializer.Serialize(memoryStream, ds);

                //return memoryStream.ToArray();
                return ds;
            }
            catch
            {
                throw;
            }
            finally
            {
                //memoryStream.Flush();
                //memoryStream.Close();
            }
        }
        #endregion


        #region DoNetworkTest
        [WebMethod]
        public bool DoNetworkTest()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings.Get("DoNetworkTest"));
        }
        #endregion


        #region DownloadTest
        [WebMethod]
        public byte[] DownloadTest()
        {
            string imageFile = Convert.ToString(ConfigurationManager.AppSettings.Get("NetworkTestFileLocation"));
            return File.ReadAllBytes(imageFile);
        }
        #endregion


        #region UploadTest
        [WebMethod]
        public void UploadTest(byte[] uploadData)
        {
            //Do nothing. Testing to see how long the input parameter uploadData takes to upload from the user's pc to the web-service
        }
        #endregion

        #region SendNetworkTestResults
        [WebMethod]
        public void SendNetworkTestResults(string emailAddress, string QIVersion, double pingTimeSeconds, int downloadSizeBytes,
                                        double downLoadTimeSeconds, int uploadSizeBytes, double uploadTimeSeconds, string errorMessage)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "spSendNetworkTestResults";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@EmailAddress", emailAddress));
                    cmd.Parameters.Add(new SqlParameter("@QIVersion", QIVersion));
                    cmd.Parameters.Add(new SqlParameter("@PingTimeSeconds", pingTimeSeconds));
                    cmd.Parameters.Add(new SqlParameter("@DownloadSizeBytes", downloadSizeBytes));
                    cmd.Parameters.Add(new SqlParameter("@DownloadTimeSeconds", downLoadTimeSeconds));
                    cmd.Parameters.Add(new SqlParameter("@UploadSizeBytes", uploadSizeBytes));
                    cmd.Parameters.Add(new SqlParameter("@UploadTimeSeconds", uploadTimeSeconds));
                    cmd.Parameters.Add(new SqlParameter("@ErrorMessage", errorMessage));

                    cmd.Connection = cn;

                    cmd.ExecuteNonQuery();

                }
            }
        }
        #endregion



        [WebMethod]
        public int GetNextBatchNumber()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SPGetNextNum";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@NumberType", "IBN"));
                    cmd.Parameters.Add(new SqlParameter("@NextNum", SqlDbType.Float));
                    cmd.Parameters[1].Direction = ParameterDirection.Output;

                    cmd.Connection = cn;

                    cmd.ExecuteNonQuery();

                    return Convert.ToInt32(cmd.Parameters[1].Value);
                }
            }
        }


        [WebMethod]
        public string SendFileOneTouch(string userName, string referenceNumber, string referenceTypeCode, int documentID
            , int folderID, string documentDescription, byte[] fileRawData, string fileExtension)
        {
            int EDSBatchNumber = this.GetNextBatchNumber();
            int documentNumber = 1;

            int referenceType = ResolveReferenceType(referenceTypeCode);
            string temporaryFileExtractionPath = ConfigurationManager.AppSettings.Get("TemporaryFileExtractionPath");
            int poolNumber = CreatePool(referenceNumber, referenceType);
            int rootID = GetRootID();
            string rootLocationPath = GetRootLocationPath(rootID);
            string fileNameOnly = "B" + EDSBatchNumber.ToString() + "D" + documentNumber.ToString() + fileExtension;
            string tempFile = temporaryFileExtractionPath + fileNameOnly;
            System.IO.File.WriteAllBytes(tempFile, fileRawData);

            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
            DateTime dateTime = Convert.ToDateTime(DateTime.Now);

            //2017-05-23 Hannes - FIX:this dateTime.ToString("MMMM") creates afrikaans month names at times..
            //string newDirectory = rootLocationPath + "\\" + dateTime.ToString("yyyy") + "\\" + dateTime.ToString("MMMM") + "\\" + Convert.ToInt32(dateTime.ToString("dd"));
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
            string Month = DateTime.Now.ToString("MMMM", culture);
            string newDirectory = rootLocationPath + "\\" + dateTime.ToString("yyyy") + "\\" + Month + "\\" + Convert.ToInt32(dateTime.ToString("dd"));

            if (!System.IO.Directory.Exists(newDirectory))
            {
                System.IO.Directory.CreateDirectory(newDirectory);
            }

            if (!System.IO.File.Exists(newDirectory + "\\" + fileNameOnly))
            {
                System.IO.File.Copy(tempFile, newDirectory + "\\" + fileNameOnly, false);
                System.IO.File.Delete(tempFile);
            }


            SQLIntroduceDocument(EDSBatchNumber, documentNumber, userName, folderID, rootID, poolNumber, fileNameOnly, documentID,
                                    documentDescription, referenceNumber, referenceType); //, sqlConnection, sqlTransaction);

            this.AssignReference(EDSBatchNumber, referenceNumber, referenceTypeCode);

            return "Success";
        }


        [WebMethod]
        public string SendFile(string userName, string referenceNumber, string referenceTypeCode, int EDSBatchNumber, int documentNumber,
                                int documentID, int folderID, string documentDescription, byte[] fileRawData, string fileExtension)
        {

            int referenceType = ResolveReferenceType(referenceTypeCode);
            string temporaryFileExtractionPath = ConfigurationManager.AppSettings.Get("TemporaryFileExtractionPath");
            int poolNumber = CreatePool(referenceNumber, referenceType);
            int rootID = GetRootID();
            string rootLocationPath = GetRootLocationPath(rootID);
            string fileNameOnly = "B" + EDSBatchNumber.ToString() + "D" + documentNumber.ToString() + fileExtension;
            string fileName = temporaryFileExtractionPath + fileNameOnly;
            System.IO.File.WriteAllBytes(fileName, fileRawData);



            try
            {

                string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
                DateTime dateTime = Convert.ToDateTime(DateTime.Now);

                //2017-05-23 Hannes - FIX:this creates afrikaans month names at times..
                //string newDirectory = rootLocationPath + "\\" + dateTime.ToString("yyyy") + "\\" + dateTime.ToString("MMMM") + "\\" + Convert.ToInt32(dateTime.ToString("dd"));
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                string Month = DateTime.Now.ToString("MMMM", culture);
                string newDirectory = rootLocationPath + "\\" + dateTime.ToString("yyyy") + "\\" + Month + "\\" + Convert.ToInt32(dateTime.ToString("dd"));

                if (!System.IO.Directory.Exists(newDirectory))
                {
                    System.IO.Directory.CreateDirectory(newDirectory);
                }

                if (!System.IO.File.Exists(newDirectory + "\\" + fileNameOnly))
                {
                    System.IO.File.Copy(fileName, newDirectory + "\\" + fileNameOnly, false);

                    //try again -- seems to have issues with this File.Copy()
                    if (!System.IO.File.Exists(newDirectory + "\\" + fileNameOnly))
                    {
                        System.IO.File.WriteAllBytes(newDirectory + "\\" + fileNameOnly, fileRawData);
                    }
                }

                SQLIntroduceDocument(EDSBatchNumber, documentNumber, userName, folderID, rootID, poolNumber, fileNameOnly, documentID,
                                        documentDescription, referenceNumber, referenceType); //, sqlConnection, sqlTransaction);


                //Read/decode barcode for id's
                string fileName2 = temporaryFileExtractionPath + "_" + fileNameOnly;
                try
                {
                    if (fileName2.ToLower().Contains("passport") || fileName2.ToLower().Contains("id/") || documentDescription.ToLower().Contains("passport") || documentDescription.ToLower().Contains("id/"))
                    {
                        System.IO.File.WriteAllBytes(fileName2, fileRawData);
                        //// find barcodes
                        BarcodeDetection.AddLog_KYC("INP", "CheckForBarcode_ID", fileName2, "inp TXT", referenceNumber + " " + documentDescription);
                        if (!File.Exists(fileName2))
                        {
                            BarcodeDetection.AddLog_KYC("ERR", "QIWS.SendFile_CheckForBarcode_ID", "File does not exists - " + fileName2, "Output TXT", "Ref: " + referenceNumber);

                        }
                        else
                        {
                            BarcodeDetection.TryReadBarcode(fileName2, newDirectory, referenceNumber, "", "", 0, EDSBatchNumber, folderID, rootID, poolNumber, documentID, "ID");
                            try { File.Delete(fileName2); }
                            catch { } 
                        }
                    }
                }
                catch (Exception e)
                {   //log the error but carry on..
                    BarcodeDetection.AddLog_KYC("ERR", "QIWS.SendFile_CheckForBarcode_ID", e.Message, "Output TXT", "DocumentIndexingWS File: " + fileName2);
                }


                //Read/decode barcode for drivers licenses
                string fileNameDL = temporaryFileExtractionPath + "dl_" + fileNameOnly;
                try
                {
                    if (fileNameDL.ToLower().Contains("drivers") || documentDescription.ToLower().Contains("drivers"))
                    {
                        System.IO.File.WriteAllBytes(fileNameDL, fileRawData);
                        BarcodeDetection.AddLog_KYC("INP", "QIWS.SendFile_CheckForBarcode_DL", documentDescription, "inp TXT", "" + fileNameDL);

                        //send the image to PBSA to decode the drivers licence barcode
                        thread_PBSA_DriversLicenceDecoding(fileNameDL, referenceNumber, "", "", connectionString, "DL");
                        //try { File.Delete(fileNameDL); }
                        //catch { }   
                    }
                }
                catch (Exception e)
                {   //log the error but carry on..
                    BarcodeDetection.AddLog_KYC("ERR", "QIWS.SendFile_CheckForBarcode_DL", e.Message, "Output TXT", "DocumentIndexingWS File: " + fileNameDL);
                }

                try { File.Delete(fileName); }
                catch { }


            }
            catch (Exception e)
            {
                BarcodeDetection.AddLog_KYC("ERR", "QIWS.ID_SendFile", e.Message, "Output TXT", "DocumentIndexingWS File: " + fileName);
                throw e;
            }

            return "Success";
        }

        //new 2017-01-30 hannes
        [WebMethod]
        public string SendFileWithAttest(string userName, string referenceNumber, string referenceTypeCode, int EDSBatchNumber, int documentNumber,
                               int documentID, int folderID, string documentDescription, byte[] fileRawData, string fileExtension, string AttestString)
        {
            string Ret = "";
            //The web service does not seem to like this threading business..

            // System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            //{
            Ret = thread_SendFileWithAttest(userName, referenceNumber, referenceTypeCode, EDSBatchNumber, documentNumber
                , documentID, folderID, documentDescription, fileRawData, fileExtension, AttestString);
            // }
            // ));
            // thread.Start();

            return Ret;
        }

        public string thread_SendFileWithAttest(string userName, string referenceNumber, string referenceTypeCode, int EDSBatchNumber, int documentNumber,
                        int documentID, int folderID, string documentDescription, byte[] fileRawData, string fileExtension, string AttestString)
        {
            int referenceType = ResolveReferenceType(referenceTypeCode);
            string temporaryFileExtractionPath = ConfigurationManager.AppSettings.Get("TemporaryFileExtractionPath");
            int poolNumber = CreatePool(referenceNumber, referenceType);
            int rootID = GetRootID();
            string rootLocationPath = GetRootLocationPath(rootID);
            string fileNameOnly = "B" + EDSBatchNumber.ToString() + "D" + documentNumber.ToString() + fileExtension;

            string fileName = temporaryFileExtractionPath + fileNameOnly;
            System.IO.File.WriteAllBytes(fileName, fileRawData);

            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
            DateTime dateTime = Convert.ToDateTime(DateTime.Now);

            int imageNumber = 0;
            // FSBarcodeRW.Result[] results = null;


            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
            string Month = DateTime.Now.ToString("MMMM", culture);
            string newDirectory = rootLocationPath + "\\" + dateTime.ToString("yyyy") + "\\" + Month + "\\" + Convert.ToInt32(dateTime.ToString("dd"));

            //Save to Imaging 
            if (!System.IO.Directory.Exists(newDirectory))
            {
                System.IO.Directory.CreateDirectory(newDirectory);
            }
            if (!System.IO.File.Exists(newDirectory + "\\" + fileNameOnly))
            {
                try
                {
                    System.IO.File.Copy(fileName, newDirectory + "\\" + fileNameOnly, false);
                }
                catch { }

                //try again -- seems to have issues with this File.Copy()
                if (!System.IO.File.Exists(newDirectory + "\\" + fileNameOnly))
                {
                    System.IO.File.WriteAllBytes(newDirectory + "\\" + fileNameOnly, fileRawData);
                }
            }

            SQLIntroduceDocument(EDSBatchNumber, documentNumber, userName, folderID, rootID, poolNumber, fileNameOnly, documentID, documentDescription, referenceNumber, referenceType); //, sqlConnection, sqlTransaction);


            //extract the attestation   *****************************************
            string AttestUserID = "";
            string AttestUserName = "";
            DateTime AttestDate = new DateTime();
            string AttestWording = "";
            bool AttestedImage = false;
            try
            {
                if (AttestString.Contains("ATTEST"))
                {
                    AttestedImage = true;
                    string[] arr = AttestString.Split(new char[] { '|' });
                    AttestUserID = arr[1];
                    AttestUserName = arr[2];
                    if (!DateTime.TryParse(arr[3], out AttestDate)) AttestDate = DateTime.Now;
                    AttestWording = arr[4];
                }
            }
            catch (Exception e)
            {//log the error but carry on..
                BarcodeDetection.AddLog_KYC("ERR", "Attest", e.Message, "Output TXT", "DocumentIndexingWS File: " + fileNameOnly);
            }


            //Read/decode barcode for id's  *****************************************
            string BarcodeDetected = "";
            FSBarcodeRW.Result[] results = null;
            string fileName2 = temporaryFileExtractionPath + "_" + fileNameOnly;
            try
            {
                if (fileName2.ToLower().Contains("passport") || fileName2.ToLower().Contains("id/") || documentDescription.ToLower().Contains("passport") || documentDescription.ToLower().Contains("id/"))
                {
                    System.IO.File.WriteAllBytes(fileName2, fileRawData);
                    //// find barcodes
                    BarcodeDetection.AddLog_KYC("INP", "CheckForBarcode_ID", documentDescription, "inp TXT", "" + fileName2);

                    if (!File.Exists(fileName2))
                    {
                        BarcodeDetection.AddLog_KYC("ERR", "QIWS.ID_BarcodeDetection.TryReadBarcode", "File does not exists - " + fileName2, "Output TXT", "Ref: " + referenceNumber);

                    }
                    else
                    {
                        results = BarcodeDetection.TryReadBarcode(fileName2, newDirectory, referenceNumber, AttestUserID, AttestUserName, 0, EDSBatchNumber, folderID, rootID, poolNumber, documentID, "ID");

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
                        try { File.Delete(fileName2); }
                        catch { } 
                    }
                }
            }
            catch (Exception e)
            {   //log the error but carry on..
                BarcodeDetection.AddLog_KYC("ERR", "BarcodeDetection in thread_SendFileWithAttest", e.Message, "Output TXT", "DocumentIndexingWS File: " + fileName2);
            }


            //Read/decode barcode for drivers licenses  *****************************************
            string fileNameDL = temporaryFileExtractionPath + "dl_" + fileNameOnly;
            try
            {
                if (fileNameDL.ToLower().Contains("drivers") || documentDescription.ToLower().Contains("drivers"))
                {
                    System.IO.File.WriteAllBytes(fileNameDL, fileRawData);
                    BarcodeDetection.AddLog_KYC("INP", "CheckForBarcode_DL", documentDescription, "inp TXT", "" + fileNameDL);

                    //send the image to PBSA to decode the drivers licence barcode
                    thread_PBSA_DriversLicenceDecoding(fileNameDL, referenceNumber, AttestUserID, AttestUserName, connectionString, "DL");
                    //try { File.Delete(fileNameDL); }
                    //catch { }   
                }
            }
            catch (Exception e)
            {   //log the error but carry on..
                BarcodeDetection.AddLog_KYC("ERR", "BarcodeDetection in thread_SendFileWithAttest", e.Message, "Output TXT", "DocumentIndexingWS File: " + fileNameDL);
            }


            //Save Attestation info
            if (AttestedImage)
                imageNumber = SQLInsertAttestation(EDSBatchNumber, documentNumber, folderID, rootID, poolNumber, documentID, fileNameOnly, referenceNumber, AttestUserID, AttestUserName, AttestDate, AttestWording, BarcodeDetected);


            // *****************************************
            try { File.Delete(fileName); }
            catch { }

            return "Success";
        }

        //new 2017-04-18 hannes
        // using a thread in order not to slow the rest of the code down
        public void thread_PBSA_DriversLicenceDecoding(string ImagePath, string Reference, string UserID, string UserName, string connectionString, string IDorDL)
        {
            BarcodeDetection.AddLog_KYC("INP", "QIWS.thread_PBSA_DriversLicenceDecoding", "ImagePath: " + ImagePath, "Output TXT", "Ref: " + Reference);
            if (!File.Exists(ImagePath))
            {
                BarcodeDetection.AddLog_KYC("ERR", "QIWS.thread_PBSA_DriversLicenceDecoding", "NOT FOUND ImagePath: " + ImagePath, "Output TXT", "Ref: " + Reference);
                return;
            }

            try
            {
                //if connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
                Bitmap _bmp = (Bitmap)Bitmap.FromFile(ImagePath);
                Image _img = null;
                if (!ImagePath.ToLower().EndsWith(".pdf"))
                {
                    _img = Image.FromFile(ImagePath);
                }

                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                {
                    bool Detected = false;
                    // try find barcode on image before sending to PBSA for decoding
                    try
                    {
                        using (var bitmap = _bmp)
                        {
                            System.Collections.Generic.List<FSBarcodeRW.BarcodeFormat> formats = new System.Collections.Generic.List<FSBarcodeRW.BarcodeFormat>();
                            formats.Add(FSBarcodeRW.BarcodeFormat.PDF_417);

                            FSBarcodeRW.BarcodeReader barcodeReader = new FSBarcodeRW.BarcodeReader();
                            barcodeReader.AutoRotate = true;
                            //hannes 20190227--barcodeReader.TryInverted = true;
                            barcodeReader.TryInverted = false;

                            FSBarcodeRW.Common.DecodingOptions options = new FSBarcodeRW.Common.DecodingOptions();
                            //hannes 20190227--options.TryHarder = true;
                            options.TryHarder = false;
                            options.PossibleFormats = formats;
                            barcodeReader.Options = options;

                            FSBarcodeRW.Result result = barcodeReader.Decode(bitmap);
                            if (result != null)
                            {
                                Detected = true;
                                //Detected = result.BarcodeDetected;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        BarcodeDetection.AddLog_KYC("ERR", "QIWS.thread_PBSA_DriversLicenceDecoding_2", e.Message + " - " + e.StackTrace, "Output TXT", "");
                    }

                    if (!Detected)
                    {
                        BarcodeDetection.AddLog_KYC("OUT", "QIWS.thread_PBSA_DriversLicenceDecoding", "Barcode not detected: " + ImagePath, "Output TXT", "Ref: " + Reference);
                    }
                    else
                    {
                        BarcodeDetection.AddLog_KYC("INP", "QIWS.thread_PBSA_DriversLicenceDecoding", "Barcode detected: " + ImagePath, "Output TXT", "Ref: " + Reference);
                        // this code is going to be executed in a separate thread
                        // place additional code that you need to be executed on a separate thread in here            
                        using (PBSA_DriversLicenceDecoding.PBSA_DL_handler pbsa = new PBSA_DriversLicenceDecoding.PBSA_DL_handler())
                        {
                            //   string path = @"C:\Images\IMG_20170405_0002.pdf"; //pdf good
                            Image img = null;
                            BarcodeDetection.AddLog_KYC("Input", "QIWS.thread_PBSA_DriversLicenceDecoding", "Start with conn: " + connectionString, "Output TXT", "Ref: " + Reference);

                            pbsa.ConnectionString = connectionString;
                            pbsa.Reference = Reference;
                            pbsa.FIUserID = UserID;
                            pbsa.FIName = UserName;
                            if (ImagePath.ToLower().EndsWith(".pdf"))
                                pbsa.ImagePath = ImagePath;
                            else
                            {
                                img = _img;
                                pbsa.image = img;
                            }

                            string pdf = "";
                            //pdf = pbsa.Start();

                            //in case the called dll does not have permissions, we can add the file to imaging here
                            if (File.Exists(pdf))
                            {
                                BarcodeDetection.AddToImaging("KYCDecodeDLBarcode", pdf, Reference);
                                BarcodeDetection.AddLog_KYC("INF", "QIWS.thread_PBSA_DriversLicenceDecoding", "File added to imaging from WS :" + pdf, "Output TXT", "ImagePath: " + ImagePath);
                            }
                        }
                    }
                    try { File.Delete(ImagePath); }
                    catch { }

                }
                ));

                thread.Start();
            }
            catch (Exception e)
            {
                BarcodeDetection.AddLog_KYC("ERR", "QIWS.thread_PBSA_DriversLicenceDecoding", e.Message, "Output TXT", "ImagePath: " + ImagePath);
            }
        }

        //new 2017-01-30 hannes
        private int SQLInsertAttestation(int batchNumber, int docNumber, int sourceFolder, int rootID, int poolNumber
            , int documentID, string fileName, string referenceNumber
            , string AttestUserID, string AttestUserName, DateTime AttestDate, string AttestWording
            , string BarcodeDetected)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
            DataSet dataSet = new DataSet();
            SqlDataAdapter sqlDataAdapter;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlParameter[] sqlParameters = new SqlParameter[13];
                sqlParameters[0] = new SqlParameter("@BatchNumber", batchNumber);
                sqlParameters[1] = new SqlParameter("@DocNumber", docNumber);
                sqlParameters[2] = new SqlParameter("@SourceFolder", sourceFolder);
                sqlParameters[3] = new SqlParameter("@RootID", rootID);
                sqlParameters[4] = new SqlParameter("@PoolNumber", poolNumber);
                sqlParameters[5] = new SqlParameter("@DocumentID", documentID);
                sqlParameters[6] = new SqlParameter("@ReferenceNumber", referenceNumber);
                sqlParameters[7] = new SqlParameter("@FileNameOnly", fileName);

                sqlParameters[8] = new SqlParameter("@AttestDate", AttestDate);
                sqlParameters[9] = new SqlParameter("@AttestUserID", AttestUserID);
                sqlParameters[10] = new SqlParameter("@AttestUserName", AttestUserName);
                sqlParameters[11] = new SqlParameter("@AttestWording", AttestWording);
                sqlParameters[12] = new SqlParameter("@BarcodeDetected", BarcodeDetected);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = "spKYC_InsertAttestation";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(sqlParameters);

                    sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataSet);
                }
            }

            int ImageNumber = 0;
            if (dataSet.Tables.Count == 0)
                return ImageNumber;
            else
            {
                foreach (DataRow dr in dataSet.Tables[0].Rows)
                {
                    ImageNumber = (int)dr["ImageNumber"];
                    break;
                }
                return ImageNumber;
            }
        }

        private int ResolveReferenceType(string referenceTypeCode)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
            DataSet dataSet = new DataSet();
            SqlDataAdapter sqlDataAdapter;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlParameter[] prms = new SqlParameter[1];

                prms[0] = new SqlParameter();
                prms[0].ParameterName = "@ReferenceTypeCode";
                prms[0].Value = referenceTypeCode;

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = "spResolveQIReferenceType";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(prms);

                    sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataSet);
                }
            }

            return Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
        }

        private int CreatePool(string referenceNumber, int referenceType)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
            DataSet dataSet = new DataSet();
            SqlDataAdapter sqlDataAdapter;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlParameter[] prms = new SqlParameter[2];

                prms[0] = new SqlParameter();
                prms[0].ParameterName = "@Reference";
                prms[0].Value = referenceNumber;

                prms[1] = new SqlParameter();
                prms[1].ParameterName = "@RefType";
                prms[1].Value = referenceType;

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = "SPCreatePool";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(prms);

                    sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataSet);
                }
            }

            return Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
        }

        private int GetRootID()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
            DataSet dataSet = new DataSet();
            SqlDataAdapter sqlDataAdapter;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlParameter[] prms = new SqlParameter[3];

                prms[0] = new SqlParameter();
                prms[0].ParameterName = "@GroupName";
                prms[0].Value = "System Parameters";

                prms[1] = new SqlParameter();
                prms[1].ParameterName = "@Section";
                prms[1].Value = "Default";

                prms[2] = new SqlParameter();
                prms[2].ParameterName = "@KeyName";
                prms[2].Value = "RootID";

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = "spGetSystemIniValue";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(prms);

                    sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataSet);
                }
            }

            return Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
        }

        private string GetRootLocationPath(int rootID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
            DataSet dataSet = new DataSet();
            SqlDataAdapter sqlDataAdapter;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlParameter[] prms = new SqlParameter[1];

                prms[0] = new SqlParameter();
                prms[0].ParameterName = "@RootID";
                prms[0].Value = rootID;

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = "spGetRootLocationPath";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(prms);

                    sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataSet);
                }
            }

            return Convert.ToString(dataSet.Tables[0].Rows[0][0]);
        }


        private DateTime SQLIntroduceDocument(int batchNumber, int docNumber, string recievedBy, int sourceFolder, int rootID, int poolNumber, string fileName
                        , int documentID, string documentDescription, string referenceNumber, int referenceType)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
            DataSet dataSet = new DataSet();
            SqlDataAdapter sqlDataAdapter;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlParameter[] sqlParameters = new SqlParameter[12];
                sqlParameters[0] = new SqlParameter("@BatchNumber", batchNumber);
                sqlParameters[1] = new SqlParameter("@DocNumber", docNumber);
                sqlParameters[2] = new SqlParameter("@RecievedBy", recievedBy);
                sqlParameters[3] = new SqlParameter("@SourceFolder", sourceFolder);
                sqlParameters[4] = new SqlParameter("@RooTID", rootID);
                sqlParameters[5] = new SqlParameter("@PoolNumber", poolNumber);
                sqlParameters[6] = new SqlParameter("@FileName", fileName);
                sqlParameters[7] = new SqlParameter("@DocumentID", documentID);
                sqlParameters[8] = new SqlParameter("@DocumentDescr", documentDescription);
                sqlParameters[9] = new SqlParameter("@ReferenceNumber", referenceNumber);
                sqlParameters[10] = new SqlParameter("@ReferenceType", referenceType);
                sqlParameters[11] = new SqlParameter("@SendTime", DateTime.Now);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = "SPIntroduceDocument_New";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(sqlParameters);

                    sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataSet);
                }
            }

            return Convert.ToDateTime(dataSet.Tables[0].Rows[0][0]);
        }

        [WebMethod]
        public void SendDocumentConfirmationEmail(string referenceNumber, string referenceTypeCode, string recipientAddress, int batchNumber)
        {
            int referenceType = ResolveReferenceType(referenceTypeCode);

            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SPSendDocumentConfirmationEmail";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@ReferenceNumber", referenceNumber));
                    cmd.Parameters.Add(new SqlParameter("@ReferenceTypeCode", referenceType));
                    cmd.Parameters.Add(new SqlParameter("@SendBy", recipientAddress));
                    cmd.Parameters.Add(new SqlParameter("@BatchNumber", batchNumber));

                    cmd.Connection = cn;

                    cmd.ExecuteNonQuery();
                }
            }
        }


        [WebMethod]
        public void UpdateDocumentEmail(string emailID, int emailNr, int EDSBatchNumber, char updateReceivedTimeYN, char updateCompleteTimeYN)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "spUpdateDocumentEmail";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@EmailID", emailID));
                    cmd.Parameters.Add(new SqlParameter("@EmailNr", emailNr));
                    cmd.Parameters.Add(new SqlParameter("@EDSBatchNumber", EDSBatchNumber));
                    cmd.Parameters.Add(new SqlParameter("@UpdateReceivedTimeYN", updateReceivedTimeYN));
                    cmd.Parameters.Add(new SqlParameter("@UpdateCompleteTimeYN", updateCompleteTimeYN));

                    cmd.Connection = cn;

                    cmd.ExecuteNonQuery();
                }
            }
        }


        [WebMethod]
        public void InsertQILogEntry(string emailGUID, int emailNr, int EDSBatchNumber, string userName, string applicationVersion, string logType, string message, string additionalDetails)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "spInsertQILogEntry";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@EmailGUID", emailGUID));
                    cmd.Parameters.Add(new SqlParameter("@EmailNr", emailNr));
                    cmd.Parameters.Add(new SqlParameter("@EDSBatchNumber", EDSBatchNumber));
                    cmd.Parameters.Add(new SqlParameter("@UserName", userName));
                    cmd.Parameters.Add(new SqlParameter("@ApplicationVersion", applicationVersion));
                    cmd.Parameters.Add(new SqlParameter("@LogType", logType));
                    cmd.Parameters.Add(new SqlParameter("@Message", message));
                    cmd.Parameters.Add(new SqlParameter("@AdditionalDetails", additionalDetails));

                    cmd.Connection = cn;

                    cmd.ExecuteNonQuery();
                }
            }
        }


        [WebMethod]
        public void AssignReference(int batchNumber, string referenceNumber, string referenceTypeCode)
        {
            int referenceType = ResolveReferenceType(referenceTypeCode);
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SPAssignReference";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@BatchNumber", batchNumber));
                    cmd.Parameters.Add(new SqlParameter("@ReferenceNumber", referenceNumber));
                    cmd.Parameters.Add(new SqlParameter("@ReferenceType", referenceType.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@NotifySomeone", 'Y'));

                    cmd.Connection = cn;

                    cmd.ExecuteNonQuery();
                }
            }
        }


        [WebMethod]
        public AuthenticateResult Authenticate_User(string DFE_UserID, string password)
        {
            AuthenticateResult result = new AuthenticateResult();
            Guid newGuid = Guid.NewGuid();
            DFE_UserID = SecurityUtil.SimpleDecrypt(DFE_UserID);
            password = SecurityUtil.SimpleDecrypt(password);

            string shortPassword = string.Empty;
            if (password.Length > 8)
            {
                shortPassword = password.Substring(0, 8);
            }

            DataSet userDetailsDataSet = Get_DFE_User_Details(DFE_UserID);
            if (userDetailsDataSet.Tables[0].Rows.Count == 0)
            {
                result.AuthenticateSuccess = false;
                result.ErrorMessage = "Dealer Point User ID not found or inactive or password expired.";
                return result;
            }

            string decryptedPassword = DecryptPassword(userDetailsDataSet.Tables[0].Rows[0]["PASSWORD"].ToString());
            if (decryptedPassword != password && decryptedPassword != shortPassword)
            {
                result.AuthenticateSuccess = false;
                result.ErrorMessage = "Password incorrect!";
                return result;
            }

            result.FIRST_NAME = userDetailsDataSet.Tables[0].Rows[0]["FIRST_NAME"].ToString();
            result.LAST_NAME = userDetailsDataSet.Tables[0].Rows[0]["LAST_NAME"].ToString();
            //2017-10-09 Hannes Malan (needed for web chat)
            result.DEALER = userDetailsDataSet.Tables[0].Rows[0]["DEALER"].ToString();
            result.BRAND = userDetailsDataSet.Tables[0].Rows[0]["BRAND"].ToString();

            result.AuthenticateSuccess = true;
            result.Token = newGuid;
            InsertQILoginEntry(DFE_UserID, result.Token);

            return result;
        }

        [WebMethod]
        public void UpdateWorkflowPriorityLevel(string referenceNumber, string referenceTypeCode)
        {
            int referenceType = ResolveReferenceType(referenceTypeCode);
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SPUpdateWorkflowPriorityLevel";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@ReferenceNumber", referenceNumber));
                    cmd.Parameters.Add(new SqlParameter("@ReferenceType", referenceType.ToString()));

                    cmd.Connection = cn;

                    cmd.ExecuteNonQuery();
                }
            }
        }


        private DataSet Get_DFE_User_Details(string userID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
            DataSet dataSet = new DataSet();
            SqlDataAdapter sqlDataAdapter;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@User_ID", userID);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = "spQI_Get_DFE_User_Details";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(sqlParameters);

                    sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataSet);
                }
            }

            return dataSet;
        }

        private string DecryptPassword(string encryptedPassword)
        {
            string privateKey = System.IO.File.ReadAllText(System.Configuration.ConfigurationManager.AppSettings["PrivateKeyPath"].ToString());

            if (string.IsNullOrEmpty(privateKey.Trim()))
            {
                throw new Exception("Private key not found/empty");
            }

            SecurityUtil securityUtil = new SecurityUtil(privateKey.Trim());
            return securityUtil.DecryptData(encryptedPassword.Trim());
        }


        private void InsertQILoginEntry(string DFE_UserID, Guid token)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "spQI_Insert_Login";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@DFE_UserID", DFE_UserID));
                    cmd.Parameters.Add(new SqlParameter("@Token", token));

                    cmd.Connection = cn;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        [WebMethod]
        public byte[] GetApps(Guid token, DateTime? startDate, DateTime? endDate, int dfeAppNum, string dealRef, int regionID, int branchCode, string FI_Uid)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[8];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@Token";
                    prms[0].Value = token;

                    prms[1] = new SqlParameter();
                    prms[1].ParameterName = "@StartDate";
                    if (startDate == null)
                    {
                        prms[1].Value = DBNull.Value;
                    }
                    else
                    {
                        prms[1].Value = startDate;
                    }

                    prms[2] = new SqlParameter();
                    prms[2].ParameterName = "@EndDate";
                    if (endDate == null)
                    {
                        prms[2].Value = DBNull.Value;
                    }
                    else
                    {
                        prms[2].Value = endDate;
                    }

                    prms[3] = new SqlParameter();
                    prms[3].ParameterName = "@DFEAppNum";
                    if (dfeAppNum == 0)
                    {
                        prms[3].Value = DBNull.Value;
                    }
                    else
                    {
                        prms[3].Value = dfeAppNum;
                    }

                    prms[4] = new SqlParameter();
                    prms[4].ParameterName = "@DealRef";
                    if (dealRef == null)
                    {
                        prms[4].Value = DBNull.Value;
                    }
                    else
                    {
                        prms[4].Value = dealRef;
                    }

                    prms[5] = new SqlParameter();
                    prms[5].ParameterName = "@RegionID";
                    if (regionID == 0)
                    {
                        prms[5].Value = DBNull.Value;
                    }
                    else
                    {
                        prms[5].Value = regionID;
                    }

                    prms[6] = new SqlParameter();
                    prms[6].ParameterName = "@BranchCode";
                    if (branchCode == 0)
                    {
                        prms[6].Value = DBNull.Value;
                    }
                    else
                    {
                        prms[6].Value = branchCode;
                    }

                    prms[7] = new SqlParameter();
                    prms[7].ParameterName = "@FI_Uid";
                    if (FI_Uid == null)
                    {
                        prms[7].Value = DBNull.Value;
                    }
                    else
                    {
                        prms[7].Value = FI_Uid;
                    }


                    using (SqlCommand comm = new SqlCommand("SET ARITHABORT ON", conn))
                    {
                        comm.ExecuteNonQuery();
                    }

                    using (SqlCommand command = new SqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandText = "spQI_GetApps";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);

                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }

        [WebMethod]
        public byte[] GetRequiredDocs(Guid token, string refNo, string referenceDescription)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[3];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@Token";
                    prms[0].Value = token;

                    prms[1] = new SqlParameter();
                    prms[1].ParameterName = "@RefNo";
                    prms[1].Value = refNo;

                    prms[2] = new SqlParameter();
                    prms[2].ParameterName = "@ReferenceDescription";
                    prms[2].Value = referenceDescription;

                    using (SqlCommand command = new SqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandText = "spQI_GetRequiredDocs";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);

                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }


        [WebMethod]
        public byte[] GetAvailableImages(Guid token, string refNo, string referenceDescription)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[3];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@Token";
                    prms[0].Value = token;

                    prms[1] = new SqlParameter();
                    prms[1].ParameterName = "@RefNo";
                    prms[1].Value = refNo;

                    prms[2] = new SqlParameter();
                    prms[2].ParameterName = "@ReferenceDescription";
                    prms[2].Value = referenceDescription;

                    using (SqlCommand command = new SqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandText = "spQI_GetAvailableImages";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);

                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }


        [WebMethod]
        public byte[] GetImageFromFileLocation(string fileLocation)
        {
            return File.ReadAllBytes(SecurityUtil.SimpleDecrypt(fileLocation));
        }


        #region Webchat
        [WebMethod]
        public bool WebchatCreditEnabled()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings.Get("WebchatCreditEnabled"));
        }
        [WebMethod]
        public string WebchatCreditLink()
        {
            return ConfigurationManager.AppSettings.Get("WebchatCreditLink");
        }


        [WebMethod]
        public bool WebchatDealerSupportEnabled()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings.Get("WebchatDealerSupportEnabled"));
        }
        [WebMethod]
        public string WebchatDealerSupportLink()
        {
            return ConfigurationManager.AppSettings.Get("WebchatDealerSupportLink");
        }

        [WebMethod]
        public bool WebchatPayoutSupportEnabled()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings.Get("WebchatPayoutsEnabled"));
        }
        [WebMethod]
        public string WebchatPayoutSupportLink()
        {
            return ConfigurationManager.AppSettings.Get("WebchatPayoutsSupportLink");
        }
        #endregion

        #region AppViewMinMonths
        [WebMethod]
        public int AppViewMinMonths()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings.Get("AppViewMinMonths"));
        }
        #endregion


        #region JpegQualityFactor
        [WebMethod]
        public int JpegQualityFactor()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings.Get("JpegQualityFactor"));
        }
        #endregion


        #region MaxPixelSizeColour
        [WebMethod]
        public int MaxPixelSizeColour()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings.Get("MaxPixelSizeColour"));
        }
        #endregion


        #region MaxPixelSizeBlackWhite
        [WebMethod]
        public int MaxPixelSizeBlackWhite()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings.Get("MaxPixelSizeBlackWhite"));
        }
        #endregion


        [WebMethod]
        public byte[] GetAppFilter()
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            MemoryStream memoryStream = new MemoryStream();

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandText = "spQI_GetAppFilter";
                        command.CommandType = CommandType.StoredProcedure;

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);

                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }

        [WebMethod]
        public byte[] GetRMList()
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            MemoryStream memoryStream = new MemoryStream();

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandText = "spQI_Get_RM_List";
                        command.CommandType = CommandType.StoredProcedure;

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);

                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }


        [WebMethod]
        public byte[] GetBranchList(int regionID)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[1];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@RegionID";
                    prms[0].Value = regionID;

                    using (SqlCommand command = new SqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandText = "spRPT_Get_Branch_List";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);

                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }


        [WebMethod]
        public byte[] GetFIList(int branchCode)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[1];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@Branch";
                    prms[0].Value = branchCode;

                    using (SqlCommand command = new SqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandText = "spQI_Get_FI_List";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);

                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }


        [WebMethod]
        public bool IsRegionalManager(string userLoggedIn)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            SqlParameter[] prms;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[1];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@User_ID";
                    prms[0].Value = userLoggedIn;

                    using (SqlCommand command = new SqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandText = "spQI_IsUserRegionalManagerYN";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                if (ds.Tables[0].Rows[0]["IsRmManager"].ToString().ToUpper() == "Y")
                    return true;
                else
                    return false;
            }
            catch
            {
                throw;
            }
        }

        [WebMethod]
        public byte[] GetAppData(Guid token, int DFEAppNum)
        {
            string connectionString = string.Empty;
            DataSet ds = new DataSet();
            SqlDataAdapter sqlAdapt;
            MemoryStream memoryStream = new MemoryStream();
            SqlParameter[] prms;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["CATConn"].ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    prms = new SqlParameter[2];

                    prms[0] = new SqlParameter();
                    prms[0].ParameterName = "@Token";
                    prms[0].Value = token;

                    prms[1] = new SqlParameter();
                    prms[1].ParameterName = "@DFEAppNum";
                    prms[1].Value = DFEAppNum;

                    using (SqlCommand command = new SqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandText = "spQI_GetAppData";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(prms);

                        sqlAdapt = new SqlDataAdapter(command);
                        sqlAdapt.Fill(ds);
                    }
                }

                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
                serializer.Serialize(memoryStream, ds);

                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Flush();
                memoryStream.Close();
            }
        }

        #region MR437_Contract Maturity F & I Report

        // Mcebisi Thomas
        // 10/10/2018

        [WebMethod]
        public bool AddReportSelection(int ReportNum, string Section, string strValue)
        {
            bool retValue = false;

            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
            SqlConnection strConnectionString = new SqlConnection(connectionString);

            try
            {
                using (strConnectionString)
                {
                    if (strConnectionString.State.ToString() == "Closed")
                        strConnectionString.Open();

                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.CommandText = "Globalini.dbo.SPAddReportSelection_Net";
                        sqlCommand.Connection = strConnectionString;
                        sqlCommand.CommandTimeout = 0;

                        // Parameters
                        sqlCommand.Parameters.AddWithValue("@ReportNum", ReportNum);
                        sqlCommand.Parameters.AddWithValue("@Section", Section);
                        sqlCommand.Parameters.AddWithValue("@Value", strValue);

                        sqlCommand.ExecuteNonQuery();
                        retValue = true;
                    }//End using
                }//End using

                return retValue;
            }
            catch
            {
                if (strConnectionString.State.ToString() == "Open")
                    strConnectionString.Close();
                throw;
            }
            finally
            {
                if (strConnectionString != null)
                    strConnectionString.Close();
                strConnectionString.Dispose();
            }
        }//End 

        [WebMethod]
        public DataSet GetRetentionUpAndComingEOT(int reportNum, bool isQuickIndex)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
            DataSet dataSet = new DataSet();
            SqlDataAdapter sqlDataAdapter;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@ReportNum", reportNum);
                sqlParameters[1] = new SqlParameter("@isQuickIndex", isQuickIndex);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = "LiveData.dbo.SPRPT_RetentionUpAndComingEOT_Net";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(sqlParameters);

                    sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataSet);
                }
            }

            return dataSet;
        }

        [WebMethod]
        public bool GetToken(Guid token)
        {
            bool retValue = false;

            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
            SqlConnection strConnectionString = new SqlConnection(connectionString);

            try
            {
                using (strConnectionString)
                {
                    if (strConnectionString.State.ToString() == "Closed")
                        strConnectionString.Open();

                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.CommandText = "Documents.dbo.spQI_GetToken";
                        sqlCommand.Connection = strConnectionString;
                        sqlCommand.CommandTimeout = 0;

                        // Parameters
                        sqlCommand.Parameters.AddWithValue("@Token ", token);

                        retValue = Convert.ToBoolean(sqlCommand.ExecuteScalar());
                    }//End using
                }//End using

                return retValue;
            }
            catch
            {
                if (strConnectionString.State.ToString() == "Open")
                    strConnectionString.Close();

                throw;
            }
            finally
            {
                if (strConnectionString != null)
                    strConnectionString.Close();
                strConnectionString.Dispose();
            }
        }//End 

        [WebMethod]
        public bool GetContractMaturityUser(string userID)
        {
            bool retValue = false;

            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();
            SqlConnection strConnectionString = new SqlConnection(connectionString);

            try
            {
                using (strConnectionString)
                {
                    if (strConnectionString.State.ToString() == "Closed")
                        strConnectionString.Open();

                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.CommandText = "Documents.dbo.spQI_GetContractMaturityUser";
                        sqlCommand.Connection = strConnectionString;
                        sqlCommand.CommandTimeout = 0;

                        // Parameters
                        sqlCommand.Parameters.AddWithValue("@USER_ID ", userID);

                        retValue = Convert.ToBoolean(sqlCommand.ExecuteScalar());
                    }//End using
                }//End using

                return retValue;
            }
            catch
            {
                if (strConnectionString.State.ToString() == "Open")
                    strConnectionString.Close();

                throw;
            }
            finally
            {
                if (strConnectionString != null)
                    strConnectionString.Close();
                strConnectionString.Dispose();
            }
        }//End 

        [WebMethod]
        public int GetNextBatchNumber_Net()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocumentConn"].ToString();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "LiveData..GetNextNum_Net";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@NumberType", "RPT"));

                    cmd.Connection = cn;

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        #endregion
    }
}
