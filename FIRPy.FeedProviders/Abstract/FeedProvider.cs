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
        public abstract List<Ticks> GetTicks(string[] quotes, int interval, int period, string[] dataPoints);
        public abstract List<Ticks> GetSavedTicks(ConfigSettings settings, string tableName);
        public abstract void SaveTicks(List<Ticks> ticks, ConfigSettings settings, string tableName);
        public abstract List<Quote> GetQuotes(string[] quotes, DateTime startDate, DateTime endDate);
        public abstract List<Volume> GetVolume(string[] symbols);
        public abstract TickReportData GenerateTickReportData(Ticks ticks);
        public string[] GetRequestURL(string url)
        {
            string result = string.Empty;
            using (var client = new WebClient())
            {
                client.Proxy = null;
                try
                {
                    result = client.DownloadString(url);
                }
                catch (WebException e) { }
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
        public string[] GetSymbolsFromList(Lists list)
        {
            string[] symbols = null;
            switch (list)
            {
                case(Lists.Penny):
                    symbols = GetRequestURL(@"https://raw.githubusercontent.com/ctufaro/FIRPy/master/Resources/lists/penny.list");
                    break;
                default:
                    break;
            }

            return symbols.Where(x => x.Length > 0).ToArray();
        }


        public List<string> GetPositions()
        {
            List<string> retPositions = new List<string>();
            var positions = GetRequestURL("https://raw.githubusercontent.com/ctufaro/FIRPy/master/Resources/positions/positions.txt");
            foreach (string pos in positions.Where(x=>x.Length>0))
            {
                retPositions.Add(pos.Split(new string[] { "," }, StringSplitOptions.None)[0]);
            }
            return retPositions;
        }


        public DateTime GetPreviousDay()
        {
            //TODO: Exclude holidays
            if (DateTime.Today.DayOfWeek.Equals(DayOfWeek.Monday))
            {
                return DateTime.Now.AddDays(-3);
            }
            else
            {
                return DateTime.Now.AddDays(-1);
            }

        }
    }
}
