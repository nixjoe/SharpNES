using System.Reflection;
using Moq;
using SharpNES.SharedCode;
using Xunit;

using Address = System.UInt16;

namespace SharpNES.standard.test
{
    public class CPUTest
    {
        private CPU sut;
        private Mock<ICpuBus> mock;

        public CPUTest()
        {
            mock = new Mock<ICpuBus>();
            sut = new CPU(mock.Object);
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

        [Fact]
        public void LDX命令を実行するとバス経由で読み取った値がXレジスタにロードされる()
        {
            const byte data = 0x20;
            var busMock = new Mock<ICpuBus>();
            busMock.Setup(bus => bus.Read(0x0000))
                .Returns(data);
            sut = new CPU(busMock.Object);
            sut.AsDynamic().LDX(CPU.AddressingMode.Immediate, (Address) 0x0000);

            var propertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = propertyInfo.GetValue(sut) as CPU.Registers;
            registers.X.Is(data);
        }

        [Fact]
        public void LDX命令を実行したときXレジスタのビット7が1ならNegativeフラグが立つ()
        {
            const byte data = 0xFF;

            var busMock = new Mock<ICpuBus>();
            busMock.Setup(bus => bus.Read(0x0000))
                .Returns(data);
            sut = new CPU(busMock.Object);
            sut.AsDynamic().LDX(CPU.AddressingMode.Immediate, (Address) 0x0000);

            var registersPropertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = registersPropertyInfo.GetValue(sut) as CPU.Registers;
            registers.X.Is(data);

            var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
            var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
            statusFlags.Negative.IsTrue();
        }

        [Fact]
        public void LDX命令を実行したときXレジスタに0がロードされたときはZeroフラグが立つ()
        {
            const byte data = 0x00;

            var busMock = new Mock<ICpuBus>();
            busMock.Setup(bus => bus.Read(0x0000))
                .Returns(data);
            sut = new CPU(busMock.Object);
            sut.AsDynamic().LDX(CPU.AddressingMode.Immediate, (Address) 0x0000);

            var registersPropertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = registersPropertyInfo.GetValue(sut) as CPU.Registers;
            registers.X.Is(data);

            var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
            var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
            statusFlags.Zero.IsTrue();
        }

        [Fact]
        public void LDA命令を実行するとバス経由で読み取った値がAレジスタにロードされる()
        {
            const byte data = 0x20;
            var busMock = new Mock<ICpuBus>();
            busMock.Setup(bus => bus.Read(0x0000))
                .Returns(data);
            sut = new CPU(busMock.Object);
            sut.AsDynamic().LDA(CPU.AddressingMode.Immediate, (Address) 0x0000);

            var propertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = propertyInfo.GetValue(sut) as CPU.Registers;
            registers.A.Is(data);
        }

        [Fact]
        public void LDA命令を実行したときAレジスタのビット7が1ならNegativeフラグが立つ()
        {
            const byte data = 0xFF;

            var busMock = new Mock<ICpuBus>();
            busMock.Setup(bus => bus.Read(0x0000))
                .Returns(data);
            sut = new CPU(busMock.Object);
            sut.AsDynamic().LDA(CPU.AddressingMode.Immediate, (Address) 0x0000);

            var registersPropertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = registersPropertyInfo.GetValue(sut) as CPU.Registers;
            registers.A.Is(data);

            var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
            var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
            statusFlags.Negative.IsTrue();
        }

        [Fact]
        public void LDA命令を実行したときAレジスタに0がロードされたときはZeroフラグが立つ()
        {
            const byte data = 0x00;

            var busMock = new Mock<ICpuBus>();
            busMock.Setup(bus => bus.Read(0x0000))
                .Returns(data);
            sut = new CPU(busMock.Object);
            sut.AsDynamic().LDA(CPU.AddressingMode.Immediate, (Address) 0x0000);

            var registersPropertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = registersPropertyInfo.GetValue(sut) as CPU.Registers;
            registers.A.Is(data);

            var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
            var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
            statusFlags.Zero.IsTrue();
        }

        [Fact]
        public void STA命令を実行したときバスを経由してデータが書き込まれる()
        {
            const byte data = 0x80;
            const Address address = 0x00FF;
            var registersPropertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = registersPropertyInfo.GetValue(sut) as CPU.Registers;
            registers.A = data;

            sut.AsDynamic().STA(CPU.AddressingMode.Immediate, address);
            
            mock.Verify(bus => bus.Write(address, data));
        }
    }
}
