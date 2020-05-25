using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace EHDMiner
{
    public partial class DeviceSelectForm : Form
    {
        private int countDeviceSelect = 0;
        public DeviceSelectForm()
        {
            InitializeComponent();
        }

        private void DeviceSelectForm_Load(object sender, EventArgs e)
        {
            btnDeviceSelect.Text = "确定";
            btnCancel.Text = "取消";
            if (mainForm.language == "en")
            {
                btnDeviceSelect.Text = "OK";
                btnCancel.Text = "Cancel";
            }
            
            Dictionary<string,string> devices = FileUtil.GetDeviceInfo();
            string sql = "select * from t_plot where F_id != 1;";
            DataTable table = DBHelper.ExecuteQuery(sql, new Dictionary<string, object>());
            countDeviceSelect = table.Rows.Count;
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
            mainForm.countDeviceSelect = checkedListBox.CheckedItems.Count - countDeviceSelect;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mainForm.checkedList.Clear();
            Close();
        }

        private void checkedListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            checkedListBox.Enabled = false;
            if(checkedListBox.CheckedItems.Count == 0)
            {
                checkedListBox.Enabled = true;
            }
        }
    }
}
