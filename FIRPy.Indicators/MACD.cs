using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIRPy.DomainObjects;
using TicTacTec.TA.Library;

namespace FIRPy.Indicators
{
    public static class MACD
    {
        public delegate void MACDHandler(object sender, MACDEventArgs e);
        public static event MACDHandler MACDBuySignal;
        public static event MACDHandler MACDSellSignal;

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

        public static Tuple<List<double>, List<double>, List<double>> GetMACDInfo(int shortEMA, int longEMA, int signalLine, List<double> data, int decimalPlaces, string symbol, Periods period)
        {
            var TwelveDayEMA = CalculateXDayEMA(shortEMA, data, decimalPlaces);
            var TwentySixDayEMA = CalculateXDayEMA(longEMA, data, decimalPlaces);
            var MACD = SubtractLists(TwelveDayEMA, TwentySixDayEMA, decimalPlaces);
            var Signal = CalculateXDayEMA(signalLine, MACD, decimalPlaces);
            var Histogram = SubtractLists(MACD, Signal, decimalPlaces);

            CheckForCrossOver(Histogram, symbol, period);

            return new Tuple<List<double>, List<double>, List<double>>(MACD.Skip(signalLine-1).ToList(), Signal, Histogram);
        }

        public static void CheckForCrossOver(List<double> histogram, string symbol, Periods period)
        {
            int lastSign = Math.Sign(histogram.Last());
            MACDEventArgs e = new MACDEventArgs
            {
                Histogram = histogram.Last(),
                Period = period,
                Symbol = symbol
            };
            if (lastSign != 0)
            {
                //reversing histogram
                for (int i = (histogram.Count - 2); i >= 0; i--)
                {
                    int currentSign = Math.Sign(histogram[i]);
                    if (currentSign == 0)
                    {
                        continue;
                    }
                    else if (lastSign == 1 && currentSign == -1)
                    {
                        MACDBuySignal(null, e);
                        return;
                    }
                    else if (lastSign == -1 && currentSign == 1)
                    {
                        MACDSellSignal(null, e);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }

    public class MACDEventArgs : System.EventArgs
    {
        public Periods Period { get; set; }
        public string Symbol { get; set; }
        public double Histogram { get; set; }
    }
}
