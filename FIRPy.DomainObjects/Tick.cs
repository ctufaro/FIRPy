using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FIRPy.DomainObjects
{
    public class Tick
    {
        public string Symbol { get; set; }
        public List<DateTime> Date { get; set; }
        public List<decimal> Close { get; set; }
        public List<decimal> High { get; set; }
        public List<decimal> Low { get; set; }
        public List<decimal> Open { get; set; }
        public List<int> Volume { get; set; }

        public Tick()
        {
            this.Date = new List<DateTime>();
            this.Close = new List<decimal>();
            this.High = new List<decimal>();
            this.Low = new List<decimal>();
            this.Open = new List<decimal>();
            this.Volume = new List<int>();
        }

    }
}
