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
        public List<Tick> TickGroup2Minutes5Days
        {
            get
            {
                if (this.TickGroup != null)
                {
                    return this.TickGroup.Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                }
                else
                {
                    return null;
                }
            }
        }
        public List<Tick> TickGroup30Minutes1Month
        {
            get
            {
                if (this.TickGroup != null)
                {
                    return this.TickGroup.Where(x => x.Date.Minute == 00 || x.Date.Minute == 30).OrderBy(x => x.Date).ToList();
                }
                else
                {
                    return null;
                } 
            }
        }

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
    }
}
