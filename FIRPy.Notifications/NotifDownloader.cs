using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Net;

namespace FIRPy.Notifications
{
    public class NotifDownloader
    {
        public static void DownloadAndSaveChartFile(string[] symbols, string saveLocation)
        {            
            Parallel.ForEach(symbols, symbol =>
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile("http://www.stockcharts.com/c-sc/sc?s=" + symbol + "&p=D&b=5&g=1&i=p97703862546", string.Format(@"{0}{1}.png", saveLocation, symbol));
            });
        }
    }
}
