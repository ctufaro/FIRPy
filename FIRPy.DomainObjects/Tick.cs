using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FIRPy.DomainObjects
{
    public class Ticks
    {
        public string Symbol { get; set; }
        public List<Tick> TickGroup { get; set; }

        public Ticks()
        {
            this.TickGroup = new List<Tick>();
        }

    }

    public struct Tick
    {
        public DateTime Date {get; set;}
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Open { get; set; }
        public int Volume { get; set; }

        //public Tick(double Close, double High, double Low, double Open, int Volume)
        //{
        //    this.Close = Close;
        //    this.High = High;
        //    this.Low = Low;
        //    this.Open = Open;
        //    this.Volume = Volume;
        //}
    }
}
