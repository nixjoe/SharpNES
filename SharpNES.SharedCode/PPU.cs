using System.Xml;
using Address = System.UInt16;

namespace SharpNES.SharedCode
{
    public class PPU
    {
        private int cycle;


        private RAM vram;
        private RAM spriteRam;

        // VRAMアドレス
        private bool isAlreadySetUpperVramAddress = false;
        private bool isCompleteVramAddress = false;
        private Address currentVramAddress;

        public PPU(RAM vram, RAM spriteRam)
        {
            this.vram = vram;
            this.spriteRam = spriteRam;
        }

        public void Write(Address address, byte data)
        {
            if (address == 0x2000 || address == 0x2001)
            {
                return;
            }

            if (address == 0x2003)
            {
                return;
            }

            if (address == 0x2004)
            {
                return;
            }

            if (address == 0x2005)
            {
                return;
            }

            if (address == 0x2006)
            {
                WriteVramAddress(data);
                return;
            }

            if (address == 0x2007)
            {
                return;
            }
        }

        /// <summary>
        /// CPUからVRAMを操作するために、操作対象のVRAMアドレスを設定する。
        /// NESの6502の仕様では、0x2006に二回書き込むことによってVRAMのアドレスを指定する。
        /// 最初がアドレスの上位バイトで、2回目がアドレスの下位バイドとなる。
        /// </summary>
        /// <param name="data"></param>
        private void WriteVramAddress(byte data)
        {
            if (isAlreadySetUpperVramAddress)
            {
                currentVramAddress += (Address) data;
                isAlreadySetUpperVramAddress = false;
                isCompleteVramAddress = true;
                return;
            }

            currentVramAddress = (Address)(data << 8);
            isAlreadySetUpperVramAddress = true;
            isCompleteVramAddress = false;
        }
    }
}
