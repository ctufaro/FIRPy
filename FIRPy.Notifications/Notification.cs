﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FIRPy.DomainObjects;

namespace FIRPy.Notifications
{
    public class Notification
    {
        public static void SendTickReportData(Dictionary<string, TickReportData> data, Delivery deliveryMethod)
        {
            //get the html template
            int count = 1;
            string html = string.Empty;
            using (StreamReader sr = new StreamReader(@"../../../FIRPy.Notifications/HTMLTemplates/TickReportData.htm"))
            {
                html = sr.ReadToEnd();
                StringBuilder sb = new StringBuilder();
                string tableFormat = "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td></tr>";
                foreach (var key in data.Keys.OrderBy(x=>x.ToLower()))
                {
                    var tickData = data[key];
                    var rsi5 = (tickData.RSI5D == -1) ? "-" : tickData.RSI5D.ToString();
                    sb.Append(string.Format(tableFormat, count++, tickData.Symbol, tickData.PrevClose, tickData.CurrentVolume.ToString("#,##0"), tickData.CurrentPrice, ChangeInText(tickData.ChangeInPrice), rsi5, tickData.MACD5DHistogram, tickData.MACD5DBuySell));
                }
                html = html.Replace("<!--data-->", sb.ToString());
                html = html.Replace("<!--css-->", GetStyleSheet());
                html = html.Replace("<!--rundate-->", DateTime.Now.ToString());
            }

            if (deliveryMethod.Equals(Delivery.FileServer))
            {
                File.WriteAllText(@"C:\temp\intraday.html",html);
            }
            else if (deliveryMethod.Equals(Delivery.Email))
            {
                Emailer.SendEmail("FIRPy 2.0 Intraday", html);
            }
            else if (deliveryMethod.Equals(Delivery.FTP))
            {
                File.WriteAllText(@"C:\temp\intraday.html",html);
                FTP.Upload(@"C:\temp\intraday.html");
            }
        }

        public static void SendMorningVolumeData(List<Volume> data, Delivery deliveryMethod, DateTime close)
        {
            int count = 1;
            string html = string.Empty;
            using (StreamReader sr = new StreamReader(@"../../../FIRPy.Notifications/HTMLTemplates/MorningVolume.htm"))
            {
                html = sr.ReadToEnd();
                StringBuilder sb = new StringBuilder();
                string tableFormat = "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>";
                foreach (var v in data.OrderByDescending(cv => cv.CurrentVolume).ThenByDescending(d => d.Difference))
                { 
                    sb.Append(string.Format(tableFormat, count++, (v.Date==null)? "-" : v.Date.Value.ToShortDateString(), v.Symbol, v.CurrentVolume.ToString("#,##0"), ChangeInText(v.Difference), v.Close));
                }
                html = html.Replace("<!--data-->", sb.ToString());
                html = html.Replace("<!--css-->", GetStyleSheet());
                html = html.Replace("<!--closedate-->", close.ToShortDateString());
            }
            if (deliveryMethod.Equals(Delivery.FileServer))
            {
                File.WriteAllText(@"C:\temp\volume.html", html);
            }
            else if (deliveryMethod.Equals(Delivery.Email))
            {
                File.WriteAllText(@"C:\temp\volume.html", html);
                Emailer.SendEmail("FIRPy 2.0 Morning Volume " + close.ToShortDateString(), "Message Attached", @"C:\temp\volume.html");
            }
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

        private static string GetStyleSheet()
        {
            string css = string.Empty;
            using (StreamReader sr = new StreamReader(@"../../../FIRPy.Notifications/CSS/style.css"))
            {
                css = sr.ReadToEnd();
            }
            return css;
        }

    }
}
