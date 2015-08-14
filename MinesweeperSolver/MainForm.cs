using System;
using System.Windows.Forms;

namespace MinesweeperSolver
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

       

        private void solveButton_Click(object sender, EventArgs e)
        {
            Solver.Solve();
        }


    }
}
