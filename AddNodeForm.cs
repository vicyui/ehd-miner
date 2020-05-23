using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EHDMiner
{
    public partial class AddNodeForm : Form
    {
        public AddNodeForm()
        {
            InitializeComponent();
        }

        private void btnAddNode_Click(object sender, EventArgs e)
        {
            mainForm.selectedNode = (Node)comboBox.SelectedItem;
            Close();
        }

        private void AddNodeForm_Load(object sender, EventArgs e)
        {
            btnAddNode.Text = "确定";
            btnAddNodeCancel.Text = "取消";
            string filePath = Path.Combine(Application.StartupPath, "config.ini");//在当前程序路径创建
            Dictionary<string, Node> nodes = new Dictionary<string, Node>
            {
                { "00", new Node("00", "免费节点", "Free node", "default", true) },
                { "01", new Node("01", "01.中国区华南节点", "01.South China node in China", "sz") },
                { "02", new Node("02", "02.中国区华中节点", "02.China central node", "sz") },
                { "03", new Node("03", "03.中国区华北节点", "03.North China node in China", "sz") },
                { "04", new Node("04", "04.中国区东北节点", "04.Northeast node of China", "sz") },
                { "05", new Node("05", "05.中国区西南节点", "05.Southwest node of China", "sz") },
                { "06", new Node("06", "06.中国区西北节点", "06.Northwest node of China", "sz") },
                { "07", new Node("07", "07.亚洲区韩国节点", "07.South Korea node in Asia", "default") },
                { "08", new Node("08", "08.亚洲区日本节点", "08.Japan node in Asia", "default") },
                { "09", new Node("09", "09.亚洲新加坡节点", "09.Singapore node in Asia", "default") },
                { "10", new Node("10", "10.欧洲区英国节点", "10.UK node in Europe", "default") },
                { "11", new Node("11", "11.欧洲区德国节点", "11.German node in Europe", "default") },
                { "12", new Node("12", "12.美州区美国节点", "12.Us node", "default") },
                { "13", new Node("13", "13.非洲区南非节点", "13.Africa South Africa node", "default") },
                { "14", new Node("14", "14.大洋洲区巴西节点", "14.Oceania Brazil node", "default") }
            };

            FileStream fs;
            StreamWriter sw;
            StreamReader sr;
            if (!File.Exists(filePath))
            {
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs);
                fs.Seek(fs.Length, SeekOrigin.End);
                foreach (KeyValuePair<string, Node> node in nodes)
                {
                    sw.WriteLine(node.Key + "=" + nodes[node.Key].Access);//开始写入值
                }
                sw.Close();
                fs.Close();
            }
            File.SetAttributes(filePath, FileAttributes.Hidden);
            fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            sr = new StreamReader(fs);
            fs.Seek(0, SeekOrigin.Begin);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if ("true".Equals(line.Split('=')[1].ToLower()))
                {
                    nodes[line.Split('=')[0]].Access = true;
                    nodes[line.Split('=')[0]].Zh_name += "[已开通]";
                    nodes[line.Split('=')[0]].En_name += "[Opened]";
                }
            }
            sr.Close();
            fs.Close();

            comboBox.DataSource = nodes.Values.ToArray();
            comboBox.DisplayMember = "Zh_name";
            comboBox.ValueMember = "Address";
            if (mainForm.language == "en")
            {
                btnAddNode.Text = "OK";
                btnAddNodeCancel.Text = "Cancel";
                comboBox.DisplayMember = "En_name";
            }
        }

        private void btnAddNodeCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddNodeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
        }
    }
}
