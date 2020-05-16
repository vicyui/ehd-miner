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

namespace EHDMiner
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
        private string language = string.Empty;
        private ComponentResourceManager resource;
        private int processId;
        private Process p;
        string[] runArgs;
        public mainForm(string[] args)
        {
            InitializeComponent();
            language = "zh";
            resource = LanguageHelper.SetLang(language, this, typeof(mainForm), resource);
            runArgs = args;
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
                    case (int)DriveType.Fixed:   //本地磁盘     
                        {
                            deviceIDs.Add(mo["DeviceID"].ToString());
                            break;
                        }
                    default:   //defalut   to   folder     
                        {
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
            MineStatus();
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            InitForm();
            InitDatabase();
            toolStripStatusLabel2.Text = resource.GetString("tsslStatusSucess");
            labelMsg.Text = resource.GetString("installSuccessTips") + "\r" + resource.GetString("startMineTips");
            tsmiStart.Enabled = true;
            tsmiInstall.Visible = false;
            tsmiImportKeystore.Visible = false;
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
            p = new Process();
            p.StartInfo.FileName = Application.StartupPath + "/bin/poc.exe";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (runArgs.Length > 0 && runArgs[0] == "showPoc")
            {
                p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            }
            p.StartInfo.Arguments = " --datadir=\"" + Application.StartupPath + "\\AppData\\Roaming\\Poc\" --mine --gcmode archive --syncmode=full --networkid 10911 --rpc --rpcaddr \"0.0.0.0\" --rpcport=\"8545\" --rpcapi \"web3,peers,net,account,personal,eth,minedev,txpool\"";
            p.Start();
            processId = p.Id;
            Thread.Sleep(100);//加上，100如果效果没有就继续加大
            SetParent(p.MainWindowHandle, panel1.Handle); //panel1.Handle为要显示外部程序的容器
            ShowWindow(p.MainWindowHandle, 3);
            toolStripStatusLabel2.Text = resource.GetString("tsslStatusStart");
            tsmiStart.Enabled = false;
            labelMsg.Text = resource.GetString("installTips");

            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = Application.StartupPath + "\\Appdata\\Roaming\\Poc\\",
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };
            //watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private void MineStatus()
        {
            try
            {
                p = Process.GetProcessById(processId);
            }
            catch (Exception)
            {
                p = null;
                tsmiStart.Enabled = true;
                labelMsg.Text = resource.GetString("exceptionTips");
                toolStripStatusLabel2.Text = resource.GetString("tsslStatusStop");
            }
            
            if (p != null && p.ProcessName == "poc")
            {
                long mineDirLength = FileUtil.DictoryLength(plotdataDir);
                if (mineDirLength == 17179869184 || mineDirLength == 8589934592)
                {
                    labelMsg.Text = resource.GetString("statusMining") + "\r" + resource.GetString("installTips");
                    toolStripStatusLabel2.Text = resource.GetString("statusMining");
                }
                else
                {
                    labelMsg.Text = resource.GetString("statusChainSync") + "\r" + resource.GetString("installTips");
                    toolStripStatusLabel2.Text = resource.GetString("statusChainSync");
                }
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
            toolStripStatusLabel2.Text = resource.GetString("statusChainSync");
        }

        private delegate void setLogTextDelegate(FileSystemEventArgs e);

        private void ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            RunProcess("cmd.exe", "curl -H \"Content - Type: application / json\" --data '{\"method\": \"admin_addPeer\", \"params\": \"enode://c789aecd75c1a346b2060b4d33b3e7ee11f591b3003c6010b44daff49d461c7d3bcae25d161bb1b8134dac2707037df102bf7f0ee763d51a04709d8a15978997@27.190.170.103:30303\"}' http://127.0.0.1:8545");
        }

        [Obsolete]
        private void ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            AddressHelper helper = new AddressHelper();
            string minerInfo = resource.GetString("localIp") + helper.GetLocalIP();
            minerInfo += "\r" + resource.GetString("localMac") + helper.GetLocalMac();
            string address = string.Empty;
            fileList = Directory.GetFiles(keystoreDir);
            if (fileList.Length > 0)
            {
                address = "0x" + Path.GetFileName(fileList[0]).Substring(37);
                minerInfo += "\r" + resource.GetString("walletAddress") + address;
            }
            if (address.Length > 0)
            {
                minerInfo += "\r" + resource.GetString("minerName") + "EHD-miner-" + address.Substring(38);
            }
            DialogResult dr = MessageBox.Show(minerInfo, resource.GetString("copyTips") + "\r" + resource.GetString("tsmiShowInfo.Text"), MessageBoxButtons.OK);
            if (dr == DialogResult.OK)
            {
                Clipboard.SetDataObject(minerInfo);
            }
        }

        private void ToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            InitForm();
            labelMsg.Text = resource.GetString("ksImportTips");
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
        }

        private void KeystoreCheck()
        {
            if (!Directory.Exists(keystoreDir)
            || Directory.GetFiles(keystoreDir).Length == 0)
            {
                tsmiInstall.Enabled = false;
                tsmiStart.Enabled = false;
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
                FileUtil.ExtractResFile("EHDMiner.Resources." + description, Application.StartupPath + "\\AppData\\Roaming\\Poc\\" + description);
            }
            if (!File.Exists(Application.StartupPath + "\\" + database))
            {
                FileUtil.ExtractResFile("EHDMiner.Resources." + database, Application.StartupPath + "\\" + database);
            }
            if (!Directory.Exists(Application.StartupPath + "\\bin"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\bin");
            }
            if (!File.Exists(Application.StartupPath + "\\bin\\" + poc))
            {
                FileUtil.ExtractResFile("EHDMiner.Resources." + poc, Application.StartupPath + "\\bin\\" + poc);
            }
        }

        private void InitDatabase()
        {
            fileList = Directory.GetFiles(keystoreDir);
            if (fileList.Length == 0)
            {
                toolStripStatusLabel2.Text = resource.GetString("ksUnfind");
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
            }
            catch (Exception)
            {
                jo = null;
            }
            if (jo == null)
            {
                MessageBox.Show(resource.GetString("errorks"), resource.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            address = jo["address"].ToString();
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

                toolStripStatusLabel2.Text = resource.GetString("ksImportSucess");
                labelMsg.Text = toolStripStatusLabel2.Text;
                textBox1.Text = string.Empty;
                textBox1.Hide();
                btnSaveKS.Hide();
                btnKSDir.Hide();

                string sql = "select count(1) from t_user where F_Username = @Username;";
                int result = Convert.ToInt32(DBHelper.ExecuteScalar(sql, new Dictionary<string, object> { { "Username", "0x" + address } }));
                if (result == 1)
                {
                    tsmiStart.Enabled = true;
                    tsmiImportKeystore.Visible = false;
                    tsmiInstall.Visible = false;
                    labelMsg.Text = resource.GetString("startMineTips");
                    toolStripStatusLabel2.Text = resource.GetString("tsslStatusSucess");
                }
                else
                {
                    tsmiInstall.Enabled = true;
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
            toolStripStatusLabel2.Text = resource.GetString("tsslStatusScaning");
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabel2.Text = resource.GetString("tsslStatusScaned");
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

            DialogResult flag = MessageBox.Show(resource.GetString("repairForkWarn"), resource.GetString("warn"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (flag == DialogResult.OK)
            {
                RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
                Thread.Sleep(3000);
                DeleteEthDir();
                toolStripStatusLabel2.Text = resource.GetString("repairForkSuccess");
                labelMsg.Text = toolStripStatusLabel2.Text;
                tsmiStart.Enabled = true;
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
            DialogResult result = MessageBox.Show(resource.GetString("quitTips"), resource.GetString("tips"),
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                notifyIcon1.Dispose();
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
            notifyIcon1.Dispose();
            Dispose();
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                WindowState = FormWindowState.Normal;
            }
        }

        private void TsmiLangEN_Click(object sender, EventArgs e)
        {
            language = "en";
            LanguageHelper.SetLang(language, this, typeof(mainForm), resource);
        }

        private void TsmiLangCN_Click(object sender, EventArgs e)
        {
            language = "zh";
            LanguageHelper.SetLang(language, this, typeof(mainForm), resource);
        }

        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            Process.Start("https://scan.ehd.io");
        }

        private void toolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            Process.Start("https://www.ehd.io");
        }
    }
}
