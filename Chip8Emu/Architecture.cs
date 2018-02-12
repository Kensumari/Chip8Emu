using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emu
{
    class Architecture
    {
        private CPU cpu;
        private Memory memory;
        private Form1 MainForm;
        private Display display;

        //private Display display;
        public Architecture(Form1 parent)
        {
            this.memory = new Memory();
            this.display = new Display(parent);
            this.cpu = new CPU(parent,this.memory,this.display);
            this.MainForm = parent;
        }

        public void LoadProgram(byte[] Program)
        {
            this.MainForm.Debug("Loading Program");
            //this.MainForm.Debug("OP " + Program[0].ToString("X")+"\r\n");
            //this.MainForm.Debug("")
            Array.Copy(Program, 0, this.memory.Contents, 512, Program.Length);
            //this.MainForm.Debug("OP " + this.memory.Contents[513].ToString("X") + "\r\n");
            this.cpu.JumpTo(512);
            //this.cpu.Tick();
        }

        public void Tick()
        {
            this.cpu.Tick();
        }
    }
}
