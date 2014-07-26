using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIRPy.DomainObjects;

namespace FIRPy.FeedAPI
{
    public static class FeedAPIFactory
    {
        public static FeedProvider GetStockFeedFactory(FeedAPIProviders stockFeedProvider)
        {
            switch (stockFeedProvider)
            {
                case FeedAPIProviders.Google:
                    return new GoogleFeedAPI();
                case FeedAPIProviders.Bloomberg:
                    return new BloombergFeedAPI();
                case FeedAPIProviders.Yahoo:
                    return new YahooFeedAPI();
                default:
                    return null;
            }

            
        }
    }
}
