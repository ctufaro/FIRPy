using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FIRPy.FeedAPI;
using FIRPy.DomainObjects;
using FIRPy.Indicators;

namespace FIRPy.Runner
{
    class Program
    {
        private static string[] symbols = new string[] { "TM" };
        private static string[] lotsSymbols = new string[] { "AEGA", "AEMD", "AGIN", "AHFD", "ALKM", "AMZZ", "ANYI", "APHD", "APPZ", "ASKE", "AWGI", "BCLI", "BFRE", "BKCT", "BLBK", "BLUU", "BMIX", "BRZG", "CANA", "CANN", "CANV", "CAPP", "CBDS", "CBIS", "CNAB", "CNRFF", "COCP", "COSR", "CRMB", "CTSO", "CYNK", "DDDX", "DLPM", "DMHI", "DPSM", "ECIG", "ECPN", "EDXC", "EHOS", "ELTP", "EMBR", "ENCR", "ENIP", "ERBB", "EXSL", "FARE", "FITX", "FMCC", "FMCKJ", "FNMA", "FNMAH", "FNMAS", "FNMAT", "FSPM", "FTTN", "GBLX", "GEIG", "GFOO", "GFOX", "GHDC", "GMUI", "GNIN", "GRNH", "GSPE", "GTHP", "GWPRF", "HEMP", "HFCO", "HIPP", "HJOE", "HKTU", "HKUP", "HORI", "HSCC", "IDNG", "IDOI", "IDST", "INIS", "INNO", "IPRU", "IRCE", "ITEN", "IWEB", "KDUS", "KEOSF", "KRED", "LIBE", "LIWA", "LQMT", "LVGI", "MAXD", "MCIG", "MDBX", "MDDD", "MDMJ", "MINA", "MJMJ", "MJNA", "MLCG", "MNTR", "MONK", "MRIC", "MWIP", "MYHI", "MYRY", "MZEI", "NHLD", "NHTC", "NMED", "NPWZ", "NSATF", "NVIV", "NVLX", "OBJE", "OCEE", "OREO", "OWOO", "PARR", "PHOT", "PMCM", "PROP", "PTRC", "PUGE", "PWDY", "PWEB", "RBCC", "RCHA", "RDMP", "REAC", "RFMK", "RIGH", "RJDG", "ROIL", "SANP", "SBDG", "SCIO", "SCRC", "SFMI", "SIAF", "SIMH", "SING", "SLNN", "SNGX", "SRNA", "SVAD", "SWVI", "TRIIE", "TRTC", "TTNP", "UAPC", "UPOT", "VAPE", "VAPO", "VASO", "VEND", "VGPR", "VNTH", "VPOR", "VSYM", "VUZI", "WHLM", "WTER" };
        private static string[] GooglePoints = new string[] { QuoteDataPoints.Date, QuoteDataPoints.Open, QuoteDataPoints.High, QuoteDataPoints.Low, QuoteDataPoints.Close, QuoteDataPoints.Volume };
  
        
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            ConfigSettings settings = new ConfigSettings();
            settings.SQLiteDatabaseLocation = @"..\..\..\..\sqlite-databases\penny.sqlite";
            FeedProvider googleFeed = FeedAPIFactory.GetStockFeedFactory(FeedAPIProviders.Google);
            stopwatch.Start();
            Console.WriteLine("Retrieving Ticks");
            var ticks = googleFeed.GetTicks(symbols, 121, 30, GooglePoints);            
            //var ticks = googleFeed.GetSavedTicks(settings, "ticks");
            //RelativeStrengthIndex.RSIEqualToOrGreaterThan70 += new RelativeStrengthIndex.RSIHandler(RelativeStrengthIndex_RSIEqualToOrGreaterThan70);
            //RelativeStrengthIndex.RSIEqualToOrLessThan30 += new RelativeStrengthIndex.RSIHandler(RelativeStrengthIndex_RSIEqualToOrLessThan30);

            foreach (var t in ticks)
            {
                RelativeStrengthIndex.GetRSI(10, t.TickGroup2Minutes5Days.Select(x => x.Close).ToList(), t.Symbol, "5D");
                RelativeStrengthIndex.GetRSI(10, t.TickGroup30Minutes1Month.Select(x => x.Close).ToList(), t.Symbol, "30D");
                //var macd = MACD.initMACD(0, t.TickGroup2Minutes5Days.Select(x => x.Close).ToList());
                var closePrices = t.TickGroup2Minutes5Days.Select(x => x.Close).ToList();
                var macd = MACD.GetMACDInfo(12, 26, 9, closePrices, 5);
                var macd2 = MACD.initMACD(0, t.TickGroup30Minutes1Month.Select(x => x.Close).ToList());


                //delete
                var xx = t.TickGroup2Minutes5Days.Select(x => x.Date).Skip(33).ToList();
                for (int i = 0; i < macd.Item3.Count; i++)
                {
                    //if (macd.Item1[i] == -0.08)
                    //{
                        Console.WriteLine("Date {0} MACD {1} {2} {3}", xx[i], macd.Item1[i], macd.Item2[i], macd.Item3[i]);
                    //}
                }

            }

            //Console.WriteLine("Ticks Saved To Memory @ {0}", stopwatch.Elapsed);
            //googleFeed.SaveTicks(ticks, settings, "ticks");
            //Console.WriteLine("Ticks Saved To Database @ {0}", stopwatch.Elapsed);            
            Console.WriteLine("Completed @ {0}", stopwatch.Elapsed);
            stopwatch.Stop();
            Console.ReadLine();
        }


        #region Indicator Events
        static void RelativeStrengthIndex_RSIEqualToOrLessThan30(object sender, RelativeStrengthIndexEventArgs e)
        {
            Console.WriteLine("{0} {1} RSI: {2}", e.Symbol, e.Period, e.RSI );
        }

        static void RelativeStrengthIndex_RSIEqualToOrGreaterThan70(object sender, RelativeStrengthIndexEventArgs e)
        {
            Console.WriteLine("{0} {1} RSI: {2}", e.Symbol, e.Period, e.RSI);
        }
        #endregion
    }
}
