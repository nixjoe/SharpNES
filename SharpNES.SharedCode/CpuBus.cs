using Address = System.UInt16;
using System;

// Memory map

/*
| addr           |  description               |   mirror       |
+----------------+----------------------------+----------------+--------------------------------------------
| 0x0000-0x07FF  |  RAM                       |                | 0000 0000 0000 0000 - 0000 0111 1111 1111 (このうちスタックは0x0100〜0x01ffの256Byte)
| 0x0800-0x1FFF  |  reserve                   | 0x0000-0x07FF  | 0000 1000 0000 0000 - 0001 1111 1111 1111
| 0x2000-0x2007  |  I/O(PPU)                  |                | 0010 0000 0000 0000 - 0010 0000 0000 0111
| 0x2008-0x3FFF  |  reserve                   | 0x2000-0x2007  | 0010 0000 0000 1000 - 0011 1111 1111 1111
| 0x4000-0x401F  |  I/O(APU, etc)             |                | 0100 0000 0000 0000 - 0100 0000 0001 1111
| 0x4020-0x5FFF  |  ex RAM                    |                | 0100 0000 0010 0000 - 0101 1111 1111 1111
| 0x6000-0x7FFF  |  battery backup RAM        |                | 0110 0000 0000 0000 - 0111 1111 1111 1111
| 0x8000-0xBFFF  |  program ROM LOW           |                | 1000 0000 0000 0000 - 1011 1111 1111 1111
| 0xC000-0xFFFF  |  program ROM HIGH          |                | 1100 0000 0000 0000 - 1111 1111 1111 1111
*/

namespace SharpNES.SharedCode
{
    class CpuBus
    {
        public CpuBus()
        {
        }

        public byte Read(Address address)
        {
            switch (address & 0xE000)
            {
                case 0x0000:
                // WRAM
                case 0x2000:
                // PPU
                case 0x4000:
                // APU, Controller I/O
                case 0x8000:
                case 0xC000:
                // Program ROM
                default:
                    throw new ArgumentException("Trying to access an address not included in the specification.");
            }
        }

        public void Write(Address address, byte data)
        {
        }
    }
}