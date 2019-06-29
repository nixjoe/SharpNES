using System;
using System.Xml;
using Address = System.UInt16;

namespace SharpNES.SharedCode
{
    public class PPU
    {
        internal class ControlRegister
        {
            private byte register;

            public void SetFrags(byte data)
            {
                    register = data;
            }

            /// <summary>
            /// PPUDATA($2007)に書き込んだ時のアドレスインクリメント値
            /// </summary>
            public Address AddressIncrement => ((register >> 2) & 0x01) == 1 ? (ushort) 0x20 : (ushort) 0x01;
        }

        internal class MaskRegister
        {
            private byte register;

            public void SetFlags(byte data)
            {
                register = data;
            }
        }

        private int cycle;


        private RAM vram;
        private RAM spriteRam;

        private ControlRegister controlRegister = new ControlRegister();
        private MaskRegister maskRegister = new MaskRegister();

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
            if (address == 0x2000)
            {
                controlRegister.SetFrags(data);
                return;
            }

            if (address == 0x2001)
            {
                maskRegister.SetFlags(data);
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
                WriteVramData(data);
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

        /// <summary>
        /// PPUが持つVRAMへデータを書き込む。
        /// </summary>
        /// <param name="data"></param>
        private void WriteVramData(byte data)
        {
            if (currentVramAddress >= 0x2000)
            {
                if (0x3f00 <= currentVramAddress && currentVramAddress < 0x4000)
                {
                    // パレットテーブルへの書き込み
                }
                else
                {
                    // ネームテーブル、属性テーブルへの書き込み
                    // 0x3000 - 0x3EFF間のミラーリングを考慮
                     var address = 0x3000 <= currentVramAddress && currentVramAddress < 0x3F00
                        ? (Address)(currentVramAddress - 0x3000)
                        : (Address)(currentVramAddress - 0x2000);
                    vram.Write(address, data);
                }
            }
            else
            {
                // TODO キャラクタRAMへの書き込み
            }

            currentVramAddress += controlRegister.AddressIncrement;
        }
    }
}
