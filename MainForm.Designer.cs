namespace EHDMiner
{
    partial class mainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmiImportKeystore = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiInstall = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiStart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAdvanced = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddPeer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPlotDir = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiChangeKeystore = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRepairFork = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScanner = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiWebsite = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLangCN = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLangEN = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClose = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tsslDate = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.tsslNode = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.panelMain = new System.Windows.Forms.Panel();
            this.btnSaveKS = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.labelMsg = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiImportKeystore,
            this.tsmiInstall,
            this.tsmiStart,
            this.tsmiAdvanced,
            this.tsmiShowInfo,
            this.tsmiScanner,
            this.tsmiWebsite,
            this.tsmiLanguage,
            this.tsmiClose});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // tsmiImportKeystore
            // 
            this.tsmiImportKeystore.Name = "tsmiImportKeystore";
            resources.ApplyResources(this.tsmiImportKeystore, "tsmiImportKeystore");
            this.tsmiImportKeystore.Click += new System.EventHandler(this.TsmiImportKeystore_Click);
            // 
            // tsmiInstall
            // 
            resources.ApplyResources(this.tsmiInstall, "tsmiInstall");
            this.tsmiInstall.Name = "tsmiInstall";
            this.tsmiInstall.Click += new System.EventHandler(this.TsmiInstall_Click);
            // 
            // tsmiStart
            // 
            resources.ApplyResources(this.tsmiStart, "tsmiStart");
            this.tsmiStart.Name = "tsmiStart";
            this.tsmiStart.Click += new System.EventHandler(this.tsmiStart_Click);
            // 
            // tsmiAdvanced
            // 
            this.tsmiAdvanced.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddPeer,
            this.tsmiPlotDir,
            this.tsmiChangeKeystore,
            this.tsmiRepairFork});
            this.tsmiAdvanced.Name = "tsmiAdvanced";
            resources.ApplyResources(this.tsmiAdvanced, "tsmiAdvanced");
            // 
            // tsmiAddPeer
            // 
            this.tsmiAddPeer.Name = "tsmiAddPeer";
            resources.ApplyResources(this.tsmiAddPeer, "tsmiAddPeer");
            this.tsmiAddPeer.Click += new System.EventHandler(this.tsmiAddPeer_Click);
            // 
            // tsmiPlotDir
            // 
            resources.ApplyResources(this.tsmiPlotDir, "tsmiPlotDir");
            this.tsmiPlotDir.Name = "tsmiPlotDir";
            this.tsmiPlotDir.Click += new System.EventHandler(this.tsmiPlotDir_Click);
            // 
            // tsmiChangeKeystore
            // 
            this.tsmiChangeKeystore.Name = "tsmiChangeKeystore";
            resources.ApplyResources(this.tsmiChangeKeystore, "tsmiChangeKeystore");
            this.tsmiChangeKeystore.Click += new System.EventHandler(this.tsmiChangeKeystore_Click);
            // 
            // tsmiRepairFork
            // 
            this.tsmiRepairFork.Name = "tsmiRepairFork";
            resources.ApplyResources(this.tsmiRepairFork, "tsmiRepairFork");
            this.tsmiRepairFork.Click += new System.EventHandler(this.TsmiRepairFork_Click);
            // 
            // tsmiShowInfo
            // 
            this.tsmiShowInfo.Name = "tsmiShowInfo";
            resources.ApplyResources(this.tsmiShowInfo, "tsmiShowInfo");
            this.tsmiShowInfo.Click += new System.EventHandler(this.tsmiShowInfo_Click);
            // 
            // tsmiScanner
            // 
            this.tsmiScanner.Name = "tsmiScanner";
            resources.ApplyResources(this.tsmiScanner, "tsmiScanner");
            this.tsmiScanner.Click += new System.EventHandler(this.TsmiScanner_Click);
            // 
            // tsmiWebsite
            // 
            this.tsmiWebsite.Name = "tsmiWebsite";
            resources.ApplyResources(this.tsmiWebsite, "tsmiWebsite");
            this.tsmiWebsite.Click += new System.EventHandler(this.TsmiWebsite_Click);
            // 
            // tsmiLanguage
            // 
            this.tsmiLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLangCN,
            this.tsmiLangEN});
            this.tsmiLanguage.Name = "tsmiLanguage";
            resources.ApplyResources(this.tsmiLanguage, "tsmiLanguage");
            // 
            // tsmiLangCN
            // 
            this.tsmiLangCN.Name = "tsmiLangCN";
            resources.ApplyResources(this.tsmiLangCN, "tsmiLangCN");
            this.tsmiLangCN.Click += new System.EventHandler(this.TsmiLangCN_Click);
            // 
            // tsmiLangEN
            // 
            this.tsmiLangEN.Name = "tsmiLangEN";
            resources.ApplyResources(this.tsmiLangEN, "tsmiLangEN");
            this.tsmiLangEN.Click += new System.EventHandler(this.TsmiLangEN_Click);
            // 
            // tsmiClose
            // 
            this.tsmiClose.Name = "tsmiClose";
            resources.ApplyResources(this.tsmiClose, "tsmiClose");
            this.tsmiClose.Click += new System.EventHandler(this.tsmiClose_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslDate,
            this.tsslStatus,
            this.toolStripProgressBar,
            this.tsslNode});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.SizingGrip = false;
            // 
            // tsslDate
            // 
            this.tsslDate.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslDate.Name = "tsslDate";
            resources.ApplyResources(this.tsslDate, "tsslDate");
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            resources.ApplyResources(this.tsslStatus, "tsslStatus");
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            resources.ApplyResources(this.toolStripProgressBar, "toolStripProgressBar");
            // 
            // tsslNode
            // 
            this.tsslNode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsslNode.Name = "tsslNode";
            resources.ApplyResources(this.tsslNode, "tsslNode");
            // 
            // timerMain
            // 
            this.timerMain.Enabled = true;
            this.timerMain.Interval = 1000;
            this.timerMain.Tick += new System.EventHandler(this.TimerMain_Tick);
            // 
            // panelMain
            // 
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Controls.Add(this.btnSaveKS);
            this.panelMain.Controls.Add(this.textBox1);
            this.panelMain.Controls.Add(this.labelMsg);
            this.panelMain.Name = "panelMain";
            // 
            // btnSaveKS
            // 
            this.btnSaveKS.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnSaveKS, "btnSaveKS");
            this.btnSaveKS.Name = "btnSaveKS";
            this.btnSaveKS.UseVisualStyleBackColor = true;
            this.btnSaveKS.Click += new System.EventHandler(this.BtnSaveKS_Click);
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // labelMsg
            // 
            resources.ApplyResources(this.labelMsg, "labelMsg");
            this.labelMsg.Name = "labelMsg";
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            resources.ApplyResources(this.notifyIcon, "notifyIcon");
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiExit});
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            resources.ApplyResources(this.tsmiExit, "tsmiExit");
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 60000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // mainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "mainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiPlotDir;
        private System.Windows.Forms.ToolStripMenuItem tsmiInstall;
        private System.Windows.Forms.ToolStripMenuItem tsmiStart;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddPeer;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowInfo;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportKeystore;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tsslDate;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.Timer timerMain;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label labelMsg;
        private System.Windows.Forms.Button btnSaveKS;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripMenuItem tsmiRepairFork;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ToolStripMenuItem tsmiAdvanced;
        private System.Windows.Forms.ToolStripMenuItem tsmiLanguage;
        private System.Windows.Forms.ToolStripMenuItem tsmiLangCN;
        private System.Windows.Forms.ToolStripMenuItem tsmiLangEN;
        private System.Windows.Forms.ToolStripMenuItem tsmiScanner;
        private System.Windows.Forms.ToolStripMenuItem tsmiWebsite;
        private System.Windows.Forms.ToolStripStatusLabel tsslNode;
        private System.Windows.Forms.ToolStripMenuItem tsmiChangeKeystore;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.ToolStripMenuItem tsmiClose;
    }
}

