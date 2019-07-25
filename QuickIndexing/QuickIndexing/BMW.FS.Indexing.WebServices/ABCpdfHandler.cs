using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using WebSupergoo.ABCpdf7;

namespace BMW.FS.Indexing.WebServices
{
    public static class ABCpdfHandler
    {
        public const double A4_HEIGHT = 842;
        public const double A4_WIDTH = 595;

        public static string OutputToPdf(string reference, string ReturnString, string FilePrefix, Image imageID, string FIUserID = "", string FIName = "")
        {
            /* 612  792
             Paper 		Width	Height in Points
                A3		842		1190
                A4		595		842
                A5		420		595
                Letter	612		792
             */
            double borderLeft = 50, borderTop = 70;
            Doc theDoc = new Doc();
            theDoc.Width = 1; //line thickness
            theDoc.MediaBox.String = "A4";
            theDoc.Rect.String = theDoc.MediaBox.String;
            theDoc.TextStyle.Justification = 1;
            theDoc.Rect.Inset(borderLeft, borderTop);	//Set the paper border
            //save page size
            RectPos rect = new RectPos();
            rect.left = theDoc.Rect.Left;
            rect.top = theDoc.Rect.Top;
            rect.width = theDoc.Rect.Width;
            rect.height = theDoc.Rect.Height;
            rect.bottom = theDoc.Rect.Bottom;
            /*
             A4 working area
             Width  495 = 595 - (50*2) 
             Height  555 = 842 - (70*2) 
            */
            //counters
            int cnt = 0;
            int page = 0;
            //setup the fonts
            int iFont_BMWTypeRegular = theDoc.AddFont("BMWTypeRegular");
            int iFont_Arial = theDoc.AddFont("Arial");
            int iFont_Consolas = theDoc.AddFont("Consolas");
            int iFont_Courier = theDoc.AddFont("Courier New");
            int iFontCurrent = 0;

            //headers
            string HeaderMain = "BMW Financial Services (Pty) Ltd South Africa" + Environment.NewLine;
            string HeaderSub = "Identification Document or Smart Card Barcode Decoding" + Environment.NewLine;
            string FIString = "Triggered by : " + FIName + " (" + FIUserID + ") " + Environment.NewLine;
            string DateString = "Date : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + Environment.NewLine;
            string Purpose = "The result of the Identification document or smart card ID barcode decoding is shown below: \r\n\r\n"
                + "As a risk aversion measure and in line with Know Your customer requirements, "
                + "kindly establish that the information corresponds with the physical document provided by the applicant." + Environment.NewLine;

            try
            {
                string[] arStr = Regex.Split(ReturnString, "\r\n");
                //set the main headers/title
                iFontCurrent = (iFont_BMWTypeRegular > 0) ? iFont_BMWTypeRegular : iFont_Arial;		//change font to Arial if bmw font does not exist

                theDoc.Color.String = "180 180 180";	//some grayish text color
                AddtxtLineAndFont(ref theDoc, HeaderMain, iFontCurrent, 20, true, false);

                theDoc.Color.String = "100 100 100";	//some grayish text color
                AddtxtLineAndFont(ref theDoc, HeaderSub, iFontCurrent, 12, true, false);

                AddtxtLineAndFont(ref theDoc, Environment.NewLine + Environment.NewLine + Environment.NewLine, iFontCurrent, 12, true, false);

                theDoc.AddLine(borderLeft, A4_HEIGHT - 105, 550, A4_HEIGHT - 105);		//left , top , with , top
                theDoc.AddLine(borderLeft, 50, 550, 50);		//left , top , with , top

                //set the other headers
                AddtxtLineAndFont(ref theDoc, FIString, iFontCurrent, 10, false, true);
                AddtxtLineAndFont(ref theDoc, DateString, iFontCurrent, 10, false, true);
                AddtxtLineAndFont(ref theDoc, Environment.NewLine + Environment.NewLine + Environment.NewLine, iFontCurrent, 12, true, false);

                theDoc.Color.String = "100 100 100";	//some grayish text color
                AddtxtLineAndFont(ref theDoc, Purpose, iFontCurrent, 10, false, true);
                AddtxtLineAndFont(ref theDoc, Environment.NewLine + Environment.NewLine + Environment.NewLine, iFontCurrent, 12, true, false);


                foreach (string line in arStr)
                {
                    if (line.StartsWith("*"))	//an indicator for bold text
                        AddtxtLineAndFont(ref theDoc, line.Replace("*", "") + "\r\n", iFontCurrent, 10, true, false);
                    else
                        AddtxtLineAndFont(ref theDoc, line + "\r\n", iFontCurrent, 10, false, false, 8);
                }

                //set images first - on first page
                SetImagePhoto(ref theDoc, imageID, rect, iFontCurrent);
                SetImageLogo(ref theDoc, null, rect);


                //save
                string dt = DateTime.Now.ToString("MMddHHmmss");
                string REF = "";
                if (reference.Length > 8)
                    REF = reference.Substring(5, 8);
                else
                    REF = reference;

                string TempPath = Path.GetTempPath();
                if (!TempPath.EndsWith(@"\")) TempPath += @"\";
                string Filename = TempPath + FilePrefix + REF + "_" + dt + ".pdf";
                theDoc.Save((Filename));
                theDoc.Clear();


                return Filename;
            }
            catch (Exception e)
            {
                BarcodeDetection.AddLog_KYC("Error", "QIWS.ABCpdfHandler.OutputToPdf", e.Message, "Output TXT", reference);
                return "";
            }
        }

        private static int AddtxtLineAndFont(ref Doc theDoc, string line, string font, int size, bool bold, bool italic)
        {
            theDoc.FontSize = size;
            theDoc.Font = theDoc.AddFont(font);
            theDoc.TextStyle.Bold = bold;
            theDoc.TextStyle.Italic = italic;
            return theDoc.AddText(line);
        }

        private static int AddtxtLineAndFont(ref Doc theDoc, string line, int font, int size, bool bold, bool italic, double indent = 0)
        {
            theDoc.FontSize = size;
            theDoc.Font = font;
            theDoc.TextStyle.Bold = bold;
            theDoc.TextStyle.Italic = italic;
            if (indent > 0 && !line.StartsWith("-----"))
                theDoc.TextStyle.Indent = indent;
            else
                theDoc.TextStyle.Indent = 0.0;

            return theDoc.AddText(line);
        }

        private static void SetFont(ref Doc theDoc, string font, int size)
        {
            theDoc.FontSize = size;
            theDoc.Font = theDoc.AddFont(font);
        }

        private static void SetImagePhoto(ref Doc theDoc, Image image, RectPos rect, int iFontCurrent)
        {
            if (image == null) return;

            Bitmap bm = new Bitmap(image);
            double W = 250;
            double H = 250;
            ////>>>bottom left
            //theDoc.Rect.Left = 50; theDoc.Rect.Bottom = 50;

            //set the photo/image 
            theDoc.Rect.Left = 100;
            theDoc.Rect.Top = A4_HEIGHT - 500;
            theDoc.Rect.Bottom = theDoc.Rect.Top - H;
            theDoc.Rect.Width = W;
            theDoc.Rect.Height = H;
            theDoc.AddImageBitmap(bm, false);

            //draw a frame around photo (the image stretches to its own frame so you cannot see a frame)
            /*
            //theDoc.Color.String = "180 180 180";	//some grayish text color
            double Offset = 10;
            theDoc.Rect.Left = 450 - Offset;
            theDoc.Rect.Top = A4_HEIGHT - 270 - Offset;
            theDoc.Rect.Bottom = theDoc.Rect.Top - H;
            theDoc.Rect.Width = W + (Offset * 2);
            theDoc.Rect.Height = H + (Offset * 2);
            ////add some text if you want
            AddtxtLineAndFont(ref theDoc, " Image \r\n", "Calibri", 8, false, false);
            theDoc.FrameRect();
            */

            //reset the saved page settings
            theDoc.Rect.Left = rect.left;
            theDoc.Rect.Top = rect.top;
            theDoc.Rect.Width = rect.width;
            theDoc.Rect.Height = rect.height;
            theDoc.Rect.Bottom = rect.bottom;

        }

        private static void SetImageLogo(ref Doc theDoc, Image image, RectPos rect)
        {
            try
            {
                Bitmap image2 = new Bitmap(BMW.FS.Indexing.WebServices.Properties.Resources.BMWMINI);
                //Bitmap image2 = new Bitmap(PBSA_DriversLicenceDecoding.Properties.Resources.BMWLOGO);

                //PBSA_DriversLicenceDecoding.Properties.Resources.BMWMINI;
                Bitmap bm = null;
                if (image == null)
                    bm = new Bitmap(image2);
                else
                    bm = new Bitmap(image);

                double W = bm.Width / 2;
                double H = bm.Height / 2;
                ////>>>top right
                theDoc.Rect.Left = 485;
                theDoc.Rect.Top = A4_HEIGHT - 20;
                theDoc.Rect.Bottom = theDoc.Rect.Top - H;
                theDoc.Rect.Width = W;
                theDoc.Rect.Height = H;
                //theDoc.FrameRect();
                theDoc.AddImageBitmap(bm, false);

                //reset the saved page settings
                theDoc.Rect.Left = rect.left;
                theDoc.Rect.Top = rect.top;
                theDoc.Rect.Width = rect.width;
                theDoc.Rect.Height = rect.height;
                theDoc.Rect.Bottom = rect.bottom;
            }
            catch { }
        }

        private class RectPos
        {
            public double left;
            public double top;
            public double width;
            public double height;
            public double bottom;
        }

    }
}
