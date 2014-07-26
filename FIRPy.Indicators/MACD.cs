using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicTacTec.TA.Library;

namespace FIRPy.Indicators
{
    public static class MACD
    {
        public delegate void MACDHandler(object sender, MACDEventArgs e);
        public static event MACDHandler MACDCrossOverEMA;
        public static event MACDHandler MACDCrossUnderEMA;

        public static List<double> initMACD(int window, List<double> data)
        {
            int begIdx, outNB = 0;
            List<double> retval = new List<double>();
            double[] outMACD = new double[data.Count];
            double[] outMACDSignal = new double[data.Count];
            double[] outMACDHist = new double[data.Count];
            Core.Macd(0, data.Count - 1, data.ToArray(), 12, 26, 9, out begIdx, out outNB, outMACD, outMACDSignal, outMACDHist);
            if (retval.Count() > 0)
            {
                return retval;
            }
            else
            {
                return new List<double> { 0 };
            }
        }

        public static List<double> GetMACDArray(int window, List<double> data)
        {
            return initMACD(window, data);
        }

        public static double GetMACD(int window, List<double> data, string id, string period)
        {
            MACDEventArgs e = new MACDEventArgs();            
            return 0;
        }
    }

    public class MACDEventArgs : System.EventArgs
    {
        public string Period { get; set; }
        public string Symbol { get; set; }
        public double RSI { get; set; }
    }
}
