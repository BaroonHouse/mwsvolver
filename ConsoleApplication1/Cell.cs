using System;
using System.Diagnostics.Contracts;
/*
namespace MinesweeperSolver
{
    /// <summary>
    /// One cell of the field. Stored inside of the Field class, used in Solver.
    /// </summary>
    struct Cell
    {
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
            get { return ToFileName(_number); }
            set { Number = ToNumber(value); }
        }

        private int _number;

        private string ToFileName(int number)
        {
            //Contract.Requires(familiarFieldsNumbers.Length == familiarFieldsFileNames.Length);
            for (int i = 0; i < Solver.familiarFieldsFileNames.Length; i++)
            {
                if (Solver.familiarFieldsNumbers[i] == number) return Solver.familiarFieldsFileNames[i];
            }
            throw new Exception("Unsupported parameter.");
        }

        private int ToNumber(string fileName)
        {
            Contract.Requires(Solver.familiarFieldsNumbers.Length == Solver.familiarFieldsFileNames.Length);
            for (int i = 0; i < Solver.familiarFieldsFileNames.Length; i++)
            {
                if (Solver.familiarFieldsFileNames[i] == fileName) return Solver.familiarFieldsNumbers[i];
            }
            throw new Exception("Unsupported parameter.");
        }
    }
}
*/