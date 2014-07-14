using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIRPy.DomainObjects;

namespace FIRPy.FeedAPIs
{
    public class GoogleFeed : IFeedProvider
    {        
        public List<Quote> GetQuotes(string[] quotes, int interval, int period, string[] dataPoints)
        {            
            throw new NotImplementedException();
        }

        public string QuotesURL
        {
            get
            {
                return "http://www.google.com/finance/getprices?q={0}&i={1}&p={2}d&f={3}";
            }
        }

        public List<string> BuiltQuoteURLS(string[] quotes, int interval, int period, string[] dataPoints)
        {
            List<string> builtURLs = new List<string>();
            string fields = string.Join(", ", dataPoints.Select(dp=>dp[0]));
            foreach (string quote in quotes)
            {
                builtURLs.Add(string.Format(this.QuotesURL, quote, interval, period, fields));
            }
            return builtURLs;
        }

    }
}
