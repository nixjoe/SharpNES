using System.ComponentModel.DataAnnotations;
using System.IO;
using SharpNES.SharedCode;
using SharpNES.standard.test.Properties;
using Xunit;

namespace SharpNES.standard.test.Integration
{
    /// <summary>
    /// Hello Worldサンプルを読み込んだときのCPUをテストする
    /// </summary>
    public class CPUforHelloWorld
    {
        private CPU cpu;
        private PPU ppu;

        public CPUforHelloWorld()
        {
            var rom = Resources.ResourceManager.GetObject("sample1");
            var cartridge = new Cartridge(new MemoryStream((byte[])rom));
            var ram = new RAM(0x0800);
            ppu = new PPU(cartridge);
            var bus = new CpuBus(ram, ppu, cartridge);
            cpu = new CPU(bus);
        }

        [Fact]
        public void HelloWolrdを実行する()
        {
            cpu.Reset();
            var count = 0;
            // while (count < 200)
            while (true)
            {
                var cycle = cpu.ExecuteInstruction();
                ppu.Run(cycle);
                count++;
            }
        }
    }
}