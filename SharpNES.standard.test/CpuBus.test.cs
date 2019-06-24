using SharpNES.SharedCode;

namespace SharpNES.standard.test
{
    public class CpuBusTest
    {
        private CpuBus sut = new CpuBus(new RAM(0x0800));
        
    }
}