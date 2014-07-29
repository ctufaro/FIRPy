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
        public static event MACDHandler MACDBuySignal;
        public static event MACDHandler MACDSellSignal;

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

        private static List<double> CalculateXDayEMA(int period, List<double> data, int decimalPlaces)
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
                retList.Add(Math.Round(t, decimalPlaces));
                average = t;
            }
            return retList;
        }

        private static List<double> SubtractLists(List<double> left, List<double> right, int decimalPlaces)
        {
            List<double> retList = new List<double>();
            int indexOfArray = Math.Abs(right.Count - left.Count);
            for (int i = 0; i < right.Count; i++)
            {
                retList.Add(Math.Round(left[indexOfArray++] - right[i], decimalPlaces));
            }
            return retList;
        }

        public static Tuple<List<double>, List<double>, List<double>> GetMACDInfo(int shortEMA, int longEMA, int signalLine, List<double> data, int decimalPlaces)
        {
            var TwelveDayEMA = CalculateXDayEMA(shortEMA, data, decimalPlaces);
            var TwentySixDayEMA = CalculateXDayEMA(longEMA, data, decimalPlaces);
            var MACD = SubtractLists(TwelveDayEMA, TwentySixDayEMA, decimalPlaces);
            var Signal = CalculateXDayEMA(signalLine, MACD, decimalPlaces);
            var Histogram = SubtractLists(MACD, Signal, decimalPlaces);

            CheckForCrossOver(Histogram);

            return new Tuple<List<double>, List<double>, List<double>>(MACD.Skip(signalLine-1).ToList(), Signal, Histogram);
        }

        private static void CheckForCrossOver(List<double> histogram)
        {
            int currentSign = 0;
            var lastElement = histogram.Last();
            MACDEventArgs e = new MACDEventArgs();
            if (Math.Sign(lastElement) == 0)
            {
                return;
            }
            else
            {
                currentSign = Math.Sign(lastElement);
                for (int i = histogram.Count-1; i > 0; i--)
                {
                    var previousValue = Math.Sign(histogram[i]);
                    if (previousValue != 0)
                    {
                        if (previousValue == -1 && currentSign == 1)
                        {
                            if (MACDBuySignal != null)
                            {
                                MACDBuySignal(null, e);
                            }
                            return;
                        }
                        else if (previousValue == 1 && currentSign == -1)
                        {
                            if (MACDSellSignal != null)
                            {
                                MACDSellSignal(null, e);
                            }
                            return;
                        }
                        return;
                    }
                }
            }

        }
    }

    public class MACDEventArgs : System.EventArgs
    {
        public string Period { get; set; }
        public string Symbol { get; set; }
        public double RSI { get; set; }
    }
}
