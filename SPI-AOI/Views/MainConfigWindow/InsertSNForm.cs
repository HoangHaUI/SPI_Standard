using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_AOI.Views.MainConfigWindow
{
    public partial class InsertSNForm : Form
    {
        public string SN_Input { get; set; }
        public InsertSNForm()
        {
            InitializeComponent();
        }

        private void InsertSNForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                this.Close();
            }
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
