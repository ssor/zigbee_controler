using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace zigbee_controler
{
    public static class staticClass
    {
        public static DateTime timeBase = DateTime.Now;

        public static string restServerIP = "192.168.1.100";
        public static string restServerPort = "9002";
        public static int interval = 15000;
        public static string serialport_name = string.Empty;
    }

}
