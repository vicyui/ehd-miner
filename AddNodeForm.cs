using System;
using System.Collections;
using System.Collections.Generic;
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
            Close();
        }

        private void AddNodeForm_Load(object sender, EventArgs e)
        {
            btnAddNode.Text = "确定";
            btnAddNodeCancel.Text = "取消";
            ArrayList arrayList = new ArrayList();
            arrayList.Add(new Node("中国区华南节点", szNode));
            arrayList.Add(new Node("中国区华中节点", szNode));
            arrayList.Add(new Node("中国区华北节点", szNode));
            arrayList.Add(new Node("中国区东北节点", szNode));
            arrayList.Add(new Node("中国区西南节点", szNode));
            arrayList.Add(new Node("中国区西北节点", szNode));
            arrayList.Add(new Node("亚洲区韩国节点", szNode));
            arrayList.Add(new Node("亚洲日本节点", szNode));
            arrayList.Add(new Node("亚洲新加坡节点", szNode));
            arrayList.Add(new Node("欧洲区英国节点", szNode));
            arrayList.Add(new Node("欧洲区德国节点", szNode));
            arrayList.Add(new Node("美州区美国节点", szNode));
            arrayList.Add(new Node("非洲区南非节点", szNode));
            arrayList.Add(new Node("大洋洲区巴西节点", szNode));
            if (mainForm.language == "en")
            {
                btnAddNode.Text = "OK";
                btnAddNodeCancel.Text = "Cancel";
                arrayList.Clear();
                arrayList.Add(new Node("South China node in China", szNode));
                arrayList.Add(new Node("China central node", szNode));
                arrayList.Add(new Node("North China node in China", szNode));
                arrayList.Add(new Node("Northeast node of China", szNode));
                arrayList.Add(new Node("outhwest node of China", szNode));
                arrayList.Add(new Node("Northwest node of China", szNode));
                arrayList.Add(new Node("South Korea node in Asia", szNode));
                arrayList.Add(new Node("Asia Japan node", szNode));
                arrayList.Add(new Node("Asia Singapore node", szNode));
                arrayList.Add(new Node("German node in Europe", szNode));
                arrayList.Add(new Node("UK node in Europe", szNode));
                arrayList.Add(new Node("Us node", szNode));
                arrayList.Add(new Node("Africa South Africa node", szNode));
                arrayList.Add(new Node("Oceania Brazil node", szNode));
            }

            comboBox.DataSource = arrayList;
            comboBox.DisplayMember = "Name";
            comboBox.ValueMember = "Address";
        }

        private void btnAddNodeCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
