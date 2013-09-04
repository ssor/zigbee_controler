using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace zigbee_controler
{
    public delegate void delZigbeeCallback(int index,int nodeID,int humi,int temp);
    public class ZigbeeHelper
    {
        List<byte> maxbuf = new List<byte>();
        public event delZigbeeCallback eventZigInfo;
        bool IsEnd(byte bValue)
        {
            bool bR = false;
            if (bValue == 0x55)
            {
                bR = true;
            }
            return bR;
        }
        bool IsFF(byte bValue)
        {
            bool bR = false;
            if (bValue == 0xff)
            {
                bR = true;
            }
            return bR;
        }
        string BytesToHexStringWithNospace(byte[] value)
        {
            string str = "";
            for (int i = 0; i < value.Length; i++)
            {
                str += value[i].ToString("X2");
            }
            return str;
        }
        string BytesToHexString(byte[] value)
        {
            string str = "";
            for (int i = 0; i < value.Length; i++)
            {
                str += " ";
                str += value[i].ToString("X2");
            }
            return str;
        }
        /// <summary>
        /// 接收数据源（串口）数据
        /// </summary>
        /// <param name="value"></param>
        public void Parse(byte[] value)
        {
            //Debug.WriteLine(string.Format("Parse -> {0}",BytesToHexString(value)));
            try
            {
                maxbuf.AddRange(value);//将buf数组添加到maxbuf,只管添加

                Debug.WriteLine(string.Format("zigbeeHelper Parse -> {0} {1} value = {2}"
                    , DateTime.Now.ToLongTimeString()
                    , DateTime.Now.Millisecond.ToString()
                    , BytesToHexString(maxbuf.ToArray())));
                while (maxbuf.Count > 0)//只要还有数据就不停的查找
                {
                    Debug.WriteLine(
                    	string.Format("ZigbeeHelper.Parse  ->  = {0}"
                    	, "while loop"));
                    //****************************************************************************
                    // 从整个数据源中找出一段命令
                    int nEndIndex = maxbuf.FindIndex(0, IsFF);
                    if (nEndIndex == -1)
                    {
                        return;
                    }
                    while (nEndIndex != -1)
                    {
                        Debug.WriteLine(
    string.Format("ZigbeeHelper.Parse  -> while loop 2 = {0}   {1}"
    , nEndIndex.ToString(),maxbuf.Count.ToString()));// 22 23
                        if (nEndIndex > 0)// 此时找到的是形如 xxxFFFF 的数组
                        {
                            if (nEndIndex + 1 < maxbuf.Count &&
                              IsFF(maxbuf[nEndIndex + 1]))//如果这个ff的后面还是ff，那说明这真是结尾
                            {
                                if ((nEndIndex >= 22))// 往前数22 是标识 00 的包头
                                {
                                    break;
                                }
                            }
                        }
                        if (nEndIndex + 1 < maxbuf.Count)
                        {
                            nEndIndex = maxbuf.FindIndex(nEndIndex + 1, IsFF);
                            if (nEndIndex == -1)
                            {
                                return;//没找到一个完整的命令字符串，无法继续处理，直接返回
                            }
                        }
                        else//当 nEndIndex = 22 maxbuf.Count = 23时，会死循环，必须返回
                        {
                            return;//没找到一个完整的命令字符串，无法继续处理，直接返回
                        }
                    }
                    //*********************************************************************************
                    List<byte> bytesCmd = maxbuf.GetRange(nEndIndex - 22, 24);
                    maxbuf.RemoveRange(0, nEndIndex + 2);//将取出的命令从源中清除

                    string strID = BytesToHexStringWithNospace(bytesCmd.GetRange(0, 2).ToArray());
                    int id = Int32.Parse(strID, NumberStyles.AllowHexSpecifier);
                    List<byte> address = bytesCmd.GetRange(2, 8);
                    string strNodeID = BytesToHexStringWithNospace(bytesCmd.GetRange(10, 2).ToArray());
                    int nodeID = Int32.Parse(strNodeID, NumberStyles.AllowHexSpecifier);
                    string strHumidity = BytesToHexStringWithNospace(bytesCmd.GetRange(12, 2).ToArray());
                    int Humidity = Int32.Parse(strHumidity, NumberStyles.AllowHexSpecifier);
                    string strTemp = BytesToHexStringWithNospace(bytesCmd.GetRange(14, 2).ToArray());
                    int temperature = Int32.Parse(strTemp, NumberStyles.AllowHexSpecifier);
                    Debug.WriteLine(string.Format("zigbeeHelper Parse -> id = {0},nodeID = {1} Humidity = {2} temperature = {3} ",
                                    id.ToString(), nodeID.ToString(), Humidity.ToString(), temperature.ToString()));
                    if (this.eventZigInfo != null)
                    {
                        this.eventZigInfo(id, nodeID, Humidity, temperature);
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("zigbeeHelper Parse Exception -> {0}", ex.Message));
            }

        }
    }
}
