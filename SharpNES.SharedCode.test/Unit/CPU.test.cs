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
            sut.AsDynamic().TXS(CPU.AddressingMode.Implied, (Address)0x0000);
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
            sut.AsDynamic().LDX(CPU.AddressingMode.Immediate, (Address)0x0000);

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
            sut.AsDynamic().LDX(CPU.AddressingMode.Immediate, (Address)0x0000);

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
            sut.AsDynamic().LDX(CPU.AddressingMode.Immediate, (Address)0x0000);

            var registersPropertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = registersPropertyInfo.GetValue(sut) as CPU.Registers;
            registers.X.Is(data);

            var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
            var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
            statusFlags.Zero.IsTrue();
        }

        [Fact]
        public void LDY命令を実行するとバス経由で読み取った値がYレジスタにロードされる()
        {
            const byte data = 0x20;
            var busMock = new Mock<ICpuBus>();
            busMock.Setup(bus => bus.Read(0x0000))
                .Returns(data);
            sut = new CPU(busMock.Object);
            sut.AsDynamic().LDY(CPU.AddressingMode.Immediate, (Address)0x0000);

            var propertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = propertyInfo.GetValue(sut) as CPU.Registers;
            registers.Y.Is(data);
        }

        [Fact]
        public void LDY命令を実行したときYレジスタのビット7が1ならNegativeフラグが立つ()
        {
            const byte data = 0xFF;

            var busMock = new Mock<ICpuBus>();
            busMock.Setup(bus => bus.Read(0x0000))
                .Returns(data);
            sut = new CPU(busMock.Object);
            sut.AsDynamic().LDY(CPU.AddressingMode.Immediate, (Address)0x0000);

            var registersPropertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = registersPropertyInfo.GetValue(sut) as CPU.Registers;
            registers.Y.Is(data);

            var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
            var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
            statusFlags.Negative.IsTrue();
        }

        [Fact]
        public void LDY命令を実行したときYレジスタに0がロードされたときはZeroフラグが立つ()
        {
            const byte data = 0x00;

            var busMock = new Mock<ICpuBus>();
            busMock.Setup(bus => bus.Read(0x0000))
                .Returns(data);
            sut = new CPU(busMock.Object);
            sut.AsDynamic().LDY(CPU.AddressingMode.Immediate, (Address)0x0000);

            var registersPropertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = registersPropertyInfo.GetValue(sut) as CPU.Registers;
            registers.Y.Is(data);

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
            sut.AsDynamic().LDA(CPU.AddressingMode.Immediate, (Address)0x0000);

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
            sut.AsDynamic().LDA(CPU.AddressingMode.Immediate, (Address)0x0000);

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
            sut.AsDynamic().LDA(CPU.AddressingMode.Immediate, (Address)0x0000);

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

        [Theory]
        [InlineData((byte)0x00)]
        [InlineData((byte)0xFF)]
        [InlineData((byte)0x7F)]
        public void INX命令を実行するとXレジスタの値がインクリメントされ結果によってはZフラグとNフラグが立つ(byte x)
        {
            var propertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = propertyInfo.GetValue(sut) as CPU.Registers;
            registers.X = x;

            sut.AsDynamic().INX(CPU.AddressingMode.Implied, (Address)0x0000);

            registers.X.Is((byte)(x + 1));

            if (x == 0x00)
            {
                var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
                statusFlags.Zero.IsFalse();
                statusFlags.Negative.IsFalse();
            }

            if (x == 0xFF)
            {
                var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
                statusFlags.Zero.IsTrue();
                statusFlags.Negative.IsFalse();
            }

            if (x == 0x7F)
            {
                var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
                statusFlags.Zero.IsFalse();
                statusFlags.Negative.IsTrue();

            }
        }

        [Theory]
        [InlineData((byte)0x00)]
        [InlineData((byte)0xFF)]
        [InlineData((byte)0x7F)]
        public void INY命令を実行するとYレジスタの値がインクリメントされ結果によってはZフラグとNフラグが立つ(byte y)
        {
            var propertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = propertyInfo.GetValue(sut) as CPU.Registers;
            registers.Y = y;

            sut.AsDynamic().INY(CPU.AddressingMode.Implied, (Address)0x0000);

            registers.Y.Is((byte)(y + 1));

            if (y == 0x00)
            {
                var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
                statusFlags.Zero.IsFalse();
                statusFlags.Negative.IsFalse();
            }

            if (y == 0xFF)
            {
                var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
                statusFlags.Zero.IsTrue();
                statusFlags.Negative.IsFalse();
            }

            if (y == 0x7F)
            {
                var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
                statusFlags.Zero.IsFalse();
                statusFlags.Negative.IsTrue();

            }
        }

        [Theory]
        [InlineData((byte)0x00)]
        [InlineData((byte)0x01)]
        [InlineData((byte)0x80)]
        public void DEX命令を実行するとXレジスタの値がインクリメントされ結果によってはZフラグとNフラグが立つ(byte x)
        {
            var propertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = propertyInfo.GetValue(sut) as CPU.Registers;
            registers.X = x;

            sut.AsDynamic().DEX(CPU.AddressingMode.Implied, (Address)0x0000);

            registers.X.Is((byte)(x - 1));

            if (x == 0x00)
            {
                var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
                statusFlags.Zero.IsFalse();
                statusFlags.Negative.IsTrue();
            }

            if (x == 0x01)
            {
                var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
                statusFlags.Zero.IsTrue();
                statusFlags.Negative.IsFalse();

            }

            if (x == 0x80)
            {
                var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
                statusFlags.Zero.IsFalse();
                statusFlags.Negative.IsFalse();

            }
        }

        [Theory]
        [InlineData((byte)0x00)]
        [InlineData((byte)0x01)]
        [InlineData((byte)0x80)]
        public void DEY命令を実行するとYレジスタの値がインクリメントされ結果によってはZフラグとNフラグが立つ(byte y)
        {
            var propertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = propertyInfo.GetValue(sut) as CPU.Registers;
            registers.Y = y;

            sut.AsDynamic().DEY(CPU.AddressingMode.Implied, (Address)0x0000);

            registers.Y.Is((byte)(y - 1));

            if (y == 0xFF)
            {
                var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
                statusFlags.Zero.IsTrue();
                statusFlags.Negative.IsFalse();
            }

            if (y == 0x7F)
            {
                var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
                statusFlags.Zero.IsFalse();
                statusFlags.Negative.IsTrue();
            }

            if (y == 0x80)
            {
                var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
                statusFlags.Zero.IsFalse();
                statusFlags.Negative.IsFalse();

            }
        }

        [Fact]
        public void JMP命令を実行するとプログラムカウンタに指定したアドレスが設定される()
        {
            const Address address = 0x8080;

            var propertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = propertyInfo.GetValue(sut) as CPU.Registers;

            sut.AsDynamic().JMP(CPU.AddressingMode.Absolute, address);

            registers.PC.Is(address);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void BNE命令を実行するとZeroフラグが立っているときだけプログラムカウンタに指定したアドレスが設定される(bool zeroFlag)
        {
            var statusFlagsPropertyInfo = sut.GetType().GetProperty("statusFlags", BindingFlags.NonPublic | BindingFlags.Instance);
            var statusFlags = statusFlagsPropertyInfo.GetValue(sut) as CPU.StatusFlags;
            statusFlags.Zero = zeroFlag;

            var registersPropertyInfo = sut.GetType().GetProperty("registers", BindingFlags.NonPublic | BindingFlags.Instance);
            var registers = registersPropertyInfo.GetValue(sut) as CPU.Registers;
            registers.PC = (Address) 0x0000;

            const Address address = 0x8080;

            sut.AsDynamic().BNE(CPU.AddressingMode.Relative, address);

            if (zeroFlag)
            {
                registers.PC.Is((Address)0x0000);
            }
            else
            {
                registers.PC.Is(address);
            }
        }
    }
}
