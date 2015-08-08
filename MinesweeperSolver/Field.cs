using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MinesweeperSolver
{
    /// <summary>
    /// Keeping a track of mine field and every cell in it.
    /// </summary>
    class Field
    {
        readonly int _width;
        readonly int _height;
        readonly int[,] _theField;

        internal Field(int width, int height)
        {
            this._width = width;
            this._height = height;
            this._theField = new int[width,height];
        }

        internal int Width
        {
            get { return _width; }
        }

        internal int Height
        {
            get { return _height; }
        }

        internal Cell GetCell(int x, int y)
        {
            if (0 <= x && x < this._width && 0 <= y && y < this._height) return new Cell(this, x, y);
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

        internal Field Clone() //TODO: this is probably broken
        {
            var result = new Field(_width, _height);
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    result.GetCell(x, y).IntRepresentation = this.GetCell(x, y).IntRepresentation;
                }
            }
            return result;
        }

        /// <summary>
        /// Very usefull for debugging. Font isn't monospaced, so it isn't very pretty.
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
            internal bool IsNumberOfMines { get { return IntRepresentation > 0; } }

            /// <summary>
            /// Does this cell contains a flag?
            /// </summary>
            internal bool IsFlag { get { return FileName == "F"; } }

            /// <summary>
            /// True if this cell wasn't clicked yet.
            /// </summary>
            internal bool IsUnknown { get { return FileName == "_"; } }

            /// <summary>
            /// Returns a number of mines, number drawn on the cell it means.
            /// Returns null if not a number in the first place.
            /// </summary>
            internal int? NumberOfMines
            {
                get
                {
                    if (IsNumberOfMines) {return IntRepresentation - ToNumber("0");}
                    else {return null;}
                }
            }

            /// <summary>
            /// Get or set an integer representation of the field.
            /// Look inside ImageFilesKeeper.cs for details on how it works.
            /// </summary>
            internal int IntRepresentation
            {
                get { return _field._theField[X,Y]; }
                set { _field._theField[X, Y] = value; }
            }

            /// <summary>
            /// Same as IntRepresentation, but is a string.
            /// </summary>
            internal string FileName
            {
                get { return ToFileName(IntRepresentation); }
                set { IntRepresentation = ToNumber(value); }
            }

            /// <summary>
            /// Iterates over all cells around current cell. It's smart enough to check for walls.
            /// </summary>
            /// <returns></returns>
            internal IEnumerable<Cell> IterateAllNearbyCells()
            {
                for (int tempy = Y - 1; tempy <= Y + 1; tempy++) 
                    for (int tempx = X - 1; tempx <= X + 1; tempx++)
                        if (0 <= tempx && tempx < _field.Width && 0 <= tempy && tempy < _field.Height && !(tempx == X && tempy == X))
                        {
                            yield return new Cell(_field, tempx, tempy);
                        }
            }

            

            string ToFileName(int number)
            {
                for (int i = 0; i < ImageFilesKeeper.CellPossibleStrings.Length; i++)
                {
                    if (ImageFilesKeeper.CellPossibleNumbers[i] == number) return ImageFilesKeeper.CellPossibleStrings[i];
                }
                throw new Exception("Name not found.");
            }

            int ToNumber(string fileName)
            {
                for (int i = 0; i < ImageFilesKeeper.CellPossibleStrings.Length; i++)
                {
                    if (ImageFilesKeeper.CellPossibleStrings[i] == fileName) return ImageFilesKeeper.CellPossibleNumbers[i];
                }
                throw new Exception("IntRepresentation not found.");
            }
        }
    }
}
