using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsGoToSleep
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void RunTextCommand(string command)
        {
            textBox1.AppendText(command);
            textBox1.AppendText(Environment.NewLine);
            textBox1.AppendText(string.Join("\r\n", PowerCfg.RunWithParams(command)));
        }

        private void RunListCommand(string command)
        {
            listBox1.Items.Clear();
            textBox1.AppendText(command);
            textBox1.AppendText(Environment.NewLine);
            listBox1.Items.AddRange(PowerCfg.RunWithParams(command).ToArray());
        }

        private void RunApplyCommand(string command)
        {
            if (listBox1.SelectedIndex > -1)
            {
                foreach (int k in listBox1.SelectedIndices)
                {
                    var cmdArg = $"{command} \"{listBox1.Items[k]}\"";
                    textBox1.AppendText(cmdArg);
                    textBox1.AppendText(Environment.NewLine);
                    textBox1.AppendText(string.Join("\r\n", PowerCfg.RunWithParams(cmdArg)));
                }
            }
            else
            {
                textBox1.AppendText("No selected items");
                textBox1.AppendText(Environment.NewLine);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            RunListCommand("-devicequery wake_from_any");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RunListCommand("-devicequery wake_armed");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RunApplyCommand("-devicedisablewake");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            RunTextCommand("-help");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RunApplyCommand("-deviceenablewake");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RunTextCommand($"-{textBox2.Text.Trim('/', '\\', '-')}");
        }
    }

    public class PowerCfg
    {
        static Process _process;

        static PowerCfg()
        {
            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powercfg",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage)
                }
            };
        }

        public static List<string> RunWithParams(string args)
        {
            _process.StartInfo.Arguments = args;

            var output = new List<string>();

            _process.Start();
            while (!_process.StandardOutput.EndOfStream)
            {
                output.Add(_process.StandardOutput.ReadLine());
            }

            return output;
        }
    }
}
