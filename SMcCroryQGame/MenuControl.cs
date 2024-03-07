using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMcCroryQGame
{
    public partial class MenuControl : UserControl
    {
        public MenuControl()
        {
            InitializeComponent();
        }

        //opening the design map form
        private void btnDesign_Click(object sender, EventArgs e)
        {
            DesignMapForm designMap = new DesignMapForm();
            designMap.Show();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        //closing the main menu
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
            
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            Play playForm = new Play();
            playForm.Show();
        }
    }
}
