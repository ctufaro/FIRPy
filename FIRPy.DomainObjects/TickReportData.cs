using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FIRPy.DomainObjects
{
    public class TickReportData
    {
        public string Symbol { get; set; }
        public double OpenPrice { get; set; }
        //public double Bid { get; set; }
        //public double Ask { get; set; }
        public int CurrentVolume { get; set; }
        public double CurrentPrice { get; set; }
        public double ChangeInPrice { get; set; }
        public double RSI5D { get; set; }
        public double RSI30D { get; set; }
        public double MACD5DHistogram { get; set; }
        public string MACD5DBuySell { get; set; }
        public double MACD30DHistogram { get; set; }
        public string MACD30DBuySell { get; set; }
    }
}
