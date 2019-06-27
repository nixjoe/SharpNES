using System.Security.Cryptography.X509Certificates;
using Address = System.UInt16;

namespace SharpNES.SharedCode
{
    public partial class CPU
    {
        internal enum AddressingMode
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

        private delegate void InstructionFunc(AddressingMode mode, Address address);

        private class Instruction
        {
            public InstructionFunc InstructionFunc { get; set; }
            public AddressingMode AddressingMode { get; set; }
            public int Cycle { get; set; }
        }

        internal class Registers
        {
            public byte A { get; set; }
            public byte X { get; set; }
            public byte Y { get; set; }
            public byte S { get; set; }
            public ushort PC { get; set; }
        }

        internal class StatusFlags
        {
            public bool Carry { get; set; }
            public bool Zero { get; set; }
            public bool Interrupt { get; set; }
            public bool Decimal { get; set; }
            public bool Break { get; set; }
            public bool Overflow { get; set; }
            public bool Negative { get; set; }
        }

        private ICpuBus bus;

        private Registers registers { get; }
        private StatusFlags statusFlags { get; }
        private Instruction[] instructionSet = new Instruction[0xFF];


        public CPU(ICpuBus bus)
        {
            registers = new Registers();
            statusFlags = new StatusFlags();
            this.bus = bus;
            InitializeInstructionSet();
        }

        public void Reset()
        {
            registers.PC = bus.Read(0xFFFC);
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
