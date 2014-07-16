using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicTacTec.TA.Library;

namespace FIRPy.Indicators
{
    public class MovingAverage
    {
        public static List<double> SimpleMovingAverage(int window, List<double> data)
        {
            int begIdx, outNB = 0;
            double[] output = new double[data.Count];
            Core.Sma(0, data.Count - 1, data.ToArray(), window, out begIdx, out outNB, output);
            return output.ToList().GetRange(0, outNB);
        }

        public static List<double> ExponentialMovingAverage(int window, List<double> data)
        {
            int begIdx, outNB = 0;
            double[] output = new double[data.Count];
            Core.Ema(0, data.Count - 1, data.ToArray(), window, out begIdx, out outNB, output);
            return output.ToList().GetRange(0, outNB);
        }
    }
}
