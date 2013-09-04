using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Net;

namespace zigbee_controler
{
    public partial class frmSysConfig : Form
    {
        public frmSysConfig()
        {
            InitializeComponent();

            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            cmbPortName.Items.AddRange(ports);

            this.Shown += new EventHandler(frmSysConfig_Shown);
        }

        void frmSysConfig_Shown(object sender, EventArgs e)
        {
            this.cmbPortName.Text = staticClass.serialport_name;

            this.txtIP.Text = staticClass.restServerIP;
            this.txtPort.Text = staticClass.restServerPort;
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(this.txtIP.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("IP地址设置不合法！" + ex.Message, "信息提示");
                return;
            }
            try
            {
                int port = int.Parse(this.txtPort.Text);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("串口设置不合法！" + ex.Message, "信息提示");
                return;
            }
            nsConfigDB.ConfigDB.saveConfig("serialport", this.cmbPortName.Text);
            nsConfigDB.ConfigDB.saveConfig("restIP", this.txtIP.Text);
            nsConfigDB.ConfigDB.saveConfig("restPort", this.txtPort.Text);

            staticClass.serialport_name = this.cmbPortName.Text;
            staticClass.restServerIP = this.txtIP.Text;
            staticClass.restServerPort = this.txtPort.Text;

            this.Close();
        }
    }
}
