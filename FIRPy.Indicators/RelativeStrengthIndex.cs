using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicTacTec.TA.Library;

namespace FIRPy.Indicators
{
    public class RelativeStrengthIndex
    {
        public static List<double> RSI(int window, List<double> data)
        {
            int begIdx, outNB = 0;
            double[] output = new double[data.Count];
            Core.Rsi(0, data.Count-1, data.ToArray(), window, out begIdx, out outNB, output);
            return output.ToList().GetRange(0, outNB);
        }
    }
}
