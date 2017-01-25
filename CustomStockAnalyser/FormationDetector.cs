using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathNet.Numerics;

namespace CustomStockAnalyser
{
    public class FormationDetector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stock">Obiekt z danymi notowań.</param>
        /// <param name="minDistance">Minimalny odstęp na osi poziomej pomiędzy ekstremami.</param>
        /// <param name="maxAreaWidth">Szerokość obszaru w którym szukane jest kolejne ekstremum.</param>
        /// <param name="headAndShoulderDeltaYMin">Minimalna różnica procentowa pomiędzy najwyższym ramieniem, a głową.</param>
        /// <param name="deltaYFromAverageMax">Maksymalna różnica procentowa pomiędzy średnią punktów, a punktem dla ramion i minimów.</param>
        public void DetectHeadAndShouldersFormation(Stock stock, int minDistance, int maxAreaWidth, double headAndShoulderDeltaYMin = 2, double deltaYFromAverageMax = 1.5)
        {
            List<Sample> extrems = new List<Sample>();
            int index = 0;

            while(index < (stock.Samples.Count - (4 * (maxAreaWidth + minDistance) + maxAreaWidth )))
            {

                //trzeba wykryć ekstrema
                Sample firstShoulder = stock.FindMaxSampleInRange(index, index + maxAreaWidth);
                index = stock.Samples.FindIndex(x => x.OpenTime == firstShoulder.OpenTime);
                extrems.Add(firstShoulder);
       
                double [] extremeValues = new double[5];
                extremeValues[0] = firstShoulder.MaxValue;
   
                for(int i = 1; i < 5; i++)
                {
                    index += minDistance;

                    if (i % 2 == 0)
                    {
                        Sample localMax = stock.FindMaxSampleInRange(index, index + maxAreaWidth);
                        extrems.Add(localMax);
                        extremeValues[i] = localMax.MaxValue;
                    }
                    else
                    {
                        Sample localMin = stock.FindMinSampleInRange(index, index + maxAreaWidth);
                        extrems.Add(localMin);
                        extremeValues[i] = localMin.MinValue;
                    }

                }
                
                    bool detected = CheckHeadAndShouldersConditions(extremeValues, headAndShoulderDeltaYMin, deltaYFromAverageMax);

                    if (detected == true)
                    {
                        index = stock.Samples.FindIndex(x => x.OpenTime == extrems[extrems.Count - 1].OpenTime);
                        ++index;
                    }
                    else
                    {
                        index = stock.Samples.FindIndex(x => x.OpenTime == extrems[2].OpenTime);
                        --index;
                    }
                               
            }
        }

        /// <summary>
        /// Sprawdza warunki wystąpienia formacji na podstawie podanych lokalnych ekstremów w tablicy.
        /// </summary>
        /// <param name="localExtrems">Lokalne ekstrema</param>
        /// <param name="headAndShoulderDeltaYMin">Minimalna różnica procentowa pomiędzy najwyższym ramieniem, a głową.</param>
        /// <param name="deltaYFromAverageMax">Maksymalna różnica procentowa pomiędzy średnią punktów, a punktem dla ramion i minimów.</param>
        /// <returns></returns>
        public bool CheckHeadAndShouldersConditions(double [] localExtrems, double headAndShoulderDeltaYMin, double deltaYFromAverageMax)
        {
            /*
             5 ekstremów w formacji Head and Shoulders:
             - e1 - pierwsze ramię
             - e2 - pierwszę minimum
             - e3 - głowa
             - e4 - drugie minimum
             - e5 - drugie ramię
             */

            //jeżeli liczba lokalnych extremów jest różna od 5 to rzuć wyjątek
            if (localExtrems.Length != 5)
                throw new ArgumentException("Liczba ekstremów w tablicy musi wynosić 5!");

           //Pierwsze minimum nie jest mniejsze od ramion
            if (localExtrems[1] >= localExtrems[0] || localExtrems[1] >= localExtrems[4])
                return false;

            //Drugie minimum nie jest mniejsze od ramion
            if (localExtrems[3] >= localExtrems[0] || localExtrems[3] >= localExtrems[4])
                return false;

            double headToShoulder = localExtrems[2] / localExtrems[0];

            //Zbyt mała różnica pomiędzy głową, a pierwszym ramieniem
            if ((headToShoulder - 1) * 100 < headAndShoulderDeltaYMin)
                return false;

            headToShoulder = localExtrems[2] / localExtrems[4];

            //Zbyt mała różnica pomiędzy głową, a drugim ramieniem
            if ((headToShoulder - 1) * 100 < headAndShoulderDeltaYMin)
                return false;


            double shouldersMean = (localExtrems[0] + localExtrems[4]) / 2;
            double minsMean = (localExtrems[1] + localExtrems[3]) / 2;

            double e1ToShouldersMean = GetPercentDifference(localExtrems[0], shouldersMean);
            double e5ToShouldersMean = GetPercentDifference(localExtrems[4], shouldersMean);

            double e2ToMinsMean = GetPercentDifference(localExtrems[1], minsMean);
            double e4ToMinsMean = GetPercentDifference(localExtrems[3], minsMean);


            /*Kolejne warunki negatywne:
             - gdy stosunek ramion do ich średniej wartości jest większy od deltaFromAverageMax [%]
             - gdy stosunek minimów do ich średniej wartości jest większy od deltaFromAverageMax [%]
             */


            if (e1ToShouldersMean > deltaYFromAverageMax ||
                e5ToShouldersMean > deltaYFromAverageMax ||
                e2ToMinsMean > deltaYFromAverageMax ||
                e4ToMinsMean > deltaYFromAverageMax)
                return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stock">Obiekt z danymi notowań.</param>
        /// <param name="minDistance">Minimalny odstęp na osi poziomej pomiędzy ekstremami.</param>
        /// <param name="maxAreaWidth">Szerokość obszaru w którym szukane jest kolejne ekstremum.</param>
        /// <param name="headAndShoulderDeltaYMin">Minimalna różnica procentowa pomiędzy najwyższym ramieniem, a głową.</param>
        /// <param name="deltaYFromAverageMax">Maksymalna różnica procentowa pomiędzy średnią punktów, a punktem dla ramion i minimów.</param>
        public void DetectInvertedHeadAndShouldersFormation(Stock stock, int minDistance, int maxAreaWidth, double headAndShoulderDeltaYMin = 2, double deltaYFromAverageMax = 1.5)
        {
            List<Sample> extrems = new List<Sample>();
            int index = 0;

            while (index < (stock.Samples.Count - (4 * (maxAreaWidth + minDistance) + maxAreaWidth)))
            {

                //trzeba wykryć ekstrema
                Sample firstShoulder = stock.FindMinSampleInRange(index, index + maxAreaWidth);
                index = stock.Samples.FindIndex(x => x.OpenTime == firstShoulder.OpenTime);
                extrems.Add(firstShoulder);

                double[] extremeValues = new double[5];
                extremeValues[0] = firstShoulder.MinValue;

                for (int i = 1; i < 5; i++)
                {
                    index += minDistance;

                    if (i % 2 == 0)
                    {
                        Sample localMin = stock.FindMinSampleInRange(index, index + maxAreaWidth);
                        extrems.Add(localMin);
                        extremeValues[i] = localMin.MinValue;
                    }
                    else
                    {
                        Sample localMax = stock.FindMaxSampleInRange(index, index + maxAreaWidth);
                        extrems.Add(localMax);
                        extremeValues[i] = localMax.MaxValue;
                    }

                }

                bool detected = CheckInvertedHeadAndShouldersConditions(extremeValues, headAndShoulderDeltaYMin, deltaYFromAverageMax);
                
                if (detected == true)
                {
                    index = stock.Samples.FindIndex(x => x.OpenTime == extrems[extrems.Count - 1].OpenTime);
                    ++index;
                }
                else
                {
                    index = stock.Samples.FindIndex(x => x.OpenTime == extrems[2].OpenTime);
                    --index;
                }

            }
           

        }

        /// <summary>
        /// Sprawdza warunki wystąpienia formacji na podstawie podanych lokalnych ekstremów w tablicy.
        /// </summary>
        /// <param name="localExtrems">Lokalne ekstrema</param>
        /// <param name="headAndShoulderDeltaYMin">Minimalna różnica procentowa pomiędzy najwyższym ramieniem, a głową.</param>
        /// <param name="deltaYFromAverageMax">Maksymalna różnica procentowa pomiędzy średnią punktów, a punktem dla ramion i maksimów.</param>
        /// <returns></returns>
        public bool CheckInvertedHeadAndShouldersConditions(double [] localExtrems, double headAndShoulderDeltaYMin, double deltaYFromAverageMax)
        {

            /*
           5 ekstremów w formacji Inverted Head and Shoulders:
           - e1 - pierwsze ramię
           - e2 - pierwszę maksimum
           - e3 - głowa
           - e4 - drugie maksimum
           - e5 - drugie ramię
           */

            //jeżeli liczba lokalnych extremów jest różna od 5 to rzuć wyjątek
            if (localExtrems.Length != 5)
                throw new ArgumentException("Liczba ekstremów w tablicy musi wynosić 5!");

            //Pierwsze maksimum nie jest większe od ramion
            if (localExtrems[1] <= localExtrems[0] || localExtrems[1] <= localExtrems[4])
                return false;
            
            //Drugie maksimum nie jest większe od ramion
            if (localExtrems[3] <= localExtrems[0] || localExtrems[3] <= localExtrems[4])
                return false;

            double shoulderToHead = localExtrems[0] / localExtrems[2];

            //Zbyt mała różnica pomiędzy głową, a pierwszym ramieniem
            if ((shoulderToHead - 1) * 100 < headAndShoulderDeltaYMin)
                return false;

            shoulderToHead = localExtrems[4] / localExtrems[2];

            //Zbyt mała różnica pomiędzy głową, a drugim ramieniem
            if ((shoulderToHead - 1) * 100 < headAndShoulderDeltaYMin)
                return false;

            double shouldersMean = (localExtrems[0] + localExtrems[4]) / 2;
            double maxMean = (localExtrems[1] + localExtrems[3]) / 2;

            double e1ToShouldersMean = GetPercentDifference(localExtrems[0], shouldersMean);
            double e5ToShouldersMean = GetPercentDifference(localExtrems[4], shouldersMean);

            double e2ToMinsMean = GetPercentDifference(localExtrems[1], maxMean);
            double e4ToMinsMean = GetPercentDifference(localExtrems[3], maxMean);


            /*Kolejne warunki negatywne:
             - gdy stosunek ramion do ich średniej wartości jest większy od deltaFromAverageMax [%]
             - gdy stosunek maksimów do ich średniej wartości jest większy od deltaFromAverageMax [%]
             */


            if (e1ToShouldersMean > deltaYFromAverageMax ||
                e5ToShouldersMean > deltaYFromAverageMax ||
                e2ToMinsMean > deltaYFromAverageMax ||
                e4ToMinsMean > deltaYFromAverageMax)
                return false;

            return true;
        }

        /// <summary>
        /// Wykrywa formację Descending Triangle
        /// </summary>
        /// <param name="stock">Obiekt z danymi notowań</param>
        /// <param name="maxAreaWidth">Szerokość obszaru, w którym szukane jest ekstremum lokalne.</param>
        /// <param name="maxDeltaYFromAverageOfValleys">Maksymalna różnica procentowa pomiędzy średnią dołków, a dołkiem.</param>
        public void DetectDescendingTrianglesFormation(Stock stock, int maxAreaWidth, int maxDeltaYFromAverageOfValleys, int upperTrendlineFit)
        {
            //For Bull market

           /* List<Sample> extremums = new List<Sample>();
            List<double> extremeValues = new List<double>();

            //musze znalezc indeks ostatniego ekstremum i dopiero od niego szukać a nie zawsze + maxAreaWidth: for => while

            int index = 0;

            while (index < stock.Samples.Count - (maxAreaWidth - 1))
            {
                if (extremums.Count % 2 == 0)
                {
                    Sample localMax = stock.FindMaxSampleInRange(index, index + (maxAreaWidth - 1));
                    extremums.Add(localMax);
                    extremeValues.Add(localMax.MaxValue);
                }
                else
                {
                    Sample localMin = stock.FindMinSampleInRange(index, index + (maxAreaWidth - 1));
                    extremums.Add(localMin);
                    extremeValues.Add(localMin.MinValue);
                }

                index += maxAreaWidth;
            }

            //Analizuj piątkami i wykryj potencjalną formację
            for(int i = 0; i < extremums.Count - 4; i += 5)
            {
                double [] temp = new double[5];

                for(int j = 0; j < 5; j++)
                {
                    temp[j] = extremeValues[i+j];
                }

                CheckDescendingTrianglesConditions(temp, maxDeltaYFromAverageOfValleys, true);
            }

            */
          
        }

        public bool CheckDescendingTrianglesConditions(double [] localExtrems, int maxDeltaYFromAverageOfValleys, bool isBullMarket)
        {
            if (isBullMarket == true)
            {
                double average = (localExtrems[1] + localExtrems[3] + localExtrems[5]) / 3;
            }

            return true;
        }

        /// <summary>
        /// Oblicza wartość bezwzględną procentowej różnicy pomiędzy jedną a drugą wartością.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public double GetPercentDifference(double value, double target)
        {
            double x = value / target;
            return (x > 1) ? (x - 1) * 100 : (1 - x) * 100;
        }
    }
}
