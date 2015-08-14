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
    /// The Brain.
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
                if (true) field.ShowViaMessageBox();

                //Dumb solving
                if (true)
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
                                }
                            }
                            if (numberOfFlagsNearby == cell.NumberOfMines)
                            {
                                if (false) MessageBox.Show(String.Format("{0}, {1}", cell.X, cell.Y)); //Debug
                                foreach (var leftClickCell in cell.IterateAllNearbyCells())
                                {
                                    if (leftClickCell.IsUnknown) leftClickCell.Click();
                                }
                            }

                        }
                    }
                }

                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }



    }
}
