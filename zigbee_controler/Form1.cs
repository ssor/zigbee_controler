using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace zigbee_controler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 系统参数SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSysConfig frm = new frmSysConfig();
            frm.ShowDialog();
        }

        private void 打开SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmControler frm = new frmControler();
            frm.ShowDialog();
        }
    }
}
