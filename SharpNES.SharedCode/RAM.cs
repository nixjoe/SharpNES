using System.Linq;
using Address = System.UInt16;

namespace SharpNES.SharedCode
{
    /// <summary>
    /// CPUのWRAM領域やPPUのVRAM、スプライトRAMなどのメモリ領域を表現する抽象クラス
    /// 各コンポーネントはこのクラスを継承し、サイズ指定やアドレスチェックなどを実装していく。
    /// </summary>
    abstract class RAM
    {
        private byte[] memory;

        protected RAM(int size)
        {
            memory = Enumerable.Repeat((byte) 0, size).ToArray();
        }

        public byte Read(Address address)
        {
            return memory[address];
        }

        public void Write(Address address, byte data)
        {
            memory[address] = data;
        }
    }
}