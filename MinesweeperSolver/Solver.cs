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
using System.Numerics;
namespace MinesweeperSolver
{
    /// <summary>
    /// The Brain. The Eyes. The Hands. All this is here.
    /// </summary>
    static class Solver
    {
        public static void Solve()
        {
            //First we must see the field.
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height,
                PixelFormat.Format32bppArgb);

            using (var gfxScreenshot = Graphics.FromImage(bmpScreenshot))
            {
                gfxScreenshot.CopyFromScreen(0, 0, 0, 0,
                Screen.PrimaryScreen.Bounds.Size,
                CopyPixelOperation.SourceCopy);
            }


            int windowX;
            int windowY;
            FindMinesweeperWindow(bmpScreenshot, out windowX, out windowY);


            //Cursor.Position = new Point(windowX, windowY);

            //save the first square, for debugging
            //place cursor on the first square
            //734.178 -- yellow smiley's head's top
            //497.213 -- first square
            //487.160 -- topleft courner of the window

            int fieldX = windowX + 10;
            int fieldY = windowY + 53;

            //Check if we found upper left courner correctly.
            Cursor.Position = new Point(fieldX, fieldY);

            


            var familiarCellNameBitmaps = new List<Tuple<Bitmap, string>>(Cell.familiarFieldsFileNames.Length);
            for (int i = 0; i < Cell.familiarFieldsFileNames.Length; i++ )
            {
                familiarCellNameBitmaps.Add(new Tuple<Bitmap, string>(
                    (Bitmap)Image.FromFile(Cell.familiarFieldsFileNames[i] + ".png"),
                    Cell.familiarFieldsFileNames[i]
                    ));
            }




            var field = new Field(30, 16);
            for (int fy = 0; fy < field.Height; fy++)
            {
                for (int fx = 0; fx < field.Width; fx++)
                {
                    Bitmap cellBitmap = bmpScreenshot.Clone(
                        new Rectangle(
                            fieldX + 16*fx,
                            fieldY + 16*fy,
                            14,
                            14)
                        , bmpScreenshot.PixelFormat);

                    bool isFamiliar = false;
                    foreach (var familiarCellBitmap in familiarCellNameBitmaps)
                    {
                        if (BitmapsAreSame(cellBitmap, familiarCellBitmap.Item1))
                        {
                            isFamiliar = true;
                            field.GetCell(fx, fy).FileName = familiarCellBitmap.Item2;
                            break;
                        }
                    }
                    if (!isFamiliar)
                    {
                        cellBitmap.Save("huh" + familiarCellNameBitmaps.Count.ToString() + ".png", ImageFormat.Png);
                        throw new Exception("Hey, I didn't expected that! I can't read that one tile properly!");

                    }
                }
            }

            //field.ShowViaMessageBox();


            //Checking readed data
            //MessageBox.Show(String.Join("", modelString));

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





        }



        static IEnumerable<Tuple<int,int>> Nearby(int fx, int fy, int X_SIZE, int Y_SIZE)
        {
            for (int tempx = fx - 1; tempx <= fx+1; tempx++)
                for (int tempy = fy - 1; tempy <= fy+1; tempy++)
                    if (0 <= tempx && tempx < X_SIZE && 0 <= tempy && tempy < Y_SIZE && !(tempx == fx && tempy == fy))
                    {
                        yield return new Tuple<int, int>(tempx, tempy);
                    }
        }









        /// <summary>
        /// Use this function to find Minesweeper window on the screen and get it's coordinates. 
        /// It simply searches the whole screen for a smiley face, 
        /// and then uses it to get the topmost leftmost pixel of a window.
        ///  
        /// Beware that it only works if Minesweeper is in the Expert mode, the one with largest field 
        /// of three modes available.
        /// </summary>
        /// <param name="bmpScreenshot">Image we search our window in. Usually a screenshot.</param>
        /// <param name="windX">X coordinate of a window.</param>
        /// <param name="windY">Y coordinate of a window.</param>
        static void FindMinesweeperWindow(Bitmap bmpScreenshot, out int windX, out int windY)
        {
            bool found = false;
            windX = -1;
            windY = -1;
            for (int iy = 0; iy < bmpScreenshot.Height && !found; iy++)
                for (int ix = 0; ix < bmpScreenshot.Width && !found; ix++)
                {
                    if (bmpScreenshot.GetPixel(ix, iy).ToArgb() == Color.Yellow.ToArgb())
                    {
                        //If we are here, when we just found the left pixel of upper row of smiley's yellow head.
                        //All checks after this point are just to make sure we found smiley and not just random yellow pixel.
                        //We could compare the whole bitmap pixel by pixel, but this is probably faster.

                        Debug.WriteLine("Yellow Coords: {0}, {1}", ix, iy);

                        if (bmpScreenshot.GetPixel(ix, iy - 1).ToArgb() == Color.Black.ToArgb() &&
                            bmpScreenshot.GetPixel(ix - 1, iy).ToArgb() == Color.Black.ToArgb())
                        {
                            if (bmpScreenshot.GetPixel(ix - 1, iy - 1).ToArgb() == Color.Silver.ToArgb())
                            {
                                windX = ix;
                                windY = iy;
                                found = true;
                            }
                        }

                    }
                }


            if (!found) throw new Exception("Minesweeper window not found!");
            windX = windX - 247;
            windY = windY - 18;
        }


        static bool BitmapsAreSame(Bitmap b1, Bitmap b2)
        {
            for (int ix = 0; ix < 14; ix++)
                for (int iy = 0; iy < 14; iy++)
                {
                    if (b1.GetPixel(ix, iy) != b2.GetPixel(ix, iy)) return false;
                }
            return true;
        }
    }
}
