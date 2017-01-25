using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStockAnalyser
{
    public class CorrelactionDetector
    {
        public Stock Stock { get; private set; }

        public CorrelactionDetector(Stock stock)
        {
            this.Stock = stock;
        }
        
      

        /// <summary>
        /// Tworzy wzorzec teraźniejszy o określonym rozmiarze stosując wybraną PatternOperation na bazie wybranego typu wartości StockValueType.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="valueType"></param>
        public List<double> CreatePresentPattern(int presentPatternSize, PatternOperation op, StockValueType valueType)
        {
            List<double> presentPattern = new List<double>();

            //index pierwszego elementu present na liście Samples
            int firstPresent = Stock.Samples.Count - presentPatternSize;

            for (int i = firstPresent; i < Stock.Samples.Count - 1; i++)
            {
                if (op == PatternOperation.DIVISION)
                {
                    if (valueType == StockValueType.CLOSE_VALUE)
                        presentPattern.Add(Stock.Samples[i + 1].CloseValue / Stock.Samples[i].CloseValue);
                    else if (valueType == StockValueType.OPEN_VALUE)
                        presentPattern.Add(Stock.Samples[i + 1].OpenValue / Stock.Samples[i].OpenValue);
                    else if (valueType == StockValueType.MIN_VALUE)
                        presentPattern.Add(Stock.Samples[i + 1].MinValue / Stock.Samples[i].MinValue);
                    else if (valueType == StockValueType.MAX_VALUE)
                        presentPattern.Add(Stock.Samples[i + 1].MaxValue / Stock.Samples[i].MaxValue);
                    else if (valueType == StockValueType.AVERAGE_VALUE)
                        presentPattern.Add(Stock.Samples[i + 1].AverageValue / Stock.Samples[i].AverageValue);
                }
                else if (op == PatternOperation.SUBJECTION)
                {
                    if (valueType == StockValueType.CLOSE_VALUE)
                        presentPattern.Add(Stock.Samples[i + 1].CloseValue - Stock.Samples[i].CloseValue);
                    else if (valueType == StockValueType.OPEN_VALUE)
                        presentPattern.Add(Stock.Samples[i + 1].OpenValue - Stock.Samples[i].OpenValue);
                    else if (valueType == StockValueType.MIN_VALUE)
                        presentPattern.Add(Stock.Samples[i + 1].MinValue - Stock.Samples[i].MinValue);
                    else if (valueType == StockValueType.MAX_VALUE)
                        presentPattern.Add(Stock.Samples[i + 1].MaxValue - Stock.Samples[i].MaxValue);
                    else if (valueType == StockValueType.AVERAGE_VALUE)
                        presentPattern.Add(Stock.Samples[i + 1].AverageValue - Stock.Samples[i].AverageValue);
                }
            }

            return presentPattern;
        }

        //Operuj na Samples a nie na PastSamples -> granicę końcową dla Samples ustal na podstawie presentPattern.Count
        //np. dla Count = 4 zostało wykorzystane 5 próbek ==> Samples.Count - (PresentPattern.Count + 1) ==> indeks pierwszego el. do Present Pattern

        /// <summary>
        /// Tworzy wzorce w przeszłości zgodnie z patternOp i porównuje je z wzorcem teraźniejszym według compOp bazując na wybranym typie wartości StockValueType. Następnie zapisuje najlepsze porównania na liście BestMatches.
        /// </summary>
        /// <param name="patternOp"></param>
        /// <param name="compOp"></param>
        /// <param name="valueType"></param>
        /// <param name="bestMatchesCount"></param>
        public List<ComparisonOutcome> CompareToPresentPattern(List<double> presentPattern, PatternOperation patternOp, ComparisionOperation compOp, StockValueType valueType, int bestMatchesCount)
        {
            List<ComparisonOutcome> bestMatches = new List<ComparisonOutcome>();

            bestMatches.Clear();

            int firstPresent = Stock.Samples.Count - presentPattern.Count;

            for (int i = 0; i < Stock.Samples.Count - (2 * presentPattern.Count); i++)
            {
                List<double> pastPattern = new List<double>();

                if (patternOp == PatternOperation.DIVISION)
                {
                    for (int j = 0; j < presentPattern.Count; j++)
                    {
                        if (valueType == StockValueType.CLOSE_VALUE)
                            pastPattern.Add(Stock.Samples[i + j + 1].CloseValue / Stock.Samples[i + j].CloseValue);
                        else if (valueType == StockValueType.OPEN_VALUE)
                            pastPattern.Add(Stock.Samples[i + j + 1].OpenValue / Stock.Samples[i + j].OpenValue);
                        else if (valueType == StockValueType.MIN_VALUE)
                            pastPattern.Add(Stock.Samples[i + j + 1].MinValue / Stock.Samples[i + j].MinValue);
                        else if (valueType == StockValueType.MAX_VALUE)
                            pastPattern.Add(Stock.Samples[i + j + 1].MaxValue / Stock.Samples[i + j].MaxValue);
                        else if (valueType == StockValueType.AVERAGE_VALUE)
                            pastPattern.Add(Stock.Samples[i + j + 1].AverageValue / Stock.Samples[i + j].AverageValue);
                    }
                }
                else if (patternOp == PatternOperation.SUBJECTION)
                {
                    for (int j = 0; j < presentPattern.Count; j++)
                    {
                        if (valueType == StockValueType.CLOSE_VALUE)
                            pastPattern.Add(Stock.Samples[i + j + 1].CloseValue - Stock.Samples[i + j].CloseValue);
                        else if (valueType == StockValueType.OPEN_VALUE)
                            pastPattern.Add(Stock.Samples[i + j + 1].OpenValue - Stock.Samples[i + j].OpenValue);
                        else if (valueType == StockValueType.MIN_VALUE)
                            pastPattern.Add(Stock.Samples[i + j + 1].MinValue - Stock.Samples[i + j].MinValue);
                        else if (valueType == StockValueType.MAX_VALUE)
                            pastPattern.Add(Stock.Samples[i + j + 1].MaxValue - Stock.Samples[i + j].MaxValue);
                        else if (valueType == StockValueType.AVERAGE_VALUE)
                            pastPattern.Add(Stock.Samples[i + j + 1].AverageValue - Stock.Samples[i + j].AverageValue);
                    }
                }

                // dokonaj porównania

                double comparisonValue = 0;
                //Średnia arytmetyczna ilorazów wartości z wzorca teraźniejszego do przeszłego
                if (compOp == ComparisionOperation.ARITHMETIC_MEAN_DIVISION)
                {
                    List<double> values = new List<double>();

                    for (int j = 0; j < presentPattern.Count; i++)
                        values.Add(presentPattern[j] / pastPattern[j]);

                    comparisonValue = MathNet.Numerics.Statistics.Statistics.Mean(values);


                }
                else if (compOp == ComparisionOperation.ARITHMETIC_MEAN_SUBJECTION)
                {
                    List<double> values = new List<double>();

                    for (int j = 0; j < presentPattern.Count; j++)
                        values.Add(presentPattern[j] - pastPattern[j]);

                    comparisonValue = MathNet.Numerics.Statistics.Statistics.Mean(values);


                }
                else if (compOp == ComparisionOperation.MEAN_SQUARED_ERROR)
                {
                    comparisonValue = MathNet.Numerics.Distance.MSE(presentPattern.ToArray(), pastPattern.ToArray());
                }

                ComparisonOutcome outcome = new ComparisonOutcome();
                outcome.ComparisonValue = comparisonValue;
                outcome.ComparisonOpearion = compOp;
                outcome.PatternOperation = patternOp;
                outcome.OpenTime = Stock.Samples[i].OpenTime;
                outcome.CloseTime = Stock.Samples[i + pastPattern.Count - 1].CloseTime;
                outcome.StockValueType = valueType;
                outcome.StockName = Stock.StockName;

                if (bestMatches.Count == 0)
                {
                    bestMatches.Add(outcome);
                    continue;
                }

                try
                {
                    //Znajdź index pierwszego elementu, który jest gorzej dopasowany do wzorca, najlepiej dopasowany jest początek listy
                    int index = bestMatches.FindIndex(x => x.ComparisonValue > comparisonValue);

                    bestMatches.Insert(index, outcome);

                    //Jeżeli liczba przechowywanych wyników jest za długa to usuń najgorszy wynik
                    if (bestMatches.Count > bestMatchesCount)
                        bestMatches.RemoveAt(bestMatches.Count - 1);

                    
                }
                catch (ArgumentNullException aE)
                {
                    //żaden element nie spełnia warunku
                }
                                

            }

            return bestMatches;
        }
    }
}
