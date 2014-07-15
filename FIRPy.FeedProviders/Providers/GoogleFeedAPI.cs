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
        
        public override List<Tick> GetTicks(string[] symbols, int interval, int period, string[] dataPoints)
        {
            Object myLock = new object();

            List<Tick> retTick = new List<Tick>();
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

        public override void SaveTicks(List<Tick> ticks, ConfigSettings settings)
        {
            SQLiteBulkInsert sbi = DataAccessFactory.GetBulkDatabase(settings,"ticks");            
            sbi.AddParameter("symbol", DbType.String);
            sbi.AddParameter("time", DbType.DateTime);
            sbi.AddParameter("open", DbType.Decimal);
            sbi.AddParameter("high", DbType.Decimal);
            sbi.AddParameter("low", DbType.Decimal);
            sbi.AddParameter("close", DbType.Decimal);
            sbi.AddParameter("volume", DbType.Int32);
            //db.ClearTable("ticks");
            int rowCount = 0;
            foreach (Tick t in ticks)
            {
                rowCount = t.Date.Count;
                for (int i = 0; i < rowCount; i++)
                {                    
                    sbi.Insert(new object[] { t.Symbol, t.Date[i], t.Open[i], t.High[i], t.Low[i], t.Close[i], t.Volume[i] });
                }
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

        private Tick ParseLineIntoTick(string symbol, string[] webLines)
        {
            Tick quote = new Tick();
            quote.Symbol = symbol;
            foreach (string line in webLines)
            {
                //TODO: Change this condition!!
                if (line.StartsWith("a1"))
                {
                    string[] retArray = line.Split(new string[]{","}, StringSplitOptions.None);
                    quote.Date.Add(FromUnixTime(long.Parse(retArray[0].Substring(1))));                    
                    quote.Close.Add(Decimal.Parse(retArray[1]));
                    quote.High.Add(Decimal.Parse(retArray[2]));
                    quote.Low.Add(Decimal.Parse(retArray[3]));
                    quote.Open.Add(Decimal.Parse(retArray[4]));
                    quote.Volume.Add(Int32.Parse(retArray[5]));                    
                }
            }
            return quote;
        }

        private DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
    }
}
