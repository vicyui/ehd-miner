namespace EHDMiner
{
    partial class QRCodeForm
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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.labelQrCode = new System.Windows.Forms.Label();
            this.btnPay = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.labelMsg = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(12, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(233, 233);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // labelQrCode
            // 
            this.labelQrCode.Font = new System.Drawing.Font("宋体", 9.5F);
            this.labelQrCode.Location = new System.Drawing.Point(15, 248);
            this.labelQrCode.Name = "labelQrCode";
            this.labelQrCode.Size = new System.Drawing.Size(230, 36);
            this.labelQrCode.TabIndex = 1;
            this.labelQrCode.Text = "labelQrCode";
            this.labelQrCode.Click += new System.EventHandler(this.labelQrCode_Click);
            this.labelQrCode.DoubleClick += new System.EventHandler(this.labelQrCode_DoubleClick);
            // 
            // btnPay
            // 
            this.btnPay.Location = new System.Drawing.Point(12, 343);
            this.btnPay.Name = "btnPay";
            this.btnPay.Size = new System.Drawing.Size(75, 23);
            this.btnPay.TabIndex = 2;
            this.btnPay.UseVisualStyleBackColor = true;
            this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(172, 343);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // labelMsg
            // 
            this.labelMsg.Location = new System.Drawing.Point(12, 284);
            this.labelMsg.Name = "labelMsg";
            this.labelMsg.Size = new System.Drawing.Size(233, 29);
            this.labelMsg.TabIndex = 4;
            this.labelMsg.Text = "labelMsg";
            this.labelMsg.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(12, 316);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(233, 21);
            this.textBox.TabIndex = 5;
            this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // QRCodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 375);
            this.ControlBox = false;
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.labelMsg);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnPay);
            this.Controls.Add(this.labelQrCode);
            this.Controls.Add(this.pictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "QRCodeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "QRCodeForm";
            this.Load += new System.EventHandler(this.QRCodeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label labelQrCode;
        private System.Windows.Forms.Button btnPay;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelMsg;
        private System.Windows.Forms.TextBox textBox;
    }
}