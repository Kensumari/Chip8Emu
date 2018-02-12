using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chip8Emu
{
    class Display
    {
        private Form1 MainForm;
        private BitArray Memory;
        private CPU cpu;

        public Display(Form1 parent)
        {
            this.MainForm = parent;
            this.InitDisplay();
            this.TestDisplay();
        }

        public void AddCpu(CPU cpu)
        {
            this.cpu = cpu;
        }

        private void TestDisplay()
        {
            this.MainForm.Debug("testing display");
            //this.SetPixel(10, 10, true);
            //this.SetPixel(10, 11, true);
            //this.SetPixel(11, 10, true);
            //this.SetPixel(11, 11, true);
            this.Draw();
        }
        private void InitDisplay()
        {
            this.Memory = new BitArray(64 * 32);
            //this.Clear();
        }

        //public void Clear()
        //{
         //   using(Graphics g = this.me)
       // }

        private void SetPixel(int nx, int ny, bool val)
        {
            //nx = (x*y) % 64;
            //ny = x %
            int x =0, y = 0;
            if (nx >= 64)
                x = nx;//% 64;
            else
                x = nx;
            if (ny >= 32)
                y = nx;// % 32;
            else
                y = ny;
            if (y == 0)
            {
                //Console.WriteLine("PIXEL X[{0},{1}] Y[{2},{3}] VAL[{4}]", nx, x, ny, y, val);
            }
            if (this.Memory[x + (y * 64)] == true)
            {
                //Console.WriteLine("COL");
                this.cpu.SetCollision(true);
            }
            else
                this.cpu.SetCollision(false);
            this.Memory[x + (y*64)] ^= val;
        }

        public static string ToBitString(BitArray bits)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < bits.Count; i++)
            {
                char c = bits[i] ? '1' : '0';
                sb.Append(c);
            }

            return sb.ToString();
        }

        public void WriteSprite(int nx, int ny, byte[] sprite)
        {
            //** int x = 0, y = 0;
            //if (x >= 64)
            //    x = nx % 64;
            //else
            //    x = nx;
            //if (y >= 32)
            //    y = nx % 32;
            //else
            //    y = ny; 
            //    */
            int x = nx;
            int y = ny;

            BitArray obitsprite = new BitArray(sprite);
            BitArray bitsprite = new BitArray(obitsprite.Length);
            for(int i = 0; i<sprite.Length;i++)
            {
                for (int u = 0; u < 8; u++)
                {
                    bitsprite[i*8+u] = obitsprite[i*8+(7-u)];
                }
            }
            //Console.WriteLine("BITSPRITE:");
            //string[] SSprite;// = ToBitString(bitsprite);
            //string source = ToBitString(bitsprite);
            /*SSprite = source.Where((c, i) => i % 8 == 0)
                .Select( (c, i) => new string(source
                .Skip(i * 8)
                .Take(8)
                .ToArray()))
                .ToArray();*/
            
            //ObjectDumper.Dumper.Dump(bitsprite, "BITSPRITE", System.Console.Out);
            //string[] SSprite = Regex.Split(source, @"(?<=\G.{8})", RegexOptions.Singleline);
            //Console.WriteLine("SPRITE X:{0} Y:{1} LEN:{2}", x, y, sprite.Length, bitsprite.Length, SSprite.Length);
            //for (int i = 0; i<(sprite.Length);i++)
            //{
                //Console.WriteLine("!{0}", SSprite[i]);
            //}
            
            //Console.WriteLine("BITSPRITE: {0}", ToBitString(bitsprite));
            //Console.WriteLine("\r\nSPRITE X:{0} Y:{1} LEN:{2}",x,y,bitsprite.Length);
            //
            //Console.WriteLine("SPRITE LEN")
            for (int sy = 0; sy < sprite.Length; sy++)
            {
                for (int sx = 0; sx < 8; sx++)
                {
                    SetPixel(x + sx, y + sy, bitsprite[(sx)+(sy*8)]);
                    //Console.WriteLine(Memory[(x + sx) + ((y + sy) * 64)]);
                }
            }
            this.Draw();
            
        }

        public void Draw()
        {
            for(int x=0; x<64; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    this.MainForm.SetPixel(x, y, this.Memory[x + (y*64)]);
                }
            }
            this.MainForm.Draw();
        }
    }
}
