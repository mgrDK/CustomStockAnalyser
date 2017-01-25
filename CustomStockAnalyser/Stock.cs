using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;
using MathNet.Numerics;

namespace CustomStockAnalyser
{
    [Serializable]
    public class Stock
    {
        //nazwa rynku
        public string MarketName { get; set; }
        public string StockName { get; set; }
        //Ścieżka do pliku źródłowego z kursem akcji
        public string StockPath { get; set; }
        //źródło danych (potrzebne do ustalenia układu kolumn i wierszy w arkuszu)
        public string DataSource { get; set; }
        //Próbki nieprzetworzone - pobrane z arkusza źródłowego
        public List<Sample> RawSamples { get; set; }
        //Próbki przetworzone (np. utworzone z próbek jednostkowych przy podanej złożoności - Complexity)
        public List<Sample> Samples { get; set; }
       
       

        public Stock(string stockName, string stockPath, string dataSource)
        { 
            RawSamples = new List<Sample>();
            Samples = new List<Sample>();
          
            StockName = stockName;
            StockPath = stockPath;
            DataSource = dataSource;
        }

        /// <summary>
        /// Wczytywanie danych z arkusza dla danego instrumentu.
        /// </summary>

        public void LoadData()
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            string str;
            int rCnt = 0;
            int cCnt = 0;



            xlApp = new Excel.Application();
            //w nowszych wersjach rozszerzenie xlsx
            xlWorkBook = xlApp.Workbooks.Open(StockPath, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range = xlWorkSheet.UsedRange;

            for (rCnt = 2; rCnt <= range.Rows.Count; rCnt++)
            {

                Sample sample = new Sample();

                if (DataSource.Equals("http://www.gpwinfostrefa.pl/"))
                {
                    sample.OpenTime = ParseDateToDatetime((range.Cells[rCnt, 2] as Excel.Range).Value, 9);
                    sample.CloseTime = sample.OpenTime.AddHours(8);
                    sample.OpenValue = double.Parse((range.Cells[rCnt, 6] as Excel.Range).Value.ToString());
                    sample.MaxValue = double.Parse((range.Cells[rCnt, 7] as Excel.Range).Value.ToString());
                    sample.MinValue = double.Parse((range.Cells[rCnt, 8] as Excel.Range).Value.ToString());
                    sample.CloseValue = double.Parse((range.Cells[rCnt, 9] as Excel.Range).Value.ToString());
                    sample.ValueChange = double.Parse((range.Cells[rCnt, 10] as Excel.Range).Value.ToString());
                    sample.Volume = int.Parse((range.Cells[rCnt, 11] as Excel.Range).Value.ToString());
                    sample.TransactionsCount = int.Parse((range.Cells[rCnt, 12] as Excel.Range).Value.ToString());
                    sample.VolumeValue = double.Parse((range.Cells[rCnt, 13] as Excel.Range).Value.ToString());
                    sample.AverageValue = -1;
                }

                RawSamples.Add(sample);
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();

            ReleaseExcelObject(xlWorkSheet);
            ReleaseExcelObject(xlWorkBook);
            ReleaseExcelObject(xlApp);
        }

        /// <summary>
        /// Parsuje stringa do formatu DateTime z dokładnością do minut.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        private DateTime ParseDateToDatetime(string date, int hour = 0, int minute = 0)
        {
            string[] parts = date.Split(new char[] { '-' });
            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int day = int.Parse(parts[2]);

            return new DateTime(year, month, day, hour, minute, 0);
        }

        public void ProcessRawSamples(int complexity = 1, AverageValueType avgType = AverageValueType.AVERAGE_CLOSE)
        {
            for(int i = 0; i < RawSamples.Count; i += complexity)
            {
                //Średnia wartość (Average Value)
                double sum = 0;
                double min = RawSamples[i].MinValue;
                double max = RawSamples[i].MaxValue;

                for(int j=0; j < complexity; j++)
                {
                    if (avgType == AverageValueType.AVERAGE_CLOSE)
                        sum += RawSamples[i + j].CloseValue;
                    else if (avgType == AverageValueType.AVERAGE_OPEN)
                        sum += RawSamples[i + j].OpenValue;
                    else if (avgType == AverageValueType.AVERAGE_MIN)
                        sum += RawSamples[i + j].MinValue;
                    else if (avgType == AverageValueType.AVERAGE_MAX)
                        sum += RawSamples[i + j].MaxValue;
                    else if (avgType == AverageValueType.AVERAGE_MIDDLE)
                        sum += (RawSamples[i + j].MinValue + RawSamples[i + j].MaxValue) / 2;

                    if (RawSamples[i + j].MinValue < min)
                        min = RawSamples[i + j].MinValue;

                    if (RawSamples[i + j].MaxValue > max)
                        max = RawSamples[i + j].MaxValue;
                }

                Sample sample = new Sample();
                sample.OpenValue = RawSamples[i].OpenValue;
                sample.OpenTime = RawSamples[i].OpenTime;
                sample.CloseValue = RawSamples[i + complexity - 1].CloseValue;
                sample.CloseTime = RawSamples[i + complexity - 1].CloseTime;
                sample.MinValue = min;
                sample.MaxValue = max;
                sample.AverageValue = sum / complexity;
            }
        }

        /*
        /// <summary>
        /// Z RawSamples tworzy Present i Past Samples z określonymi parametrami złożoności i rozmiarem wzorca teraźniejszego przy użyciu m.in. określonego typu wartości średniej.
        /// </summary>
        /// <param name="presentSize"></param>
        /// <param name="presentComplexity"></param>
        /// <param name="pastComplexity"></param>
        public void SplitPastAndPresentSamples(int presentSize, int presentComplexity, int pastComplexity, AverageValueType avgType)
        {
            ///////// Próbki teraźniejsze:

            //index pierwszego elementu present na liście RawSamples
            int firstPresent = RawSamples.Count - presentSize * presentComplexity;

            for (int i = firstPresent; i < RawSamples.Count; i += presentComplexity)
            {
                //Średnia wartość (Average Value)
                double sum = 0;
                double min = RawSamples[i].MinValue;
                double max = RawSamples[i].MaxValue;

                for (int j = 0; j < presentComplexity; j++)
                {
                    if (avgType == AverageValueType.AVERAGE_CLOSE)
                        sum += RawSamples[i + j].CloseValue;
                    else if (avgType == AverageValueType.AVERAGE_OPEN)
                        sum += RawSamples[i + j].OpenValue;
                    else if (avgType == AverageValueType.AVERAGE_MIN)
                        sum += RawSamples[i + j].MinValue;
                    else if (avgType == AverageValueType.AVERAGE_MAX)
                        sum += RawSamples[i + j].MaxValue;
                    else if (avgType == AverageValueType.AVERAGE_MIDDLE)
                        sum += (RawSamples[i + j].MinValue + RawSamples[i + j].MaxValue) / 2;

                    if (RawSamples[i + j].MinValue < min)
                        min = RawSamples[i + j].MinValue;

                    if (RawSamples[i + j].MaxValue > max)
                        max = RawSamples[i + j].MaxValue;
                }

                Sample presentSample = new Sample();

                presentSample.OpenValue = RawSamples[i].OpenValue;
                presentSample.CloseValue = RawSamples[i + presentComplexity - 1].CloseValue;
                presentSample.MinValue = min;
                presentSample.MaxValue = max;
                presentSample.AverageValue = sum / presentComplexity;

                PresentSamples.Add(presentSample);
            }

            //próbki przeszłe:
            for (int i = 0; i < firstPresent - (pastComplexity - 1); i += pastComplexity)
            {
                double sum = 0;
                double min = 0;
                double max = 0;

                for (int j = 0; j < pastComplexity; j++)
                {
                    if (avgType == AverageValueType.AVERAGE_CLOSE)
                        sum += RawSamples[i + j].CloseValue;
                    else if (avgType == AverageValueType.AVERAGE_OPEN)
                        sum += RawSamples[i + j].OpenValue;
                    else if (avgType == AverageValueType.AVERAGE_MIN)
                        sum += RawSamples[i + j].MinValue;
                    else if (avgType == AverageValueType.AVERAGE_MAX)
                        sum += RawSamples[i + j].MaxValue;
                    else if (avgType == AverageValueType.AVERAGE_MIDDLE)
                        sum += (RawSamples[i + j].MinValue + RawSamples[i + j].MaxValue) / 2;

                    if (RawSamples[i + j].MinValue < min)
                        min = RawSamples[i + j].MinValue;

                    if (RawSamples[i + j].MaxValue > max)
                        max = RawSamples[i + j].MaxValue;
                }

                Sample pastSample = new Sample();

                pastSample.OpenValue = RawSamples[i].OpenValue;
                pastSample.CloseValue = RawSamples[i + pastComplexity - 1].CloseValue;
                pastSample.MinValue = min;
                pastSample.MaxValue = max;
                pastSample.AverageValue = sum / pastComplexity;

                PastSamples.Add(pastSample);
            }
        }

        
        */
       

        /// <summary>
        /// Zwraca z przedziału próbkę z minimalną wartością.
        /// </summary>
        /// <param name="range"></param>
        public Sample FindMinSampleInRange(int startIndex, int endIndex)
        {
            List<Sample> tempList = Samples.GetRange(startIndex, endIndex - startIndex + 1);
            double minValue = tempList.Min(x => x.MinValue);
            return tempList.Find(x => x.MinValue == minValue);
        }

        /// <summary>
        /// Zwraca z przedziału próbkę z maksymalną wartością.
        /// </summary>
        /// <param name="range"></param>
        public Sample FindMaxSampleInRange(int startIndex, int endIndex)
        {
            List<Sample> tempList = Samples.GetRange(startIndex, endIndex - startIndex + 1);
            double maxValue = tempList.Max(x => x.MaxValue);
            return tempList.Find(x => x.MaxValue == maxValue);
        }

       

        

        private void ReleaseExcelObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Console.WriteLine("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        } 
    }
}
