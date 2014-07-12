using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIRPy.DomainObjects;

namespace FIRPy.Factory
{
    public static class StockFeedsFactory
    {
        public static StockFeeds GetStockFeeds(StockFeedProviders stockFeedProvider)
        {
            StockFeeds xx = null;
            switch (stockFeedProvider)
            {
                case StockFeedProviders.Google:
                    xx = null;
                    break;
            }

            return xx;
        }
    }
}
