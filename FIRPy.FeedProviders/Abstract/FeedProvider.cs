﻿using System;
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

        public string[] GetRequestURL(string url)
        {
            string result = string.Empty;
            using (var client = new WebClient())
            {
                client.Proxy = null;
                result = client.DownloadString(url);
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

    }
}