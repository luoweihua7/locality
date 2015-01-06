using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Locality
{
    public partial class PopupForm : Form
    {
        private System.Threading.Timer timer;
        private int infinite = Timeout.Infinite;
        private long dueTime = ConfigService.DueTime;

        public PopupForm()
        {
            InitializeComponent();

            Prepare();
        }

        public void Prepare()
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            //显示区域
            //说明一下:ListBox比较变态,高度必须是ItemHeight的整数倍,然后加上Border之类,否则会自动缩减高度
            Rectangle workingArea = Screen.FromControl(this).WorkingArea;
            this.Location = new Point(workingArea.Width - this.Width, workingArea.Height - this.Height);

            //初始化定时器
            AutoResetEvent autoEvent = new AutoResetEvent(true);
            timer = new System.Threading.Timer(new TimerCallback(Hide), autoEvent, infinite, infinite);

            //不显示窗体
            this.Hide();
        }

        /// <summary>
        /// 添加要提示的文件
        /// <para>通过添加文件来触发提示窗口</para>
        /// </summary>
        /// <param name="string">文件名称</param>
        public void Show(string fileName)
        {
            this.fileList.Items.Add(fileName);
            //滚到最下面
            this.fileList.TopIndex = this.fileList.Items.Count - 1;

            this.Show();

            //启动定时器
            timer.Change(dueTime, infinite);
        }

        private void Hide(object e)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)e;
            autoEvent.WaitOne(); //等线程

            try
            {
                //只隐藏,不关闭
                this.fileList.Items.Clear();
                this.Hide();
            }
            catch (Exception ex)
            {
                this.Hide();
            }

            autoEvent.Set();
        }
    }
}
