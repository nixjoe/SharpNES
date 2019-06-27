using System.IO;
using SharpNES.SharedCode;
using SharpNES.standard.test.Properties;
using Xunit;

namespace SharpNES.standard.test
{
    public class CartridgeTest
    {
        private Cartridge sut;

        public CartridgeTest()
        {
            var binaryStream = new MemoryStream(Resources.sample1);
            sut = new Cartridge(binaryStream);
        }

        [Fact]
        public void HelloWorldサンプルのマッパー番号は0としてロードできている()
        {
            sut.MapperNumber.Is((byte)0);
        }

        [Fact]
        public void HelloWorldサンプルのプログラムROMサイズは2としてロードできている()
        {
            sut.ProgramRomSize.Is((byte)2);
            sut.CharacterRom.IsNot(null);
        }

        [Fact]
        public void HelloWorldサンプルのキャラクタROMサイズは1としてロードできている()
        {
            sut.CharacterRomSize.Is((byte)1);
            sut.CharacterRom.IsNot(null);
        }

        [Fact]
        public void HelloWorldサンプルはバッテリーメモリを持たない()
        {
            sut.HasBatteryBackedMemory.IsFalse();
        }

        [Fact]
        public void HelloWorldサンプルは垂直ミラー()
        {
            sut.IsVerticalVramMirroring.IsTrue();
        }
    }
}
