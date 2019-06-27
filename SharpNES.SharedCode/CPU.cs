using System;
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

            public void SetStatusFlags(byte flags)
            {
                Carry = BitSet(flags, 0);
                Zero = BitSet(flags, 1);
                Interrupt = BitSet(flags, 2);
                Decimal = BitSet(flags, 3);
                Break = BitSet(flags, 4);
                Overflow = BitSet(flags, 5);
                Negative = BitSet(flags, 6);
            }

            private bool BitSet(byte value, int index)
            {
                return (value & (1 << index)) == 1;
            }
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
            statusFlags.SetStatusFlags((byte)0x24);
        }

        public int ExecuteInstruction()
        {
            // TODO 割り込み処理

            var opcode = fetch(registers.PC);
            var instruction = instructionSet[opcode];
            var address = getAddress(instruction.AddressingMode);
            instruction.InstructionFunc(instruction.AddressingMode, address);
            // TODO サイクルの調整
            return instruction.Cycle;
        }

        private byte fetch(Address address)
        {
            registers.PC++;
            return bus.Read(address);
        }

        private Address fetchWord(Address address)
        {
            var lower = fetch(address);
            var upper = fetch((Address)(address + 1));
            return (Address)(upper << 8 | lower);
        }

        private Address getAddress(AddressingMode mode)
        {
            switch (mode)
            {
                case AddressingMode.Absolute:
                    return fetchWord(registers.PC);
                case AddressingMode.AbsoluteX:
                    {
                        var address = fetchWord(registers.PC);
                        return (Address)(address + registers.X);
                        // TODO サイクル調整
                    }
                case AddressingMode.AbsoluteY:
                    {
                        var address = fetchWord(registers.PC);
                        return (Address)(address + registers.Y);
                        // TODO サイクル調整
                    }
                case AddressingMode.Accumulator:
                    return 0x0000;
                case AddressingMode.Immediate:
                    return fetch(registers.PC);
                case AddressingMode.Implied:
                    return 0x0000;  // Impliedはアドレスを使用しないのでダミーを返す
                case AddressingMode.IndexedIndirect:
                    break;
                case AddressingMode.Indirect:
                    break;
                case AddressingMode.IndirectIndexed:
                    break;
                case AddressingMode.Relative:
                    {
                        var offset = fetch(registers.PC);
                        return (Address)(registers.PC + offset);
                    }
                case AddressingMode.ZeroPage:
                    break;
                case AddressingMode.ZeroPageX:
                    break;
                case AddressingMode.ZeroPageY:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
            throw new NotImplementedException();
        }
    }
}
