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

    public class Paths
    {
        public readonly static string CSSStylePath = @"../../../FIRPy.Notifications/CSS/style.css";
        public readonly static string TickReportDataHtmlPath = @"../../../FIRPy.Notifications/HTMLTemplates/TickReportData.htm";
        public readonly static string MorningVolumeHtmlPath = @"../../../FIRPy.Notifications/HTMLTemplates/MorningVolume.htm";
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
        FileServer,
        FTP
    }
}
