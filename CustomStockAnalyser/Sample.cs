using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStockAnalyser
{
    public class Sample
    {
        public Sample()
        { }

        public Sample(double value)
        {
            this.CloseValue = value;
        }

        //czas otwarcia
        public DateTime OpenTime { get; set; }
        //czas zamknięcia
        public DateTime CloseTime { get; set; }
        //wartość otwarcia
        public double OpenValue { get; set; }
        //wartość zamknięcia
        public double CloseValue { get; set; }

        //Zmiana wartości w przedziale
        public double ValueChange { get; set; }

        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double AverageValue { get; set; }

        //Wolumen obrotu (liczba akcji)
        public int Volume { get; set; }
        //Wartość obrotu (w danej walucie)
        public double VolumeValue { get; set; }
        public int TransactionsCount { get; set; }
    }
}
