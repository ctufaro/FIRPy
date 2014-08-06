using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FIRPy.FeedAPI;
using FIRPy.DomainObjects;

namespace FIRPy.ChartEngine
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitChart();
        }

        public void InitChart()
        {

            chart1.Dock = DockStyle.Fill;
            
            //// fake the DB data with a simple list
            //List<dbdata> k = new List<dbdata> { 
            //new dbdata("1/1/2012", 10f, 8f, 9f, 9.5f),
            //new dbdata("2/1/2012", 15F, 10F, 12F, 13F),
            //new dbdata("3/1/2012", 5F, 10F, 8F, 6F),
            //new dbdata("4/1/2012", 25F, 10F, 18F, 16F)
            //};

            Series price = new Series("price"); // <<== make sure to name the series "price"
            chart1.Series.Add(price);

            // Set series chart type
            chart1.Series["price"].ChartType = SeriesChartType.Candlestick;

            // Set the style of the open-close marks
            chart1.Series["price"]["OpenCloseStyle"] = "Triangle";

            // Show both open and close marks
            chart1.Series["price"]["ShowOpenClose"] = "Both";

            // Set point width
            chart1.Series["price"]["PointWidth"] = "2.0";

            // Set colors bars
            chart1.Series["price"]["PriceUpColor"] = "Green"; // <<== use text indexer for series
            chart1.Series["price"]["PriceDownColor"] = "Red"; // <<== use text indexer for series

            FeedProvider mainProvider = FeedAPIFactory.GetStockFeedFactory(FeedAPIProviders.Google);
            var GooglePoints = new string[] { QuoteDataPoints.Date, QuoteDataPoints.Open, QuoteDataPoints.High, QuoteDataPoints.Low, QuoteDataPoints.Close, QuoteDataPoints.Volume };
            var ticks = mainProvider.GetTicks(new string[] { "GEIG" }, 121, 30, GooglePoints).First();

            int i = 0;
            foreach (var k in ticks.TickGroup.Where(d => d.Date.ToShortDateString().Equals("8/6/2014")))
            {
                // adding date and high
                chart1.Series["price"].Points.AddXY(k.Date, k.High);
                // adding low
                chart1.Series["price"].Points[i].YValues[1] = k.Low;
                //adding open
                chart1.Series["price"].Points[i].YValues[2] = k.Open;
                // adding close
                chart1.Series["price"].Points[i].YValues[3] = k.Close;
                i++;
            }
        }
    }

    class dbdata
    {
        public string Date;
        public float High;
        public float Low;
        public float Open;
        public float Close;
        public dbdata(string d, float h, float l, float o, float c) { Date = d; High = h; Low = l; Open = o; Close = c; }
    }
}
