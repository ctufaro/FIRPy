using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FIRPy.FeedAPI
{
    public static class FeedAPIFactory
    {
        public static FeedProvider GetStockFeedFactory(FeedProviders stockFeedProvider)
        {
            switch (stockFeedProvider)
            {
                case FeedProviders.Google:
                    return new GoogleFeedAPI();
                case FeedProviders.Bloomberg:
                    return new BloombergFeedAPI();
                case FeedProviders.Yahoo:
                    return new YahooFeedAPI();
                default:
                    return null;
            }

            
        }
    }
}
