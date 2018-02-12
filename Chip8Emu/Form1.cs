using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Chip8Emu
{
    public partial class Form1 : Form
    {
        private Emulator emulator;
        private Bitmap Display;
        private Graphics Graphics;
        private void StartClick(object sender, EventArgs e)
        {
            //this.DebugBox.Text += "Hello from Start!\r\n";
            //byte[] program = File.ReadAllBytes("BRIX");
            this.emulator = new Emulator(this);
            this.emulator.Start();
        }

        private void StopClick(object sender, EventArgs e)
        {
            //this.Invoke(new MethodInvoker(() => this.emulator.Stop()));
            this.emulator.Stop();
        }

        private void NextClick(object sender, EventArgs e)
        {
            this.emulator.Next();
        }

        public void Debug(string S)
        {
            this.Invoke(new MethodInvoker(() => this.WriteConsole(S+"\r\n"))); 
        }

        public void SetPixel(int x, int y, bool val)
        {
            //Console.WriteLine("SETPIXEL X:{0} Y:{1} V:{2}", x, y,val);
            if (val==true)
                this.Display.SetPixel(x, y, Color.White);
            else
                this.Display.SetPixel(x, y, Color.Black);
        }

        public void Draw()
        {
            //this.Display.SetPixel(1, 1, Color.Red);
            Bitmap destination = new Bitmap(256, 128);
            using (Graphics g = Graphics.FromImage(destination))
            {

                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(this.Display, new Rectangle(0, 0, destination.Width, destination.Height));
                this.Graphics.DrawImage(destination, new Point(0, 0));
            }
            
        }

        public void Clear()
        {
            using (Graphics g = Graphics.FromImage(this.Display))
            {
                g.Clear(Color.Black);
                this.Draw();
            }
        }

        private void WriteConsole(string S)
        {
            this.DebugBox.AppendText(S);
        }
        
        public Form1()
        {
            InitializeComponent();
            //this.DebugBox.Text = "hello debug output\r\n";
            this.StartButton.Click += new EventHandler(this.StartClick);
            this.StopButton.Click += new EventHandler(this.StopClick);
            this.NextButton.Click += new EventHandler(this.NextClick);
            //this.pictureBox1.OnPain
            this.Display = new Bitmap(64, 32, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            this.Graphics = this.pictureBox1.CreateGraphics();
        }

        public void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            this.Debug("WORKS");
        }
    }
}
