using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chip8Emu
{
    class CPU
    {
        private byte[] V; // V1-VF Registers
        private UInt16 I; // I Register
        private Stack<UInt16> Stack; // Stack

        private UInt16 PC; // Program Counter
        private byte SP; // Stack Pointer
        private byte DT; // Delay Timer
        private byte ST; // Sound Timer
        private Memory memory;
        private Display display;

        private Form1 MainForm;

        public CPU(Form1 parent, Memory memory, Display display)
        {
            this.V = new byte[16];
            this.Stack = new Stack<UInt16>(16);
            this.PC = 0;
            this.SP = 0;
            this.I = 0;
            this.memory = memory;
            this.display = display;
            this.display.AddCpu(this);
            this.MainForm = parent;
        }

        public void SetCollision(bool val)
        {
            if (val == true)
                this.V[0xF] = 1;
            else
                this.V[0xF] = 0;
            //Console.WriteLine(this.V[15]);
        }

        public void JumpTo(UInt16 Address)
        {
            this.PC = Address;
        }

        public void Tick()
        {
            // Execute OPCODE
            this.ReadOPCODE();
            if (this.DT > 0) this.DT--;
            if (this.ST > 0) this.ST--;
            //this.display.Draw(); 
            
        }

        public void ReadOPCODE()
        {
            UInt16 OP = this.memory.Read16(this.PC);
            this.PC += 2;
            // OP FORMATS
            // oxyn
            // oxkk
            // onnn
            byte o  = (byte)(OP >> 12);
            byte z  = (byte)(OP & 0x000F);
            byte x  = (byte)((OP & 0x0F00) >> 8);
            byte y  = (byte)((OP & 0x00F0) >> 4);
            byte kk = (byte)(OP & 0x00FF);
            UInt16 nnn  = (UInt16)(OP & 0x0FFF);
            /* Console.WriteLine("V[0:{0},1:{1},2:{2},3:{3},4:{4},5:{5},6:{6},7:{7}" +
                              ",8:{8},9:{9},A:{10},B:{11},C:{12},D:{13},E:{14},F:{15}] " +
                              "I[{16}] DT[{17}] ST[{18}]",
                              V[0], V[1], V[2], V[3], V[4], V[5], V[6], V[7],
                              V[8], V[9], V[0xA], V[0xB], V[0xC], V[0xD], V[0xE], V[0xF],
                              I, DT, ST); */
            Console.Write("{0:X}[{1:X}] ", PC,OP);
            
            switch (o)
            {
                case 0xF:
                    switch (kk)
                    {
                        case 0x33:
                            // Fx33 - LD B, Vx
                            // Store BCD representation of Vx in memory locations I, I + 1, and I+2.

                            byte[] bcd = new byte[3];
                           
                            for (int i = 0; i < 3; i++)
                            {
                                bcd[i] = (byte)(V[x] % 10);
                                x /= 10;
                                bcd[i] |= (byte)((V[x] % 10) << 4);
                                x /= 10;
                            }
                            this.memory.Contents[this.I] = bcd[2];
                            this.memory.Contents[this.I + 1] = bcd[1];
                            this.memory.Contents[this.I + 2] = bcd[0];
                            Console.WriteLine("Fx33 - LD B, V{0:X} [{1},{2},{3}]",x, bcd[2],bcd[1],bcd[0]);
                            break;

                        case 0x29:
                            // Fx29 - LD F, Vx
                            // Set I = location of sprite for digit Vx
                            Console.WriteLine("Fx29 - LD I, V{0:X} -> addr {1:X}", V[x], V[x] * 5);
                            this.I = //(UInt16)(this.memory.Contents[x*6]);
                                (UInt16)(V[x] * 5);
                            break;

                        case 0x18:
                            // Fx18 - LD ST, Vx
                            // Set sound timer = Vx.
                            Console.WriteLine("Fx18 - LD ST, V{0:X}[{1}]", x, V[x]);
                            this.ST = this.V[x];
                            break;

                        case 0x15:
                            // Fx15 - LD DT, Vx
                            // Set delay timer = Vx.
                            this.DT = this.V[x];
                            break;

                        case 0x07:
                            // Fx07 - LD Vx, DT
                            // Set Vx = delay timer value.
                            this.V[x] = (byte) this.DT;
                            break;

                        case 0x65:
                            // Fx65 - LD Vx, [I]
                            // Read registers V0 through Vx from memory starting at location I.
                            for(int i = 0; i<=x; i++)
                            {
                                V[i] = this.memory.Contents[this.I + i];
                            }
                            break;

                        case 0x1E:
                            // Fx1E - ADD I, Vx
                            // Set I = I + Vx.
                            Console.WriteLine("Fx1E - ADD I, V{0:X} [I={1} V{0}={2}]",x,I,V[x]);
                            I += V[x];
                            break;

                        default:
                            this.MainForm.Debug("!!UNKNOWN OPCODE: " + OP.ToString("X4"));
                            Thread.Sleep(5000);
                            break;
                    }
                    break;

                case 0xE: //TODO INPUT
                    switch (kk)
                    {
                        case 0xA1:
                            // KEYPRESS CHECK
                            Console.WriteLine();
                            this.PC += 2;
                            break;
                        case 0x9E:
                            Console.WriteLine();
                            //this.PC += 2;
                            break;

                    }
                    break;
                

                case 0xD:
                    // Dxyn - Draw sprite at Vx, Vy from VI, size n
                    // Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.
                    Console.WriteLine("Dxyn - DRW V{0:X}, V{1:X}, {2} [V{0:X}={3},V{1:X}=4]", x, y, z,V[x],V[y]);
                    byte[] sprite = new byte[z];
                    Array.Copy(this.memory.Contents, I, sprite, 0, z);
                    //Console.WriteLine("SPRITE: {0}", Convert.ToString(sprite[0],2));
                    this.display.WriteSprite(V[x], V[y], sprite);
                    break;

                case 0xC:
                    // Cxkk - RND Vx, byte
                    // Set Vx = random byte AND kk.
                    byte[] rnd = { 0 };
                    Random r = new Random();
                    r.NextBytes(rnd);
                    this.V[x] = (byte)(rnd[0] & kk);
                    Console.WriteLine("Cxkk - RND V{0:X}, {1} [{2}]", x,kk, rnd[0] % kk);
                    break;

                case 0xA:
                    // Annn - load value nnn into register I
                    // Set I = nnn.
                    Console.WriteLine("Annn - LOAD I, {0:X}", nnn);
                    this.I = nnn;
                    break;

                case 0x8:

                    switch (z)
                    {
                        case 0x0:
                            // 8xy0 - LD Vx, Vy
                            // Set Vx = Vy.
                            Console.Write("8xy0 - LD V{0:X}, V{1:X} [V{0:X}={2},V{1:X}={3}", x, y, V[x], V[y]);
                            this.V[x] = V[y];
                            Console.WriteLine(",SUM={0}]", V[x]);
                            break;

                        case 0x2:
                            // 8xy2 - AND Vx, Vy
                            // Set Vx = Vx AND Vy.
                            Console.Write("8xy2 - AND V{0:X}, V{1:X} [V{0:X}={2},V{1:X}={3}", x, y, V[x], V[y]);
                            this.V[x] = (byte)(V[x] & V[y]);
                            Console.WriteLine(",AND={0}]", V[x]);
                            break;

                        case 0x5:
                            // 8xy5 - SUB Vx, Vy
                            // Set Vx = Vx - Vy, set VF = NOT borrow.
                            // If Vx > Vy, then VF is set to 1, otherwise 0.
                            // Then Vy is subtracted from Vx, and the results stored in Vx.
                            int ox = V[x];
                            Console.Write("8xy5 - SUB V{0:X}, V{1:X} [V{0:X}={2},V{1:X}={3}", x, y, V[x], V[y]);
                            if (V[x] > V[y])
                                V[0xF] = 1;
                            else
                                V[0XF] = 0;
                            V[x] = (byte)(V[y] - V[x]);
                            //Console.WriteLine("SUB {0}-{1}={2} F{3}", V[y], ox, V[x], V[0xF]);
                            Console.WriteLine(",SUB={0},VF=]", V[x], V[0xF]);
                            break;

                        case 0x6:
                            // 8xy6 - SHR Vx {, Vy}
                            // Set Vx = Vx SHR 1.
                            Console.Write("8xy6 - SHR V{0:X} [V{0:X}={1}", x, V[x]);
                            V[0xF] = (byte)(V[x] & 0x1);
                            V[x] >>= 1;
                            Console.WriteLine(",SHR={0},VF={1}", V[x], V[0xF]);
                            break;

                        case 0x4:
                            // 8xy4 - ADD Vx, Vy
                            // Set Vx = Vx + Vy, set VF = carry.
                            // The values of Vx and Vy are added together. 
                            // If the result is greater than 8 bits(i.e., > 255,) VF is set to 1, otherwise 0.
                            byte oldx = V[x];
                            try
                            {
                                checked
                                {
                                    this.V[x] = (byte)(V[x] + V[y]);
                                    this.V[0xF] = 0;
                                }
                            } catch (OverflowException e) { this.V[0xF] = 1; }
                            Console.WriteLine("8xy4 - ADD V{0:X}, V{2:X} [{1}+{3}={4},VF={5}]", x, oldx, y,V[y], V[x], V[0xF]);
                            break;

                        default:
                            Console.WriteLine("@@@UNKNOWN OPCODE {0:X}", OP);
                            this.MainForm.Debug("UNKNOWN OPCODE " + OP);
                            Thread.Sleep(5000);
                            break;
                    }
                    break;

                case 0x7:
                    // 7xkk - ADD Vx, kk
                    // Set Vx = Vx + kk.
                    //Console.WriteLine("ADD V" + ((OP & 0x0F00) >> 8).ToString("X") + ", " + (OP & 0x00FF));
                    Console.Write("7xkk - ADD V{0:X}, {1} [V{0:X}={2}", x, kk, V[x]);
                    this.V[x] += kk;
                    Console.WriteLine(",SUM={0}]", V[x]);
                    break;

                case 0x6:
                    // 6xkk - load value kk into register vx
                    // Set Vx = kk.
                    Console.WriteLine("6xkk - LD V{0:X}, {1:X}",x,kk);
                    this.V[x] = kk;
                    break;

                case 0x4:
                    // 4xkk - SNE Vx, byte
                    // Skip next instruction if Vx != kk.
                    Console.Write("SNE V{0:X},{1:X} [V{0:X}={2:X}] ", x, kk, V[x]);
                    if (this.V[x] != kk)
                    {
                        Console.WriteLine("[TRUE]");
                        this.PC += 2;
                    } else
                    {
                        Console.WriteLine("[FALSE]");
                    }
                    break;

                case 0x3:
                    // 3xkk - SE Vx, kk
                    // Skip next instruction if Vx = kk.
                    Console.Write("3xkk - SE V{0}, {1} [V{0}={2}] ", x, kk, V[x]);
                    if (this.V[x] == kk)
                    {
                        this.PC += 2;
                        Console.WriteLine("[TRUE]");
                    } else
                    {
                        Console.WriteLine("[FALSE]");
                    }
                    break;

                case 0x2:
                    // 2nnn - CALL addr
                    // Call subroutine at nnn.
                    // The interpreter increments the stack pointer, 
                    // then puts the current PC on the top of the stack.
                    Console.WriteLine("2nnn - CALL {1:X} [{0:X} -> {1:X}]",this.PC,nnn);
                    this.Stack.Push((UInt16)(this.PC));
                    this.PC = (UInt16)(nnn);
                    break;

                case 0x1:
                    // 1nnn - JP addr
                    // Jump to location nnn.
                    // The interpreter sets the program counter to nnn.
                    Console.WriteLine("1nnn - JP {1:X} [{0:X} -> {1:X}]", PC, nnn);
                    this.PC = (UInt16)(nnn);
                    break;

                case 0x0:
                    switch (kk)
                    {
                        case 0xEE:
                            // 00EE - RET
                            // The interpreter sets the program counter 
                            // to the address at the top of the stack, 
                            // then subtracts 1 from the stack pointer.
                            UInt16 dst = this.Stack.Pop();
                            Console.WriteLine("00EE - RET [{0} -> {1}]",this.PC,dst);
                            this.PC = dst;
                            break;

                        default:
                            this.MainForm.Debug("!!UNKNOWN OPCODE: " + OP.ToString("X4"));
                            Console.WriteLine("@@@UNKNOWN OPCODE {0:X}", OP);
                            Thread.Sleep(5000);
                            break;
                    }
                   
                    break;
                    
                default:
                    this.MainForm.Debug("!!UNKNOWN OPCODE: " + OP.ToString("X4"));
                    Console.WriteLine("@@@UNKNOWN OPCODE {0:X}", OP);
                    Thread.Sleep(5000);
                    break;

            }
        }

    }
}
/* Registers
        private Int16 A; //Accumulator
        private Int16 iX; //Index register X
        private Int16 iY; // Index register Y
        private Int16 SP; // Stack Pointer
        private Int16 PC; // Program Counter
        private BitArray P; // Processor status

        public CPU()
        {
            // Init
            this.A = 0;
            this.iX = 0;
            this.iY = 0;
            this.SP = 0;
            this.PC = 0;
            this.P = new BitArray(8);
        }
        */