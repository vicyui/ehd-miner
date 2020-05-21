namespace EHDMiner
{
    partial class AddNodeForm
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
            this.btnAddNode = new System.Windows.Forms.Button();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.btnAddNodeCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnAddNode
            // 
            this.btnAddNode.Location = new System.Drawing.Point(12, 38);
            this.btnAddNode.Name = "btnAddNode";
            this.btnAddNode.Size = new System.Drawing.Size(75, 23);
            this.btnAddNode.TabIndex = 2;
            this.btnAddNode.UseVisualStyleBackColor = true;
            this.btnAddNode.Click += new System.EventHandler(this.btnAddNode_Click);
            // 
            // comboBox
            // 
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(12, 12);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(180, 20);
            this.comboBox.TabIndex = 3;
            // 
            // btnAddNodeCancel
            // 
            this.btnAddNodeCancel.Location = new System.Drawing.Point(117, 38);
            this.btnAddNodeCancel.Name = "btnAddNodeCancel";
            this.btnAddNodeCancel.Size = new System.Drawing.Size(75, 23);
            this.btnAddNodeCancel.TabIndex = 4;
            this.btnAddNodeCancel.UseVisualStyleBackColor = true;
            this.btnAddNodeCancel.Click += new System.EventHandler(this.btnAddNodeCancel_Click);
            // 
            // AddNodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(204, 71);
            this.ControlBox = false;
            this.Controls.Add(this.btnAddNodeCancel);
            this.Controls.Add(this.comboBox);
            this.Controls.Add(this.btnAddNode);
            this.Name = "AddNodeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddNodeForm";
            this.Load += new System.EventHandler(this.AddNodeForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnAddNode;
        private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.Button btnAddNodeCancel;
    }
}