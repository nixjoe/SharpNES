using Address = System.UInt16;

namespace SharpNES.SharedCode
{
    public class CPU
    {
        private enum AddressingMode
        {
            Absolute,
            AbsoluteX,
            AbsoluteY,
            Accumulator,
            Immediate,
            Implied,
            IndexedIndirect,
            Indirect,
            IndirectIndexed,
            Relative,
            ZeroPage,
            ZeroPageX,
            ZeroPageY,
        }

        private delegate void Instruction(AddressingMode mode, Address address);

        private class OpcodeProperties
        {
            private Instruction instruction;
            private AddressingMode addressingMode;
            private int cycle;
        }

        private class Registers
        {
            public byte A { get; set; }
            public byte X { get; set; }
            public byte Y { get; set; }
            public byte S { get; set; }
            public ushort PC { get; set; }
        }

        private Registers registers;
        private CpuBus bus;

        private OpcodeProperties[] opcodePorpertiesList = {
            null, null,
        };


        private int[] instructionCycles = {
            7, 6, 2, 8, 3, 3, 5, 5, 3, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            6, 6, 2, 8, 3, 3, 5, 5, 4, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            6, 6, 2, 8, 3, 3, 5, 5, 3, 2, 2, 2, 3, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            6, 6, 2, 8, 3, 3, 5, 5, 4, 2, 2, 2, 5, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            2, 6, 2, 6, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4,
            2, 6, 2, 6, 4, 4, 4, 4, 2, 5, 2, 5, 5, 5, 5, 5,
            2, 6, 2, 6, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4,
            2, 5, 2, 5, 4, 4, 4, 4, 2, 4, 2, 4, 4, 4, 4, 4,
            2, 6, 2, 8, 3, 3, 5, 5, 2, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            2, 6, 2, 8, 3, 3, 5, 5, 2, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
        };

        public CPU(CpuBus bus)
        {
            registers = new Registers();
            this.bus = bus;
        }

        public void Reset()
        {
            // TODO プログラムカウンタの初期値は0xFFFCから読んでくる
            registers.S = 0xFD;
            registers.A = 0;
            registers.X = 0;
            registers.Y = 0;
        }

        public void Run()
        {
            // TODO 割り込み処理
            // TODO オペコードの取得
            // TODO オペコードから命令種別とアドレッシングモードを解析する
            // TODO アドレッシングモードから処理対象のアドレスを解析
            // TODO 命令の実行
        }
    }
}
