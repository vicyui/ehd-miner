using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace EHDMiner
{
    public partial class mainForm : Form
    {
        private BackgroundWorker backgroundWorker;
        private string[] fileList;
        private string address = string.Empty;
        private readonly string description = "description.xml";
        private readonly string database = "database.sqlite";
        private readonly string poc = "poc.exe";
        private readonly string keystoreDir = Application.StartupPath + "\\AppData\\Roaming\\Poc\\keystore";
        private readonly string plotdataDir = Application.StartupPath + "\\AppData\\Roaming\\Poc\\plotdata";
        private readonly string ethDir = Application.StartupPath + "\\AppData\\Roaming\\Poc\\eth";
        public static string language = string.Empty;
        private ComponentResourceManager resource;
        private int processId;
        private Process p;
        private string[] runArgs;
        public static ArrayList checkedList = new ArrayList();
        public static Node selectedNode;
        public static bool isPay = true;
        public static string userInputAddress = string.Empty;

        public mainForm(string[] args)
        {
            InitializeComponent();
            language = "zh";
            resource = LanguageHelper.SetLang(language, this, typeof(mainForm), resource);
            runArgs = args;
        }

        private void tsmiPlotDir_Click(object sender, EventArgs e)
        {
            DeviceSelectForm deviceSelectForm = new DeviceSelectForm();
            deviceSelectForm.Text = tsmiPlotDir.Text;
            deviceSelectForm.ShowDialog();

            IDbConnection conn = DBHelper.CreateConnection();
            Dictionary<string, object> ups;
            string sql = "REPLACE INTO t_plot(F_Name, F_Path, F_Uuid, F_PlotSeed, F_PlotDir, F_PlotSize, F_PlotParam, F_Status, F_CreateTime, F_ModifyTime) " +
                "VALUES(@Path, @Path, '', @Address, '/plotdata', '8589934592', '{\"startNonce\":0}', 1, @CreateTime, @CreateTime);";
            foreach (KeyValuePair<string,string> item in checkedList)
            {
                ups = new Dictionary<string, object>
                {
                    { "Path", item.Key },
                    { "Address", "0x" + address },
                    { "CreateTime", tsslDate.Text } 
                };
                if (!Directory.Exists(item.Key + "/plotdata"))
                {
                    Directory.CreateDirectory(item.Key + "/plotdata");
                }
                DBHelper.ExecuteNonQuery(conn, sql, ups);
            }
            conn.Close();
            InitializeBackgroundWorker();
            backgroundWorker.RunWorkerAsync();
        }

        private void ScanDevices(BackgroundWorker worker, DoWorkEventArgs e)
        {
            worker.ReportProgress(1);
            Thread.Sleep(100);
            long dirLength;
            foreach (KeyValuePair<string, string> device in checkedList)
            {
                worker.ReportProgress(checkedList.IndexOf(device) / checkedList.Count * 10 * 9);
                Thread.Sleep(100);
                if (!Directory.Exists(device.Key + "\\plotdata"))
                {
                    continue;
                }
                dirLength = FileUtil.DictoryLength(device.Key + "\\plotdata");
                if(dirLength == 8589934592)
                {
                    continue;
                }
                Directory.Delete(device.Key + "\\plotdata", false);
                FileUtil.CopyOldFilesToNewPath(plotdataDir, device.Key + "\\plotdata");
            }
            worker.ReportProgress(100);
            Thread.Sleep(100);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            tsslDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            KeystoreCheck();
            MineStatus();
            if(toolStripProgressBar.Value == 100 || toolStripProgressBar.Value == 0)
            {
                toolStripProgressBar.Visible = false;
            }
        }

        private void TsmiInstall_Click(object sender, EventArgs e)
        {
            InitForm();
            InitDatabase();
            tsslStatus.Text = resource.GetString("tsslStatusSucess");
            labelMsg.Text = resource.GetString("installSuccessTips") + "\r" + resource.GetString("startMineTips");
            tsmiStart.Enabled = true;
            tsmiInstall.Visible = false;
            tsmiImportKeystore.Visible = false;
        }

        [DllImport("User32.dll ", EntryPoint = "SetParent")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll ", EntryPoint = "ShowWindow")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        private void tsmiStart_Click(object sender, EventArgs e)
        {
            InitForm();
            RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
            //DeleteEthDir();
            p = new Process();
            p.StartInfo.FileName = Application.StartupPath + "/bin/poc.exe";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            /*if (runArgs.Length > 0 && runArgs[0] == "showPoc")
            {
                p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            }*/
            p.StartInfo.Arguments = " --datadir=\"" + Application.StartupPath + "\\AppData\\Roaming\\Poc\" --mine --gcmode archive --syncmode=full --networkid 10911 --rpc --rpcaddr \"0.0.0.0\" --rpcport=\"8545\" --rpcapi \"web3,peers,net,account,personal,eth,minedev,txpool,admin\"";
            p.Start();
            processId = p.Id;
            Thread.Sleep(100);//加上，100如果效果没有就继续加大
            SetParent(p.MainWindowHandle, panel1.Handle); //panel1.Handle为要显示外部程序的容器
            ShowWindow(p.MainWindowHandle, 3);
            tsslStatus.Text = resource.GetString("tsslStatusStart");
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
                tsslStatus.Text = resource.GetString("tsslStatusStop");
                tsmiAddPeer.Enabled = false;
            }

            if (p != null && p.ProcessName == "poc")
            {
                tsmiStart.Enabled = false;
                tsmiAddPeer.Enabled = true;
                long mineDirLength = FileUtil.DictoryLength(plotdataDir);
                if (fileList.Length >= 8 && mineDirLength % 1024 == 0)
                {
                    labelMsg.Text = resource.GetString("statusMining") + "\r" + resource.GetString("installTips");
                    tsslStatus.Text = resource.GetString("statusMining");
                }
                else
                {
                    labelMsg.Text = resource.GetString("statusChainSync") + "\r" + resource.GetString("installTips");
                    tsslStatus.Text = resource.GetString("statusChainSync");
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
            tsslStatus.Text = resource.GetString("statusChainSync");
        }

        private delegate void setLogTextDelegate(FileSystemEventArgs e);

        private void tsmiAddPeer_Click(object sender, EventArgs e)
        {
            AddNodeForm addNodeForm = new AddNodeForm();
            addNodeForm.Text = tsmiAddPeer.Text;
            addNodeForm.ShowDialog();
            if (null == selectedNode) return;

            if (!selectedNode.Access)
            {
                QRCodeForm qRCodeForm = new QRCodeForm(language, "100");
                qRCodeForm.ShowDialog();
                if (!isPay) return;

                bool paySuccess = false;
                try
                {
                    string token = "6275dadb8c94c201dbcfedca72f8308fafd1bb4788e1be3061ce5c3bc2e1d0be";
                    RestClient client = new RestClient("https://api.ehd.io/");
                    //string apiResult = client.Get("api?module=block&action=getblocknobytime&timestamp=" + UtcTime.GetTimeStamp() + "&closest=before&apikey=" + token);
                    //JObject json = JsonConvert.DeserializeObject<JObject>(apiResult);
                    //string latestBlock = json["result"].ToString();
                    if (userInputAddress.Length == 0) return;
                    // 取付费记录
                    string apiResult = client.Get("?method=usdt.index&address=" + userInputAddress + "&token=" + token);
                    JObject json = JsonConvert.DeserializeObject<JObject>(apiResult);
                    Block[] blocks = JsonConvert.DeserializeObject<Block[]>(json["data"].ToString());
                    string toAddress = "0x595C230fBfc95A168eD893089C5748Ec8e413694";
                    foreach (Block block in blocks)
                    {
                        if(toAddress.ToLower().Equals(block.To) && "100000000".Equals(block.Value))
                        {
                            paySuccess = true;
                        }
                    }
                    if (!paySuccess) return;
                    // 修改配置文件
                    string strFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
                    if (File.Exists(strFilePath))
                    {
                        string strContent = File.ReadAllText(strFilePath);
                        strContent = Regex.Replace(strContent, selectedNode.NodeId + "=False", selectedNode.NodeId + "=True");
                        File.WriteAllText(strFilePath, strContent);
                    }
                }
                catch (WebException webEx)
                {
                    Console.WriteLine(webEx.ToString());
                    return;
                }
            }

            RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
            Thread.Sleep(1000);
            //WebClient webClient = new WebClient();
            //webClient.DownloadFile("", Application.StartupPath + "\\bin\\" + poc);
            toolStripProgressBar.Visible = true;
            FileDownloadUtil file = new FileDownloadUtil("需要下载的网络地址", "文件下载到的文件夹路径", "带后缀的文件名", "文件ID，按需求自行删除");
            file.ProgressChanged += File_ProgressChanged;
            file.DownloadFinish += File_DownloadFinish;
            file.Download();
            //RunProcess("cmd.exe", "curl  -H \"Content-Type: application/json\" --data \"{\\\"jsonrpc\\\":\\\"2.0\\\",\\\"method\\\":\\\"admin_addPeer\\\",\\\"params\\\":[\\\"enode://" + selectedNode.Address + ":30303\\\"],\\\"id\\\":1}\" http://127.0.0.1:8545");
            MessageBox.Show(resource.GetString("nodeDownloadTips"));
        }

        private void File_DownloadFinish(bool isSuccess, string downloadPath, string fileId, string msg = null)
        {
            if (isSuccess)
            {
                //Console.WriteLine("文件 " + fileId + " 下载成功，保存位置：" + downloadPath);
                MessageBox.Show(resource.GetString("addNodeMsg"));
            }
            else
            {
                Console.WriteLine("文件 " + fileId + " 下载失败 " + msg == null ? "" : "原因：" + msg);
            }
            toolStripProgressBar.Visible = false;
        }

        private void File_ProgressChanged(int progress, string fileId)
        {
            Console.WriteLine("文件 " + fileId + " 下载进度：" + progress);
            toolStripProgressBar.Value = progress;
        }

        [Obsolete]
        private void tsmiShowInfo_Click(object sender, EventArgs e)
        {
            string minerInfo = resource.GetString("localIp") + AddressHelper.GetLocalIP();
            minerInfo += "\r" + resource.GetString("localMac") + AddressHelper.GetLocalMac();
            fileList = Directory.GetFiles(keystoreDir);
            if (fileList.Length > 0)
            {
                minerInfo += "\r" + resource.GetString("walletAddress") + "0x" + address;
            }
            if (address.Length > 0)
            {
                minerInfo += "\r" + resource.GetString("minerName") + "EHD-miner-" + address.Substring(36);
            }
            DialogResult dr = MessageBox.Show(minerInfo, resource.GetString("copyTips") + "\r" + resource.GetString("tsmiShowInfo.Text"), MessageBoxButtons.OK);
            if (dr == DialogResult.OK)
            {
                Clipboard.SetDataObject(minerInfo);
            }
        }

        private void TsmiImportKeystore_Click(object sender, EventArgs e)
        {
            InitForm();
            labelMsg.Text = resource.GetString("ksImportTips");
            textBox1.Show();
            btnSaveKS.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
            InitForm();
            InitDirectoryAndFile();
            KeystoreCheck();
        }

        private void InitForm()
        {
            labelMsg.Text = string.Empty;
            textBox1.Hide();
            btnSaveKS.Hide();
            toolStripProgressBar.Visible = false;
        }

        private void KeystoreCheck()
        {
            if (!Directory.Exists(keystoreDir)
            || Directory.GetFiles(keystoreDir).Length == 0)
            {
                tsmiInstall.Enabled = false;
                tsmiStart.Enabled = false;
            }
            else
            {
                textBox1.Hide();
                btnSaveKS.Hide();

                fileList = Directory.GetFiles(keystoreDir);
                address = Path.GetFileName(fileList[0]).Substring(37);
                string sql = "select count(1) from t_user where F_Username = @Username;";
                int result = Convert.ToInt32(DBHelper.ExecuteScalar(sql, new Dictionary<string, object> { { "Username", "0x" + address } }));
                if (result == 1)
                {
                    tsmiStart.Enabled = true;
                    tsmiImportKeystore.Visible = false;
                    tsmiInstall.Visible = false;
                    long mineDirLength = FileUtil.DictoryLength(plotdataDir);
                    if (mineDirLength == 17179869184 || mineDirLength == 8589934592)
                    {
                        tsmiPlotDir.Enabled = true;
                    }
                    labelMsg.Text = resource.GetString("congratulations") + "\r" + resource.GetString("startMineTips");
                    //tsslStatus.Text = resource.GetString("tsslStatusSucess");
                }
                else
                {
                    tsmiInstall.Enabled = true;
                }
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
            File.SetAttributes(Application.StartupPath + "\\AppData", FileAttributes.Hidden);
            File.SetAttributes(Application.StartupPath + "\\bin", FileAttributes.Hidden);
            File.SetAttributes(Application.StartupPath + "\\" + database, FileAttributes.Hidden);
        }

        private void InitDatabase()
        {
            fileList = Directory.GetFiles(keystoreDir);
            if (fileList.Length == 0)
            {
                tsslStatus.Text = resource.GetString("ksUnfind");
                return;
            }
            IDbConnection conn = DBHelper.CreateConnection();
            string index = "CREATE UNIQUE INDEX index_path ON t_plot ( F_path );";
            DBHelper.ExecuteNonQuery(conn, index, new Dictionary<string, object>());

            string sql = "REPLACE INTO t_host(F_id, F_Hostname, F_Status, F_CreateTime,F_ModifyTime) VALUES(1, @HostName, 0, @CreateTime, @CreateTime);";
            Dictionary<string, object> ups = new Dictionary<string, object>
            {
                { "HostName", "EHD-miner-" + address.Substring(36) },//EHD-miner- 钱包后4位
                { "CreateTime", tsslDate.Text }
            };
            DBHelper.ExecuteNonQuery(conn, sql, ups);

            string sql2 = "REPLACE INTO t_user(F_id, F_Username,F_Password, F_CreateTime, F_ModifyTime) VALUES(1, @Address, @Password, @CreateTime, @CreateTime);";
            Dictionary<string, object> ups2 = new Dictionary<string, object>
            {
                { "Address", "0x" + address },//keystore文件名第37位起
                { "Password", "ehd123123" },//keystore文件名第37位起
                { "CreateTime", tsslDate.Text }
            };
            DBHelper.ExecuteNonQuery(conn, sql2, ups2);

            string sql3 = "REPLACE INTO t_plot(F_id, F_Name, F_Path, F_Uuid, F_PlotSeed, F_PlotDir, F_PlotSize, F_PlotParam, F_Status, F_CreateTime, F_ModifyTime) " +
                "VALUES(1, 'default', @Path, '', @Address, @PlotDir, '8589934592', '{\"startNonce\":0}', 1, @CreateTime, @CreateTime);";
            Dictionary<string, object> ups3 = new Dictionary<string, object>
            {
                { "Path", Application.StartupPath },
                { "Address", "0x" + address },
                { "PlotDir", "/AppData/Roaming/Poc/plotdata" },
                { "CreateTime", tsslDate.Text }
            };
            DBHelper.ExecuteNonQuery(conn, sql3, ups3);
            conn.Close();
        }

        private void BtnSaveKS_Click(object sender, EventArgs e)
        {
            string utcTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss.fffffff00Z");
            JObject jo;
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

                tsslStatus.Text = resource.GetString("ksImportSucess");
                labelMsg.Text = tsslStatus.Text;
                textBox1.Text = string.Empty;
                textBox1.Hide();
                btnSaveKS.Hide();

                string sql = "select count(1) from t_user where F_Username = @Username;";
                int result = Convert.ToInt32(DBHelper.ExecuteScalar(sql, new Dictionary<string, object> { { "Username", "0x" + address } }));
                if (result == 1)
                {
                    tsmiStart.Enabled = true;
                    tsmiImportKeystore.Visible = false;
                    tsmiInstall.Visible = false;
                    labelMsg.Text = resource.GetString("congratulations") + "\r" + resource.GetString("startMineTips");
                    tsslStatus.Text = resource.GetString("tsslStatusSucess");
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

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ScanDevices(sender as BackgroundWorker, e);
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tsslStatus.Text = resource.GetString("tsslStatusScaning");
            toolStripProgressBar.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tsslStatus.Text = resource.GetString("tsslStatusScaned");
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker_ProgressChanged);
            toolStripProgressBar.Visible = true;
        }

        private void TsmiRepairFork_Click(object sender, EventArgs e)
        {

            DialogResult flag = MessageBox.Show(resource.GetString("repairForkWarn"), resource.GetString("warn"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (flag == DialogResult.OK)
            {
                RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
                Thread.Sleep(1000);
                DeleteEthDir();
                tsslStatus.Text = resource.GetString("repairForkSuccess");
                labelMsg.Text = tsslStatus.Text;
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
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;    //取消"关闭窗口"事件
                WindowState = FormWindowState.Minimized;    //使关闭时窗口向右下角缩小的效果
                Hide();
                return;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon1.Dispose();
            Dispose();
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                return;
            }
            else
            {
                Show();
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

        private void TsmiScanner_Click(object sender, EventArgs e)
        {
            Process.Start("https://scan.ehd.io");
        }

        private void TsmiWebsite_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.ehd.io");
        }
    }
}
