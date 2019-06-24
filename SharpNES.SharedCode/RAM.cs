using System;
using System.Linq;
using Address = System.UInt16;

namespace SharpNES.SharedCode
{
    /// <summary>
    /// CPUのWRAM領域やPPUのVRAM、スプライトRAMなどのメモリ領域を表現する抽象クラス
    /// 各コンポーネントはこのクラスを継承し、サイズ指定やアドレスチェックなどを実装していく。
    /// </summary>
    public class RAM
    {
        private readonly byte[] memory;
        private readonly int size;
        
        public RAM(int size)
        {
            this.size = size;
            memory = Enumerable.Repeat((byte) 0, size).ToArray();
        }

        public virtual byte Read(Address address)
        {
            if (address >= size)
            {
                throw new ArgumentException("Trying to access an address outside the specified range");
            }
            return memory[address];
        }

        public virtual void Write(Address address, byte data)
        {
            if (address >= size)
            {
                throw new ArgumentException("Trying to access an address outside the specified range");
            }
            memory[address] = data;
        }
    }
}
