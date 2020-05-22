using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EHDMiner
{
    public partial class AddNodeForm : Form
    {
        private static string szNode = "be9a4f00f45f603edaa86168a87de0d664550d88626f49991533ab31115ad7176eaa3b24069bca65b8b12e96cf360698346287087772e42db02b915fe24e3f7f@119.23.110.73";
        public AddNodeForm()
        {
            InitializeComponent();
        }

        private void btnAddNode_Click(object sender, EventArgs e)
        {
            mainForm.addNodeString = ((Node)comboBox.SelectedItem).Address;
            mainForm.addNodeId = ((Node)comboBox.SelectedItem).NodeId;
            Close();
        }

        private void AddNodeForm_Load(object sender, EventArgs e)
        {
            btnAddNode.Text = "确定";
            btnAddNodeCancel.Text = "取消";
            string filePath = Path.Combine(Application.StartupPath, "config.ini");//在当前程序路径创建
            Dictionary<string, Node> nodes = new Dictionary<string, Node>
            {
                { "00", new Node("00", "免费节点", "Free node", szNode, true) },
                { "01", new Node("01", "中国区华南节点", "South China node in China", szNode) },
                { "02", new Node("02", "中国区华中节点", "China central node", szNode) },
                { "03", new Node("03", "中国区华北节点", "North China node in China", szNode) },
                { "04", new Node("04", "中国区东北节点", "Northeast node of China", szNode) },
                { "05", new Node("05", "中国区西南节点", "Southwest node of China", szNode) },
                { "06", new Node("06", "中国区西北节点", "Northwest node of China", szNode) },
                { "07", new Node("07", "亚洲区韩国节点", "South Korea node in Asia", szNode) },
                { "08", new Node("08", "亚洲区日本节点", "Japan node in Asia", szNode) },
                { "09", new Node("09", "亚洲新加坡节点", "Singapore node in Asia", szNode) },
                { "10", new Node("10", "欧洲区英国节点", "UK node in Europe", szNode) },
                { "11", new Node("11", "欧洲区德国节点", "German node in Europe", szNode) },
                { "12", new Node("12", "美州区美国节点", "Us node", szNode) },
                { "13", new Node("13", "非洲区南非节点", "Africa South Africa node", szNode) },
                { "14", new Node("14", "大洋洲区巴西节点", "Oceania Brazil node", szNode) }
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
