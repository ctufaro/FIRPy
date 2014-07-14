using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FIRPy.Factory;
using FIRPy.FeedAPIs;
using FIRPy.DomainObjects;


namespace FIRPy.UnitTests
{
    [TestClass]
    public class FactoryTest
    {

        private string[] GooglePoints = new string[] {QuoteDataPoints.Date,QuoteDataPoints.Open,QuoteDataPoints.High,QuoteDataPoints.Low,QuoteDataPoints.Close,QuoteDataPoints.Volume };

        [TestMethod]
        public void Get_URL_Request_Test()
        {
            Stopwatch stopwatch = new Stopwatch();
            FeedProvider googleFeed = StockFeedsFactory.GetStockFeedFactory(FeedProviders.Google);
            stopwatch.Start();
            //var quotes = googleFeed.GetQuotes(new string[] { "AEGA", "AEMD", "AGIN", "AHFD", "ALKM", "AMZZ", "ANYI", "APHD", "APPZ", "ASKE", "AWGI", "BCLI", "BFRE", "BKCT", "BLBK", "BLUU", "BMIX", "BRZG", "CANA", "CANN", "CANV", "CAPP", "CBDS", "CBIS", "CNAB", "CNRFF", "COCP", "COSR", "CRMB", "CTSO", "CYNK", "DDDX", "DLPM", "DMHI", "DPSM", "ECIG", "ECPN", "EDXC", "EHOS", "ELTP", "EMBR", "ENCR", "ENIP", "ERBB", "EXSL", "FARE", "FITX", "FMCC", "FMCKJ", "FNMA", "FNMAH", "FNMAS", "FNMAT", "FSPM", "FTTN", "GBLX", "GEIG", "GFOO", "GFOX", "GHDC", "GMUI", "GNIN", "GRNH", "GSPE", "GTHP", "GWPRF", "HEMP", "HFCO", "HIPP", "HJOE", "HKTU", "HKUP", "HORI", "HSCC", "IDNG", "IDOI", "IDST", "INIS", "INNO", "IPRU", "IRCE", "ITEN", "IWEB", "KDUS", "KEOSF", "KRED", "LIBE", "LIWA", "LQMT", "LVGI", "MAXD", "MCIG", "MDBX", "MDDD", "MDMJ", "MINA", "MJMJ", "MJNA", "MLCG", "MNTR", "MONK", "MRIC", "MWIP", "MYHI", "MYRY", "MZEI", "NHLD", "NHTC", "NMED", "NPWZ", "NSATF", "NVIV", "NVLX", "OBJE", "OCEE", "OREO", "OWOO", "PARR", "PHOT", "PMCM", "PROP", "PTRC", "PUGE", "PWDY", "PWEB", "RBCC", "RCHA", "RDMP", "REAC", "RFMK", "RIGH", "RJDG", "ROIL", "SANP", "SBDG", "SCIO", "SCRC", "SFMI", "SIAF", "SIMH", "SING", "SLNN", "SNGX", "SRNA", "SVAD", "SWVI", "TRIIE", "TRTC", "TTNP", "UAPC", "UPOT", "VAPE", "VAPO", "VASO", "VEND", "VGPR", "VNTH", "VPOR", "VSYM", "VUZI", "WHLM", "WTER" }, 1801, 15, GooglePoints);
            var quotes = googleFeed.GetQuotes(new string[] {"AEGA"}, 1801, 15, GooglePoints);
            stopwatch.Stop();
        }
    }
}
