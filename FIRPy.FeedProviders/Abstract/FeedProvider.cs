using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using FIRPy.DomainObjects;

namespace FIRPy.FeedAPI
{
    public abstract class FeedProvider
    {
        public abstract string TickFeedURL { get; }
        public abstract string QuoteFeedURL { get; }
        public abstract List<Tick> GetTicks(string[] quotes, int interval, int period, string[] dataPoints);
        public abstract void SaveTicks(List<Tick> ticks, ConfigSettings settings, string tableName);
        public abstract List<Quote> GetQuotes(string[] quotes, DateTime startDate, DateTime endDate);
        public string[] GetRequestURL(string url)
        {
            string result = string.Empty;
            using (var client = new WebClient())
            {
                client.Proxy = null;
                result = client.DownloadString(url);
            }

            if (!string.IsNullOrEmpty(result))
            {
                return result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            }
            else
            {
                return null;
            }
            
        }

    }
}
