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

        public override List<Ticks> GetSavedTicks(ConfigSettings settings, string tableName)
        {
            Dictionary<string, bool> symbols = new Dictionary<string, bool>();
            List<Ticks> retTick = new List<Ticks>();
            string sql = "SELECT id,symbol,datetime(time),open,high,low,close,volume FROM {0}";
            BaseDataAccess sqlite = DataAccessFactory.GetDatabase("SQLite", settings);
            DataTable dt = sqlite.GetDataTable(string.Format(sql, tableName));
            Ticks t = null;
            foreach (DataRow row in dt.Rows)
            {
                var symbol = row[1].ToString();

                if (!symbols.ContainsKey(symbol))
                {

                    if (symbols.Count() > 0)
                    {
                        retTick.Add(t);
                    }
                    
                    symbols.Add(symbol, true);
                    t = new Ticks();
                    t.Symbol = symbol;
                }

                t.TickGroup.Add(new Tick()
                {
                    Date = DateTime.Parse(row[2].ToString()),
                    Close = Double.Parse(row[6].ToString()),
                    High = Double.Parse(row[4].ToString()),
                    Low = Double.Parse(row[5].ToString()),
                    Open = Double.Parse(row[3].ToString()),
                    Volume = Int32.Parse(row[7].ToString())
                });
                
            }
            retTick.Add(t);
            return retTick;
        }

        public override List<Quote> GetQuotes(string[] quotes, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public override void SaveTicks(List<Ticks> ticks, ConfigSettings settings, string tableName)
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
            foreach (Ticks tt in ticks)
            {
                foreach (var t in tt.TickGroup)
                {
                    sbi.Insert(new object[] { tt.Symbol, t.Date, t.Open, t.High, t.Low, t.Close, t.Volume });    
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

        private Ticks ParseLineIntoTick(string symbol, string[] webLines)
        {
            if (webLines == null)
                webLines = new string[]{""};
            
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

        [ObsoleteAttribute("This method is obsolete. Call GetVolume instead.", true)] 
        public List<Volume> GetTickVolume(string[] symbols, DateTime startDate, DateTime endDate)
        {
            //Google Historicals rounds to the nearent cent, not very helpful for penny stocks
            //We need to make a seperate call just for the closing prices
            var closingPrices = GetTicks(symbols,30001,3, new string[] { QuoteDataPoints.Date, QuoteDataPoints.Open, QuoteDataPoints.High, QuoteDataPoints.Low, QuoteDataPoints.Close, QuoteDataPoints.Volume });
            var filterCP = (from cp in closingPrices
                            where cp.TickGroup.Count()>0
                            select new
                            {
                              Symbol = cp.Symbol,
                              Close = cp.TickGroup.Last().Close
                            }).ToList();

            List<Volume> retList = new List<Volume>();          
            string url = "http://www.google.com/finance/historical?q={0}&startdate={1}&enddate={2}&output=csv";
            Parallel.ForEach(symbols, symbol =>
            {
                Object myLock = new object();
                
                var requestUrl = string.Format(url, symbol, startDate.ToShortDateString(), endDate.ToShortDateString());
                string[] retArray = GetRequestURL(requestUrl);
                if (retArray != null)
                {
                    var elements = retArray.Where(x => x.Length > 1).Skip(1).Take(2).ToList();
                    if (elements.Count() >= 2)
                    {
                        string[] dayOne = elements[0].Split(new string[] { "," }, StringSplitOptions.None);
                        string[] dayTwo = elements[1].Split(new string[] { "," }, StringSplitOptions.None);

                        lock (myLock)
                        {
                            retList.Add(new Volume
                            {
                                Close = filterCP.Exists(a => a.Symbol.Equals(symbol)) ? filterCP.Where(a=>a.Symbol.Equals(symbol)).First().Close: 0,
                                CurrentVolume = Int32.Parse(dayOne[5]),
                                Date = DateTime.Parse(dayOne[0]),
                                Difference = Math.Round(((Double.Parse(dayOne[5]) - Double.Parse(dayTwo[5])) / Double.Parse(dayTwo[5])) * 100, 0),
                                Symbol = symbol

                            });
                        }
                    }
                }
            });
            return retList;
        }

        public override List<Volume> GetVolume(string[] symbols)
        {
            List<Volume> retVolume = new List<Volume>();
            var tickData = GetTicks(symbols, 121, 30, new string[] { QuoteDataPoints.Date, QuoteDataPoints.Open, QuoteDataPoints.High, QuoteDataPoints.Low, QuoteDataPoints.Close, QuoteDataPoints.Volume });
            foreach (var td in tickData.Where(x=>x.TickGroup.Count()>0))
            {
                
                    var symbol = td.Symbol;
                    try
                    {
                        var currentdate = td.TickGroup.Last().Date.ToShortDateString();
                        var currentClose = td.TickGroup.Last().Close;
                        var prevDate = td.TickGroup.Where(x => !x.Date.ToShortDateString().Equals(currentdate)).Last().Date.ToShortDateString();
                        var currVolume = td.TickGroup.Where(x => x.Date.ToShortDateString().Equals(currentdate)).Sum(y => y.Volume);
                        var prevVolume = td.TickGroup.Where(x => x.Date.ToShortDateString().Equals(prevDate)).Sum(y => y.Volume);
                        var percentDiff = Math.Round(((Convert.ToDouble(currVolume) - Convert.ToDouble(prevVolume)) / Convert.ToDouble(prevVolume)) * 100, 0);

                        retVolume.Add(new Volume
                        {
                            Close = currentClose,
                            CurrentVolume = currVolume,
                            Date = DateTime.Parse(currentdate),
                            Difference = percentDiff,
                            Symbol = symbol
                        });
                    }
                    catch
                    {
                        retVolume.Add(new Volume
                        {
                            Close = 0,
                            CurrentVolume = 0,
                            Date = null,
                            Difference = 0,
                            Symbol = "!"+symbol
                        });
                    }
            }
            return retVolume;
        }

        public override TickReportData GenerateTickReportData(Ticks tick)
        {
            TickReportData retData = new TickReportData();
            var currentDayData = tick.TickGroup.Where(x => x.Date.ToShortDateString().Equals(DateTime.Today.ToShortDateString())).OrderByDescending(x => x.Date);
            var prevDayData = tick.TickGroup.Where(x => !x.Date.ToShortDateString().Equals(DateTime.Today.ToShortDateString())).Last();
            var currentVolume = currentDayData.Sum(v => v.Volume);
            var symbol = tick.Symbol;
            var prevClose = prevDayData.Close;
            var currentPrice = currentDayData.First().Close;
            var changeInPrice = (currentPrice - prevClose);
            var changePricePercent = (changeInPrice / prevClose) * 100;
            var changeInVolumeArray = currentDayData.Take(2).Where(x => x.Volume > 0).ToArray();
            var changeInVolume = Math.Round(Convert.ToDouble(changeInVolumeArray[0].Volume) - Convert.ToDouble(changeInVolumeArray[1].Volume) / Convert.ToDouble(changeInVolumeArray[1].Volume), 0);
            
            retData.Symbol = symbol;
            retData.ChangeInPrice = Math.Round(changePricePercent, 2);
            retData.CurrentPrice = currentPrice;
            retData.CurrentVolume = currentVolume;
            retData.PrevClose = prevClose;

            return retData;
        }
    }
}
