using System;
using System.Text;
using System.Collections.Generic;
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
        [TestMethod]
        public void Get_Built_URLs_Test()
        {
            FeedProvider googleFeed = StockFeedsFactory.GetStockFeedFactory(FeedProviders.Google);
            var quotes = googleFeed.BuiltQuoteURLS(new string[] { "PMCM", "VGPR" }, 1801, 15,
                                                            new string[] { 
                                                                QuoteDataPoints.Date, 
                                                                QuoteDataPoints.Open, 
                                                                QuoteDataPoints.High, 
                                                                QuoteDataPoints.Low, 
                                                                QuoteDataPoints.Close, 
                                                                QuoteDataPoints.Volume });
        }

        [TestMethod]
        public void Get_URL_Request_Test()
        {
            FeedProvider googleFeed = StockFeedsFactory.GetStockFeedFactory(FeedProviders.Google);
            var quotes = googleFeed.GetQuotes(new string[] { "PMCM", "VGPR" }, 1801, 15,
                                                            new string[] { 
                                                                QuoteDataPoints.Date, 
                                                                QuoteDataPoints.Open, 
                                                                QuoteDataPoints.High, 
                                                                QuoteDataPoints.Low, 
                                                                QuoteDataPoints.Close, 
                                                                QuoteDataPoints.Volume });
        }
    }
}
