using System;
using System.Windows.Forms;

namespace EHDMiner
{
    public partial class CooperativeForm : Form
    {
        public CooperativeForm()
        {
            InitializeComponent();
        }

        private void CooperativeForm_Load(object sender, EventArgs e)
        {
            mainForm.userPIN = string.Empty;
            if (mainForm.language.Equals("zh"))
            {
                labelCooperative.Text = "加入合作挖矿,共享挖矿收益\r确定要加入吗?";
                btnJoin.Text = "确定";
                btnCancel.Text = "取消";
            }
            else
            {
                labelCooperative.Text = "Join the cooperative mining and share the mining revenue.\rAre you sure you want to join?";
                btnJoin.Text = "OK";
                btnCancel.Text = "Cancel";
            }
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            mainForm.userPIN = textBox.Text;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
