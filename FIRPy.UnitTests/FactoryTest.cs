using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FIRPy.FeedAPI;
using FIRPy.DomainObjects;


namespace FIRPy.UnitTests
{
    [TestClass]
    public class FactoryTest
    {

        [TestMethod]
        public void Get_URL_Request_Test()
        {

        }

        [TestMethod]
        public void Save_Ticks_To_SQLite_Database()
        {
            
        }

        [TestMethod]
        public void Get_Symbols_Volume_Test()
        {
            string[] symbols = new string[] { "GEIG", "VGPR" };
            FeedProvider googleFeed = FeedAPIFactory.GetStockFeedFactory(FeedAPIProviders.Google);
            var volume = googleFeed.GetVolume(symbols, DateTime.Parse("07/29/2014"), DateTime.Parse("07/30/2014"));
            Assert.IsTrue(true);
        }
    }
}
