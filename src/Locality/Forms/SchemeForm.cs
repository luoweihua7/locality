using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Locality
{
    public partial class SchemeForm : Form
    {
        private int selectedIndex = -1;

        public SchemeForm()
        {
            InitializeComponent();

            Initialize();
        }

        public void Initialize()
        {
            SchemeService.schemeList.ForEach(scheme =>
            {
                schemeList.Items.Add(scheme.Name);
            });
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            selectedIndex = -1;
            ClearInput();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (schemeList.SelectedItems.Count > 0)
            {
                int index = schemeList.SelectedIndex;
                string schemeName = schemeList.Items[index].ToString();

                schemeList.Items.RemoveAt(index);
                ToolStripController.Remove(schemeName);

                if (selectedIndex > -1)
                {
                    if (selectedIndex == index)
                    {
                        //删除了当前编辑的数据
                        selectedIndex = -1;
                        ClearInput();
                    }
                    else if (selectedIndex > index)
                    {
                        //删除了前面的某条数据，索引上移
                        selectedIndex--;
                    }

                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool enable = cbEnable.Checked;
            string schemeName = tbxName.Text;
            string hostsStr = tbxHosts.Text.Trim().Replace(" ", string.Empty).Replace("\r\n", ",").Replace(";", ",").ToLower();

            if (string.IsNullOrEmpty(schemeName))
            {
                tbxName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(hostsStr))
            {
                tbxHosts.Focus();
                return;
            }

            string[] hostColl = hostsStr.Split(new char[] { ',' });
            List<string> hosts = new List<string>();
            foreach (string host in hostColl)
            {
                hosts.Add(host);
            }

            if (selectedIndex == -1)
            {
                if (ToolStripController.GetToolStripButton(schemeName) != null)
                {
                    MessageBox.Show("Scheme name exists !");
                    return;
                }

                ToolStripController.Add(schemeName, hosts, enable);
                schemeList.Items.Add(schemeName);
            }
            else
            {
                string modifyName = schemeList.Items[selectedIndex].ToString();
                ToolStripController.Update(modifyName, schemeName, hosts, enable);
                schemeList.Items[selectedIndex] = schemeName;
            }

            //清理状态和内容
            ClearInput();
        }

        private void schemeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (schemeList.SelectedItems.Count > 0)
            {
                string schemeName = schemeList.SelectedItem.ToString();
                Scheme scheme = SchemeService.Find(schemeName);
                if (scheme != null)
                {
                    StringBuilder sb = new StringBuilder();
                    scheme.Hosts.ForEach(host => { sb.AppendLine(host); });

                    selectedIndex = schemeList.SelectedIndex;

                    cbEnable.Checked = scheme.Enable;
                    tbxName.Text = scheme.Name;
                    tbxHosts.Text = sb.ToString();
                }
            }
        }

        private void ClearInput()
        {
            cbEnable.Checked = true;
            tbxName.Text = string.Empty;
            tbxHosts.Text = string.Empty;

            selectedIndex = -1;
        }
    }
}
