using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIRPy.DomainObjects;

namespace FIRPy.FeedAPIs
{
    public class YahooFeed : FeedProvider
    {
        public override List<Quote> GetQuotes(string[] quotes, int interval, int period, string[] dataPoints)
        {
            throw new NotImplementedException();
        }

        public override string QuotesURL
        {
            get
            {
                throw new NotImplementedException();
            }
        }

    }
}
