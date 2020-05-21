using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.QRCode.Codec;

namespace EHDMiner
{
    public partial class QRCodeForm : Form
    {
        private string lang = string.Empty;
        public QRCodeForm(string language)
        {
            InitializeComponent();
            lang = language;
        }

        private void QRCodeForm_Load(object sender, EventArgs e)
        {
            if (lang.Equals("zh"))
            {
                Text = "支付100USDT";
                labelMsg.Text = "请输入您的转账地址:";
                btnPay.Text = "确定";
                btnCancel.Text = "取消";
            }
            else
            {
                Text = "Pay 100USDT";
                labelMsg.Text = "Please enter your transfer address :";
                btnPay.Text = "OK";
                btnCancel.Text = "Cancel";
            }
            pictureBox.Image = CodeImage("0x595C230fBfc95A168eD893089C5748Ec8e413694");
            labelQrCode.Text = "0x595C230fBfc95A168eD893089C5748Ec8e413694";
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
            mainForm.userInputAddress = textBox.Text;
            Clipboard.SetText("0x595C230fBfc95A168eD893089C5748Ec8e413694");
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mainForm.isPay = false;
            Close();
        }
    }
}
