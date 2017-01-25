using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStockAnalyser
{
    public static class StockIndicators
    {
        /// <summary>
        /// Oblicza błąd średniokwadratowy zbioru wartości do podanej w stringu funkcji trygonometrycznej.  Każda kolejna wartość przyrównywana jest do wartości funkcji dla kąta większego o angleDelta zaczynając od angleStarter.
        /// </summary>
        /// <param name="sampleValues"></param>
        /// <param name="radianDelta"></param>
        /// <param name="trigFunction"></param>
        public static double MeanSquareForTrig(List<double> sampleValues, string trigFunction, double angleDelta, double angleStarter = 0)
        {
            double currentAngle = angleStarter;
            double[] funValues = new double[sampleValues.Count];


            for (int i = 0; i < sampleValues.Count; i++)
            {
                if (trigFunction.Equals("sinus") || trigFunction.Equals("sin"))
                    funValues[i] = Math.Sin(currentAngle);
                else if (trigFunction.Equals("cosinus") || trigFunction.Equals("cos"))
                    funValues[i] = Math.Cos(currentAngle);
                else if (trigFunction.Equals("tangens") || trigFunction.Equals("tan"))
                    funValues[i] = Math.Tan(currentAngle);

                currentAngle += angleDelta;
            }

            return MathNet.Numerics.Distance.MSE(funValues, sampleValues.ToArray());
        }

        /// <summary>
        /// Oblicza błąd średniokwadratowy zbioru wartości do wielomianu o określonych współczynnikach. Każda kolejna wartość przyrównywana jest do wartości wielomianu w punkcie przesuniętym o określoną liczbę xDelta zaczynając od xStarter.
        /// </summary>
        /// <param name="sampleValues"></param>
        /// <param name="radianScale"></param>
        /// <param name="polymonialCoefficients"></param>
        public static double MeanSquareErrorForPolymonial(List<double> sampleValues, double[] polymonialCoefficients, double xDelta, double xStarter = 0)
        {
            //oszacuj wartość wielomianu w punkcie Evaluate Polymonial
            //http://numerics.mathdotnet.com/api/MathNet.Numerics/Evaluate.htm
            // oblicz bład średnio kwadratowy wartości wielomianu i punktu który porównujemy

            double currentX = xStarter;
            double[] funValues = new double[sampleValues.Count];


            for (int i = 0; i < sampleValues.Count; i++)
            {
                funValues[i] = MathNet.Numerics.Evaluate.Polynomial(sampleValues[i], polymonialCoefficients);
                currentX += xDelta;
            }

            return MathNet.Numerics.Distance.MSE(funValues, sampleValues.ToArray());

        }

        /// <summary>
        /// Zwraca listę najbardziej znaczących szczytów.
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="minCount">minimalna liczba szczytów</param>
        /// <param name="maxCount">maksymalna liczba szczytów</param>
        /// <param name="peakVicinity">liczba następnych próbek, które muszą być mniejsze od szczytu</param>
        /// <returns></returns>
        public static List<Sample> GetPeaks(Stock stock, int minCount, int maxCount, int peakVicinity = 5)
        {
           int areaWidth = (int) Math.Ceiling( (double)stock.Samples.Count / minCount); // początkowa szerokość obszaru

            //Regulacja parametru areaWidth
            for (; areaWidth >= Math.Ceiling((double)stock.Samples.Count / maxCount); areaWidth--)
            {
                //szczyty
                List<Sample> peaks = new List<Sample>();

                //Sprawdzanie kolejnych obszarów w celu znalezienia szczytu
                for (int i = 0; i < stock.Samples.Count - (areaWidth - 1); i = i + areaWidth)
                {
                    Sample localMax = stock.FindMaxSampleInRange(i, i + areaWidth - 1);
                    int maxIndex = stock.Samples.FindIndex(x => ReferenceEquals(x, localMax));
                    Sample vicinityMax;

                    //Jeżeli w kolekcji jest mniej niż peakVicinity próbek do końca to skończ na ostatnim elemencie
                    if (maxIndex + peakVicinity >= stock.Samples.Count)
                        vicinityMax = stock.FindMaxSampleInRange(maxIndex, stock.Samples.Count - 1);
                    else
                        vicinityMax = stock.FindMaxSampleInRange(maxIndex, maxIndex + peakVicinity);

                    if (ReferenceEquals(vicinityMax, localMax))
                        peaks.Add(localMax);

                }

                if (peaks.Count >= minCount && peaks.Count <= maxCount)
                    return peaks;
            }

            return null;
        }

        public static List<Sample> GetValleys()
        {
            return null;
        }

        /// <summary>
        /// Analizuje podane notowania (wybrane okno czasowe) i określa typ rynku. 
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        public static MarketType CheckMarketType(List<Sample> samples)
        {
            return MarketType.UNIDENTIFIED;
        }
    }
}
