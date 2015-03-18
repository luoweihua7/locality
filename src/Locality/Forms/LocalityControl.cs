using Fiddler;
using Locality.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Locality
{
    public class LocalityControl : FiddlerService
    {
        #region 控件
        private TabPage tabPage;
        private ToolStrip toolBar;
        private CheckBox cbEnable;
        private CheckBox cbShowTip;
        private CheckBox cbScheme;
        private CheckBox cbStrictMode;
        private ListView listView;
        private List<Control> controls;
        private OpenFileDialog dialog = new OpenFileDialog();
        private PopupForm frmPopup = new PopupForm();
        #endregion

        public void InitializeComponent()
        {
            #region 实例化控件

            TabPage tp = new TabPage() { Text = ConfigService.AppName };

            Panel pHead = new Panel() { Dock = DockStyle.Top, Location = new Point(0, 0), Height = 30, TabIndex = 0 };
            Panel pTool = new Panel() { Dock = DockStyle.Top, Location = new Point(0, 30), Height = 25 };
            Panel pMain = new Panel() { Dock = DockStyle.Fill, Location = new Point(0, 55) };
            Panel pBottom = new Panel() { Dock = DockStyle.Bottom, Height = 35 };

            CheckBox cbEnable = new CheckBox() { Text = "Enable", AutoSize = true, Location = new Point(3, 8), UseVisualStyleBackColor = true };
            CheckBox cbScheme = new CheckBox() { Text = "Enable Scheme", AutoSize = true, Location = new Point(125, 8), UseVisualStyleBackColor = true };
            CheckBox cbStrictMode = new CheckBox() { Text = "Strict Mode", AutoSize = true, Location = new Point(230, 8), UseVisualStyleBackColor = true };
            CheckBox cbShowTip = new CheckBox() { Text = "Enable Tip", AutoSize = true, Location = new Point(320, 8), UseVisualStyleBackColor = true };

            ToolStrip toolStrip = new ToolStrip() { Location = new Point(0, 0), RenderMode = ToolStripRenderMode.System, Height = 25 };
            ToolStripButton tsbMgr = new ToolStripButton() { DisplayStyle = ToolStripItemDisplayStyle.Image, ImageTransparentColor = Color.Magenta, Image = Resources.scheme };

            ListView lv = new ListView() { AllowDrop = true, CheckBoxes = true, Dock = DockStyle.Fill, FullRowSelect = true, GridLines = true, Location = new Point(0, 0), View = View.Details };

            Button btnSelectAll = new Button() { Location = new Point(3, 6), Size = new Size(69, 23), Text = "Select All", UseVisualStyleBackColor = true };
            Button btnAdd = new Button() { Location = new Point(78, 6), Size = new Size(69, 23), Text = "Add", UseVisualStyleBackColor = true };
            Button btnRemove = new Button() { Location = new Point(153, 6), Size = new Size(69, 23), Text = "Remove", UseVisualStyleBackColor = true };
            Button btnClear = new Button() { Location = new Point(228, 6), Size = new Size(69, 23), Text = "Clear", UseVisualStyleBackColor = true };
            //Button btnRefresh = new Button() { Location = new Point(303, 6), Size = new Size(69, 23), Text = "Refresh", UseVisualStyleBackColor = true };

            #endregion

            #region 控件层级添加

            tp.SuspendLayout();
            tp.Controls.AddRange(new Control[] { pMain, pBottom, pTool, pHead }); //添加顺序为:Dock>Bottom>Top,有2个Top,则最顶上的最后加
            pHead.Controls.AddRange(new Control[] { cbEnable, cbShowTip, cbScheme, cbStrictMode });
            toolStrip.Items.AddRange(new ToolStripItem[] { tsbMgr, new ToolStripSeparator() { Size = new System.Drawing.Size(6, 25) } });
            pTool.Controls.Add(toolStrip);
            lv.Columns.AddRange(new ColumnHeader[] { new ColumnHeader() { Text = "File", Width = 150 }, new ColumnHeader() { Text = "Path", Width = 380 } });
            pMain.Controls.Add(lv);
            pBottom.Controls.AddRange(new Control[] { btnSelectAll, btnAdd, btnRemove, btnClear }); //btnRefresh
            tp.ResumeLayout(false);

            #endregion

            #region 控件事件

            cbEnable.CheckedChanged += new EventHandler(ChangeEnableEventArgs);
            cbShowTip.CheckedChanged += new EventHandler(ChangeShowTipEventArgs);
            cbScheme.CheckedChanged += new EventHandler(ChangeSchemeEventArgs);
            cbStrictMode.CheckedChanged += new EventHandler(ChangeStrictModeEventArgs);

            tsbMgr.Click += new EventHandler(OnManageProfileClick);

            lv.KeyDown += new KeyEventHandler(OnListViewKeyDown); //CTRL+C/V
            lv.DragEnter += new DragEventHandler(OnListViewDropEnter);
            lv.DragDrop += new DragEventHandler(OnListViewDragDrop); //拖进来
            lv.ItemChecked += new ItemCheckedEventHandler(OnListViewChecked); //点击勾选

            btnSelectAll.Click += new EventHandler(OnClickSelectAllButton);
            btnAdd.Click += new EventHandler(OnClickAddButton);
            btnRemove.Click += new EventHandler(OnClickRemoveButton);
            btnClear.Click += new EventHandler(OnClickClearButton);

            #endregion

            //保存到全局,方便调用
            this.controls = new List<Control>() { cbShowTip, cbScheme, cbStrictMode, toolStrip, lv, btnSelectAll, btnAdd, btnRemove, btnClear };
            this.tabPage = tp;
            this.cbEnable = cbEnable;
            this.cbShowTip = cbShowTip;
            this.cbScheme = cbScheme;
            this.cbStrictMode = cbStrictMode;
            this.toolBar = toolStrip;
            this.listView = lv;

            //关联
            ListViewController.listView = listView;
            ToolStripController.toolBar = toolStrip;

            //加入到Fiddler中
            FiddlerApplication.UI.imglSessionIcons.Images.Add(ConfigService.ActiveIcon, Locality.Properties.Resources.active);
            FiddlerApplication.UI.imglSessionIcons.Images.Add(ConfigService.InactiveIcon, Locality.Properties.Resources.inactive);
            FiddlerApplication.UI.tabsViews.TabPages.Add(this.tabPage);
            this.tabPage.ImageKey = ConfigService.InactiveIcon;
        }

        /// <summary>
        /// restore from config saved
        /// </summary>
        public void Restore()
        {
            //load configs
            ConfigService.Load();

            //initialize profiles and files
            ToolStripController.Initialize(ConfigService.Schemes);
            ListViewController.Initialize(ConfigService.Files); //Controller 调用 Service

            //restore switches
            this.cbShowTip.Checked = ConfigService.EnableTip;
            this.cbScheme.Checked = ConfigService.EnableScheme;
            this.cbStrictMode.Checked = ConfigService.StrictMode;
            this.cbEnable.Checked = ConfigService.Enable; //last set, fire checked change event

            if (!ConfigService.EnableScheme)
            {
                //fire event when disabled
                ChangeSchemeEventArgs(this.cbScheme, null);
            }

            if (!ConfigService.Enable)
            {
                //fire event when disabled
                ChangeEnableEventArgs(this.cbEnable, null);
            }
        }

        #region 控件事件实现

        void ChangeEnableEventArgs(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;

            var enable = cb.Checked;
            foreach (Control control in controls)
            {
                control.Enabled = enable;
            }
            ConfigService.Enable = enable;

            //启用和禁用的图标
            if (enable)
            {
                this.tabPage.ImageKey = ConfigService.ActiveIcon;
            }
            else
            {
                this.tabPage.ImageKey = ConfigService.InactiveIcon;
            }
        }
        void ChangeShowTipEventArgs(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            ConfigService.EnableTip = cb.Checked;
        }
        void ChangeSchemeEventArgs(object sender, EventArgs e)
        {
            var enable = ((CheckBox)sender).Checked;
            ConfigService.EnableScheme = enable;
            ToolStripController.Set(enable);
        }
        void ChangeStrictModeEventArgs(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            ConfigService.StrictMode = cb.Checked;
        }

        void OnManageProfileClick(object sender, EventArgs e)
        {
            new SchemeForm().ShowDialog();
        }

        void OnListViewKeyDown(object sender, KeyEventArgs e)
        {
            if (FiddlerApplication.UI.tabsViews.SelectedTab == this.tabPage)
            {
                if (e.Control && ConfigService.Enable)
                {
                    if (e.KeyCode == Keys.V) //paste
                    {
                        foreach (string file in Clipboard.GetFileDropList())
                        {
                            ListViewController.Add(file.ToLower());
                        }

                        e.Handled = true;
                        return;
                    }
                    else if (e.KeyCode == Keys.A) //select all
                    {
                        foreach (ListViewItem item in this.listView.Items)
                        {
                            item.Selected = true;
                        }

                        e.Handled = true;
                        return;
                    }
                }

                if (e.KeyCode == Keys.Delete) //remove selected items
                {
                    if (this.listView.SelectedItems.Count > 0)
                    {
                        ListViewController.RemoveSelected();
                    }
                }
            }
        }
        void OnListViewDropEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
        void OnListViewDragDrop(object sender, DragEventArgs e)
        {
            string[] dropList = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in dropList)
            {
                ListViewController.Add(file.ToLower());
            }
        }
        void OnListViewChecked(object sender, ItemCheckedEventArgs e)
        {
            var enable = e.Item.Checked;
            var file = e.Item.SubItems[1].Text.ToLower();

            ListViewController.Update(file, enable);
        }

        void OnClickSelectAllButton(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = true; //自动触发OnListViewChecked
            }
        }
        void OnClickAddButton(object sender, EventArgs e)
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in dialog.FileNames)
                {
                    ListViewController.Add(file.ToLower());
                }
            }
        }
        void OnClickRemoveButton(object sender, EventArgs e)
        {
            ListViewController.RemoveSelected();
        }
        void OnClickClearButton(object sender, EventArgs e)
        {
            ListViewController.Clear();
        }

        #endregion

        #region 功能实现

        private void Initialize()
        {
            this.InitializeComponent();
            this.Restore();
        }

        #endregion

        #region 重载,实现Fiddler功能

        public override void OnLoad()
        {
            Initialize(); //功能入口
        }

        public override void OnMatchSession(string fileName)
        {
            if (ConfigService.EnableTip)
            {
                frmPopup.Show(fileName);
            }
        }

        #endregion
    }
}
