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
            Dictionary<string,string> devices = FileUtil.GetDeviceInfo();
            string sql = "select * from t_plot where F_id != 1;";
            DataTable table = DBHelper.ExecuteQuery(sql, new Dictionary<string, object>());
            foreach (var device in devices)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row["F_Path"].Equals(device.Key))
                    {
                        checkedListBox.Items.Add(device, true);
                    }
                }
                if (!checkedListBox.Items.Contains(device))
                {
                    checkedListBox.Items.Add(device);
                }
            }
        }

        private void BtnDeviceSelect_Click(object sender, EventArgs e)
        {
            mainForm.checkedList = new ArrayList(checkedListBox.CheckedItems);
            Close();
        }
    }
}
