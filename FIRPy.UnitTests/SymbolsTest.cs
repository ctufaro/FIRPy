using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FIRPy.FeedAPI;
using FIRPy.DomainObjects;

namespace FIRPy.UnitTests
{
    [TestClass]
    public class SymbolsTest
    {
        [TestMethod]
        public void Get_Symbols_List_From_Github()
        {
            FeedProvider googleFeed = FeedAPIFactory.GetStockFeedFactory(FeedAPIProviders.Google);
            var symbols = googleFeed.GetSymbolsFromList(Lists.Penny);
            Assert.IsTrue(symbols.Length > 0);
        }

        [TestMethod]
        public void Get_Positions_From_Github()
        {
            FeedProvider googleFeed = FeedAPIFactory.GetStockFeedFactory(FeedAPIProviders.Google);
            var symbols = googleFeed.GetPositions();
            Assert.IsTrue(symbols.Count() > 0);
        }
    }
}
