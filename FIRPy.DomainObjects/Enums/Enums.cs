using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FIRPy.DomainObjects
{
    public class QuoteDataPoints
    {
        public readonly static string Open = "open";
        public readonly static string High = "high";
        public readonly static string Low = "low";
        public readonly static string Close = "close";
        public readonly static string Volume = "volume";
        public readonly static string Date = "date";
    }

    public enum FeedAPIProviders
    {
        Google, Yahoo, Bloomberg
    }

    public enum Lists
    {
        Penny, SP500, ETFs
    }

    public enum Periods
    {
        TwoMinutesFiveDays,
        ThirtyMinutesThirtyDays
    }

    public enum Delivery
    {
        Email,
        SMS,
        FileServer
    }
}
