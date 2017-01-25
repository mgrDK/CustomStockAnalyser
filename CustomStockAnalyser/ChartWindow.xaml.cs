using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CustomStockAnalyser
{
    /// <summary>
    /// Interaction logic for ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        public ChartWindow(string windowTitle, string stockName, List<KeyValuePair<string,double>> data)
        {
            InitializeComponent();

            ChartTitleTextBlock.Text = windowTitle;
            StockChartSeries.ItemsSource = data;
            StockChartSeries.Title = stockName;
        
        }
    }
}
