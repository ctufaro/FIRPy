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

        private static List<double> CalculateXDayEMA(int period, List<double> data)
        {
            List<double> retList = new List<double>();
            double average = data.Take(period).Average();
            retList.Add(Math.Round(average,2));
            foreach (var d in data.Skip(period))
            {
                var j = 2.0 / (double)(period + 1);
                var k = d * j;
                var l = (1 - j);
                var t = (k + average * l);
                retList.Add(Math.Round(t,2));
                average = t;
            }
            return retList;
        }

        private static List<double> SubtractLists(List<double> left, List<double> right)
        {
            List<double> retList = new List<double>();
            int indexOfArray = Math.Abs(right.Count - left.Count);
            for (int i = 0; i < right.Count; i++)
            {
                retList.Add(Math.Round(left[indexOfArray++] - right[i],2));
            }
            return retList;
        }

        public static Tuple<List<double>, List<double>, List<double>> GetMACDInfo(int shortEMA, int longEMA, int signalLine, List<double> data)
        {
            var TwelveDayEMA = CalculateXDayEMA(shortEMA, data);
            var TwentySixDayEMA = CalculateXDayEMA(longEMA, data);
            var MACD = SubtractLists(TwelveDayEMA, TwentySixDayEMA);
            var Signal = CalculateXDayEMA(signalLine, MACD);
            var Histogram = SubtractLists(MACD, Signal);
            return new Tuple<List<double>, List<double>, List<double>>(MACD, Signal, Histogram);
        }  
    }

    public class MACDEventArgs : System.EventArgs
    {
        public string Period { get; set; }
        public string Symbol { get; set; }
        public double RSI { get; set; }
    }
}
