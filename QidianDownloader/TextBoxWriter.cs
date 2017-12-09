using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QidianDownloader
{
    public class TextBoxWriter : System.IO.TextWriter
    {
        TextBox textBox;

        public TextBoxWriter(TextBox box)
        {
            textBox = box;
        }

        public delegate void textbox_delegate(string msg);
        public override void Write(string value)
        {
            if (textBox.InvokeRequired)
            {
                textbox_delegate dt = new textbox_delegate(Write);
                textBox.Invoke(dt, new object[] { value });
            }
            else
            {
                textBox.AppendText(value);//更新textbox内容
            }
        }

        public override void WriteLine(string value)
        {
            if (textBox.InvokeRequired)
            {
                textbox_delegate dt = new textbox_delegate(WriteLine);
                textBox.Invoke(dt, new object[] { value });
            }
            else
            {
                textBox.AppendText(value);//更新textbox内容
                textBox.AppendText("\r\n");//更新textbox内容
            }
        }

        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
