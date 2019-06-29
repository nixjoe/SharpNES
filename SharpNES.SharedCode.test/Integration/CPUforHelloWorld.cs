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
        
        public CPUforHelloWorld()
        {
            var rom = Resources.ResourceManager.GetObject("sample1");
            var cartridge = new Cartridge(new MemoryStream((byte[])rom));
            var ram = new RAM(0x0800);
            var bus = new CpuBus(ram, cartridge);
            cpu = new CPU(bus);
        }

        [Fact]
        public void HelloWolrdを実行する()
        {
            cpu.Reset();
            var count = 0;
            while (count < 200)
            {
                var cycle = cpu.ExecuteInstruction();
                count++;
            }
        }
    }
}