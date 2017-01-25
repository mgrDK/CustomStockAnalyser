using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStockAnalyser
{
    public class ComparisonOutcome
    {
        /// <summary>
        /// nazwa akcji
        /// </summary>
        public string StockName { get; set; }
        /// <summary>
        /// czas otwarcia
        /// </summary>
        public DateTime OpenTime { get; set; }
        /// <summary>
        /// czas zamknięcia
        /// </summary>
        public DateTime CloseTime { get; set; }
        /// <summary>
        /// Operacja wykonana przy porównaniu
        /// </summary>
        public ComparisionOperation ComparisonOpearion { get; set; }
        /// <summary>
        /// Wartość otrzymana w wyniku porównania
        /// </summary>
        public double ComparisonValue { get; set; }
        /// <summary>
        /// Operacja używana do wyznaczenia wzorca.
        /// </summary>
        public PatternOperation PatternOperation { get; set; }
        /// <summary>
        /// Typ wartości poddawanej analizie. np. Close Value lub Max Value
        /// </summary>
        public StockValueType StockValueType { get; set; }
    }
}
