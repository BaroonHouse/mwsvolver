using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics.Contracts;

namespace MinesweeperSolver
{
    /// <summary>
    /// One cell of the field. Stored inside of the Field class, used in Solver.
    /// </summary>
    struct Cell
    {
        public static readonly string[] familiarFieldsFileNames = new string[] 
        { "_", "0", "1", "2", "3", "4", "5", "6", "7", "8", "+", "X", "F", "W", "O" };
        private static readonly int[] familiarFieldsNumbers = new int[] 
        { -1,   0,   1,   2,   3,   4,   5,   6,   7,   8,  -2,  -3,  -4,  -5,  -6 };

        private int _number;

        public bool IsNumber { get { return _number > 0; } }
        public bool IsFlag { get { return _number == -4; } }
        public bool IsUnknown { get { return _number == -1; } }

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public string FileName
        {
            get { return ToFileName(Number); }
            set { Number = ToNumber(value); }
        }

        private string ToFileName(int number)
        {
            Contract.Requires(familiarFieldsNumbers.Length == familiarFieldsFileNames.Length);
            for (int i = 0; i < familiarFieldsFileNames.Length; i++)
            {
                if (familiarFieldsNumbers[i] == number) return familiarFieldsFileNames[i];
            }
            throw new Exception("Unsupported parameter.");
        }

        private int ToNumber(string fileName)
        {
            Contract.Requires(familiarFieldsNumbers.Length == familiarFieldsFileNames.Length);
            for (int i = 0; i < familiarFieldsFileNames.Length; i++)
            {
                if (familiarFieldsFileNames[i] == fileName) return familiarFieldsNumbers[i];
            }
            throw new Exception("Unsupported parameter.");
        }
    }
}
