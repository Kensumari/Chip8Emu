using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emu
{
    class Memory
    {
        public byte[] Contents;
        byte[] hexChars = {

              0xF0, 0x90, 0x90, 0x90, 0xF0,  // 0

              0x20, 0x60, 0x20, 0x20, 0x70,  // 1

              0xF0, 0x10, 0xF0, 0x80, 0xF0,  // 2

              0xF0, 0x10, 0xF0, 0x10, 0xF0,   // 3

              0x90, 0x90, 0xF0, 0x10, 0x10,  // 4

              0xF0, 0x80, 0xF0, 0x10, 0xF0,  // 5

              0xF0, 0x80, 0xF0, 0x90, 0xF0,  // 6

              0xF0, 0x10, 0x20, 0x40, 0x40,  // 7

              0xF0, 0x90, 0xF0, 0x90, 0xF0,  // 8

              0xF0, 0x90, 0xF0, 0x10, 0xF0,  // 9

              0xF0, 0x90, 0xF0, 0x90, 0x90,  // A

              0xE0, 0x90, 0xE0, 0x90, 0xE0,  // B

              0xF0, 0x80, 0x80, 0x80, 0xF0,  // C

              0xE0, 0x90, 0x90, 0x90, 0xE0, // D

              0xF0, 0x80, 0xF0, 0x80, 0xF0, // E

              0xF0, 0x80, 0xF0, 0x80, 0x80 // F

          };


        public byte[] direct
        {
            get { return this.Contents; }
            set { this.Contents = value; }
        }

        public Memory()
        {
            this.Contents = new byte[4096];
            this.Contents.Initialize();
            this.PopulateLetters();
        }

        private void PopulateLetters()
        {
            Array.Copy(this.hexChars, this.Contents, this.hexChars.Length);
        }

        public byte Read8(UInt16 Addr)
        {
            return this.Contents[Addr];
        }

        public UInt16 Read16(int Addr)
        {
            //tmp = new byte[] { this.Contents[Addr / 8], this.Contents[Addr / 8 + 1] };
            // return BitConverter.ToUInt16(this.Contents, Addr);
            return (UInt16)(this.Contents[Addr] << 8 | this.Contents[Addr + 1]);
        }

        

    }
}
