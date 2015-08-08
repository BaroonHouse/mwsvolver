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
        /// <summary>
        /// THE MAIN METHOD.
        /// </summary>
        public static void Solve()
        {
            bringMinesweeperToFront();

            //Bitmap bmpScreenshot = GetScreenBitmap();

            //Field field = ReadFieldFromBitmap(bmpScreenshot);

            //field.ShowViaMessageBox();





        }


        /// <summary>
        /// Splits minesweeper's field on screenshotBitmap into many 16x16 bitmaps,
        /// and then compares each of them to a presaved bitmaps in the bin/debug directory.
        /// Result is a Field object that reflects the content of the screen.
        /// </summary>
        /// <param name="bmpScreenshot">Bitmap for reading minesweeper window from. Usually a screenshot of a whole window.</param>
        /// <returns></returns>
        private static Field ReadFieldFromBitmap(Bitmap bmpScreenshot)
        {

            int windowX;
            int windowY;
            int fieldX;
            int fieldY;
            FindMinesweeperWindow(bmpScreenshot, out windowX, out windowY, out fieldX, out fieldY);

            //Debug
            if (false) Cursor.Position = new Point(windowX, windowY);

            

            //Debug
            Cursor.Position = new Point(fieldX, fieldY);



            //Create Field object, fill it with values from the screenshot, and return it.
            var field = new Field(30, 16);
            foreach (var cell in field.IterateAllCells())
            {
                Bitmap cellBitmap = bmpScreenshot.Clone(
                        new Rectangle(
                            fieldX + 16 * cell.X,
                            fieldY + 16 * cell.Y,
                            14,
                            14)
                        , bmpScreenshot.PixelFormat);

                bool isFamiliar = false;
                for (int i = 0; i < ImageFilesKeeper.CellLength; i++)
                {
                    if (BitmapsAreSame(cellBitmap, ImageFilesKeeper.CellBitmaps[i]))
                    {
                        isFamiliar = true;
                        field.GetCell(cell.X, cell.Y).IntRepresentation = ImageFilesKeeper.CellPossibleNumbers[i];
                        break;
                    }
                }
                if (!isFamiliar)
                {
                    cellBitmap.Save("WhatIsThisCell7.png", ImageFormat.Png);
                    throw new Exception("Hey, I didn't expected that! I can't read that one tile properly! Saved this in WhatIsThisCell7.png, look at this later.");

                }
            }

            return field;
        }

        /// <summary>
        /// Gets everything we see on the screen, and returns it as a bitmap.
        /// </summary>
        /// <returns></returns>
        internal static Bitmap GetScreenBitmap()
        {
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height,
                PixelFormat.Format32bppArgb);

            using (var gfxScreenshot = Graphics.FromImage(bmpScreenshot))
            {
                gfxScreenshot.CopyFromScreen(0, 0, 0, 0,
                    Screen.PrimaryScreen.Bounds.Size,
                    CopyPixelOperation.SourceCopy);
            }
            return bmpScreenshot;
        }






        /// <summary>
        /// Finds minesweeper's window on the bmpScreenshot and gets it's coordinates. 
        /// It simply searches the whole screen for a smiley face, 
        /// and then uses it to get the topmost leftmost pixel of a window.
        ///  
        /// Beware that it only works if Minesweeper is in the Expert mode, the one with largest field 
        /// of three modes available.
        /// </summary>
        /// <param name="bmpScreenshot">Image we search our window in. Usually a screenshot.</param>
        /// <param name="windowX">X coordinate of a window.</param>
        /// <param name="windowY">Y coordinate of a window.</param>
        /// <param name="fieldX">X coordinate of left upper courner of left upper cell.</param>
        /// <param name="fieldY">Y co... you have an idea.</param>
        static void FindMinesweeperWindow(Bitmap bmpScreenshot, out int windowX, out int windowY, out int fieldX, out int fieldY)
        {
            bool found = false;
            windowX = -1;
            windowY = -1;
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
                                windowX = ix;
                                windowY = iy;
                                found = true;
                            }
                        }

                    }
                }


            if (!found) throw new Exception("Minesweeper window not found!");
            windowX = windowX - 247;
            windowY = windowY - 18;
            //save the first square, for debugging
            //place cursor on the first square
            //734.178 -- yellow smiley's head's top
            //497.213 -- first square
            //487.160 -- topleft courner of the window

            fieldX = windowX + 10;
            fieldY = windowY + 53;
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

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void bringMinesweeperToFront()
        {
            //From http://stackoverflow.com/questions/2636721/bring-another-processes-window-to-foreground-when-it-has-showintaskbar-false

            //TODO: doesn't work if minimised

            // Get a handle to the Calculator application.
            IntPtr handle = FindWindow(null, "Minesweeper");

            // Verify that Calculator is a running process.
            if (handle == IntPtr.Zero)
            {
                return;
            }

            // Make Calculator the foreground application
            SetForegroundWindow(handle);
        }
    }
}
