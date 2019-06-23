using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNES.core
{
    class CPU
    {
        private class Registers
        {
            public byte A { get; set; }
            public byte X { get; set; }
            public byte Y { get; set; }
            public byte S { get; set; }
            public ushort PC { get; set; }
        }

        private Registers registers;

        public CPU()
        {
            registers = new Registers();
        }

        public void Reset()
        {
            // TODO プログラムカウンタの初期値は0xFFFCから読んでくる
            registers.S = 0xFD;
            registers.A = 0;
            registers.X = 0;
            registers.Y = 0;
        }
    }
}
