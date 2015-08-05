using System;
using System.Windows.Forms;

namespace MinesweeperSolver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

       

        private void solveButton_Click(object sender, EventArgs e)
        {
            Solver.Solve();
        }

        private void debugButton_Click(object sender, EventArgs e)
        {
            
        }

    }
}
