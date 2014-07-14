using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIRPy.DomainObjects;

namespace FIRPy.FeedAPI
{
    public class BloombergFeedAPI : FeedProvider
    {
        public override List<Tick> GetTicks(string[] quotes, int interval, int period, string[] dataPoints)
        {
            throw new NotImplementedException();
        }

        public override string TickFeedURL
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public override string QuoteFeedURL
        {
            get { throw new NotImplementedException(); }
        }

        public override List<Quote> GetQuotes(string[] quotes, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public override void SaveTicks(List<Tick> ticks)
        {
            throw new NotImplementedException();
        }
    }
}
