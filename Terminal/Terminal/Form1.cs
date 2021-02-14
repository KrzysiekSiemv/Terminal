using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Terminal
{
    public partial class Form1 : Form
    {
        Process process;
        Thread thread;
        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            textBox1.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                if (textBox1.Text == "exit" || textBox1.Text == "EXIT" || textBox1.Text == "Exit")
                    Application.Exit();
                else if (textBox1.Text == "cls")
                {
                    process.StandardInput.WriteLine(textBox1.Text);
                    richTextBox1.Text = "";
                }
                else
                {
                    textBox1.Enabled = false;
                    process.StandardInput.WriteLine(textBox1.Text);
                    textBox1.Text = "";
                    textBox1.Enabled = true;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
            process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    Arguments = "/k",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true
                }
            };

            thread = new Thread(() =>
            {
                process.Start();
                while (!process.StandardOutput.EndOfStream)
                {
                    var output = process.StandardOutput.ReadLine();
                    BeginInvoke(new Action(() =>
                    {
                        richTextBox1.Text += output + Environment.NewLine;
                    }));
                }

                while (!process.StandardError.EndOfStream)
                {
                    var errors = process.StandardError.ReadLine();
                    BeginInvoke(new Action(() =>
                    {
                        richTextBox1.Text += errors + Environment.NewLine;
                    }));
                }
                process.WaitForExit();
            });
            thread.Start();
            textBox1.Text = "";
            textBox1.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            process.Kill();
        }
    }
}
