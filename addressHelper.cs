using System;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;


namespace EHD_Miner
{
    internal class AddressHelper
    {
        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(int dest, int host, ref long mac, ref int length);
        [DllImport("Ws2_32.dll")]
        private static extern int Inet_addr(string ip);

        //获取本机的IP

        [Obsolete]
        public string GetLocalIP()
        {
            string strHostName = Dns.GetHostName(); //得到本机的主机名

            IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP

            string strAddr = ipEntry.AddressList[0].ToString();
            return (strAddr);
        }
        //获取本机的MAC

        public string GetLocalMac()
        {
            string mac = null;
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                if (mo["IPEnabled"].ToString() == "True")
                {
                    mac = mo["MacAddress"].ToString();
                }
            }
            return (mac);
        }

        //获取远程主机IP

        [Obsolete]
        public string[] GetRemoteIP(string RemoteHostName)
        {
            IPHostEntry ipEntry = Dns.GetHostByName(RemoteHostName);
            IPAddress[] IpAddr = ipEntry.AddressList;
            string[] strAddr = new string[IpAddr.Length];
            for (int i = 0; i < IpAddr.Length; i++)
            {
                strAddr[i] = IpAddr[i].ToString();
            }
            return (strAddr);
        }
        //获取远程主机MAC

        public string GetRemoteMac(string localIP, string remoteIP)
        {
            int ldest = Inet_addr(remoteIP); //目的ip 

            int lhost = Inet_addr(localIP); //本地ip


            try
            {
                long macinfo = new long();
                int len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);
                return Convert.ToString(macinfo, 16);
            }
            catch (Exception err)
            {
                Console.WriteLine("Error:{0}", err.Message);
            }
            return 0.ToString();
        }
    }
}
