using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;

using Leadtools;
using Leadtools.Barcode;
using Leadtools.Drawing;
using Leadtools.WinForms;
using Leadtools.Codecs;
using Leadtools.ImageProcessing;

namespace BMW.FS.Indexing.WebServices
{
    public class BarcodeReadHandler
    {
        string _ImageFilename;
        public string ImageFilename
        {
            get
            {
                return this._ImageFilename;
            }
            set
            {
                this._ImageFilename = value;
                LoadFile();
            }
        }
        private LeadRect _readArea;
        private BarcodeUnit _unit;
        private int _readMaxBarcodesCount = 1;
        private bool _useColor;
        private bool _useRegion;
        private Barcode1d _barcodeRead1d;
        private BarcodeReadPdf _barcodeReadPDF;
        private BarcodeColor _barcodeReadColor;
        private BarcodeReadFlags _barcodeReadFlags;

        private bool bSearchAllStd1D;
        private bool bSearchAllNoRSS;
        private BarcodeSearchTypeFlags ulSearchStd1DType;
        private BarcodeSearchTypeFlags _readBarcodeTypes;
        private Barcode1d StdBar1D;
        private BarcodeReadFlags uFlags_Std1DPg;
        private bool _cbReturnCheckDigitS1D = false;
        private bool _cbAvoidBlock = false;
        private bool _cbPartialRead = false;

        BarcodeEngine _barcodeEngine;
        RasterImageViewer _viewer = new RasterImageViewer();

        public BarcodeReadHandler()
        {
        }

        public BarcodeReadHandler(string Filename)
        {
            ImageFilename = Filename;

        }

        public void LoadFile()
        {
            using (var codecs = new RasterCodecs())
            {
                codecs.Options.Load.Format = RasterImageFormat.Tif;
                _viewer.Image = codecs.Load(ImageFilename);
            }
        }

        public string ReadBarcode()
        {
            string bcode = "";

            bool result = ReadIDBarcode(BarcodeDirectionFlags.Horizontal, ref bcode);
            if (!result)
                ReadIDBarcode(BarcodeDirectionFlags.Vertical, ref bcode);

            return bcode;
        }

        private bool ReadIDBarcode(BarcodeDirectionFlags direction, ref string Barcode)
        {
            //Validate General
            _readMaxBarcodesCount = 1;
            _unit = (BarcodeUnit.Millimeters);

            //Validate Location
            _readArea = new LeadRect(0, 0, 0, 0);
            _useRegion = false;

            //Validate Color
            _useColor = false;

            _barcodeReadColor = new BarcodeColor();
            _barcodeReadColor.BarColor = FromGdiPlusColor(Color.Black);
            _barcodeReadColor.SpaceColor = FromGdiPlusColor(Color.White);

            //Validate All Tab controls
            SetToReadStandard1DControls(direction);

            _barcodeReadFlags = 0;
            if (_useColor)
            {
                _barcodeReadFlags |= BarcodeReadFlags.UseColors;
            }


            if (bSearchAllStd1D)
                _readBarcodeTypes = BarcodeSearchTypeFlags.Barcode1dReadAnyType;
            else if (bSearchAllNoRSS)
                _readBarcodeTypes = BarcodeSearchTypeFlags.Barcode1dReadAnyTypeNoRss14;
            else
                _readBarcodeTypes = ulSearchStd1DType;

            _barcodeReadFlags |= uFlags_Std1DPg;
            _barcodeRead1d = StdBar1D;
            _barcodeReadPDF = new BarcodeReadPdf();


            try
            {
                LeadRect area = LeadRect.Empty;
                if (_useRegion)
                {
                    area = LeadRect.Empty;
                }
                else
                {
                    if (_readArea == LeadRect.Empty)
                    {
                        area = new LeadRect(0, 0, _viewer.Image.Width, _viewer.Image.Height);
                    }
                    else
                    {
                        area = _readArea;
                    }
                }
                RasterCollection<BarcodeData> barcodeData = new RasterCollection<BarcodeData>();
                _barcodeEngine = new BarcodeEngine();

                barcodeData = _barcodeEngine.Read(_viewer.Image,
                                    area,
                                    _readBarcodeTypes,
                                    _unit,
                                    _barcodeReadFlags,
                                    _readMaxBarcodesCount,
                                    _barcodeRead1d,
                                    _barcodeReadPDF,
                                    _barcodeReadColor);

                //string msg;
                //msg = string.Format("Total Bar Code Symbols Found is: {0}", barcodeData.Count);

                RasterCollection<BarcodeData> _barcodeDataCollection;
                _barcodeDataCollection = barcodeData;
                BarcodeData data = (BarcodeData)_barcodeDataCollection[0];

                // Extract the barcode data string
                byte[] screenData = new byte[data.Data.Length];
                for (int dataIndex = 0, screenIndex = 0; dataIndex < data.Data.Length; dataIndex++)
                {
                    if (data.Data[dataIndex] != 0)
                    {
                        //if 3 then change to 32(space)
                        screenData[screenIndex] = data.Data[dataIndex] < (byte)32 ? (byte)32 : data.Data[dataIndex];
                        screenIndex++;
                    }
                }
                string dataString = BarcodeData.ConvertToStringArray(screenData)[0];
                Barcode = dataString;

            }
            catch (BarcodeException ex)
            {
                if (ex.Code == BarcodeExceptionCode.NotFound)
                {
                    Barcode = "barcode not found";
                    return false;
                }
                else
                    throw;   // MessageBox.Show(ex.Message);
            }
            catch //(Exception ex)
            {
                throw;//MessageBox.Show(ex.Message);
            }
            return true;
        }

        private int SetToReadStandard1DControls(BarcodeDirectionFlags direction)
        {
            bSearchAllStd1D = true;
            bSearchAllNoRSS = false;
            //Sub types
            ulSearchStd1DType = 0;

            //Set MSI types
            StdBar1D = new Barcode1d();
            StdBar1D.StandardFlags &= (Barcode1dStandardFlags)0xFFFFFFF0;
            StdBar1D.StandardFlags = Barcode1dStandardFlags.Barcode1dMsiModulo10;//--------->
            //Set Code11 types
            StdBar1D.StandardFlags &= (Barcode1dStandardFlags)0xFFFFFF0F;
            StdBar1D.StandardFlags |= Barcode1dStandardFlags.Barcode1dCode11C;//--------->

            //Direction
            //  StdBar1D.Direction |= BarcodeDirectionFlags.Horizontal;//--------->
            StdBar1D.Direction |= direction;//--------->

            //Speed
            StdBar1D.StandardFlags &= (Barcode1dStandardFlags)0xFFFFF0FF;
            //StdBar1D.StandardFlags |= Barcode1dStandardFlags.Barcode1dFast;//--------->
            StdBar1D.StandardFlags |= Barcode1dStandardFlags.Barcode1dNormal;

            //Options:Granularity
            // Gets or sets the number of scanned lines per column to skip when reading a barcode.
            // Number of scanned lines per column to skip when reading a barcode. 
            // The default value is 9, this means that each tenth line will be scanned while looking for a barcode.
            // Scanning every line will slow the search process while skipping too many lines may skip over the barcode.
            StdBar1D.Granularity = 6;

            //Options:MinimumLength
            // Gets or set the minimum length of a barcode string when searching for a non-fixed length barcode
            StdBar1D.MinimumLength = 12;

            //Options:MaximumLength
            // Gets or set the maximum length of a barcode string when searching for a non-fixed length barcode
            StdBar1D.MaximumLength = 13;

            //Options:WhiteLines
            // Gets or sets the number of lines of white space above and below the barcode symbol.
            StdBar1D.WhiteLines = 1; //was 10 in demo

            StdBar1D.ErrorCheck = false;

            uFlags_Std1DPg = 0;
            if (_cbReturnCheckDigitS1D)
            {
                uFlags_Std1DPg |= BarcodeReadFlags.ReturnCheck;
            }
            if (_cbAvoidBlock)
            {
                uFlags_Std1DPg |= BarcodeReadFlags.BlockSearch;
            }
            if (_cbPartialRead)
            {
                uFlags_Std1DPg |= BarcodeReadFlags.Markers;
            }
            return 1;
        }


        private static void Unlock(bool check)
        {
            RasterSupport.Unlock(RasterSupportType.Abc, "");
            RasterSupport.Unlock(RasterSupportType.AbicRead, "");
            RasterSupport.Unlock(RasterSupportType.AbicSave, "");
            RasterSupport.Unlock(RasterSupportType.Barcodes1D, "");
            RasterSupport.Unlock(RasterSupportType.Barcodes1DSilver, "");
            RasterSupport.Unlock(RasterSupportType.BarcodesDataMatrixRead, "");
            RasterSupport.Unlock(RasterSupportType.BarcodesDataMatrixWrite, "");
            RasterSupport.Unlock(RasterSupportType.BarcodesPdfRead, "");
            RasterSupport.Unlock(RasterSupportType.BarcodesPdfWrite, "");
            RasterSupport.Unlock(RasterSupportType.BarcodesQRRead, "");
            RasterSupport.Unlock(RasterSupportType.BarcodesQRWrite, "");
            RasterSupport.Unlock(RasterSupportType.Bitonal, "");
            RasterSupport.Unlock(RasterSupportType.Ccow, "");
            RasterSupport.Unlock(RasterSupportType.Cmw, "");
            RasterSupport.Unlock(RasterSupportType.Dicom, "");
            RasterSupport.Unlock(RasterSupportType.Document, "");
            RasterSupport.Unlock(RasterSupportType.DocumentWriters, "");
            RasterSupport.Unlock(RasterSupportType.DocumentWritersPdf, "");
            RasterSupport.Unlock(RasterSupportType.ExtGray, "");
            RasterSupport.Unlock(RasterSupportType.Forms, "");
            RasterSupport.Unlock(RasterSupportType.IcrPlus, "");
            RasterSupport.Unlock(RasterSupportType.IcrProfessional, "");
            RasterSupport.Unlock(RasterSupportType.J2k, "");
            RasterSupport.Unlock(RasterSupportType.Jbig2, "");
            RasterSupport.Unlock(RasterSupportType.Jpip, "");
            RasterSupport.Unlock(RasterSupportType.Pro, "");
            RasterSupport.Unlock(RasterSupportType.LeadOmr, "");
            RasterSupport.Unlock(RasterSupportType.MediaWriter, "");
            RasterSupport.Unlock(RasterSupportType.Medical, "");
            RasterSupport.Unlock(RasterSupportType.Medical3d, "");
            RasterSupport.Unlock(RasterSupportType.MedicalNet, "");
            RasterSupport.Unlock(RasterSupportType.Mobile, "");
            RasterSupport.Unlock(RasterSupportType.Nitf, "");
            RasterSupport.Unlock(RasterSupportType.OcrAdvantage, "");
            RasterSupport.Unlock(RasterSupportType.OcrAdvantagePdfLeadOutput, "");
            RasterSupport.Unlock(RasterSupportType.OcrArabic, "");
            RasterSupport.Unlock(RasterSupportType.OcrArabicPdfLeadOutput, "");
            RasterSupport.Unlock(RasterSupportType.OcrPlus, "");
            RasterSupport.Unlock(RasterSupportType.OcrPlusPdfOutput, "");
            RasterSupport.Unlock(RasterSupportType.OcrPlusPdfLeadOutput, "");
            RasterSupport.Unlock(RasterSupportType.OcrProfessional, "");
            RasterSupport.Unlock(RasterSupportType.OcrProfessionalAsian, "");
            RasterSupport.Unlock(RasterSupportType.OcrProfessionalPdfOutput, "");
            RasterSupport.Unlock(RasterSupportType.OcrProfessionalPdfLeadOutput, "");
            RasterSupport.Unlock(RasterSupportType.PdfAdvanced, "");
            RasterSupport.Unlock(RasterSupportType.PdfRead, "");
            RasterSupport.Unlock(RasterSupportType.PdfSave, "");
            RasterSupport.Unlock(RasterSupportType.PrintDriver, "");
            RasterSupport.Unlock(RasterSupportType.PrintDriverServer, "");
            RasterSupport.Unlock(RasterSupportType.Vector, "");
        }

        #region Converters

        private static Color ToGdiPlusColor(RasterColor color)
        {
            return RasterColorConverter.ToColor(color);
        }

        private static RasterColor FromGdiPlusColor(Color color)
        {
            return RasterColorConverter.FromColor(color);
        }

        private static LeadRect ConvertRect(Rectangle rc)
        {
            return LeadRect.FromLTRB(rc.Left, rc.Top, rc.Right, rc.Bottom);
        }

        private static Rectangle ConvertRect(LeadRect rc)
        {
            return Rectangle.FromLTRB(rc.Left, rc.Top, rc.Right, rc.Bottom);
        }

        private static LeadPoint ConvertPoint(Point p)
        {
            return new LeadPoint(p.X, p.Y);
        }

        private static Point ConvertPoint(LeadPoint p)
        {
            return new Point(p.X, p.Y);
        }

        #endregion
    }
}
