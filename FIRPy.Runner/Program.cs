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
        private static string[] symbols = new string[] { "AEGA", "AEMD", "AGIN" };
        private static string[] lotsSymbols = new string[] { "AEGA", "AEMD", "AGIN", "AHFD", "ALKM", "AMZZ", "ANYI", "APHD", "APPZ", "ASKE", "AWGI", "BCLI", "BFRE", "BKCT", "BLBK", "BLUU", "BMIX", "BRZG", "CANA", "CANN", "CANV", "CAPP", "CBDS", "CBIS", "CNAB", "CNRFF", "COCP", "COSR", "CRMB", "CTSO", "CYNK", "DDDX", "DLPM", "DMHI", "DPSM", "ECIG", "ECPN", "EDXC", "EHOS", "ELTP", "EMBR", "ENCR", "ENIP", "ERBB", "EXSL", "FARE", "FITX", "FMCC", "FMCKJ", "FNMA", "FNMAH", "FNMAS", "FNMAT", "FSPM", "FTTN", "GBLX", "GEIG", "GFOO", "GFOX", "GHDC", "GMUI", "GNIN", "GRNH", "GSPE", "GTHP", "GWPRF", "HEMP", "HFCO", "HIPP", "HJOE", "HKTU", "HKUP", "HORI", "HSCC", "IDNG", "IDOI", "IDST", "INIS", "INNO", "IPRU", "IRCE", "ITEN", "IWEB", "KDUS", "KEOSF", "KRED", "LIBE", "LIWA", "LQMT", "LVGI", "MAXD", "MCIG", "MDBX", "MDDD", "MDMJ", "MINA", "MJMJ", "MJNA", "MLCG", "MNTR", "MONK", "MRIC", "MWIP", "MYHI", "MYRY", "MZEI", "NHLD", "NHTC", "NMED", "NPWZ", "NSATF", "NVIV", "NVLX", "OBJE", "OCEE", "OREO", "OWOO", "PARR", "PHOT", "PMCM", "PROP", "PTRC", "PUGE", "PWDY", "PWEB", "RBCC", "RCHA", "RDMP", "REAC", "RFMK", "RIGH", "RJDG", "ROIL", "SANP", "SBDG", "SCIO", "SCRC", "SFMI", "SIAF", "SIMH", "SING", "SLNN", "SNGX", "SRNA", "SVAD", "SWVI", "TRIIE", "TRTC", "TTNP", "UAPC", "UPOT", "VAPE", "VAPO", "VASO", "VEND", "VGPR", "VNTH", "VPOR", "VSYM", "VUZI", "WHLM", "WTER" };
        private static string[] GooglePoints = new string[] { QuoteDataPoints.Date, QuoteDataPoints.Open, QuoteDataPoints.High, QuoteDataPoints.Low, QuoteDataPoints.Close, QuoteDataPoints.Volume };
  
        
        static void Main(string[] args)
        {
            LoadAndSaveTicks();
        }

        static void LoadAndSaveTicks()
        {
            Stopwatch stopwatch = new Stopwatch();
            ConfigSettings settings = new ConfigSettings();
            settings.SQLiteDatabaseLocation = @"..\..\..\..\sqlite-databases\penny.sqlite";
            FeedProvider googleFeed = FeedAPIFactory.GetStockFeedFactory(FeedProviders.Google);
            stopwatch.Start();
            Console.WriteLine("Retrieving Ticks");
            var ticks = googleFeed.GetTicks(lotsSymbols, 121, 30, GooglePoints);
            string symbol = "";
            string RSId = "";
            string RSIm = "";

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\WriteLines2.txt"))
            {

                foreach (var tck in ticks)
                {             
                    symbol = tck.Symbol;

                    try
                    {
                        //7 days - (2 mins, seven days from current)
                        var twoMinutesFiveDays = tck.TickGroup.Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date);
                        int twoc = twoMinutesFiveDays.Count();
                        var d = twoMinutesFiveDays.Select(x => x.Close).ToList();
                        RSId = RelativeStrengthIndex.RSI(10, d).Last().ToString();
                    }
                    catch { }

                    try
                    {
                        //1 month - (30 mins, all given dates)
                        var thirtyMinutesOneMonth = tck.TickGroup.Where(x => x.Date.Minute == 00 || x.Date.Minute == 30).OrderBy(x => x.Date);
                        int thirtyC = thirtyMinutesOneMonth.Count();
                        var m = thirtyMinutesOneMonth.Select(x => x.Close).ToList();
                        RSIm = RelativeStrengthIndex.RSI(10, m).Last().ToString();
                    }
                    catch { }

                    file.WriteLine("Symbol:{0}, RSI7:{1}, RSI30:{2}", symbol, RSId, RSIm);
                    RSId = "";
                    RSIm = "";
                }
            }

            Console.WriteLine("Ticks Saved To Memory, RSI calculated @ {0}", stopwatch.Elapsed);
            googleFeed.SaveTicks(ticks, settings, "ticks");
            Console.WriteLine("Ticks Saved To Database @ {0}", stopwatch.Elapsed);
            stopwatch.Stop();
            Console.ReadLine();
        }
    }
}
