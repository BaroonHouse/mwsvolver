using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MinesweeperSolver
{
    /// <summary>
    /// This is a representation of a minefield.
    /// </summary>
    class Field
    {
        const int _width = 30;
        const int _height = 16;
        readonly ImageFilesKeeper.PossibleFieldsEnum[,] _theField;

        /// <summary>
        /// Set to true if data in this class may differ from that is on the screen.
        /// </summary>
        bool _outdated = true;
        private int _fieldXOnScreen;
        private int _fieldYOnScreen;

        /// <summary>
        /// Creates new representation of a minefield.
        /// </summary>
        internal Field()
        {
            _theField = new ImageFilesKeeper.PossibleFieldsEnum[_width, _height];
        }

        /// <summary>
        /// Number of columns in field.
        /// </summary>
        internal int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// Number of rows in field.
        /// </summary>
        internal int Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Returns Cell at coordinates x and y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal Cell GetCell(int x, int y)
        {
            if (0 <= x && x < _width && 0 <= y && y < _height) return new Cell(this, x, y);
            throw new Exception("These coordinates are bigger than field itself!"); //TODO can be assertion?
        }

        /// <summary>
        /// Iterates all cells from the field, and that's it. Notice how it iterates on Y axis first and on X axis afterwards. Going from left to right and from top to bottom is more natural for humans, it's the way we read.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<Cell> IterateAllCells()
        {
            for (int fy = 0; fy < Height; fy++)
            {
                for (int fx = 0; fx < Width; fx++)
                {
                    yield return new Cell(this, fx, fy);
                }
            }
        }

        /// <summary>
        /// Returns true if no cells in a field were clicked. Returns false if even one cell was clicked.
        /// </summary>
        /// <returns></returns>
        internal bool HasNoClickedCells()
        {
            foreach (var cell in IterateAllCells())
            {
                if (cell.EnumRepresentation != ImageFilesKeeper.PossibleFieldsEnum.Unknown)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a DeepCopy of a method.
        /// </summary>
        /// <returns></returns>
        internal Field DeepCopy()
        {
            var result = new Field();
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    result.GetCell(x, y).EnumRepresentation = this.GetCell(x, y).EnumRepresentation;
                }
            }
            return result;
        }

        /// <summary>
        /// Displays content of Field object via MessageBox. Very usefull for debugging. Font isn't monospaced, so it isn't very pretty.
        /// </summary>
        internal void ShowViaMessageBox()
        {
            var messageString = new List<string>(_width * _height + _height);
            for (int fy = 0; fy < _height; fy++)
            {
                for (int fx = 0; fx < _width; fx++)
                {
                    messageString.Add(this.GetCell(fx,fy).FileName);
                }
                messageString.Add("\n");
            }
            MessageBox.Show(String.Join("", messageString));
        }


        /// <summary>
        /// Splits minesweeper's field on screenshotBitmap into many 16x16 bitmaps,
        /// and then compares each of them to a presaved bitmaps in the bin/debug directory.
        /// Result is a Field object that reflects the content of the screen.
        /// </summary>
        /// <returns></returns>
        internal void ReadFromScreen()
        {
            bringMinesweeperToFront();

            Bitmap bmpScreenshot = GetScreenBitmap();

            int windowX;
            int windowY;
            int fieldX;
            int fieldY;
            FindMinesweeperWindow(bmpScreenshot, out windowX, out windowY, out fieldX, out fieldY);
            _fieldXOnScreen = fieldX;
            _fieldYOnScreen = fieldY;

            //Debug
            if (false) Cursor.Position = new Point(windowX, windowY);



            //Debug
            if (false) Cursor.Position = new Point(fieldX, fieldY);


            if (windowX < 0 ||
                windowY < 0 ||
                fieldX + 16 * _width > bmpScreenshot.Width ||
                fieldY + 16 * _height > bmpScreenshot.Height)
            {
                throw new Exception("Minesweeper doesn't fit on the screen, place it so I can see everything!");
            }

            foreach (var cell in this.IterateAllCells())
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
                        this.GetCell(cell.X, cell.Y).EnumRepresentation = (ImageFilesKeeper.PossibleFieldsEnum) i;
                        break;
                    }
                }
                if (!isFamiliar)
                {
                    cellBitmap.Save("WhatIsThisCell7.png", ImageFormat.Png);
                    throw new Exception("Hey, I didn't expected that! I can't read that one tile properly! Saved this in WhatIsThisCell7.png, look at this later.");

                }
            }

            _outdated = false;
        }


        #region ReadFromScreenRelated
        /// <summary>
        /// Gets everything we see on the screen, and returns it as a bitmap.
        /// </summary>
        /// <returns></returns>
        static Bitmap GetScreenBitmap()
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
        /// <param name="fieldY">Y co... you got an idea.</param>
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
            if (bmpScreenshot.GetPixel(windowX, windowY).ToArgb() != Color.Silver.ToArgb())
            {
                throw new Exception("Please set Minesweeper to an expert mode!");
            }
            //MessageBox.Show(bmpScreenshot.GetPixel(windowX, windowY).ToString());
            //save the first square, for debugging
            //place cursor on the first square
            //734.178 -- yellow smiley's head's top
            //497.213 -- first square
            //487.160 -- topleft courner of the window

            fieldX = windowX + 10;
            fieldY = windowY + 53;
            //MessageBox.Show(bmpScreenshot.GetPixel(fieldX, fieldY).ToString());
            if (bmpScreenshot.GetPixel(fieldX, fieldY).ToArgb() != Color.White.ToArgb())
            {
                throw new Exception("Please set Minesweeper to an expert mode!");
            }
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

#region bringMinesweeperToFront

        /// <summary>Enumeration of the different ways of showing a window using
        /// ShowWindow</summary>
        private enum ShowWindowCommands : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,
            /// <summary>Activates and displays a window. If the window is minimized
            /// or maximized, the system restores it to its original size and
            /// position. An application should specify this flag when displaying
            /// the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,
            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,
            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,
            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,
            /// <summary>Displays a window in its most recent size and position.
            /// This value is similar to "ShowNormal", except the window is not
            /// actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,
            /// <summary>Activates the window and displays it in its current size
            /// and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,
            /// <summary>Minimizes the specified window and activates the next
            /// top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,
            /// <summary>Displays the window as a minimized window. This value is
            /// similar to "ShowMinimized", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,
            /// <summary>Displays the window in its current size and position. This
            /// value is similar to "Show", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,
            /// <summary>Activates and displays the window. If the window is
            /// minimized or maximized, the system restores it to its original size
            /// and position. An application should specify this flag when restoring
            /// a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,
            /// <summary>Sets the show state based on the SW_ value specified in the
            /// STARTUPINFO structure passed to the CreateProcess function by the
            /// program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,
            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread
            /// that owns the window is hung. This flag should only be used when
            /// minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("USER32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        [DllImport("USER32.DLL")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        static extern IntPtr GetForegroundWindow();

        static void bringMinesweeperToFront()
        {

            //TODO: for some reason doesn't work if started to quickly, maybe it's looses focus instantly or something. I'd like this to run right from an IDE, without clicking anything on the form.

            // Get a handle to the Calculator application.
            IntPtr handle = FindWindow(null, "Minesweeper");

            // Verify that Calculator is a running process.
            if (handle == IntPtr.Zero) throw new Exception("Window with a title \"Minesweeper\" wasn't found. I can't work without it!");

            // Maximise window
            ShowWindow(handle, ShowWindowCommands.Restore);

            // Make Calculator the foreground application
            SetForegroundWindow(handle);

            IntPtr currentTopHandle = GetForegroundWindow();
            if (currentTopHandle != handle) throw new Exception(
                    "Minesweeper window ins't on top, do something! It should be fully visible for me to work!");

        }
#endregion

        #endregion


        /// <summary>
        /// Represents a single cell of the field. Use this to see or change value of each field.
        /// </summary>
        internal class Cell
        {
            private readonly Field _field;
            private readonly int _x;
            private readonly int _y;

            internal Cell(Field field, int x, int y)
            {
                _field = field;
                this._x = x;
                this._y = y;
            }

            /// <summary>
            /// Gets X position of a cell.
            /// </summary>
            public int X
            {
                get { return _x; }
            }

            /// <summary>
            /// Gets Y position of a cell.
            /// </summary>
            public int Y
            {
                get { return _y; }
            }

            /// <summary>
            /// Does this cell contains a number? Number of mines, from 0 to 8?
            /// </summary>
            internal bool IsNumberOfMines
            {
                get
                {
                    return ((int)EnumRepresentation) >= 0 && ((int)EnumRepresentation) <= 8;
                }
            }

            /// <summary>
            /// Does this cell contains a flag?
            /// </summary>
            internal bool IsFlag { get { return FileName == "F"; } }

            /// <summary>
            /// True if this cell wasn't clicked yet.
            /// </summary>
            internal bool IsUnknown { get { return FileName == "_"; } }

            /// <summary>
            /// Something we can click on.
            /// </summary>
            internal bool IsClickable { 
                get
                {
                    return FileName == "_" ||
                           FileName == "F" ||
                           FileName == "W";
                }
            }

            /// <summary>
            /// Something we can't click on.
            /// </summary>
            internal bool IsUnclickable
            {
                get { return IsNumberOfMines; }
            }

            /// <summary>
            /// Returns a number of mines, number drawn on the cell it means.
            /// Returns null if not a number in the first place.
            /// </summary>
            internal int NumberOfMines
            {
                get
                {
                    if (IsNumberOfMines) { return (int)EnumRepresentation; }
                    else {throw new Exception("This isn't a number, something is wrong. You should check if this is a number of mines before using this property.");}
                }
            }

            /// <summary>
            /// Value of a cell.
            /// </summary>
            internal ImageFilesKeeper.PossibleFieldsEnum EnumRepresentation
            {
                get { return _field._theField[X, Y]; }
                set { _field._theField[X, Y] = value; }
            }


            /// <summary>
            /// Same as EnumRepresentation, but is a string.
            /// </summary>
            internal string FileName
            {
                get
                {
                    int IntRepresentation = (int) EnumRepresentation;
                    if (0 <= IntRepresentation && IntRepresentation <= ImageFilesKeeper.CellPossibleStrings.Length)
                    {
                        return ImageFilesKeeper.CellPossibleStrings[IntRepresentation];
                    }
                    else
                    {
                        throw new Exception("Name not found.");
                    }
                    
                    
                }
                set 
                {
                    for (int i = 0; i < ImageFilesKeeper.CellPossibleStrings.Length; i++)
                    {
                        if (ImageFilesKeeper.CellPossibleStrings[i] == value)
                        {
                            EnumRepresentation = (ImageFilesKeeper.PossibleFieldsEnum) i;
                            return;
                        }
                    }
                    throw new Exception("Name not found.");
                }
            }

            /// <summary>
            /// Iterates over all cells around current cell.
            /// </summary>
            /// <returns></returns>
            internal IEnumerable<Cell> IterateAllNearbyCells()
            {
                for (int tempy = Y - 1; tempy <= Y + 1; tempy++) 
                    for (int tempx = X - 1; tempx <= X + 1; tempx++)
                        if (0 <= tempx && 
                            tempx < _field.Width && 
                            0 <= tempy && tempy < _field.Height &&
                            !(tempx == X && tempy == X)
                            )
                        {
                            yield return new Cell(_field, tempx, tempy);
                        }
            }




            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

            private const int MOUSEEVENTF_LEFTDOWN = 0x02;
            private const int MOUSEEVENTF_LEFTUP = 0x04;
            private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
            private const int MOUSEEVENTF_RIGHTUP = 0x10;

            internal void Click()
            {
                _field._outdated = true;

                Cursor.Position = new Point(_field._fieldXOnScreen + _x * 16 + 8,
                    _field._fieldYOnScreen + _y * 16 + 8);

                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP,
                    (uint)(_field._fieldXOnScreen + _x * 16 + 8),
                    (uint)(_field._fieldYOnScreen + _y * 16 + 8),
                    0,
                    0);

            }

            internal void RightClick()
            {
                _field._outdated = true;

                Cursor.Position = new Point(_field._fieldXOnScreen + _x * 16 + 8,
                    _field._fieldYOnScreen + _y * 16 + 8);

                mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 
                    (uint) (_field._fieldXOnScreen + _x * 16 + 8), 
                    (uint) (_field._fieldYOnScreen + _y * 16 + 8), 
                    0, 
                    0);
                
            }

        }
    }
}
