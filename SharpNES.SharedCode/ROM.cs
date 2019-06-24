using System;
using System.Linq;
using Address = System.UInt16;

namespace SharpNES.SharedCode
{
    public class ROM
    {
        private readonly byte[] memory;
        private readonly int size;

        public ROM(byte[] data)
        {
            this.size = data.Length;
            memory = data;
        }

        public virtual byte Read(Address address)
        {
            if (address >= size)
            {
                throw new ArgumentException("Trying to access an address outside the specified range");
            }
            return memory[address];
        }
    }
}
