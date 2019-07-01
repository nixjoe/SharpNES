using System;
using System.Xml;
using Address = System.UInt16;

namespace SharpNES.SharedCode
{
    public partial class PPU
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
            public Address AddressIncrement => ((register >> 2) & 0x01) == 1 ? (Address)0x20 : (Address)0x01;
        }

        internal class MaskRegister
        {
            private byte register;

            public void SetFlags(byte data)
            {
                register = data;
            }

            public Address BackgroundPatternTableAddress =>
                ((register >> 4) & 0x01) == 1 ? (Address)0x1000 : (Address)0x0000;
        }

        /* カートリッジのキャラクタROM */
        private ROM characterROM;


        /* PPUレジスタ */
        private readonly ControlRegister controlRegister = new ControlRegister();
        private readonly MaskRegister maskRegister = new MaskRegister();

        /* PPU内部メモリ */
        private readonly RAM videoRam;
        private readonly RAM spriteRam;
        private readonly RAM paletteRam;

        /* VRAM操作関連 */
        private bool isAlreadySetUpperVideoRamAddress = false;
        private bool isCompleteVideoRamAddress = false;
        private Address currentVideoRamAddress;

        /* スプライトRAM操作関連 */
        private Address currentSpriteRamAddress;

        public PPU(Cartridge cartridge)
        {
            paletteRam = new RAM(0x20); // パレットRAMは32Byte
            spriteRam = new RAM(0x100); // スプライトRAMは256Byte
            videoRam = new RAM(0x0800); // ビデオRAMは2KByte

            characterROM = cartridge.CharacterRom;

            cycle = 0;
            scanLine = 0;
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
                SetSpriteRamAddress(data);
                return;
            }

            if (address == 0x2004)
            {
                WriteSpriteRamData(data);
                return;
            }

            if (address == 0x2005)
            {
                return;
            }

            if (address == 0x2006)
            {
                SetVideoRamAddress(data);
                return;
            }

            if (address == 0x2007)
            {
                WriteVideoRamData(data);
                return;
            }
        }

        /// <summary>
        /// CPUからスプライトRAMを操作するために、操作対象のスプライトRAMアドレスを設定する。
        /// </summary>
        /// <param name="data"></param>
        private void SetSpriteRamAddress(byte data)
        {
            currentSpriteRamAddress = data;
        }

        /// <summary>
        /// スプライトRAMにデータを書き込む。
        /// </summary>
        /// <param name="data"></param>
        private void WriteSpriteRamData(byte data)
        {
            spriteRam.Write(currentVideoRamAddress, data);
            currentVideoRamAddress += 0x01;
        }

        /// <summary>
        /// CPUからVRAMを操作するために、操作対象のVRAMアドレスを設定する。
        /// NESの6502の仕様では、0x2006に二回書き込むことによってVRAMのアドレスを指定する。
        /// 最初がアドレスの上位バイトで、2回目がアドレスの下位バイドとなる。
        /// </summary>
        /// <param name="data"></param>
        private void SetVideoRamAddress(byte data)
        {
            if (isAlreadySetUpperVideoRamAddress)
            {
                currentVideoRamAddress += (Address)data;
                isAlreadySetUpperVideoRamAddress = false;
                isCompleteVideoRamAddress = true;
                return;
            }

            currentVideoRamAddress = (Address)(data << 8);
            isAlreadySetUpperVideoRamAddress = true;
            isCompleteVideoRamAddress = false;
        }

        /// <summary>
        /// PPUが持つVRAMへデータを書き込む。
        /// </summary>
        /// <param name="data"></param>
        private void WriteVideoRamData(byte data)
        {
            if (currentVideoRamAddress >= 0x2000)
            {
                if (0x3f00 <= currentVideoRamAddress && currentVideoRamAddress < 0x4000)
                {
                    // パレットテーブルへの書き込み
                    var address = (Address)(currentVideoRamAddress & 0x1F);
                    paletteRam.Write(address, data);
                }
                else
                {
                    // ネームテーブル、属性テーブルへの書き込み
                    // 0x3000 - 0x3EFF間のミラーリングを考慮
                    var address = 0x3000 <= currentVideoRamAddress && currentVideoRamAddress < 0x3F00
                       ? (Address)(currentVideoRamAddress - 0x3000)
                       : (Address)(currentVideoRamAddress - 0x2000);
                    videoRam.Write(address, data);
                }
            }
            else
            {
                // TODO キャラクタRAMへの書き込み
            }
            currentVideoRamAddress += controlRegister.AddressIncrement;
        }
    }
}
