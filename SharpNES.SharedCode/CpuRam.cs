using Address = System.UInt16;

namespace SharpNES.SharedCode
{
    public class CpuRam: RAM
    {
        public CpuRam(): base(0x800)
        {
        }
    }
}