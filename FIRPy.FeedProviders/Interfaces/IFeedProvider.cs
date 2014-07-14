using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIRPy.DomainObjects;

namespace FIRPy.FeedAPIs
{
    public interface IFeedProvider
    {
        string QuotesURL { get; }
        List<Quote> GetQuotes(string[] quotes, int interval, int period, string[] dataPoints);
        List<string> BuiltQuoteURLS(string[] quotes, int interval, int period, string[] dataPoints);
    }
}
