﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FIRPy.FeedAPI;
using FIRPy.DomainObjects;
using FIRPy.Indicators;
using FIRPy.Notifications;

namespace FIRPy.Runner
{
    class Program
    {
        private static string[] symbols = new string[] { "GEIG" };
        private static string[] lotsSymbols = new string[] { "AEGA", "AEMD", "AGIN", "AHFD", "ALKM", "AMZZ", "ANYI", "APHD", "APPZ", "ASKE", "AWGI", "BCLI", "BFRE", "BKCT", "BLBK", "BLUU", "BMIX", "BRZG", "CANA", "CANN", "CANV", "CAPP", "CBDS", "CBIS", "CNAB", "CNRFF", "COCP", "COSR", "CRMB", "CTSO", "CYNK", "DDDX", "DLPM", "DMHI", "DPSM", "ECIG", "ECPN", "EDXC", "EHOS", "ELTP", "EMBR", "ENCR", "ENIP", "ERBB", "EXSL", "FARE", "FITX", "FMCC", "FMCKJ", "FNMA", "FNMAH", "FNMAS", "FNMAT", "FSPM", "FTTN", "GBLX", "GEIG", "GFOO", "GFOX", "GHDC", "GMUI", "GNIN", "GRNH", "GSPE", "GTHP", "GWPRF", "HEMP", "HFCO", "HIPP", "HJOE", "HKTU", "HKUP", "HORI", "HSCC", "IDNG", "IDOI", "IDST", "INIS", "INNO", "IPRU", "IRCE", "ITEN", "IWEB", "KDUS", "KEOSF", "KRED", "LIBE", "LIWA", "LQMT", "LVGI", "MAXD", "MCIG", "MDBX", "MDDD", "MDMJ", "MINA", "MJMJ", "MJNA", "MLCG", "MNTR", "MONK", "MRIC", "MWIP", "MYHI", "MYRY", "MZEI", "NHLD", "NHTC", "NMED", "NPWZ", "NSATF", "NVIV", "NVLX", "OBJE", "OCEE", "OREO", "OWOO", "PARR", "PHOT", "PMCM", "PROP", "PTRC", "PUGE", "PWDY", "PWEB", "RBCC", "RCHA", "RDMP", "REAC", "RFMK", "RIGH", "RJDG", "ROIL", "SANP", "SBDG", "SCIO", "SCRC", "SFMI", "SIAF", "SIMH", "SING", "SLNN", "SNGX", "SRNA", "SVAD", "SWVI", "TRIIE", "TRTC", "TTNP", "UAPC", "UPOT", "VAPE", "VAPO", "VASO", "VEND", "VGPR", "VNTH", "VPOR", "VSYM", "VUZI", "WHLM", "WTER" };
        private static string[] GooglePoints = new string[] { QuoteDataPoints.Date, QuoteDataPoints.Open, QuoteDataPoints.High, QuoteDataPoints.Low, QuoteDataPoints.Close, QuoteDataPoints.Volume };
        private static Dictionary<string, TickReportData> notificationsList = new Dictionary<string, TickReportData>();
        
        static void Main(string[] args)
        {
            Intraday();   
        }

        static void Intraday()
        {
            Stopwatch stopwatch = new Stopwatch();
            ConfigSettings settings = new ConfigSettings();
            settings.SQLiteDatabaseLocation = @"..\..\..\..\sqlite-databases\penny.sqlite";
            FeedProvider googleFeed = FeedAPIFactory.GetStockFeedFactory(FeedAPIProviders.Google);
            stopwatch.Start();
            Console.WriteLine("Retrieving Ticks");
            //var ticks = googleFeed.GetTicks(symbols, 121, 30, GooglePoints);
            var ticks = googleFeed.GetSavedTicks(settings, "ticks");

            RelativeStrengthIndex.RSIGreaterThan70 += new RelativeStrengthIndex.RSIHandler(RelativeStrengthIndex_RSIGreaterThan70);
            RelativeStrengthIndex.RSILessThan30 += new RelativeStrengthIndex.RSIHandler(RelativeStrengthIndex_RSILessThan30);
            RelativeStrengthIndex.RSIFlat += new RelativeStrengthIndex.RSIHandler(RelativeStrengthIndex_RSIFlat);
            MACD.MACDBuySignal += new MACD.MACDHandler(MACD_MACDBuySignal);
            MACD.MACDSellSignal += new MACD.MACDHandler(MACD_MACDSellSignal);

            foreach (var t in ticks)
            {
                var currentDayData = t.TickGroup.Where(x => x.Date.ToShortDateString().Equals(DateTime.Today.ToShortDateString())).OrderBy(x => x.Date);

                if (currentDayData.Count() <= 0)
                    continue;

                var currentVolume = currentDayData.Sum(v => v.Volume);
                var symbol = t.Symbol;
                var openPrice = currentDayData.First().Open;
                var currentPrice = currentDayData.Last().Close;
                var changeInPrice = (currentPrice - openPrice);
                var changePercent = (changeInPrice / openPrice) * 100;

                notificationsList.Add(symbol, new TickReportData()
                {
                    Symbol = symbol,
                    ChangeInPrice = changeInPrice,
                    CurrentPrice = currentPrice,
                    CurrentVolume = currentVolume,
                    OpenPrice = openPrice
                });

                RelativeStrengthIndex.GetRSI(10, t.TickGroup2Minutes5Days.Select(x => x.Close).ToList(), t.Symbol, Periods.TwoMinutesFiveDays);
                RelativeStrengthIndex.GetRSI(10, t.TickGroup30Minutes1Month.Select(x => x.Close).ToList(), t.Symbol, Periods.ThirtyMinutesThirtyDays);
                MACD.GetMACDInfo(12, 26, 9, t.TickGroup2Minutes5Days.Select(x => x.Close).ToList(), 5, t.Symbol, Periods.TwoMinutesFiveDays);
                MACD.GetMACDInfo(12, 26, 9, t.TickGroup30Minutes1Month.Select(x => x.Close).ToList(), 5, t.Symbol, Periods.ThirtyMinutesThirtyDays);
            }

            //googleFeed.SaveTicks(ticks, settings, "ticks"); 

            Console.WriteLine("Completed @ {0}", stopwatch.Elapsed);

            Notification.SendTickReportData(notificationsList, Delivery.Email);

            stopwatch.Stop();
            Console.ReadLine();
        }

        #region Indicator Events
        static void MACD_MACDSellSignal(object sender, MACDEventArgs e)
        {
            switch (e.Period)
            {
                case(Periods.ThirtyMinutesThirtyDays):
                    notificationsList[e.Symbol].MACD30DBuySell = "SELL";
                    notificationsList[e.Symbol].MACD30DHistogram = e.Histogram;
                    break;
                case(Periods.TwoMinutesFiveDays):
                    notificationsList[e.Symbol].MACD5DBuySell = "SELL";
                    notificationsList[e.Symbol].MACD5DHistogram = e.Histogram;
                    break;
            }
        }

        static void MACD_MACDBuySignal(object sender, MACDEventArgs e)
        {
            switch (e.Period)
            {
                case (Periods.ThirtyMinutesThirtyDays):
                    notificationsList[e.Symbol].MACD30DBuySell = "BUY";
                    notificationsList[e.Symbol].MACD30DHistogram = e.Histogram;
                    break;
                case (Periods.TwoMinutesFiveDays):
                    notificationsList[e.Symbol].MACD5DBuySell = "BUY";
                    notificationsList[e.Symbol].MACD5DHistogram = e.Histogram;
                    break;
            }
        }

        static void RelativeStrengthIndex_RSILessThan30(object sender, RelativeStrengthIndexEventArgs e)
        {
            switch (e.Period)
            {
                case (Periods.ThirtyMinutesThirtyDays):
                    notificationsList[e.Symbol].RSI30D = e.RSI;
                    break;
                case (Periods.TwoMinutesFiveDays):
                    notificationsList[e.Symbol].RSI5D = e.RSI;
                    break;
            }
        }

        static void RelativeStrengthIndex_RSIGreaterThan70(object sender, RelativeStrengthIndexEventArgs e)
        {
            switch (e.Period)
            {
                case (Periods.ThirtyMinutesThirtyDays):
                    notificationsList[e.Symbol].RSI30D = e.RSI;
                    break;
                case (Periods.TwoMinutesFiveDays):
                    notificationsList[e.Symbol].RSI5D = e.RSI;
                    break;
            }
        }

        static void RelativeStrengthIndex_RSIFlat(object sender, RelativeStrengthIndexEventArgs e)
        {
            switch (e.Period)
            {
                case (Periods.ThirtyMinutesThirtyDays):
                    notificationsList[e.Symbol].RSI30D = e.RSI;
                    break;
                case (Periods.TwoMinutesFiveDays):
                    notificationsList[e.Symbol].RSI5D = e.RSI;
                    break;
            }
        }
        #endregion
    }
}
