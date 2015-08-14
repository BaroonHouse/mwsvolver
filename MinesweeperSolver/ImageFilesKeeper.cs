using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MinesweeperSolver
{
    /// <summary>
    /// Class for keeping information about possible cell types.
    /// </summary>
    class ImageFilesKeeper
    {
        /// <summary>
        /// Length of all arrays here, and also a number of different cells known.
        /// </summary>
        internal const int CellLength = 15;

        /// <summary>
        /// This array keeps representation of cells as text. This is easier to understand than just numbers, I hope?
        /// Also they are used as filenames.
        /// /// 0 to 8 is... you should be able to guess if you played minesweeper at least once.
        /// _ is a yet unclicked cell
        /// + is a bomb
        /// X is a red bomb, the bomb you was lucky to click
        /// F is a flag
        /// W is question mark. Why it's not "?" then, you might ask? I couldn't save file ?.png, and 7.png is already used, so this is the way it is.
        /// O is just for debugging, it can't be met in the game. O.png is just a random garbage.
        /// </summary>
        internal static readonly string[] CellPossibleStrings = new string[CellLength] 
            { "0", "1", "2", "3", "4", "5", "6", "7", "8", "_", "+", "X", "F", "W", "O" };

        /// <summary>
        /// Same thing as CellPossibleStrings, but numbers are used. Int is more compact than string, so...
        /// </summary>
        internal static readonly int[] CellPossibleNumbers = new int[CellLength] 
            {  0,   1,   2,   3,   4,   5,   6,   7,   8,   9,  10,  11,  12,  13,  14  };

        internal enum PossibleFieldsEnum
        {
            ZeroMines,  //"0"
            OneMine,    //"1"
            TwoMines,   //...
            ThreeMines,
            FourMines,
            FiveMines,
            SixMines,
            SevenMines,
            EightMies,  //"8"
            Unknown,    //"_"
            Mine,       //"+"
            ExplodedMine,   //"X"
            Flag,       //"F"
            QuestionMark,   //"W", actually "?" but limitation
            Debug1,     //"O"

        }

        
        private static readonly Bitmap[] _CellBitmaps = new Bitmap[CellLength];
        private static bool _CellBitmapsIsLoaded = false;

        /// <summary>
        /// Bitmaps of all possible cells are stored here. They are automatically loaded on the first access.
        /// </summary>
        internal static Bitmap[] CellBitmaps
        {
            get
            {
                if (!_CellBitmapsIsLoaded)
                {
                    LoadBitmapsFromDisk();
                    _CellBitmapsIsLoaded = true;
                }
                return _CellBitmaps;
            }
        }

        private static void LoadBitmapsFromDisk()
        {
            for (int i = 0; i < _CellBitmaps.Length; i++)
            {
                _CellBitmaps[i] = (Bitmap)Image.FromFile(CellPossibleStrings[i] + ".png");
            }
        }

    }
}
