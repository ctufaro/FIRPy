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
            using (StreamReader sr = new StreamReader(@"../../../FIRPy.Notifications/HTMLTemplates/TickReportData.htm"))
            {
                var html = sr.ReadToEnd();
            }
        }
    }
}
