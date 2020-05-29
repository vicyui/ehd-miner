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
using System.Threading;
using System.Windows.Forms;

namespace EHDMiner
{
    public partial class mainForm : Form
    {
        private string[] fileList;
        private string address = string.Empty;
        private readonly string description = "description.xml";
        private readonly string database = "database.sqlite";
        private readonly string poc = "poc.exe";
        private readonly string keystoreDir = Application.StartupPath + "\\AppData\\Roaming\\Poc\\keystore";
        private readonly string plotdataDir = Application.StartupPath + "\\AppData\\Roaming\\Poc\\plotdata";
        private readonly string ethDir = Application.StartupPath + "\\AppData\\Roaming\\Poc\\eth";
        public static string language = string.Empty;
        private readonly ComponentResourceManager resource;
        private int processId;
        private Process p;
        private readonly string[] runArgs;
        public static ArrayList checkedList = new ArrayList();
        public static Node selectedNode;
        public static bool isPay = true;
        public static string userInput = string.Empty;
        public static int countDeviceSelect = 0;
        private readonly string token = "6275dadb8c94c201dbcfedca72f8308fafd1bb4788e1be3061ce5c3bc2e1d0be";
        private readonly string toAddress = "0x595C230fBfc95A168eD893089C5748Ec8e413694";
        private readonly RestClient client = new RestClient("https://api.ehd.io/");
        private int position = 0;

        public mainForm(string[] args)
        {
            InitializeComponent();
            language = "zh";
            resource = LanguageHelper.SetLang(language, this, typeof(mainForm), resource);
            runArgs = args;
        }

        private void tsmiPlotDir_Click(object sender, EventArgs e)
        {
            DeviceSelectForm deviceSelectForm = new DeviceSelectForm
            {
                Text = tsmiPlotDir.Text
            };
            deviceSelectForm.ShowDialog();

            if (checkedList.Count == 0) return;

            if (countDeviceSelect > 0)
            {
                QRCodeForm qRCodeForm = new QRCodeForm("10");
                qRCodeForm.ShowDialog();

                if (!isPay) return;
                bool paySuccess = false;
                if (userInput.Length == 0) return;

                try
                {
                    // 取付费记录
                    string apiResult = client.Get("?method=usdt.verify&hash=" + userInput + "&token=" + token);
                    JObject json = JsonConvert.DeserializeObject<JObject>(apiResult);
                    Block block = JsonConvert.DeserializeObject<Block>(json["data"].ToString());
                    //foreach (Block block in blocks)
                    //{
                    if (DateTime.Compare(DateTime.Now, block.Used_at.AddHours(3)) > 0)
                    {
                        MessageBox.Show(resource.GetString("orderNotFound"));
                        return;
                    }
                    if (toAddress.ToLower().Equals(block.To.ToLower()) && "10000000".Equals(block.Value))
                    {
                        paySuccess = true;
                    }
                    //}
                    if (!paySuccess)
                    {
                        MessageBox.Show(resource.GetString("orderNotFound"));
                        return;
                    }
                }
                catch (WebException)
                {
                    MessageBox.Show(resource.GetString("networkException"));
                    return;
                }
                catch (Exception)
                {
                    return;
                }
            }

            IDbConnection conn = DBHelper.CreateConnection();
            Dictionary<string, object> ups;
            string sql = "REPLACE INTO t_plot(F_Name, F_Path, F_Uuid, F_PlotSeed, F_PlotDir, F_PlotSize, F_PlotParam, F_Status, F_CreateTime, F_ModifyTime) " +
                "VALUES(@Path, @Path, '', @Address, '/plotdata', '8589934592', '{\"startNonce\":0}', 1, @CreateTime, @CreateTime);";
            foreach (KeyValuePair<string, string> item in checkedList)
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

            tsmiPlotDir.Enabled = false;
            Thread thread = new Thread(new ThreadStart(CopyPlotdata));
            thread.Start();
        }

        private void CopyPlotdata()
        {
            position = 1;
            int count = 0;
            long space;
            try
            {
                foreach (KeyValuePair<string, string> device in checkedList)
                {
                    space = FileUtil.GetHardDiskSpace(device.Key);
                    for (int i = 0; i < space - 10; i++)
                    {
                        File.Copy(plotdataDir + "\\" + address + "_0_4096", device.Key + "\\plotdata\\" + address + "_" + count + "_4096", true);
                        count += 4096;
                        //Console.WriteLine((i + 1)*1000 / (Convert.ToInt32(space)*1000));// * 100 * ((checkedList.IndexOf(device) + 1) / checkedList.Count));
                        //position = ((i + 1) * 1000) / (Convert.ToInt32(space) * 1000) * ((checkedList.IndexOf(device) + 1) / checkedList.Count) / 10;
                    }
                    //position = (checkedList.IndexOf(device) + 1) / checkedList.Count * 100;
                }
            }
            catch (Exception)
            {
                return;
            }
            MessageBox.Show(resource.GetString("plotSucess"),resource.GetString("tips"));
            position = 2;
        }

        private void TimerMain_Tick(object sender, EventArgs e)
        {
            tsslDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            KeystoreCheck();
            MineStatus();
            if (toolStripProgressBar.Value > 0)
            {
                toolStripProgressBar.Visible = true;
            }
            else
            {
                toolStripProgressBar.Visible = false;
            }
            InitNodeStatus();
        }

        private void TsmiInstall_Click(object sender, EventArgs e)
        {
            InitForm();
            InitDatabase();
            /*tsslStatus.Text = resource.GetString("tsslStatusSucess");
            labelMsg.Text = resource.GetString("installSuccessTips") + "\r" + resource.GetString("startMineTips");*/
            tsmiStart.Enabled = true;
            tsmiInstall.Visible = false;
            tsmiImportKeystore.Visible = false;
        }

        [DllImport("User32.dll ", EntryPoint = "SetParent")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll ", EntryPoint = "ShowWindow")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [Obsolete]
        private void tsmiStart_Click(object sender, EventArgs e)
        {
            InitForm();
            RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
            //DeleteEthDir();
            try
            {
                p = new Process();
                p.StartInfo.FileName = Application.StartupPath + "/bin/poc.exe";
                p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                /*if (runArgs.Length > 0 && runArgs[0] == "showPoc")
                {
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                }*/
                p.StartInfo.Arguments = " --datadir=\"" + Application.StartupPath + "\\AppData\\Roaming\\Poc\" --mine --gcmode archive --syncmode=full --networkid 10911 --rpc --rpcaddr \"0.0.0.0\" --rpcport=\"8545\" --rpcapi \"web3,peers,net,account,personal,eth,minedev,txpool,admin\"";
                p.Start();
            }
            catch (Exception)
            {
                FileUtil.ExtractResFile("EHDMiner.Resources." + poc, Application.StartupPath + "\\bin\\" + poc);
                return;
            }
            processId = p.Id;
            Thread.Sleep(100);//加上，100如果效果没有就继续加大
            SetParent(p.MainWindowHandle, panelMain.Handle); //panel1.Handle为要显示外部程序的容器
            ShowWindow(p.MainWindowHandle, 3);
            tsmiStart.Enabled = false;
            updateMinerInfo();
            timerUpdate.Enabled = true;
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
                timerUpdate.Enabled = false;
            }

            if (p != null && p.ProcessName == "poc")
            {
                tsmiStart.Enabled = false;
                long mineDirLength = FileUtil.PlotdataDictoryLength(plotdataDir);
                string[] plotdatas = Directory.GetFiles(plotdataDir);
                if (plotdatas.Length >= 8 && mineDirLength % 1024 == 0)
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

        private void tsmiAddPeer_Click(object sender, EventArgs e)
        {
            tsmiAddPeer.Enabled = false;
            AddNodeForm addNodeForm = new AddNodeForm
            {
                Text = tsmiAddPeer.Text
            };
            addNodeForm.ShowDialog();
            if (null == selectedNode)
            {
                tsmiAddPeer.Enabled = true;
                return;
            }

            if (0 == selectedNode.Access)
            {
                QRCodeForm qRCodeForm = new QRCodeForm("100");
                qRCodeForm.ShowDialog();
                if (!isPay)
                {
                    tsmiAddPeer.Enabled = true;
                    return;
                }
                bool paySuccess = false;
                if (userInput.Length == 0) return;
                
                try
                {
                    //string apiResult = client.Get("api?module=block&action=getblocknobytime&timestamp=" + UtcTime.GetTimeStamp() + "&closest=before&apikey=" + token);
                    //JObject json = JsonConvert.DeserializeObject<JObject>(apiResult);
                    //string latestBlock = json["result"].ToString();
                    // 付费记录
                    string apiResult = client.Get("?method=usdt.verify&hash=" + userInput + "&token=" + token);
                    JObject json = JsonConvert.DeserializeObject<JObject>(apiResult);
                    Block block = JsonConvert.DeserializeObject<Block>(json["data"].ToString());
                    // foreach (Block block in blocks)
                    //{
                    if (DateTime.Compare(DateTime.Now, block.Used_at.AddHours(3)) > 0)
                    {
                        MessageBox.Show(resource.GetString("orderNotFound"));
                        return;
                    }
                    if (toAddress.ToLower().Equals(block.To.ToLower()) && "100000000".Equals(block.Value))
                    {
                        paySuccess = true;
                    }
                    //}
                    if (!paySuccess)
                    {
                        MessageBox.Show(resource.GetString("orderNotFound"));
                        return;
                    }
                    
                    // 修改配置文件
                    //string strFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config");
                    //if (File.Exists(strFilePath))
                    //{
                    //    string strContent = File.ReadAllText(strFilePath);
                    //    strContent = Regex.Replace(strContent, selectedNode.Id + "=False", selectedNode.Id + "=True");
                    //    File.WriteAllText(strFilePath, strContent);
                    //}
                }
                catch (WebException)
                {
                    MessageBox.Show(resource.GetString("networkException"));
                    return;
                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    tsmiAddPeer.Enabled = true;
                }
            }
            tsmiAddPeer.Enabled = true;

            IDbConnection conn = DBHelper.CreateConnection();
            string offUsedNode = "update t_node set on_used = 0;";//取消所有已选择
            DBHelper.ExecuteNonQuery(conn, offUsedNode, new Dictionary<string, object>());
            string updateNode = string.Empty;
            if (selectedNode.Access == 0)
            {
                updateNode = "update t_node set on_used = 1,access = 1 ,end_date=@date where id=@id;";
                DBHelper.ExecuteNonQuery(conn, updateNode, new Dictionary<string, object>()
                        {
                            { "id", selectedNode.Id },
                            { "date", DateTime.Now.AddDays(30).ToString("yyyy-MM-dd") }
                        });
            }
            else
            {
                updateNode = "update t_node set on_used = 1,access = 1 where id=@id;";
                DBHelper.ExecuteNonQuery(conn, updateNode, new Dictionary<string, object>()
                        {
                            { "id", selectedNode.Id }
                        });
            }

            DialogResult dr = MessageBox.Show(resource.GetString("nodeDownloadTips"), resource.GetString("tips"), MessageBoxButtons.OKCancel);
            if (dr == DialogResult.Cancel)
            {
                return;
            }
            RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
            Thread.Sleep(2000);
            tsmiStart.Visible = false;
            tsmiAdvanced.Enabled = false;

            if (selectedNode.Address == "sz")
            {
                string downloadUrl = client.Get("?method=mine.version&client=EHDMiner&token=" + token);
                JObject jsonUrl = JsonConvert.DeserializeObject<JObject>(downloadUrl);
                string url = jsonUrl["data"]["url"].ToString();
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFileAsync(new Uri(url), Path.Combine(Application.StartupPath + "\\bin\\", "poc.exe"));
                    webClient.DownloadProgressChanged += client_DownloadProgressChanged;
                    webClient.DownloadFileCompleted += client_DownloadFileCompleted;
                }
            }
            else
            {
                FileUtil.ExtractResFile("EHDMiner.Resources." + poc, Application.StartupPath + "\\bin\\" + poc);
                MessageBox.Show(resource.GetString("addNodeMsg"));
                tsmiStart.Visible = true;
                tsmiAdvanced.Enabled = true;
            }
            //RunProcess("cmd.exe", "curl  -H \"Content-Type: application/json\" --data \"{\\\"jsonrpc\\\":\\\"2.0\\\",\\\"method\\\":\\\"admin_addPeer\\\",\\\"params\\\":[\\\"enode://" + selectedNode.Address + ":30303\\\"],\\\"id\\\":1}\" http://127.0.0.1:8545");
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            toolStripProgressBar.Value = e.ProgressPercentage;
            tsmiStart.Visible = false;
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;
            MessageBox.Show(resource.GetString("addNodeMsg"));
            tsmiStart.Visible = true;
            tsmiAdvanced.Enabled = true;
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
                tsmiPlotDir.Enabled = false;
                tsslStatus.Text = resource.GetString("keystoreTips");
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
                    long mineDirLength = FileUtil.PlotdataDictoryLength(plotdataDir);
                    if (position == 0 && (mineDirLength == 17179869184 || mineDirLength == 8589934592))
                    {
                        tsmiPlotDir.Enabled = true;
                    }
                    if(position == 1)
                    {
                        tsslStatus.Text = resource.GetString("statusPlotting");
                        tsmiPlotDir.Enabled = false;
                    }
                    if(position == 2)
                    {
                        position = 0;
                        tsslStatus.Text = resource.GetString("statusPlotFinish");
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
            notifyIcon.Dispose();
            RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
            Dispose();
            Application.Exit();
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
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

        private void InitNodeStatus()
        {
            tsslNode.Alignment = ToolStripItemAlignment.Right;
            /*tsslNode.Text = "Free node";
            if (language == "zh")
            {
                tsslNode.Text = "免费节点";
            }*/
            //string selectTable = "SELECT COUNT(*) FROM sqlite_master where type = 'table' and name = 't_node';";
            //int count = Convert.ToInt32(DBHelper.ExecuteScalar(selectTable, new Dictionary<string, object>()));

            IDbConnection conn = DBHelper.CreateConnection();
            string createTable = "CREATE TABLE if not exists \"t_node\"(\"id\" integer NOT NULL PRIMARY KEY AUTOINCREMENT,\"name_zh\" text NOT NULL,\"name_en\" text NOT NULL,\"address\" text NOT NULL  DEFAULT deafult,\"access\" integer NOT NULL DEFAULT 0,\"end_date\" text,\"on_used\" integer NOT NULL DEFAULT 0); ";
            DBHelper.ExecuteNonQuery(conn, createTable, new Dictionary<string, object>());
            string insertNodes =
                "INSERT OR IGNORE INTO \"t_node\" VALUES(0, '免费节点', 'Free node', 'deafult', 1, NULL, 1); " +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(1, '中国区华南节点', 'South China node in China', 'sz', 0, NULL, 0);" +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(2, '中国区华中节点', 'China central node', 'sz', 0, NULL, 0);" +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(3, '中国区华北节点', 'North China node in China', 'sz', 0, NULL, 0);" +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(4, '中国区东北节点', 'Northeast node of China', 'sz', 0, NULL, 0);" +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(5, '中国区西南节点', 'Southwest node of China', 'sz', 0, NULL, 0);" +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(6, '中国区西北节点', 'Northwest node of China', 'sz', 0, NULL, 0);" +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(7, '亚洲区韩国节点', 'South Korea node in Asia', 'deafult', 0, NULL, 0);" +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(8, '亚洲区日本节点', 'Japan node in Asia', 'deafult', 0, NULL, 0);" +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(9, '亚洲新加坡节点', 'Singapore node in Asia', 'deafult', 0, NULL, 0);" +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(10, '欧洲区英国节点', 'UK node in Europe', 'deafult', 0, NULL, 0);" +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(11, '欧洲区德国节点', 'German node in Europe', 'deafult', 0, NULL, 0);" +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(12, '美州区美国节点', 'Us node', 'deafult', 0, NULL, 0);" +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(13, '非洲区南非节点', 'Africa South Africa node', 'deafult', 0, NULL, 0);" +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(14, '大洋洲区巴西节点', 'Oceania Brazil node', 'deafult', 0, NULL, 0); " +
                "INSERT OR IGNORE INTO \"t_node\" VALUES(15, '中国区华东节点', 'East China node in China', 'sz', 0, NULL, 0); " +
                "UPDATE \"t_node\" SET name_en = 'Oceania Brazil node' where name_en = '.Oceania Brazil node'; ";
            DBHelper.ExecuteNonQuery(conn, insertNodes, new Dictionary<string, object>());
            conn.Close();
            
            string sql = "select * from t_node where on_used = 1;";
            DataTable table = DBHelper.ExecuteQuery(sql, new Dictionary<string, object>());
            IList<Node> nodes = DataTableConverterHelper<Node>.ConvertToModelList(table);

            if (null != nodes[0])
            {
                tsslNode.Text = nodes[0].Name_zh;
                if (language == "en")
                {
                    tsslNode.Text = nodes[0].Name_en;
                }

                if(nodes[0].End_date != null)
                {
                    tsslNode.Text += "|" + resource.GetString("nodeStatusTips") + nodes[0].End_date;
                }

                if(nodes[0].End_date == DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
                    string updateNode = "update t_node set on_used = 0,access = 0,end_date = @date where id = @id;"
                        + "update t_node set on_used =1 where id = 0";
                    DBHelper.ExecuteNonQuery(updateNode, new Dictionary<string, object> { { "id", nodes[0].Id }, { "date", null } });
                    FileUtil.ExtractResFile("EHDMiner.Resources." + poc, Application.StartupPath + "\\bin\\" + poc);
                }
            }
        }

        private void tsmiChangeKeystore_Click(object sender, EventArgs e)
        {
            QRCodeForm qRCodeForm = new QRCodeForm("50");
            qRCodeForm.ShowDialog();
            if (!isPay) return;

            bool paySuccess = false;
            if (userInput.Length == 0) return;

            try
            {
                // 付费记录
                string apiResult = client.Get("?method=usdt.verify&hash=" + userInput + "&token=" + token);
                JObject json = JsonConvert.DeserializeObject<JObject>(apiResult);
                Block block = JsonConvert.DeserializeObject<Block>(json["data"].ToString());
                //foreach (Block block in blocks)
                //{
                if (DateTime.Compare(DateTime.Now, block.Used_at.AddHours(3)) > 0)
                {
                    MessageBox.Show(resource.GetString("orderNotFound"));
                    return;
                }
                if (toAddress.ToLower().Equals(block.To.ToLower()) && "50000000".Equals(block.Value))
                {
                    paySuccess = true;
                }
                //}
                if (!paySuccess)
                {
                    MessageBox.Show(resource.GetString("orderNotFound"));
                    return;
                }

                Directory.Delete(keystoreDir, true);
                Directory.CreateDirectory(keystoreDir);
                MessageBox.Show(resource.GetString("changeKSSuccess"));
                tsmiImportKeystore.Visible = true;
                tsmiInstall.Visible = true;
                labelMsg.Text = string.Empty;
            }
            catch (WebException)
            {
                MessageBox.Show(resource.GetString("networkException"));
                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            //控件弹出菜单来自于NotifyIconMenu
            DialogResult flag = MessageBox.Show(resource.GetString("quitTips"), resource.GetString("tips"), MessageBoxButtons.OKCancel);
            if (flag == DialogResult.OK)
            {
                RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
                Dispose();
                Application.Exit();
            }
        }

        [Obsolete]
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            if(0 == DateTime.Now.Minute || 30 == DateTime.Now.Minute)
                updateMinerInfo();
        }

        [Obsolete]
        private void updateMinerInfo()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("token", token);

            param.Add("method", "miner.register");

            //string ip = RunProcess("cmd.exe", "curl ipinfo.io");
            //param.Add("ip", JsonConvert.DeserializeObject<JObject>(ip)["ip"]);
            param.Add("mac", AddressHelper.GetLocalMac());

            param.Add("coinbase", "0x" + address);

            long[] size = FileUtil.GetHardDiskSpace();
            param.Add("total_disk_size", size[0]);
            param.Add("total_plotted_disk_size", size[1] + FileUtil.PlotdataDictoryLength(plotdataDir) / 2);// 每个盘符的plotdata目录大小+程序本身批盘目录大小

            string nodekey;
            do
            {
                nodekey = RunProcess("cmd.exe", "curl  -H \"Content-Type: application/json\" --data \"{\\\"jsonrpc\\\":\\\"2.0\\\",\\\"method\\\":\\\"admin_nodeInfo\\\",\\\"params\\\":[],\\\"id\\\":1}\" http://127.0.0.1:8545");
                Thread.Sleep(1000);
            } while (nodekey.Length == 0);
            param.Add("nodekey", JsonConvert.DeserializeObject<JObject>(nodekey)["result"]["id"]);

            string result = RunProcess("cmd.exe", "curl  -H \"Content-Type: application/json\" --data \"{\\\"jsonrpc\\\":\\\"2.0\\\",\\\"method\\\":\\\"admin_peers\\\",\\\"params\\\":[],\\\"id\\\":1}\" http://127.0.0.1:8545");
            JObject resultJson = JsonConvert.DeserializeObject<JObject>(result);
            var resultArray = JArray.Parse(resultJson["result"].ToString());
            IList<JValue> peers = new List<JValue>();
            foreach (var item in resultArray)
            {
                peers.Add((JValue)item["id"]);
            }
            param.Add("peers", peers);

            string is_mining = RunProcess("cmd.exe", "curl  -H \"Content-Type: application/json\" --data \"{\\\"jsonrpc\\\":\\\"2.0\\\",\\\"method\\\":\\\"eth_mining\\\",\\\"params\\\":[],\\\"id\\\":1}\" http://127.0.0.1:8545");
            if(JsonConvert.DeserializeObject<JObject>(is_mining)["result"].ToString() == "True")
            {
                is_mining = "Y";
            }
            else
            {
                is_mining = "N";
            }
            param.Add("is_mining", is_mining);

            string is_syncing = RunProcess("cmd.exe", "curl  -H \"Content-Type: application/json\" --data \"{\\\"jsonrpc\\\":\\\"2.0\\\",\\\"method\\\":\\\"eth_syncing\\\",\\\"params\\\":[],\\\"id\\\":1}\" http://127.0.0.1:8545");
            if (JsonConvert.DeserializeObject<JObject>(is_syncing)["result"].ToString() == "True")
            {
                is_syncing = "Y";
            }
            else
            {
                is_syncing = "N";
            }
            param.Add("is_syncing", is_syncing);

            string block_number = RunProcess("cmd.exe", "curl  -H \"Content-Type: application/json\" --data \"{\\\"jsonrpc\\\":\\\"2.0\\\",\\\"method\\\":\\\"eth_blockNumber\\\",\\\"params\\\":[],\\\"id\\\":1}\" http://127.0.0.1:8545");
            param.Add("block_number", Convert.ToInt32(JsonConvert.DeserializeObject<JObject>(block_number)["result"].ToString(),16));

            string sql = "select * from t_node where on_used = 1;";
            DataTable table = DBHelper.ExecuteQuery(sql, new Dictionary<string, object>());
            IList<Node> nodes = DataTableConverterHelper<Node>.ConvertToModelList(table);
            string paid_node = nodes[0].Name_en;
            param.Add("paid_node", paid_node);
            //Console.WriteLine(JsonConvert.SerializeObject(param));
            try
            {
                client.Post(JsonConvert.SerializeObject(param), "");
            }
            catch (Exception)
            {
                return;
            }
        }

        private void tsmiClose_Click(object sender, EventArgs e)
        {
            DialogResult flag = MessageBox.Show(resource.GetString("quitTips"), resource.GetString("tips"), MessageBoxButtons.OKCancel);
            if (flag == DialogResult.OK)
            {
                RunProcess("cmd.exe", "taskkill /F /IM poc.exe");
                Dispose();
                Application.Exit();
            }
        }
    }
}
