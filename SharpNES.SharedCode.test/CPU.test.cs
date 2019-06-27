using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Moq;
using SharpNES.SharedCode;
using Xunit;

using Address = System.UInt16;

namespace SharpNES.standard.test
{
    public class CPUTest
    {
        private CPU sut;

        public CPUTest()
        {
            var busMock = new Mock<ICpuBus>();
            sut = new CPU(busMock.Object);
        }

        [Fact]
        public void SEI命令を実行するとIntteruptフラグが立つ()
        {
            sut.AsDynamic().SEI(CPU.AddressingMode.Implied, (Address)0x0000);
            var propertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
            var statusFlags = propertyInfo.GetValue(sut) as CPU.StatusFlags;
            statusFlags.Interrupt.IsTrue();
        }

        [Fact]
        public void TXS命令を実行するとXレジスタの値がSレジスタにロードされる()
        {
            var propertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = propertyInfo.GetValue(sut) as CPU.Registers;
            registers.X = 0x10;
            sut.AsDynamic().TXS(CPU.AddressingMode.Implied, (Address) 0x0000);
            registers.S.Is(registers.X);
        }
    }
}
