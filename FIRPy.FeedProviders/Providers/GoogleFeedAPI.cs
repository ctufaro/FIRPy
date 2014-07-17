using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using FIRPy.DomainObjects;
using FIRPy.DataAccess;

namespace FIRPy.FeedAPI
{
    public class GoogleFeedAPI : FeedProvider
    {
        public override string TickFeedURL
        {
            get { return "http://www.google.com/finance/getprices?q={0}&i={1}&p={2}d&f={3}"; }
        }

        public override string QuoteFeedURL
        {
            get { return "http://www.google.com/finance/historical?q={0}&startdate={1}&enddate={2}&output=csv"; }
        }
        
        public override List<Ticks> GetTicks(string[] symbols, int interval, int period, string[] dataPoints)
        {
            Object myLock = new object();

            List<Ticks> retTick = new List<Ticks>();
            List<string> urls = BuiltTickURLS(symbols, interval, period, dataPoints);
            Parallel.ForEach(urls, url =>
            {
                string symbol = url.Split(new string[] { "q=" }, StringSplitOptions.None)[1].Split(new string[] { "&" }, StringSplitOptions.None)[0];
                string[] retval = base.GetRequestURL(url);
                lock (myLock)
                {
                    retTick.Add(ParseLineIntoTick(symbol, retval));
                }
            });
            return retTick;
        }

        public override List<Quote> GetQuotes(string[] quotes, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public override void SaveTicks(List<Tick> ticks, ConfigSettings settings, string tableName)
        {
            SQLiteBulkInsert sbi = DataAccessFactory.GetBulkDatabase(settings, tableName);

            sbi.ClearTable(tableName);

            sbi.AddParameter("symbol", DbType.String);
            sbi.AddParameter("time", DbType.DateTime);
            sbi.AddParameter("open", DbType.Decimal);
            sbi.AddParameter("high", DbType.Decimal);
            sbi.AddParameter("low", DbType.Decimal);
            sbi.AddParameter("close", DbType.Decimal);
            sbi.AddParameter("volume", DbType.Int32);
            int rowCount = 0;
            foreach (Tick t in ticks)
            {
                //rowCount = t.Date.Count;
                //for (int i = 0; i < rowCount; i++)
                //{                    
                //    sbi.Insert(new object[] { t.Symbol, t.Date[i], t.Open[i], t.High[i], t.Low[i], t.Close[i], t.Volume[i] });
                //}
            }
            sbi.Flush();
        }

        private List<string> BuiltTickURLS(string[] symbols, int interval, int period, string[] dataPoints)
        {
            List<string> builtURLs = new List<string>();
            string fields = string.Join(",", dataPoints.Select(dp=>dp[0]));            
            foreach (string symbol in symbols)
            {
                builtURLs.Add(string.Format(this.TickFeedURL, symbol, interval, period, fields));
            }
            return builtURLs;
        }

        private Ticks ParseLineIntoTick(string symbol, string[] webLines)
        {
            Ticks quote = new Ticks();
            quote.Symbol = symbol;
            foreach (string line in webLines)
            {
                //TODO: Change this condition!!
                if (line.StartsWith("a1"))
                {
                    string[] retArray = line.Split(new string[]{","}, StringSplitOptions.None);
                    quote.TickGroup.Add(new Tick
                    {
                        Date = FromUnixTime(long.Parse(retArray[0].Substring(1))),
                        Close = Double.Parse(retArray[1]),
                        High = Double.Parse(retArray[2]),
                        Low = Double.Parse(retArray[3]),
                        Open = Double.Parse(retArray[4]),
                        Volume = Int32.Parse(retArray[5])
                    });                
                }
            }
            return quote;
        }

        private DateTime FromUnixTime(long unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
