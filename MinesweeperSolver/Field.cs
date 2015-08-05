using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MinesweeperSolver
{
    /// <summary>
    /// This class is for keeping a track of mine field and every cell in it.
    /// 
    /// If you edit this, don't forget to also edit the Clone method.
    /// </summary>
    class Field
    {
        public int Width;
        public int Height;
        private readonly Cell[,] _theField;

        public Field(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this._theField = new Cell[width,height];
            for (int y = 0; y<height; y++)
                for (int x = 0; x < width; x++)
                {
                    this._theField[x, y] = new Cell();
                }
        }

        public Cell GetCell(int x, int y)
        {
            if (0 <= x && x < this.Width && 0 <= y && y < this.Height) return this._theField[x, y];
            return null;
        }

        public Field Clone()
        {
            var result = new Field(Width, Height);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    result.GetCell(x, y).Number = this.GetCell(x, y).Number;
                }
            }
            return result;
        }

        /// <summary>
        /// Very usefull for debugging.
        /// </summary>
        internal void ShowViaMessageBox()
        {
            var messageString = new List<string>(Width * Height + Height);
            for (int fy = 0; fy < Height; fy++)
            {
                for (int fx = 0; fx < Width; fx++)
                {
                    messageString.Add(this.GetCell(fx,fy).FileName);
                }
                messageString.Add("\n");
            }
            MessageBox.Show(String.Join("", messageString));
        }
    }
}
