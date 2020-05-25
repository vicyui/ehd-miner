using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.QRCode.Codec;

namespace EHDMiner
{
    public partial class QRCodeForm : Form
    {
        private string usdtPrice = string.Empty;
        private string tips = string.Empty;
        private string tips_en = string.Empty;
        public QRCodeForm(string price)
        {
            InitializeComponent();
            usdtPrice = price;
        }

        private void QRCodeForm_Load(object sender, EventArgs e)
        {
            switch (usdtPrice)
            {
                case "100":
                    tips = "有效期1个月";
                    tips_en = "Valid for 1 month";
                    break;
                case "10":
                    tips = "有效期1个次";
                    tips_en = "Validity period 1 time";
                    break;
                default:
                    break;
            }

            if (mainForm.language.Equals("zh"))
            {
                Text = "支付" + usdtPrice + "USDT " + tips;
                labelMsg.Text = "请输入您的USDT转账或充币的Hash地址：";
                btnPay.Text = "确定";
                btnCancel.Text = "取消";
            }
            else
            {
                Text = "Pay " + usdtPrice + "USDT " + tips_en;
                labelMsg.Text = "Please enter the hash address of your usdt transfer or recharging :";
                btnPay.Text = "OK";
                btnCancel.Text = "Cancel";
            }
            pictureBox.Image = CodeImage("0x595C230fBfc95A168eD893089C5748Ec8e413694");
            labelQrCode.Text = "0x595C230fBfc95A168eD893089C5748Ec8e413694";
            btnPay.Enabled = false;
        }

        private Bitmap CodeImage(string str)
        {
            //实例化一个生成二维码的对象
            QRCodeEncoder qrEncoder = new QRCodeEncoder();
            //设置二维码的编码模式
            qrEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //二维码像素宽度
            qrEncoder.QRCodeScale = 8;
            //设置版本
            qrEncoder.QRCodeVersion = 3;
            //根据内容生成二维码图像
            Bitmap image = qrEncoder.Encode(str, Encoding.UTF8);
            return image;
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            mainForm.userInput = textBox.Text;
            mainForm.isPay = true;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mainForm.isPay = false;
            Close();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            btnPay.Enabled = true;
            if (textBox.Text.Length < 66)
            {
                btnPay.Enabled = false;
            }
            if (textBox.Text.ToLower().Equals("0x595C230fBfc95A168eD893089C5748Ec8e413694".ToLower()))
            {
                btnPay.Enabled = false;
            }
        }

        private void labelQrCode_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetText(labelQrCode.Text);
        }
    }
}
