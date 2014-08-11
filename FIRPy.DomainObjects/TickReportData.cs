using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FIRPy.DomainObjects
{
    public class TickReportData
    {
        public string HasPosition { get; set; }
        public string Symbol { get; set; }
        public double PrevClose { get; set; }
        public int CurrentVolume { get; set; }
        public double CurrentPrice { get; set; }
        public double ChangeInPrice { get; set; }
        public double ChangeInVolume { get; set; }
        public double RSI5D { get; set; }
        public double MACD5DHistogram { get; set; }
        public string MACD5DBuySell { get; set; }
    }
}
