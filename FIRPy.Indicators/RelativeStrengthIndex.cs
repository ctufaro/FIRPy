using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIRPy.DomainObjects;
using TicTacTec.TA.Library;

namespace FIRPy.Indicators
{
    public static class RelativeStrengthIndex
    {
        public delegate void RSIHandler(object sender, RelativeStrengthIndexEventArgs e);
        public static event RSIHandler RSIGreaterThan70;
        public static event RSIHandler RSILessThan30;
        public static event RSIHandler RSIFlat;  
        
        private static List<double> initRSI(int window, List<double> data)
        {
            int begIdx, outNB = 0;
            List<double> retval = new List<double>();
            double[] output = new double[data.Count];
            Core.Rsi(0, data.Count-1, data.ToArray(), window, out begIdx, out outNB, output);
            retval = output.ToList().GetRange(0, outNB);
            if (retval.Count() > 0)
            {
                return retval;
            }
            else
            {
                return new List<double> { 0 };
            }
        }

        public static List<double> GetRSIArray(int window, List<double> data)
        {            
            return initRSI(window, data);
        }

        public static double GetRSI(int window, List<double> data, string id, Periods period)
        {
            double retval = initRSI(window, data).Last();
            RelativeStrengthIndexEventArgs e = new RelativeStrengthIndexEventArgs();
            e.Symbol = id;
            e.RSI = Math.Round(retval,0);
            e.Period = period;
            if (retval < 30 && retval >= 0)
            {
                if (RSILessThan30 != null)
                {
                    RSILessThan30(null, e);
                }
            }
            else if (retval > 70 && retval < 100)
            {
                if (RSIGreaterThan70 != null)
                {                    
                   RSIGreaterThan70(null, e);
                }
            }
            else if (retval >= 30 && retval <= 70)
            {
                if (RSIFlat != null)
                {
                    e.RSI = -1;
                    RSIFlat(null, e);
                }
            }
            return retval;
        }

    }

    public class RelativeStrengthIndexEventArgs : System.EventArgs
    {
        public Periods Period { get; set; }
        public string Symbol { get; set; }
        public double RSI { get; set; }
    }
}
