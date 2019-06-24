using System;
using SharpNES.SharedCode;
using Xunit;
using Address = System.UInt16;

namespace SharpNES.standard.test
{
    public class CpuRamTest
    {
        private CpuRam sut = new CpuRam();

        [Theory]
        [InlineData(0x0000)]
        [InlineData(0x0400)]
        [InlineData(0x07FF)]
        public void 初期化時メモリは0で埋められる(Address address)
        {
            var data = sut.Read(address);
            data.Is((byte) 0x0000);
        }

        [Theory]
        [InlineData(0x0800)]
        [InlineData(0x0801)]
        public void 範囲外のアドレスを指定したときは例外が発生する(Address address)
        {
            Assert.Throws<ArgumentException>(() => { sut.Read(address); });
        }

        [Theory]
        [InlineData(0x0000, 0x10)]
        [InlineData(0x0400, 0x00)]
        [InlineData(0x07FF, 0xFF)]
        public void アドレスを指定してメモリにデータを書き込むことができる(Address address, byte data)
        {
            sut.Write(address, data);
            var readData = sut.Read(address);
            readData.Is(data);
        }

        [Theory]
        [InlineData(0x0800)]
        [InlineData(0x0801)]
        public void 範囲外のアドレスを指定して書き込もうとすると例外が発生する(Address address)
        {
            Assert.Throws<ArgumentException>(() => sut.Write(address, 0x00));
        }
    }
}