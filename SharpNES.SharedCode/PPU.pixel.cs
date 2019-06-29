using Address = System.UInt16;

namespace SharpNES.SharedCode
{
    public partial class PPU
    {
        private int cycle;
        private int scanLine;
        public void Run(int cycle)
        {
            this.cycle += cycle;

            if (scanLine == 0)
            {
                // TODO スプライトのセットアップ
            }

            // 341クロック経過したら1ライン描画する
            if (this.cycle >= 341)
            {
                this.cycle -= 314;
                scanLine++;

                if (this.scanLine <= 240 && this.scanLine % 8 == 0)
                {
                }
            }
        }

        private void BuildBackground()
        {
            // TODO スクロール
            var tileY = scanLine / 8;
            var clampedTileY = tileY % 30;
            // スクロール中の場合などは30をオーバーする→次のネームテーブル
            var tableOffset = ((tileY / 30) % 2) == 1 ? 2 : 0;

            for (var x = 0; x <= 32; x++)
            {
                // TODO スクロール
                var tileX = x;
                var clampedTileX = x % 32;
                // スクロール中の場合まどは32をオーバーする→次のネームテーブル
                var nameTableId = (tileX / 32) % 2 + tableOffset;
                var offsetAddressByNameTable = nameTableId * 0x400;
                BuildTile(clampedTileX, clampedTileY, offsetAddressByNameTable);
            }

        }

        private void BuildTile(int tileX, int tileY, int offset)
        {
            // タイルがどのブロックに所属するか
            var blockId = GetBlockId(tileX, tileY);
            var spriteId = GetSpriteId(tileX, tileY, offset);
            var attribute = GetAttribute(tileX, tileY, offset);
            // 1ブロック(2x2タイル)は左上→右上→左下→右下の順でIDが振られている
            // 属性は1バイトで1ブロック中のタイルがどのパレットを使うかブロック単位で指定する
            // バイトの内訳としては0b(右下)(左下)(右上)(左上)
            // なので属性をブロックIDx2だけビットシフトして0b11でマスクする
            var paletteId = (attribute >> (blockId * 2)) & 0x03;
        }

        private int GetBlockId(int tileX, int tileY)
        {
            return (tileX % 4 / 2) + (tileY % 4 / 2 * 2);
        }

        private int GetSpriteId(int tileX, int tileY, int offset)
        {
            var tileNumber = tileY * 32 + tileX;
            return this.videoRam.Read((Address)tileNumber);
        }

        private int GetAttribute(int tileX, int tileY, int offset)
        {
            var address = (tileX / 4) + (tileY / 4 * 8) + 0x03C0 + offset;
            return this.videoRam.Read((Address)address);
        }

    }
}
