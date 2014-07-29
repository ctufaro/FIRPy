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
        public double Bid { get; set; }
        public double Ask { get; set; }
        public int CurrentVolume { get; set; }
        public double CurrentPrice { get; set; }
        public double ChangeInPrice { get; set; }
        public double RSI { get; set; }
        public string RSIBuySell { get; set; }
        public double MACDHistogram { get; set; }
        public string MACDBuySell { get; set; }
    }
}
