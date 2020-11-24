using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Lunar.Editor
{
    public class DebugLogger : TextWriter
    {
        TextBox textBox;
        char lastChar;

        public DebugLogger(TextBox output)
        {
            textBox = output;
            Console.SetOut(this);
            lastChar = '\n';
        }

        public override void Write(char value)
        {
            if (lastChar == '\n')
            {
                foreach(char s in DateTime.Now.ToString().Split(' ')[1] + "    ")
                {
                    base.Write(s);
                    textBox.Dispatcher.BeginInvoke(new Action(() => { textBox.AppendText(s.ToString()); }));
                }
            }

            base.Write(value);
            textBox.Dispatcher.BeginInvoke(new Action(() => { textBox.AppendText(value.ToString()); }));

            lastChar = value;
        }

        public override Encoding Encoding { get => Encoding.UTF8; }
    }
}
