using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FIRPy.DomainObjects
{
    public class Volume
    {
        public DateTime? Date { get; set; }
        public string Symbol { get; set; }
        public int CurrentVolume { get; set; }
        public double Difference { get; set; }
        public double? Close { get; set; }
    }
}
