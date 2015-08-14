using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;



namespace MinesweeperSolver
{
    /// <summary>
    /// The Brain. All solving happens here.
    /// </summary>
    static class Solver
    {
        /// <summary>
        /// THE MAIN METHOD.
        /// </summary>
        public static void Solve()
        {
            try
            {
                var field = new Field();
                field.ReadFromScreen();
                if (false) field.ShowViaMessageBox();

                //Dumb solving
                if (true)
                {
                    bool clickedSomething = false;

                    if (field.HasNoClickedCells())
                    {
                        field.GetCell(15, 7).Click();
                        clickedSomething = true;
                    }
                    else
                    {
                        
                        foreach (var cell in field.IterateAllCells())
                        {
                            if (cell.IsNumberOfMines && cell.NumberOfMines > 0)
                            {
                                int numberOfFlagsNearby = 0;
                                int numberOfUnknownNearby = 0;
                                foreach (var nearbyCell in cell.IterateAllNearbyCells())
                                {
                                    if (nearbyCell.IsFlag) numberOfFlagsNearby++;
                                    if (nearbyCell.IsUnknown) numberOfUnknownNearby++;
                                }
                                if (numberOfUnknownNearby == cell.NumberOfMines && numberOfFlagsNearby == 0)
                                {
                                    if (false) MessageBox.Show(String.Format("{0}, {1}", cell.X, cell.Y)); //Debug
                                    foreach (var rightClickCell in cell.IterateAllNearbyCells())
                                    {
                                        if (rightClickCell.IsUnknown) rightClickCell.RightClick();
                                        clickedSomething = true;
                                    }
                                }
                                if (numberOfFlagsNearby == cell.NumberOfMines)
                                {
                                    if (false) MessageBox.Show(String.Format("{0}, {1}", cell.X, cell.Y)); //Debug
                                    foreach (var leftClickCell in cell.IterateAllNearbyCells())
                                    {
                                        if (leftClickCell.IsUnknown) leftClickCell.Click();
                                        clickedSomething = true;
                                    }
                                }
                            }
                        }

                        
                    }
                    if (!clickedSomething) throw new Exception("I don't know what to click on, I'm too dumb. Try clicking by yourself.");
                }


                //This is previous failed try of solving it, you can ignore it.
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


        */
                }

                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }



    }
}
