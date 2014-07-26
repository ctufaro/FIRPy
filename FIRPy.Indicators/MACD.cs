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
            int startIdx = 0;
            int endIdx = data.Count - 1;
            int optInFastPeriod = 12;
            int optInSlowPeriod = 26;
            int optInSignalPeriod = 9;
            double[] inReal = data.ToArray();

            int outBegIdx;
            int outNBElement;

            double[] outMACD = new double[endIdx - startIdx + 1];
            double[] outMACDSignal = new double[endIdx - startIdx + 1];
            double[] outMACDHist = new double[endIdx - startIdx + 1];

            Core.RetCode res = Core.Macd(startIdx, endIdx, inReal, optInFastPeriod, optInSlowPeriod, optInSignalPeriod, out  outBegIdx, out  outNBElement, outMACD, outMACDSignal, outMACDHist);
            return outMACDHist.ToList();
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
