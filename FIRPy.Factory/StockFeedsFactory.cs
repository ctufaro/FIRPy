using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIRPy.FeedAPIs;


namespace FIRPy.Factory
{
    public static class StockFeedsFactory
    {
        public static FeedProvider GetStockFeedFactory(FeedProviders stockFeedProvider)
        {
            switch (stockFeedProvider)
            {
                case FeedProviders.Google:
                    return new GoogleFeed();
                case FeedProviders.Bloomberg:
                    return new BloombergFeed();
                case FeedProviders.Yahoo:
                    return new YahooFeed();
                default:
                    return null;
            }

            
        }
    }
}
