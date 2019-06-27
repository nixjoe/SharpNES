﻿using System;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Address = System.UInt16;

namespace SharpNES.SharedCode
{
    public partial class CPU
    {
        private void InitializeInstructionSet()
        {
            var test = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var methodInfo in test)
            {
                object[] attributes = methodInfo.GetCustomAttributes(typeof(OpcodeProperty), false);
                foreach (OpcodeProperty attribute in attributes)
                {
                    var instruction = new Instruction()
                    {
                        InstructionFunc = (InstructionFunc)Delegate.CreateDelegate(typeof(InstructionFunc), this, methodInfo),
                        AddressingMode = attribute.Mode,
                        Cycle = attribute.Cycle
                    };
                    instructionSet[attribute.Opcode] = instruction;
                }
            }
        }

        [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
        private class OpcodeProperty : Attribute
        {
            public byte Opcode;
            public int Cycle;
            public AddressingMode Mode;
        }


        [OpcodeProperty(Opcode = 0x78, Cycle = 2, Mode = AddressingMode.Implied)]
        private void SEI(AddressingMode mode, Address address)
        {
            statusFlags.Interrupt = true;
        }


        [OpcodeProperty(Opcode = 0xA2, Cycle = 2, Mode = AddressingMode.Immediate)]
        [OpcodeProperty(Opcode = 0xA6, Cycle = 3, Mode = AddressingMode.ZeroPage)]
        [OpcodeProperty(Opcode = 0xAE, Cycle = 4, Mode = AddressingMode.Absolute)]
        [OpcodeProperty(Opcode = 0xB2, Cycle = 4, Mode = AddressingMode.ZeroPageY)]
        [OpcodeProperty(Opcode = 0xBE, Cycle = 4, Mode = AddressingMode.AbsoluteY)]
        private void LDX(AddressingMode mode, Address address)
        {
            registers.X = bus.Read(address);
            SetZn(registers.X);
        }

        [OpcodeProperty(Opcode = 0x9A, Cycle = 2, Mode = AddressingMode.Implied)]
        private void TXS(AddressingMode mode, Address address)
        {
            registers.S = registers.X;
        }

        private void SetZn(byte value)
        {
            statusFlags.Zero = value == 0;
            statusFlags.Negative = ((value >> 7) & 1) == 1;
        }
    }
}