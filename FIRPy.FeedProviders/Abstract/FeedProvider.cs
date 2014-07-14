using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using FIRPy.DomainObjects;

namespace FIRPy.FeedAPIs
{
    public abstract class FeedProvider
    {
        public abstract string QuotesURL { get; }
        public abstract List<Quote> GetQuotes(string[] quotes, int interval, int period, string[] dataPoints);
        public abstract List<string> BuiltQuoteURLS(string[] quotes, int interval, int period, string[] dataPoints);

        public string GetRequestURL(string url)
        {
            string result = string.Empty;
            using (var client = new WebClient())
            {
                client.Proxy = null;
                result = client.DownloadString(url);
            }
            return result;
        }


        //Stopwatch stopwatch = new Stopwatch();
        //    string result = string.Empty;
        //    string url = @"http://www.google.com/finance/getprices?q=T&i=1801&p=15d&f=d,o,h,l,c,v";
        //    using (var client = new WebClient())
        //    {
        //        client.Proxy = null;
        //        stopwatch.Start();
        //        result = client.DownloadString(url);
        //        stopwatch.Stop();
        //    }
        //    //Console.WriteLine(result);
        //    Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
        //    Console.ReadLine();
    }
}
