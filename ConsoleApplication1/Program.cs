using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {/*
            //Finding all fields near numbers
            var perimeter = new int[field.Width, field.Height];
            var perimeterNumbers = new int[field.Width, field.Height];

            for (int fy = 0; fy < field.Height; fy++)
                for (int fx = 0; fx < field.Width; fx++)
                {
                    if (field.GetCell(fx, fy).IsUnknown)
                        foreach (var nearbyFieldXY in Nearby(fx, fy, field.Width, field.Height))
                        {
                            int nearbyX = nearbyFieldXY.Item1;
                            int nearbyY = nearbyFieldXY.Item2;
                            if (field.GetCell(nearbyX, nearbyY).isNumber)
                            {
                                perimeter[fx, fy] = 1;

                                perimeterNumbers[nearbyX, nearbyY] = 1;
                            }
                        }
                }

            var perimeterCoordinates = new List<Tuple<int, int>>(field.Width * field.Height);
            var perimeterNumberCoordinates = new List<Tuple<int, int>>(field.Width * field.Height);
            for (int fy = 0; fy < field.Height; fy++)
                for (int fx = 0; fx < field.Width; fx++)
                {
                    if (perimeter[fx, fy] == 1) perimeterCoordinates.Add(new Tuple<int, int>(fx, fy));
                    if (perimeterNumbers[fx, fy] == 1) perimeterNumberCoordinates.Add(new Tuple<int, int>(fx, fy));
                }



            var bigBitCounter = new BigInteger(0);
            var bigBitCounterMax = BigInteger.Pow(2, perimeterCoordinates.Count());
            for (; bigBitCounter < bigBitCounterMax; bigBitCounter++)
            {
                var probableField = field.Clone();
                BigInteger tempBitCounter = bigBitCounter;
                BigInteger tempSmallestBit;
                foreach (var onePerimeterCoordinate in perimeterCoordinates)
                {
                    tempBitCounter = BigInteger.DivRem(tempBitCounter, 2, out tempSmallestBit);
                    if (tempSmallestBit == 1)
                        probableField.GetCell(onePerimeterCoordinate.Item1, onePerimeterCoordinate.Item2).FileName = "F";
                    else probableField.GetCell(onePerimeterCoordinate.Item1, onePerimeterCoordinate.Item2).FileName = "O";
                }




                foreach (var onePerimeterNumberCoordinate in perimeterNumberCoordinates)
                {
                    int numX = onePerimeterNumberCoordinate.Item1;
                    int numY = onePerimeterNumberCoordinate.Item2;
                    int numberOfFlagsArround = 0;
                    foreach (var nearbyField in Nearby(numX, numY, probableField.Width, probableField.Height))
                    {
                        int perX = nearbyField.Item1;
                        int perY = nearbyField.Item2;

                        if (probableField.GetCell(perX, perY).FileName == "F") numberOfFlagsArround++;
                    }
                    //Debug.WriteLine("{2},{3}: Current: {0}, Real: {1}", numberOfFlagsArround, probableField.GetCell(numX, numY).Number, numX, numY);
                    if (numberOfFlagsArround != probableField.GetCell(numX, numY).Number) goto nextProbableField;
                }

                probableField.ShowViaMessageBox();

            nextProbableField:
                //Debug.WriteLine("Doh!");
                //probableField.ShowViaMessageBox();
                continue;
            }


        */}
    }
}
