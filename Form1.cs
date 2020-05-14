using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace EHD_Miner
{
    public partial class mainForm : Form
    {
        private BackgroundWorker backgroundWorker;
        public mainForm()
        {
            InitializeComponent();
        }

        public List<string> GetDeviceID()
        {
            List<string> deviceIDs = new List<string>();
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT  *  From  Win32_LogicalDisk ");
            ManagementObjectCollection queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {

                switch (int.Parse(mo["DriveType"].ToString()))
                {
                    case (int)DriveType.Removable:   //可以移动磁盘     
                        {
                            //MessageBox.Show("可以移动磁盘");
                            //deviceIDs.Add(mo["DeviceID"].ToString());
                            break;
                        }
                    case (int)DriveType.Fixed:   //本地磁盘     
                        {
                            //MessageBox.Show("本地磁盘");
                            deviceIDs.Add(mo["DeviceID"].ToString());
                            break;
                        }
                    case (int)DriveType.CDRom:   //CD   rom   drives     
                        {
                            //MessageBox.Show("CD   rom   drives ");
                            break;
                        }
                    case (int)DriveType.Network:   //网络驱动   
                        {
                            //MessageBox.Show("网络驱动器 ");
                            break;
                        }
                    case (int)DriveType.Ram:
                        {
                            //MessageBox.Show("驱动器是一个 RAM 磁盘 ");
                            break;
                        }
                    case (int)DriveType.NoRootDirectory:
                        {
                            //MessageBox.Show("驱动器没有根目录 ");
                            break;
                        }
                    default:   //defalut   to   folder     
                        {
                            //MessageBox.Show("驱动器类型未知 ");
                            break;
                        }
                }

            }
            return deviceIDs;
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            InitForm();
            InitializeBackgroundWorker();
            backgroundWorker.RunWorkerAsync();
        }

        private void ScanDevices(BackgroundWorker worker, DoWorkEventArgs e)
        {
            worker.ReportProgress(0);
            List<string> devices = GetDeviceID();
            string plotdataDir = "plotdata";
            Dictionary<string, long> deviceMap = new Dictionary<string, long>();
            foreach (string device in devices)
            {
                if (!Directory.Exists(device + "\\" + plotdataDir))
                {
                    continue;
                }
                deviceMap.Add(device + "\\" + plotdataDir, FileUtil.DictoryLength(device + "\\" + plotdataDir));
                worker.ReportProgress(devices.IndexOf(device) / devices.Count * 10 * 4);
            }
            if (deviceMap.Count > 0)
            {
                long mineDirLength = FileUtil.DictoryLength(Application.StartupPath + "/AppData/Roaming/Poc/plotdata");
                if (mineDirLength < 17179869184 && deviceMap.First().Value > mineDirLength)
                {
                    Directory.Delete(Application.StartupPath + "/AppData/Roaming/Poc/plotdata", false);
                    worker.ReportProgress(50);
                    deviceMap = deviceMap.OrderByDescending(o => o.Value).ToDictionary(k => k.Key, v => v.Value);
                    FileUtil.CopyOldLabFilesToNewLab(deviceMap.First().Key, Application.StartupPath + "/AppData/Roaming/Poc/plotdata");
                }
            }
            worker.ReportProgress(100);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = DateTime.Now.ToString();
            KeystoreCheck();
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            InitForm();
            InitDirectoryAndFile();
            // 初始化数据需要校验keystore是否存在
            InitDatabase();
        }

        [DllImport("User32.dll ", EntryPoint = "SetParent")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll ", EntryPoint = "ShowWindow")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            InitForm();
            RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
            Process p = new Process();
            p.StartInfo.FileName = Application.StartupPath + "/bin/poc.exe";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;//加上这句效果更好
            p.StartInfo.Arguments = " --datadir=\"" + Application.StartupPath + "\\AppData\\Roaming\\Poc\" --mine --gcmode archive --syncmode=full --networkid 10911 --rpc --rpcaddr \"0.0.0.0\" --rpcport=\"8545\" --rpcapi \"web3,peers,net,account,personal,eth,minedev,txpool\"";
            p.Start();
            Thread.Sleep(100);//加上，100如果效果没有就继续加大

            SetParent(p.MainWindowHandle, panel1.Handle); //panel1.Handle为要显示外部程序的容器
            ShowWindow(p.MainWindowHandle, 3);
            toolStripStatusLabel2.Text = "启动成功";
        }

        private void ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            InitForm();
            RunProcess("cmd.exe","taskkill /F /IM poc.exe");
            toolStripStatusLabel2.Text = "停止成功";
            labelMsg.Text = toolStripStatusLabel2.Text;
        }

        [Obsolete]
        private void ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            InitForm();
            AddressHelper helper = new AddressHelper();
            labelMsg.Text += "本机IP  : " + helper.GetLocalIP() + "\r";
            labelMsg.Text += "本机MAC : " + helper.GetLocalMac();
        }

        private void ToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            InitForm();
            labelMsg.Text = "请输入EHD钱包keystore文件内容";
            labelTips.Text = "请打开keystore文件夹确认文件是否为本人所有,如要替换请删除文件夹内多余的keystore文件";
            labelTips.Show();
            textBox1.Show();
            btnSaveKS.Show();
            btnKSDir.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitForm();
        }

        private void InitForm()
        {
            labelMsg.Text = string.Empty;
            textBox1.Hide();
            labelTips.Hide();
            btnSaveKS.Hide();
            btnKSDir.Hide();
            toolStripProgressBar1.Visible = false;
            KeystoreCheck();
        }

        private void KeystoreCheck()
        {
            if (Directory.GetFiles(Application.StartupPath + "\\" + AppData + "\\" + Roaming + "\\" + Poc + "\\" + keystore).Length == 0)
            {
                toolStripMenuItem1.Enabled = false;
                toolStripMenuItem2.Enabled = false;
                toolStripMenuItem3.Enabled = false;
            }
            else
            {
                toolStripMenuItem1.Enabled = true;
                toolStripMenuItem2.Enabled = true;
                toolStripMenuItem3.Enabled = true;
            }
        }

        private string RunProcess(string program ,string command)
        {
            //实例一个Process类，启动一个独立进程
            Process p = new Process();
            //Process类有一个StartInfo属性
            //设定程序名
            p.StartInfo.FileName = program;
            //设定程式执行参数 
            p.StartInfo.Arguments = "/c " + command;
            //关闭Shell的使用 
            p.StartInfo.UseShellExecute = false;
            //重定向标准输入 
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            //重定向错误输出 
            p.StartInfo.RedirectStandardError = true;
            //设置不显示窗口
            p.StartInfo.CreateNoWindow = true;
            //启动
            p.Start();
            //也可以用这种方式输入要执行的命令
            //不过要记得加上Exit要不然下一行程式执行的时候会当机
            //p.StandardInput.WriteLine(command);
            //p.StandardInput.WriteLine("exit"); 
            //从输出流取得命令执行结果
            return p.StandardOutput.ReadToEnd();
        }

        readonly string AppData = "AppData";
        readonly string Roaming = "Roaming";
        readonly string Poc = "Poc";
        readonly string keystore = "keystore";
        readonly string plotdata = "plotdata";
        readonly string description = "description.xml";
        readonly string database = "database.sqlite";
        readonly string poc = "poc.exe";

        private void InitDirectoryAndFile()
        {
            if (!Directory.Exists(Application.StartupPath + "\\" + AppData))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\" + AppData);
            }
            if (!Directory.Exists(Application.StartupPath + "\\" + AppData +"\\"+ Roaming))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\" + AppData + "\\" + Roaming);
            }
            if (!Directory.Exists(Application.StartupPath + "\\" + AppData + "\\" + Roaming+"\\"+ Poc))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\" + AppData + "\\" + Roaming + "\\" + Poc);
            }
            if (!Directory.Exists(Application.StartupPath + "\\" + AppData + "\\" + Roaming + "\\" + Poc+"\\"+ keystore))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\" + AppData + "\\" + Roaming + "\\" + Poc + "\\" + keystore);
            }
            if (!Directory.Exists(Application.StartupPath + "\\" + AppData + "\\" + Roaming + "\\" + Poc + "\\" + plotdata))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\" + AppData + "\\" + Roaming + "\\" + Poc + "\\" + plotdata);
            }
            if (!File.Exists(Application.StartupPath + "\\" + AppData + "\\" + Roaming + "\\" + Poc+"\\"+ description))
            {
                FileUtil.ExtractResFile("EHD_Miner.Resources." + description, Application.StartupPath + "\\" + AppData + "\\" + Roaming + "\\" + Poc + "\\" + description);
            }
            if (!File.Exists(Application.StartupPath + "\\" + database))
            {
                FileUtil.ExtractResFile("EHD_Miner.Resources." + database, Application.StartupPath + "\\" + database);
            }
            if (!Directory.Exists(Application.StartupPath + "\\bin"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\bin");
            }
            if (!File.Exists(Application.StartupPath + "\\bin\\" + poc))
            {
                FileUtil.ExtractResFile("EHD_Miner.Resources." + poc, Application.StartupPath + "\\bin\\" + poc);
            }
            toolStripStatusLabel2.Text = "初始化结构成功";
        }

        private void InitDatabase()  
        {
            string[] flieList = Directory.GetFiles(Application.StartupPath + "\\" + AppData + "\\" + Roaming + "\\" + Poc + "\\" + keystore);
            if (flieList.Length == 0)
            {
                toolStripStatusLabel2.Text = "未找到keystore";
                return;
            }
            string keystoreName = Path.GetFileName(flieList[0]);
            IDbConnection conn = DBHelper.CreateConnection();
            string nowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "REPLACE INTO t_host(F_id, F_Hostname, F_Status, F_CreateTime,F_ModifyTime) VALUES(1, @HostName, 0, @CreateTime, @CreateTime);";
            Dictionary<string, object> ups = new Dictionary<string, object>
            {
                { "HostName", "EHD-miner-" + keystoreName.Substring(73,4) },//EHD-miner- 钱包后4位
                { "CreateTime", nowDate }
            };
            DBHelper.ExecuteNonQuery(conn, sql, ups);

            string sql2 = "REPLACE INTO t_user(F_id, F_Username,F_Password, F_CreateTime, F_ModifyTime) VALUES(1, @Address, @Password, @CreateTime, @CreateTime);";
            Dictionary<string, object> ups2 = new Dictionary<string, object>
            {
                { "Address", "0x" + keystoreName.Substring(37) },//keystore文件名第37位起
                { "Password", "ehd123123" },//keystore文件名第37位起
                { "CreateTime", nowDate }
            };
            DBHelper.ExecuteNonQuery(conn, sql2, ups2);

            string sql3 = "REPLACE INTO t_plot(F_id, F_Name, F_Path, F_Uuid, F_PlotSeed, F_PlotDir, F_PlotSize, F_PlotParam, F_Status, F_CreateTime, F_ModifyTime) " +
                "VALUES(1, 'default', @Path, '', @Address, @PlotDir, '8589934592', '{\"startNonce\":0}', 1, @CreateTime, @CreateTime);";
            Dictionary<string, object> ups3 = new Dictionary<string, object>
            {
                { "Path", Application.StartupPath },
                { "Address", "0x" + keystoreName.Substring(37) },
                { "PlotDir", "/AppData/Roaming/Poc/plotdata" },
                { "CreateTime", nowDate }
            };
            DBHelper.ExecuteNonQuery(conn, sql3, ups3);
            conn.Close();
        }

        private void BtnSaveKS_Click(object sender, EventArgs e)
        {
            string utcTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss.fffffff00Z");
            JObject jo = JsonConvert.DeserializeObject<JObject>(textBox1.Text);
            if(jo == null)
            {
                return;
            }
            string address = jo["address"].ToString();
            string fileName = "UTC--" + utcTime + "--" + address;
            string[] flieList = Directory.GetFiles(Application.StartupPath + "\\" + AppData + "\\" + Roaming + "\\" + Poc + "\\" + keystore);
            if(flieList.Length == 0)
            {
                FileStream fs = new FileStream(Application.StartupPath + "\\" + AppData + "\\" + Roaming + "\\" + Poc + "\\" + keystore + "\\" + fileName, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                long fl = fs.Length;
                fs.Seek(fl, SeekOrigin.End);
                sw.WriteLine(textBox1.Text);//开始写入值
                sw.Close();
                fs.Close();
                toolStripStatusLabel2.Text = "keystore导入成功";
                textBox1.Text = string.Empty;
            }
            else
            {
                toolStripStatusLabel2.Text = "keystore已存在,请检查";
            }
        }

        private void BtnKSDir_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Application.StartupPath + "\\" + AppData + "\\" + Roaming + "\\" + Poc + "\\" + keystore);
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ScanDevices(sender as BackgroundWorker,e);
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripStatusLabel2.Text = "扫描磁盘中";
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabel2.Text = "磁盘扫描完成";
            labelMsg.Text = toolStripStatusLabel2.Text;
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker1_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker1_ProgressChanged);
            toolStripProgressBar1.Visible = true;
        }

        private void ToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            DialogResult flag = MessageBox.Show("确认处理", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (flag.Equals(DialogResult.OK))
            {
                Directory.Delete(Application.StartupPath + "/AppData/Roaming/Poc/eth", false);
            }
        }
    }
}
