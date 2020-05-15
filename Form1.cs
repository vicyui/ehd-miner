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
        string[] fileList;
        private readonly string description = "description.xml";
        private readonly string database = "database.sqlite";
        private readonly string poc = "poc.exe";
        private readonly string keystoreDir = Application.StartupPath + "\\AppData\\Roaming\\Poc\\keystore";
        private readonly string plotdataDir = Application.StartupPath + "\\AppData\\Roaming\\Poc\\plotdata";
        private readonly string ethDir = Application.StartupPath + "\\AppData\\Roaming\\Poc\\eth";
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
            Dictionary<string, long> deviceMap = new Dictionary<string, long>();
            foreach (string device in devices)
            {
                if (!Directory.Exists(device + "\\plotdata"))
                {
                    continue;
                }
                deviceMap.Add(device + "\\plotdata", FileUtil.DictoryLength(device + "\\plotdata"));
                worker.ReportProgress(devices.IndexOf(device) / devices.Count * 10 * 4);
            }
            if (deviceMap.Count > 0)
            {
                long mineDirLength = FileUtil.DictoryLength(plotdataDir);
                if (mineDirLength < 17179869184 && deviceMap.First().Value > mineDirLength)
                {
                    Directory.Delete(plotdataDir, false);
                    worker.ReportProgress(50);
                    deviceMap = deviceMap.OrderByDescending(o => o.Value).ToDictionary(k => k.Key, v => v.Value);
                    FileUtil.CopyOldLabFilesToNewLab(deviceMap.First().Key, plotdataDir);
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
            InitDatabase();
            toolStripStatusLabel2.Text = "挖矿系统安装成功";
            labelMsg.Text = toolStripStatusLabel2.Text;
        }

        [DllImport("User32.dll ", EntryPoint = "SetParent")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll ", EntryPoint = "ShowWindow")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            InitForm();
            RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
            DeleteEthDir();
            Process p = new Process();
            p.StartInfo.FileName = Application.StartupPath + "/bin/poc.exe";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.Arguments = " --datadir=\"" + Application.StartupPath + "\\AppData\\Roaming\\Poc\" --mine --gcmode archive --syncmode=full --networkid 10911 --rpc --rpcaddr \"0.0.0.0\" --rpcport=\"8545\" --rpcapi \"web3,peers,net,account,personal,eth,minedev,txpool\"";
            p.Start();
            Thread.Sleep(100);//加上，100如果效果没有就继续加大
            SetParent(p.MainWindowHandle, panel1.Handle); //panel1.Handle为要显示外部程序的容器
            ShowWindow(p.MainWindowHandle, 3);
            toolStripStatusLabel2.Text = "启动成功";
            MineStatus();

            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = Application.StartupPath + "\\Appdata\\Roaming\\Poc\\",
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private void MineStatus()
        {
            long mineDirLength = FileUtil.DictoryLength(plotdataDir);
            if (mineDirLength == 17179869184 || mineDirLength == 8589934592)
            {
                labelMsg.Text = "挖矿中...";
            }
            else
            {
                labelMsg.Text = "P盘中...";
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new setLogTextDelegate(SetLogText), new object[] { e });
            }
        }

        private void SetLogText(FileSystemEventArgs e)
        {
            toolStripStatusLabel2.Text = "区块同步中...";
        }

        private delegate void setLogTextDelegate(FileSystemEventArgs e);

        private void ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            RunProcess("cmd.exe", "curl -H \"Content - Type: application / json\" --data '{\"method\": \"admin_addPeer\", \"params\": \"enode://c789aecd75c1a346b2060b4d33b3e7ee11f591b3003c6010b44daff49d461c7d3bcae25d161bb1b8134dac2707037df102bf7f0ee763d51a04709d8a15978997@27.190.170.103:30303\"}' http://127.0.0.1:8545");
        }

        [Obsolete]
        private void ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            InitForm();
            AddressHelper helper = new AddressHelper();
            labelMsg.Text += "本机IP  : " + helper.GetLocalIP() + "\r";
            labelMsg.Text += "本机MAC : " + helper.GetLocalMac()+ "\r";
            string address = string.Empty;
            fileList = Directory.GetFiles(keystoreDir);
            if(fileList.Length > 0)
            {
                address = "0x" + Path.GetFileName(fileList[0]).Substring(37);
            }
            labelMsg.Text += "钱包地址 : " + address;
        }

        private void ToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            InitForm();
            labelMsg.Text = "请输入EHD钱包keystore文件内容";
            textBox1.Show();
            btnSaveKS.Show();
            btnKSDir.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitForm();
            InitDirectoryAndFile();
            ClearKeystore();
        }

        private void InitForm()
        {
            labelMsg.Text = string.Empty;
            textBox1.Hide();
            btnSaveKS.Hide();
            btnKSDir.Hide();
            toolStripProgressBar1.Visible = false;
            KeystoreCheck();
        }

        private void KeystoreCheck()
        {
            if (!Directory.Exists(keystoreDir) 
            || Directory.GetFiles(keystoreDir).Length == 0)
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

        private string RunProcess(string program, string command)
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

        private void InitDirectoryAndFile()
        {
            if (!Directory.Exists(Application.StartupPath + "\\AppData"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\AppData\\Roaming\\Poc");
            }
            if (!Directory.Exists(keystoreDir))
            {
                Directory.CreateDirectory(keystoreDir);
            }
            if (!Directory.Exists(plotdataDir))
            {
                Directory.CreateDirectory(plotdataDir);
            }
            if (!File.Exists(Application.StartupPath + "\\AppData\\Roaming\\Poc\\" + description))
            {
                FileUtil.ExtractResFile("EHD_Miner.Resources." + description, Application.StartupPath + "\\AppData\\Roaming\\Poc\\" + description);
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
        }

        private void InitDatabase()
        {
            fileList = Directory.GetFiles(keystoreDir);
            if (fileList.Length == 0)
            {
                toolStripStatusLabel2.Text = "未找到keystore";
                return;
            }
            string keystoreName = Path.GetFileName(fileList[0]);
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
            JObject jo;
            string address = string.Empty;
            try
            {
                jo = JsonConvert.DeserializeObject<JObject>(textBox1.Text.Trim());
                address = jo["address"].ToString();
            }
            catch (Exception)
            {
                jo = null;
            }
            if (jo == null)
            {
                MessageBox.Show("非法的keystore", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string fileName = "UTC--" + utcTime + "--" + address;
            fileList = Directory.GetFiles(keystoreDir);
            if (fileList.Length == 0)
            {
                FileStream fs = new FileStream(keystoreDir + "\\" + fileName, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                long fl = fs.Length;
                fs.Seek(fl, SeekOrigin.End);
                sw.WriteLine(textBox1.Text);//开始写入值
                sw.Close();
                fs.Close();
                toolStripStatusLabel2.Text = "keystore导入成功";
                textBox1.Text = string.Empty;
                string sql = "select count(1) from t_user where F_Username = @Username;";
                int result = Convert.ToInt32(DBHelper.ExecuteScalar(sql, new Dictionary<string, object> { { "Username", "0x" + address } }));
                if (result == 1)
                {
                    toolStripMenuItem6.Visible = false;
                    toolStripMenuItem2.Visible = false;
                    btnSaveKS.Visible = false;
                    btnKSDir.Visible = false;
                    textBox1.Visible = false;
                    labelMsg.Text = "恭喜!您的挖矿环境已经准备就绪,请点击启动挖矿";
                    toolStripStatusLabel2.Text = "准备就绪";
                }
            }
        }

        private void BtnKSDir_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", keystoreDir);
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ScanDevices(sender as BackgroundWorker, e);
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
            DialogResult flag = MessageBox.Show("确定要修复分叉吗?", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (flag == DialogResult.OK)
            {
                DeleteEthDir();
                toolStripStatusLabel2.Text = "修复成功";
                labelMsg.Text = toolStripStatusLabel2.Text;
            }
        }

        private void DeleteEthDir()
        {
            if (Directory.Exists(ethDir))
            {
                Directory.Delete(ethDir, true);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("确认退出吗？", "退出询问",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                ClearKeystore();
                Dispose();
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void ClearKeystore()
        {
            fileList = Directory.GetFiles(keystoreDir);
            if (fileList.Length > 0)
            {
                File.Delete(fileList[0]);
            }
            RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
        }
    }
}
