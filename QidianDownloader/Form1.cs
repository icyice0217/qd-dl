using Bin.Qidian.Sdk.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QidianDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(DialogResult.OK == folderBrowserDialog1.ShowDialog())
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var i = 0;
            Console.WriteLine("保存路径及下载URL不能为空" + i++);
            if (string.IsNullOrEmpty(textBox1.Text) && string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("保存路径及下载URL不能为空");
                return;
            }
            textBox3.Clear();

            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
                try
                {
                    DownloadUtil.DownloadChapter(textBox2.Text, textBox1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.SetOut(new TextBoxWriter(textBox3));
        }
    }
}
