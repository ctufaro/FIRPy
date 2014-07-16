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
        private static string[] symbols = new string[] { "GEIG", "AGIN" };
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
            var ticks = googleFeed.GetTicks(lotsSymbols, 61, 30, GooglePoints);

            foreach (Tick t in ticks)
            {
                var RSI = RelativeStrengthIndex.RSI(10, t.Close);
            }

            Console.WriteLine("Ticks Saved To Memory, RSI calculated @ {0}", stopwatch.Elapsed);
            googleFeed.SaveTicks(ticks, settings, "ticks");
            Console.WriteLine("Ticks Saved To Database @ {0}", stopwatch.Elapsed);
            stopwatch.Stop();
            Console.ReadLine();
        }
    }
}
