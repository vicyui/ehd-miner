namespace EHDMiner
{
    partial class DeviceSelectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkedListBox = new System.Windows.Forms.CheckedListBox();
            this.btnDeviceSelect = new System.Windows.Forms.Button();
            this.labelDSF = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkedListBox
            // 
            this.checkedListBox.CheckOnClick = true;
            this.checkedListBox.ColumnWidth = 50;
            this.checkedListBox.FormattingEnabled = true;
            this.checkedListBox.Location = new System.Drawing.Point(12, 50);
            this.checkedListBox.MultiColumn = true;
            this.checkedListBox.Name = "checkedListBox";
            this.checkedListBox.Size = new System.Drawing.Size(203, 100);
            this.checkedListBox.TabIndex = 0;
            // 
            // btnDeviceSelect
            // 
            this.btnDeviceSelect.Location = new System.Drawing.Point(71, 156);
            this.btnDeviceSelect.Name = "btnDeviceSelect";
            this.btnDeviceSelect.Size = new System.Drawing.Size(75, 23);
            this.btnDeviceSelect.TabIndex = 1;
            this.btnDeviceSelect.Text = "button1";
            this.btnDeviceSelect.UseVisualStyleBackColor = true;
            this.btnDeviceSelect.Click += new System.EventHandler(this.BtnDeviceSelect_Click);
            // 
            // labelDSF
            // 
            this.labelDSF.AutoSize = true;
            this.labelDSF.Location = new System.Drawing.Point(12, 9);
            this.labelDSF.Name = "labelDSF";
            this.labelDSF.Size = new System.Drawing.Size(41, 12);
            this.labelDSF.TabIndex = 2;
            this.labelDSF.Text = "label1";
            // 
            // DeviceSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(227, 191);
            this.ControlBox = false;
            this.Controls.Add(this.labelDSF);
            this.Controls.Add(this.btnDeviceSelect);
            this.Controls.Add(this.checkedListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DeviceSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DeviceSelectForm";
            this.Load += new System.EventHandler(this.DeviceSelectForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox;
        private System.Windows.Forms.Button btnDeviceSelect;
        private System.Windows.Forms.Label labelDSF;
    }
}