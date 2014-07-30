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
                foreach (var key in data.Keys)
                {
                    var tickData = data[key];
                    sb.Append(string.Format(tableFormat, tickData.Symbol, tickData.OpenPrice, tickData.CurrentVolume, tickData.CurrentPrice, tickData.ChangeInPrice, tickData.RSI5D, tickData.RSI30D, tickData.MACD5DHistogram, tickData.MACD5DBuySell, tickData.MACD30DHistogram, tickData.MACD30DBuySell));
                }
                html = html.Replace("<!--data-->", sb.ToString());
            }

            if (deliveryMethod.Equals(Delivery.FileServer))
            {
                File.WriteAllText(@"C:\temp\output.html",html);
            }
        }
    }
}
