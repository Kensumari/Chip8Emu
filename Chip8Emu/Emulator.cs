using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chip8Emu
{
    class Emulator
    {
        private bool Run;
        private Thread MainThread;
        private Form1 MainForm;
        private Architecture Arch;

        public Emulator(Form1 parent)
        {
            this.MainForm = parent;
            this.Run = false;
            this.MainThread = new Thread(this.MainLoop);
            this.MainThread.Start();
            this.Arch = new Architecture(parent);
            this.Arch.LoadProgram(File.ReadAllBytes("BRIX"));
        }

        public void Next()
        {
            this.Arch.Tick();
        }

        public void Start()
        {
            this.Run = true;
        }

        public void Stop()
        {
            this.MainForm.Debug(" !STOPING! ");
            this.Run = false;
        }

        private void MainLoop()
        {
            while (true)
            {
                if (this.Run)
                {
                    //this.MainForm.Debug("Cycle-");
                    this.Arch.Tick();
                }
                Thread.Sleep(15);
            }
        }
    }
}
