﻿namespace EHDMiner
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
            this.tsmiRepairFork = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScan = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScanner = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiWebsite = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLangCN = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLangEN = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSaveKS = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.labelMsg = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiImportKeystore,
            this.tsmiInstall,
            this.tsmiStart,
            this.tsmiAdvanced,
            this.tsmiShowInfo,
            this.tsmiScanner,
            this.tsmiWebsite,
            this.tsmiLanguage});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // tsmiImportKeystore
            // 
            resources.ApplyResources(this.tsmiImportKeystore, "tsmiImportKeystore");
            this.tsmiImportKeystore.Name = "tsmiImportKeystore";
            this.tsmiImportKeystore.Click += new System.EventHandler(this.tsmiImportKeystore_Click);
            // 
            // tsmiInstall
            // 
            resources.ApplyResources(this.tsmiInstall, "tsmiInstall");
            this.tsmiInstall.Name = "tsmiInstall";
            this.tsmiInstall.Click += new System.EventHandler(this.tsmiInstall_Click);
            // 
            // tsmiStart
            // 
            resources.ApplyResources(this.tsmiStart, "tsmiStart");
            this.tsmiStart.Name = "tsmiStart";
            this.tsmiStart.Click += new System.EventHandler(this.tsmiStart_Click);
            // 
            // tsmiAdvanced
            // 
            resources.ApplyResources(this.tsmiAdvanced, "tsmiAdvanced");
            this.tsmiAdvanced.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddPeer,
            this.tsmiRepairFork,
            this.tsmiScan});
            this.tsmiAdvanced.Name = "tsmiAdvanced";
            // 
            // tsmiAddPeer
            // 
            resources.ApplyResources(this.tsmiAddPeer, "tsmiAddPeer");
            this.tsmiAddPeer.Name = "tsmiAddPeer";
            this.tsmiAddPeer.Click += new System.EventHandler(this.tsmiAddPeer_Click);
            // 
            // tsmiRepairFork
            // 
            resources.ApplyResources(this.tsmiRepairFork, "tsmiRepairFork");
            this.tsmiRepairFork.Name = "tsmiRepairFork";
            this.tsmiRepairFork.Click += new System.EventHandler(this.tsmiRepairFork_Click);
            // 
            // tsmiScan
            // 
            resources.ApplyResources(this.tsmiScan, "tsmiScan");
            this.tsmiScan.Name = "tsmiScan";
            this.tsmiScan.Click += new System.EventHandler(this.tsmiScan_Click);
            // 
            // tsmiShowInfo
            // 
            resources.ApplyResources(this.tsmiShowInfo, "tsmiShowInfo");
            this.tsmiShowInfo.Name = "tsmiShowInfo";
            this.tsmiShowInfo.Click += new System.EventHandler(this.tsmiShowInfo_Click);
            // 
            // tsmiScanner
            // 
            resources.ApplyResources(this.tsmiScanner, "tsmiScanner");
            this.tsmiScanner.Name = "tsmiScanner";
            this.tsmiScanner.Click += new System.EventHandler(this.tsmiScanner_Click);
            // 
            // tsmiWebsite
            // 
            resources.ApplyResources(this.tsmiWebsite, "tsmiWebsite");
            this.tsmiWebsite.Name = "tsmiWebsite";
            this.tsmiWebsite.Click += new System.EventHandler(this.tsmiWebsite_Click);
            // 
            // tsmiLanguage
            // 
            resources.ApplyResources(this.tsmiLanguage, "tsmiLanguage");
            this.tsmiLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLangCN,
            this.tsmiLangEN});
            this.tsmiLanguage.Name = "tsmiLanguage";
            // 
            // tsmiLangCN
            // 
            resources.ApplyResources(this.tsmiLangCN, "tsmiLangCN");
            this.tsmiLangCN.Name = "tsmiLangCN";
            this.tsmiLangCN.Click += new System.EventHandler(this.TsmiLangCN_Click);
            // 
            // tsmiLangEN
            // 
            resources.ApplyResources(this.tsmiLangEN, "tsmiLangEN");
            this.tsmiLangEN.Name = "tsmiLangEN";
            this.tsmiLangEN.Click += new System.EventHandler(this.TsmiLangEN_Click);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripProgressBar1});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            resources.ApplyResources(this.toolStripStatusLabel2, "toolStripStatusLabel2");
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            // 
            // toolStripProgressBar1
            // 
            resources.ApplyResources(this.toolStripProgressBar1, "toolStripProgressBar1");
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.btnSaveKS);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.labelMsg);
            this.panel1.Name = "panel1";
            // 
            // btnSaveKS
            // 
            resources.ApplyResources(this.btnSaveKS, "btnSaveKS");
            this.btnSaveKS.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
            // notifyIcon1
            // 
            resources.ApplyResources(this.notifyIcon1, "notifyIcon1");
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon1_MouseDoubleClick);
            // 
            // mainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "mainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiScan;
        private System.Windows.Forms.ToolStripMenuItem tsmiInstall;
        private System.Windows.Forms.ToolStripMenuItem tsmiStart;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddPeer;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowInfo;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportKeystore;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelMsg;
        private System.Windows.Forms.Button btnSaveKS;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripMenuItem tsmiRepairFork;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem tsmiAdvanced;
        private System.Windows.Forms.ToolStripMenuItem tsmiLanguage;
        private System.Windows.Forms.ToolStripMenuItem tsmiLangCN;
        private System.Windows.Forms.ToolStripMenuItem tsmiLangEN;
        private System.Windows.Forms.ToolStripMenuItem tsmiScanner;
        private System.Windows.Forms.ToolStripMenuItem tsmiWebsite;
    }
}

