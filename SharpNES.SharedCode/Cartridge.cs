using System.IO;

namespace SharpNES.SharedCode
{
    /// <summary>
    /// ファミコンソフトのカートリッジが保持しているデータを表現するクラス
    /// プログラムデータとキャラクタデータを保持している
    /// </summary>
    public class Cartridge
    {
        public byte MapperNumber { get; private set; }
        public bool IsVerticalVramMirroring { get; private set; }
        public bool HasBatteryBackedMemory { get; private set; }
        public byte ProgramRomSize { get; private set; }
        public byte CharacterRomSize { get; private set; }
        public ROM ProgramRom { get; private set; }
        public ROM CharacterRom { get; private set; }

        /// <summary>
        /// NESファイルのストリームを渡して、カートリッジのインスタンスを作成する。
        /// NESファイルのヘッダを読み込み、ヘッダ値をもとにプログラムROMとキャラクタROMをロードする
        /// </summary>
        /// <param name="nesStream"></param>
        public Cartridge(Stream nesStream)
        {
            var binaryReader = new BinaryReader(nesStream);
            ParseHeader(binaryReader);
            LoadRom(binaryReader);
        }

        private void ParseHeader(BinaryReader binaryReader)
        {
            var magicNumber = binaryReader.ReadUInt32();
            // マジックナンバーのチェック
            if (magicNumber != 0x1A53454E)
            {
                throw new InvalidDataException("This file does not match the NES file format specification.");
            }

            // プログラムROMとキャラクターROMのサイズを取得する
            ProgramRomSize = binaryReader.ReadByte();
            CharacterRomSize = binaryReader.ReadByte();

            // Flag6
            // http://wiki.nesdev.com/w/index.php/INES#Flags_6
            var flag6 = binaryReader.ReadByte();
            IsVerticalVramMirroring = (flag6 & 0x01) == 1;
            HasBatteryBackedMemory = (flag6 & 0x02) == 1;

            // Flag7
            // http://wiki.nesdev.com/w/index.php/INES#Flags_7
            var flag7 = binaryReader.ReadByte();

            MapperNumber = (byte)((byte) (flag7 & 0xF0) | (byte) (flag6 >> 4 & 0x0f));
        }

        private void LoadRom(BinaryReader binaryReader)
        {
            // NESファイルの先頭16byteがヘッダーとなるため、プログラムROMの先頭にストリームをシークする
            binaryReader.BaseStream.Seek(16, SeekOrigin.Begin);
            LoadProgramRom(binaryReader);
            LoadCharacterRom(binaryReader);
        }
        private void LoadProgramRom(BinaryReader binaryReader) 
        {
            // プログラムROMは16KB/Unit
            var programRomData = new byte[ProgramRomSize * 0x4000];
            binaryReader.Read(programRomData, 0, ProgramRomSize * 0x4000);
            ProgramRom = new ROM(programRomData);
        }

        private void LoadCharacterRom(BinaryReader binaryReader)
        {
            // キャラクタROMは8KB/Unit
            var characterRomData = new byte[CharacterRomSize * 0x2000];
            binaryReader.Read(characterRomData, 0, CharacterRomSize * 0x2000);
            CharacterRom = new ROM(characterRomData);
        }
    }
}
