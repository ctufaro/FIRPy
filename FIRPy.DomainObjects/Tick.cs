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
        public List<double> Close { get; set; }
        public List<double> High { get; set; }
        public List<double> Low { get; set; }
        public List<double> Open { get; set; }
        public List<int> Volume { get; set; }

        public Tick()
        {
            this.Date = new List<DateTime>();
            this.Close = new List<double>();
            this.High = new List<double>();
            this.Low = new List<double>();
            this.Open = new List<double>();
            this.Volume = new List<int>();
        }

    }
}
