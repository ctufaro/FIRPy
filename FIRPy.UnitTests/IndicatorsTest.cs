using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using FIRPy.Indicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FIRPy.UnitTests
{
    [TestClass]
    public class IndicatorsTest
    {
        public bool SellSignal = false;
        public bool BuySignal = false;
        
        [TestMethod]
        public void Test_Simple_Moving_Average_Window_2()
        {
            //Arrange
            List<double> closePrices = new List<double>() { 1, 2, 3, 4, 5 };
            double[] expectedPrices = new double[] { 1.5, 2.5, 3.5, 4.5 };
            int window = 2;

            //Act
            List<double> retArray = MovingAverage.SimpleMovingAverage(window, closePrices);

            //Assert
            CollectionAssert.AreEqual(expectedPrices, retArray);
        }

        [TestMethod]
        public void Test_Simple_Moving_Average_Window_3()
        {
            //Arrange
            List<double> closePrices = new List<double>() { 1, 2, 3, 4, 5 };
            double[] expectedPrices = new double[] { 2,3,4 };
            int window = 3;

            //Act
            List<double> retArray = MovingAverage.SimpleMovingAverage(window, closePrices);

            //Assert
            CollectionAssert.AreEqual(expectedPrices, retArray);
        }

        [TestMethod]
        public void Test_Simple_Relative_Strength_Index()
        {
            //Arrange
            List<double> closePrices = new List<double>() { 1, 2, 3, 4, 5 };
            double[] expectedPrices = new double[] { 100,100 };
            int window = 3;

            //Act
            List<double> retArray = RelativeStrengthIndex.GetRSIArray(window, closePrices);

            //Assert
            CollectionAssert.AreEqual(expectedPrices, retArray);
        }

        [TestMethod]
        public void Test_MACD_Crossover_Signals_Using_Last_Element_Of_Histogram()
        {
            MACD.MACDBuySignal += new MACD.MACDHandler(MACD_MACDBuySignal);
            MACD.MACDSellSignal += new MACD.MACDHandler(MACD_MACDSellSignal);
            
            //no crossover should not generate a signal
            List<double> retset = new List<double> { 0, 0, 2, 3 };
            MACD.CheckForCrossOver(retset, "TEST", DomainObjects.Periods.ThirtyMinutesThirtyDays);
            Assert.IsFalse(SellSignal);
            Assert.IsFalse(BuySignal);
            ResetSignals();

            //last element is a zero, should not generate a signal
            retset = new List<double> { 0, 0, 0, 0 };
            MACD.CheckForCrossOver(retset, "TEST", DomainObjects.Periods.ThirtyMinutesThirtyDays);
            Assert.IsFalse(SellSignal);
            Assert.IsFalse(BuySignal);
            ResetSignals();

            //last element is a positive, pass zeros, to a negative, this is a buy
            retset = new List<double> { -1, 0, 0, 1 };
            MACD.CheckForCrossOver(retset, "TEST", DomainObjects.Periods.ThirtyMinutesThirtyDays);
            Assert.IsTrue(BuySignal);
            Assert.IsFalse(SellSignal);
            ResetSignals();

            //last element is a negative, pass zeros, to a negative, this not a signal
            retset = new List<double> { -1, -1, 0, -1 };
            MACD.CheckForCrossOver(retset, "TEST", DomainObjects.Periods.ThirtyMinutesThirtyDays);
            Assert.IsFalse(BuySignal);
            Assert.IsFalse(SellSignal);
            ResetSignals();

            //last element is a negative, pass zeros, this is not a signal
            retset = new List<double> { 0, 0, 0, -1 };
            MACD.CheckForCrossOver(retset, "TEST", DomainObjects.Periods.ThirtyMinutesThirtyDays);
            Assert.IsFalse(BuySignal);
            Assert.IsFalse(SellSignal);
            ResetSignals();

            //last element is a negative, pass zeros, to a positive, this a sell
            retset = new List<double> { -1, 2, 0, -1 };
            MACD.CheckForCrossOver(retset, "TEST", DomainObjects.Periods.ThirtyMinutesThirtyDays);
            Assert.IsFalse(BuySignal);
            Assert.IsTrue(SellSignal);
            ResetSignals();

            //last element is a negative, next is a positive, this is a sell
            retset = new List<double> { 3, -1 };
            MACD.CheckForCrossOver(retset, "TEST", DomainObjects.Periods.ThirtyMinutesThirtyDays);
            Assert.IsFalse(BuySignal);
            Assert.IsTrue(SellSignal);
            ResetSignals();

            //last element is a positive, next is a negative, this is a buy
            retset = new List<double> { -3, 1 };
            MACD.CheckForCrossOver(retset, "TEST", DomainObjects.Periods.ThirtyMinutesThirtyDays);
            Assert.IsTrue(BuySignal);
            Assert.IsFalse(SellSignal);
            ResetSignals();

        }

        void MACD_MACDSellSignal(object sender, MACDEventArgs e)
        {
            SellSignal = true;
        }

        void MACD_MACDBuySignal(object sender, MACDEventArgs e)
        {
            BuySignal = true;
        }

        void ResetSignals()
        {
            SellSignal = false;
            BuySignal = false;
        }
    }
}
