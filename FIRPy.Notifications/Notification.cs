using System;
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
            string html = string.Empty;
            using (StreamReader sr = new StreamReader(@"../../../FIRPy.Notifications/HTMLTemplates/TickReportData.htm"))
            {
                html = sr.ReadToEnd();
                StringBuilder sb = new StringBuilder();
                string tableFormat = "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td><td>{9}</td><td>{10}</td></tr>";
                foreach (var key in data.Keys.OrderBy(x=>x.ToLower()))
                {
                    var tickData = data[key];
                    string style = "";
                    switch (Math.Sign(tickData.ChangeInPrice))
                    {
                        case(-1):
                            style = "color:red;font-weight:bold;";
                            break;
                        case(0):
                            style = "black";
                            break;
                        case(1):
                            style = "color:green;font-weight:bold;";
                            break;
                    }
                    var change = "<span style='"+style+"'>"+tickData.ChangeInPrice+"%</span>";

                    var rsi5 = (tickData.RSI5D == -1) ? "-" : tickData.RSI5D.ToString();
                    var rsi30 = (tickData.RSI30D == -1) ? "-" : tickData.RSI30D.ToString();

                    sb.Append(string.Format(tableFormat, tickData.Symbol, tickData.OpenPrice, tickData.CurrentVolume.ToString("#,##0"), tickData.CurrentPrice, change, rsi5, rsi30, tickData.MACD5DHistogram, tickData.MACD5DBuySell, tickData.MACD30DHistogram, tickData.MACD30DBuySell));
                }
                html = html.Replace("<!--data-->", sb.ToString());
            }

            if (deliveryMethod.Equals(Delivery.FileServer))
            {
                File.WriteAllText(@"C:\temp\intraday.html",html);
            }
        }

        public static void SendMorningVolumeData(List<Volume> data, Delivery deliveryMethod)
        {
            //get the html template
            string html = string.Empty;
            using (StreamReader sr = new StreamReader(@"../../../FIRPy.Notifications/HTMLTemplates/MorningVolume.htm"))
            {
                html = sr.ReadToEnd();
                StringBuilder sb = new StringBuilder();
                string tableFormat = "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>";
                foreach (var v in data.OrderBy(cv => cv.CurrentVolume).ThenBy(d => d.Difference))
                {
                    sb.Append(string.Format(tableFormat, v.Date.ToShortDateString(), v.Symbol, v.CurrentVolume, v.Difference, v.Close ));
                }
                html = html.Replace("<!--data-->", sb.ToString());
            }
            if (deliveryMethod.Equals(Delivery.FileServer))
            {
                File.WriteAllText(@"C:\temp\volu me.html", html);
            }
        }
    }
}
