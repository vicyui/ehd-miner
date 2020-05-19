using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace EHDMiner
{
    public partial class DeviceSelectForm : Form
    {
        public DeviceSelectForm()
        {
            InitializeComponent();
        }

        private void DeviceSelectForm_Load(object sender, EventArgs e)
        {
            List<string> devices = FileUtil.GetDeviceID();
            string sql = "select * from t_plot where F_id != 1;";
            DataTable table = DBHelper.ExecuteQuery(sql, new Dictionary<string, object>());
            foreach (DataRow row in table.Rows)
            {
                Console.WriteLine($"{row["F_id"]},{row["F_Path"]},{row["F_PlotDir"]}");
            }

            foreach (string device in devices)
            {
                checkedListBox.Items.Add(device);
            }
        }

        private void BtnDeviceSelect_Click(object sender, EventArgs e)
        {
            mainForm.checkedList = new ArrayList(checkedListBox.CheckedItems);
            Close();
        }
    }
}
