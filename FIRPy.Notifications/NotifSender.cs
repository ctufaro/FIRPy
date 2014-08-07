using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using FIRPy.DomainObjects;

namespace FIRPy.Notifications
{
    public class NotifSender
    {
        private static string TickReportDataHtmlPath = ConfigurationSettings.AppSettings["TickReportDataHtmlPath"];
        private static string MorningVolumeHtmlPath = ConfigurationSettings.AppSettings["MorningVolumeHtmlPath"];
        private static string CSSStylePath = ConfigurationSettings.AppSettings["CSSStylePath"];
        private static string TableSortJS = ConfigurationSettings.AppSettings["TableSortJS"];
        private static string TempGraphsDirectory = ConfigurationSettings.AppSettings["TempGraphsDirectory"]; 
        
        public static void SendTickReportData(Dictionary<string, TickReportData> data, Delivery deliveryMethod)
        {
            //TODO: Refactor this pile
            foreach (string s in Directory.GetFiles(TempGraphsDirectory)) { File.Delete(s); }
            NotifDownloader.DownloadAndSaveChartFile(data.Keys.ToArray(), TempGraphsDirectory);
            if (deliveryMethod.Equals(Delivery.FTP))
            {
                NotifFTP.DeleteFiles("graphs");
                NotifFTP.UploadFiles(Directory.GetFiles(TempGraphsDirectory), "graphs");
            }
            //TODO: Refactor this pile
           
            int count = 1;
            string html = GetResourceFromPath(TickReportDataHtmlPath);
            StringBuilder sb = new StringBuilder();
            string tableFormat = @"<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td><td><img src='graphs/{9}.png'/></td></tr>";
            
            foreach (var key in data.Keys.OrderBy(x => x.ToLower()))
            {
                var tickData = data[key];
                var rsi5 = (tickData.RSI5D == -1) ? "-" : tickData.RSI5D.ToString();
                sb.Append(string.Format(tableFormat, count++, tickData.Symbol, tickData.PrevClose, tickData.CurrentVolume, tickData.CurrentPrice, ChangeInText(tickData.ChangeInPrice), rsi5, tickData.MACD5DHistogram, tickData.MACD5DBuySell, tickData.Symbol));
            }
            
            html = html.Replace("<!--data-->", sb.ToString());
            html = html.Replace("<!--css-->", GetResourceFromPath(CSSStylePath));
            html = html.Replace("<!--js-->", GetResourceFromPath(TableSortJS));
            html = html.Replace("<!--rundate-->", DateTime.Now.ToString());

            Send(html, deliveryMethod, @"C:\temp\intraday.html", "FIRPy 2.0 Intraday");
        }

        public static void SendMorningVolumeData(List<Volume> data, Delivery deliveryMethod, DateTime close)
        {
            int count = 1;
            string html = GetResourceFromPath(MorningVolumeHtmlPath);
            StringBuilder sb = new StringBuilder();
            string tableFormat = "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>";
            
            foreach (var v in data.OrderByDescending(cv => cv.CurrentVolume).ThenByDescending(d => d.Difference))
            {
                sb.Append(string.Format(tableFormat, count++, (v.Date == null) ? "-" : v.Date.Value.ToShortDateString(), v.Symbol, v.CurrentVolume, ChangeInText(v.Difference), v.Close));
            }

            html = html.Replace("<!--data-->", sb.ToString());
            html = html.Replace("<!--css-->", GetResourceFromPath(CSSStylePath));
            html = html.Replace("<!--js-->", GetResourceFromPath(TableSortJS));
            html = html.Replace("<!--closedate-->", close.ToShortDateString());
            
            Send(html, deliveryMethod, @"C:\temp\volume.html", "FIRPy 2.0 Morning Volume");
        }

        private static void Send(string html, Delivery deliveryMethod, string filePathAndName, string subject)
        {
            if (deliveryMethod.Equals(Delivery.FileServer))
            {
                File.WriteAllText(filePathAndName, html);
            }
            else if (deliveryMethod.Equals(Delivery.Email))
            {
                File.WriteAllText(filePathAndName, html);
                NotifEmailer.SendEmail(subject, "Message Attached", filePathAndName);
            }
            else if (deliveryMethod.Equals(Delivery.FTP))
            {
                File.WriteAllText(filePathAndName, html);
                NotifFTP.UploadFile(filePathAndName);
            }
        }

        private static string GetResourceFromPath(string path)
        {
            string returnHTML = string.Empty;
            using (StreamReader sr = new StreamReader(path))
            {
                returnHTML = sr.ReadToEnd();
            }
            return returnHTML;
        }

        private static string ChangeInText(double price)
        {
            string style = "";
            if (Double.IsNaN(price)) { price = 0; }
            switch (Math.Sign(price))
            {
                case (-1):
                    style = "color:red;font-weight:bold;";
                    break;
                case (0):
                    style = "black";
                    break;
                case (1):
                    style = "color:green;font-weight:bold;";
                    break;
            }
            return "<span style='" + style + "'>" + price + "%</span>";
        }

    }
}
