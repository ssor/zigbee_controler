using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;

namespace zigbee_controler
{
    public partial class frmControler : Form
    {
        Dictionary<string, bool> itemDic = new Dictionary<string, bool>();
        SerialPort comport = new SerialPort();
        ZigbeeHelper helper = new ZigbeeHelper();
        bool bStopListening = false;

        public frmControler()
        {
            InitializeComponent();

            // 串口
            this.comport.PortName = staticClass.serialport_name;
            this.comport.StopBits = StopBits.One;
            this.comport.Parity = Parity.None;
            this.comport.DataBits = 8;
            this.comport.BaudRate = 19200;

            comport.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);

            this.helper.eventZigInfo += new delZigbeeCallback(HandletxtLog);


            this.checkedListBox1.Items.Clear();
            //string item_name = "item1";
            //this.checkedListBox1.Items.Add(item_name, true);
            //itemDic.Add(item_name, true);
            //item_name = "item2";
            //this.checkedListBox1.Items.Add(item_name, true);
            //itemDic.Add(item_name, true);

            this.checkedListBox1.ItemCheck += new ItemCheckEventHandler(checkedListBox1_ItemCheck);

            this.Shown += new EventHandler(frmControler_Shown);
            this.FormClosing += new FormClosingEventHandler(frmControler_FormClosing);
        }

        void frmControler_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.comport.Close();
        }

        void frmControler_Shown(object sender, EventArgs e)
        {
            this.comport.Open();
        }

        void HandletxtLog(int index, int nodeID, int humi, int temp)
        {
            delZigbeeCallback dele = delegate(int _index, int _nodeID, int _humi, int _temp)
            {
                //send_info_to_server(index, nodeID, humi, temp);

                Debug.WriteLine(string.Format("node => {0}    humi =>  {1}    temp => {2}", _nodeID.ToString(), _humi.ToString(), _temp.ToString()));
                //首先检查列表中是否已经存在
                if (!this.itemDic.ContainsKey(_nodeID.ToString()))
                {
                    this.itemDic.Add(_nodeID.ToString(), false);

                    this.checkedListBox1.Items.Add(_nodeID.ToString());
                }
                //其次检查是否要发送
                bool bstate = this.itemDic[_nodeID.ToString()];
                if (bstate == true)
                {
                    //发送到服务器


                    string log =
                         string.Format("数据包号：{0} 节点ID：{1} 湿度：{2} 温度：{3}",
                        _index.ToString(),
                        _nodeID.ToString(),
                        _humi.ToString(),
                        _temp.ToString());
                    this.appendLog(log);
                }
            };
            this.Invoke(dele, index, nodeID, humi, temp);
        }
        void appendLog(string log)
        {
            if (this.txtLog.Text != null && this.txtLog.Text.Length > 4096)
            {
                this.txtLog.Text = string.Empty;
            }
            this.txtLog.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + log + "\r\n" + this.txtLog.Text;
        }
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (bStopListening == true)
            {
                return;
            }
            try
            {
                int n = comport.BytesToRead;//n为返回的字节数
                byte[] buf = new byte[n];//初始化buf 长度为n
                comport.Read(buf, 0, n);//读取返回数据并赋值到数组
                //_RFIDHelper.Parse(buf,true);
                helper.Parse(buf);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int index = e.Index;
            if (index >= 0)
            {
                string item_name = (string)this.checkedListBox1.Items[index];
                if (item_name != null)
                {
                    if (e.NewValue == CheckState.Checked)
                    {
                        itemDic[item_name] = true;
                    }
                    else
                    {
                        itemDic[item_name] = false;
                    }
                }
            }
        }



        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //private void send_info_to_server(int index, int nodeID, int humi, int temp)
        //{
        //    Debug.WriteLine(string.Format("node => {0}    humi =>  {1}    temp => {2}", nodeID.ToString(), humi.ToString(), temp.ToString()));
        //    //首先检查列表中是否已经存在
        //    if (!this.itemDic.ContainsKey(nodeID.ToString()))
        //    {
        //        this.itemDic.Add(nodeID.ToString(), true);

        //        this.checkedListBox1.Items.Add(nodeID.ToString());
        //    }
        //    //其次检查是否要发送
        //    bool bstate = this.itemDic[nodeID.ToString()];
        //    if (bstate == true)
        //    {
        //        //发送到服务器
        //        string log =
        //             string.Format("数据包号：{0} 节点ID：{1} 湿度：{2} 温度：{3}",
        //            index.ToString(),
        //            nodeID.ToString(),
        //            humi.ToString(),
        //            temp.ToString());
        //            this.appendLog(log);
        //    }
        //}

    }
}
