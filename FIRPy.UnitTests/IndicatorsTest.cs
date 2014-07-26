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
        public void Test_MACD_Calculations()
        {
            List<double> closePrices = new List<double>() { 1, 2, 3, 4, 5 };
            var retMACD = MACD.initMACD(0, closePrices);
            Assert.IsTrue(true);
        }
    }
}
