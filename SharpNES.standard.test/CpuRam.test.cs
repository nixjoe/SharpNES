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
            data.Is((byte)0x0000);
        }

        [Theory]
        [InlineData(0x0800)]
        [InlineData(0x0801)]
        public void 範囲外のアドレスを指定したときは例外が発生する(Address address)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                sut.Read(address);
            });
        }
    }
}