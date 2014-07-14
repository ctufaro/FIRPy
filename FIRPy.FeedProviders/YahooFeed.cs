using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIRPy.DomainObjects;

namespace FIRPy.FeedAPIs
{
    public class YahooFeed : IFeedProvider
    {
        public List<Quote> GetQuotes(string[] quotes, int interval, int period, string[] dataPoints)
        {
            throw new NotImplementedException();
        }

        public string QuotesURL
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public List<string> BuiltQuoteURLS(string[] quotes, int interval, int period, string[] dataPoints)
        {
            throw new NotImplementedException();
        }
    }
}
