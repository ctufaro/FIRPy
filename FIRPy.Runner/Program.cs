using System;
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
        #region Private Variables
        private static string[] symbols = new string[] { "GEIG", "DNAX", "VGPR", "VLNX" };
        private static string[] lotsSymbols = new string[] { "AEGA", "AEMD", "AGIN", "AHFD", "ALKM", "AMZZ", "ANYI", "APHD", "APPZ", "ASKE", "AWGI", "BCLI", "BFRE", "BKCT", "BLBK", "BLUU", "BMIX", "BRZG", "CANA", "CANN", "CANV", "CAPP", "CBDS", "CBIS", "CNAB", "CNRFF", "COCP", "COSR", "CRMB", "CTSO", "CYNK", "DDDX", "DLPM", "DMHI", "DPSM", "ECIG", "ECPN", "EDXC", "EHOS", "ELTP", "EMBR", "ENCR", "ENIP", "ERBB", "EXSL", "FARE", "FITX", "FMCC", "FMCKJ", "FNMA", "FNMAH", "FNMAS", "FNMAT", "FSPM", "FTTN", "GBLX", "GEIG", "GFOO", "GFOX", "GHDC", "GMUI", "GNIN", "GRNH", "GSPE", "GTHP", "GWPRF", "HEMP", "HFCO", "HIPP", "HJOE", "HKTU", "HKUP", "HORI", "HSCC", "IDNG", "IDOI", "IDST", "INIS", "INNO", "IPRU", "IRCE", "ITEN", "IWEB", "KDUS", "KEOSF", "KRED", "LIBE", "LIWA", "LQMT", "LVGI", "MAXD", "MCIG", "MDBX", "MDDD", "MDMJ", "MINA", "MJMJ", "MJNA", "MLCG", "MNTR", "MONK", "MRIC", "MWIP", "MYHI", "MYRY", "MZEI", "NHLD", "NHTC", "NMED", "NPWZ", "NSATF", "NVIV", "NVLX", "OBJE", "OCEE", "OREO", "OWOO", "PARR", "PHOT", "PMCM", "PROP", "PTRC", "PUGE", "PWDY", "PWEB", "RBCC", "RCHA", "RDMP", "REAC", "RFMK", "RIGH", "RJDG", "ROIL", "SANP", "SBDG", "SCIO", "SCRC", "SFMI", "SIAF", "SIMH", "SING", "SLNN", "SNGX", "SRNA", "SVAD", "SWVI", "TRIIE", "TRTC", "TTNP", "UAPC", "UPOT", "VAPE", "VAPO", "VASO", "VEND", "VGPR", "VNTH", "VPOR", "VSYM", "VUZI", "WHLM", "WTER" };
        private static string[] GooglePoints = new string[] { QuoteDataPoints.Date, QuoteDataPoints.Open, QuoteDataPoints.High, QuoteDataPoints.Low, QuoteDataPoints.Close, QuoteDataPoints.Volume };
        private static Dictionary<string, TickReportData> notificationsList = new Dictionary<string, TickReportData>();
        private static FeedProvider mainProvider = null;
        #endregion

        /// <summary>
        /// FIRPy Main Starting Point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            mainProvider = FeedAPIFactory.GetStockFeedFactory(FeedAPIProviders.Google);
            
            if (args.Length > 0)
            {                
                //Penny Stocks
                symbols = mainProvider.GetSymbolsFromList(Lists.Penny);
                
                switch (args[0])
                {
                    //Morning Volume
                    case("-movo"):
                        MorningVolume();
                        break;
                    //Intraday
                    case("-intra"):
                        Intraday();
                        break;
                    //Morning Twitter/RSS
                    case("-motr"):
                        TwitterRSSFeeds();
                        break;
                }
            }
            else
            {
                Intraday();
                //MorningVolume();
            }
        }

        static void MorningVolume()
        {
            var endDate = mainProvider.GetPreviousDay();

            var volume = mainProvider.GetVolume(symbols);

            NotifSender.SendMorningVolumeData(volume, Delivery.FTP, endDate);
        }

        static void TwitterRSSFeeds()
        {
        }

        static void Intraday()
        {
            SubscribeEvents();
            
            var ticks = mainProvider.GetTicks(symbols, 121, 30, GooglePoints);

            #region Main Symbol Loop
            foreach (var tick in ticks.Where(t=>t.TickGroup.Count()>0))
            {
                notificationsList.Add(tick.Symbol,mainProvider.GenerateTickReportData(tick));

                #region Intraday Ticks Indicators Events
                var twoMinutesFiveDaysClosePrices = tick.TickGroup2Minutes5Days.Select(x => x.Close).ToList();
                if (twoMinutesFiveDaysClosePrices.Count() > 0)
                {
                    RelativeStrengthIndex.GetRSI(10, twoMinutesFiveDaysClosePrices, tick.Symbol, Periods.TwoMinutesFiveDays);
                    MACD.GetMACDInfo(12, 26, 9, twoMinutesFiveDaysClosePrices, 5, tick.Symbol, Periods.TwoMinutesFiveDays);
                }
                #endregion
            }
            #endregion

            NotifSender.SendTickReportData(notificationsList, Delivery.FTP);
        }

        #region Subscribed Events
        static void SubscribeEvents()
        {
            RelativeStrengthIndex.RSIGreaterThan70 += new RelativeStrengthIndex.RSIHandler(RelativeStrengthIndex_RSIGreaterThan70);
            RelativeStrengthIndex.RSILessThan30 += new RelativeStrengthIndex.RSIHandler(RelativeStrengthIndex_RSILessThan30);
            RelativeStrengthIndex.RSIFlat += new RelativeStrengthIndex.RSIHandler(RelativeStrengthIndex_RSIFlat);
            MACD.MACDBuySignal += new MACD.MACDHandler(MACD_MACDBuySignal);
            MACD.MACDSellSignal += new MACD.MACDHandler(MACD_MACDSellSignal);
        }
        #endregion

        #region Indicator Events
        static void MACD_MACDSellSignal(object sender, MACDEventArgs e)
        {
            switch (e.Period)
            {
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
                case (Periods.TwoMinutesFiveDays):
                    notificationsList[e.Symbol].RSI5D = e.RSI;
                    break;
            }
        }

        static void RelativeStrengthIndex_RSIGreaterThan70(object sender, RelativeStrengthIndexEventArgs e)
        {
            switch (e.Period)
            {
                case (Periods.TwoMinutesFiveDays):
                    notificationsList[e.Symbol].RSI5D = e.RSI;
                    break;
            }
        }

        static void RelativeStrengthIndex_RSIFlat(object sender, RelativeStrengthIndexEventArgs e)
        {
            switch (e.Period)
            {
                case (Periods.TwoMinutesFiveDays):
                    notificationsList[e.Symbol].RSI5D = e.RSI;
                    break;
            }
        }
        #endregion
    }
}
