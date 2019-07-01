using System;
using System.Collections.Generic;
using System.Linq;
using Address = System.UInt16;

namespace SharpNES.SharedCode
{
    public partial class PPU
    {
        private int cycle;
        private int scanLine;
        private List<Tile> background = new List<Tile>();
        private uint[] bitmapdata = new uint[256 * 240];

        private readonly uint[] colors =
        {
            0x7C7C7C, 0x0000FC, 0x0000BC, 0x4428BC, 0x940084, 0xA80020, 0xA81000, 0x881400,
            0x503000, 0x007800, 0x006800, 0x005800, 0x004058, 0x000000, 0x000000, 0x000000,
            0xBCBCBC, 0x0078F8, 0x0058F8, 0x6844FC, 0xD800CC, 0xE40058, 0xF83800, 0xE45C10,
            0xAC7C00, 0x00B800, 0x00A800, 0x00A844, 0x008888, 0x000000, 0x000000, 0x000000,
            0xF8F8F8, 0x3CBCFC, 0x6888FC, 0x9878F8, 0xF878F8, 0xF85898, 0xF87858, 0xFCA044,
            0xF8B800, 0xB8F818, 0x58D854, 0x58F898, 0x00E8D8, 0x787878, 0x000000, 0x000000,
            0xFCFCFC, 0xA4E4FC, 0xB8B8F8, 0xD8B8F8, 0xF8B8F8, 0xF8A4C0, 0xF0D0B0, 0xFCE0A8,
            0xF8D878, 0xD8F878, 0xB8F8B8, 0xB8F8D8, 0x00FCFC, 0xF8D8F8, 0x000000, 0x000000
        };

        private class Sprite
        {
            public int[,] Data { get; }

            public Sprite(int[,] data)
            {
                Data = data;
            }
        }

        private class Tile
        {
            public Sprite Sprite { get; set; }
            public int PaletteId { get; set; }
            public int ScrollX { get; set; }
            public int ScrollY { get; set; }
        }

        public void Run(int cycle)
        {
            this.cycle += cycle;

            if (scanLine == 0)
            {
                background.Clear();
                // TODO スプライトのセットアップ
            }

            // 341クロック経過したら1ライン描画する
            if (this.cycle >= 341)
            {
                this.cycle -= 314;
                scanLine++;

                if (scanLine <= 240 && scanLine % 8 == 0)
                {
                    BuildBackground();
                }

                if (scanLine == 262)
                {
                    // TODO VBlank
                    scanLine = 0;
                    BuildBitMap();
                    // TODO イベント？
                }
            }
        }

        private void BuildBitMap()
        {
            RenderBackground();
        }

        private void RenderBackground()
        {
            for (var index = 0; index < background.Count; index++)
            {
                //  0 0
                //  1 8
                //  2 16
                //  3 24
                //  4 32
                //  5 40
                //  6 48
                //  7 56
                //  8 64
                //  9 72
                // 10 80
                // 11 88
                // 12 96
                // 13 104
                // 14 112
                // 15 120
                // 16 128
                // 17 136
                // 18 144
                // 19 152
                // 20 160
                // 21 168
                // 22 176
                // 23 184
                // 24 192
                // 25 200
                // 26 208
                // 27 216
                // 28 224
                // 29 232
                // 30 240
                // 31 248
                // 32 256

                var x = (index % 33) * 8;
                var y = (index / 33) * 8;
                RenderTile(background[index], x, y);
            }
        }

        private void RenderTile(Tile tile, int tileX, int tileY)
        {
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    // PaletteId 0-3
                    // PaletteIndex 0 - 15
                    var paletteIndex = (Address) (tile.PaletteId * 4 + tile.Sprite.Data[i, j]);
                    // パレットテーブルにアクセスしてカラーIDを取得する
                    var paletteAddress = GetPaletteRamAddress(paletteIndex);
                    var colorId = paletteRam.Read(paletteAddress);
                    var color = colors[colorId];
                    var x = tileX + j;
                    var y = tileY + i;
                    if (0 <= x && x < 256 && 0 <= y && y < 240)
                    {
                        var index = (x + y * 256);
                        bitmapdata[index] = color;
                    }
                }
            }
        }

        private Address GetPaletteRamAddress(Address address)
        {
            // 04 08 0C は 00 のミラー
            if (address == 0x04 || address == 0x08 || address == 0x0C)
            {
                return (Address) 0x00;
            }

            // 10 14 18 1C は 00 04 08 0Cのミラー
            if (address == 0x10 || address == 0x14 || address == 0x18 || address == 0x1C)
            {
                return (Address) (address - 0x10);
            }

            return (Address) address;
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
                var tile = BuildTile(clampedTileX, clampedTileY, offsetAddressByNameTable);
                background.Add(tile);
            }
        }

        /// <summary>
        /// スプライト、パレットIDを作ることによってタイルを組み立てる
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private Tile BuildTile(int tileX, int tileY, int offset)
        {
            // タイルが0-3のどのブロックに所属するか
            var blockId = GetBlockId(tileX, tileY);
            // タイルに設定されているスプライト番号を取得
            var spriteId = GetSpriteId(tileX, tileY, offset);
            // タイルにひもづくパレットを取得するため、属性テーブルにアクセス
            var attribute = GetAttribute(tileX, tileY, offset);
            // 1ブロック(2x2タイル)は左上→右上→左下→右下の順でIDが振られている
            // 属性は1バイトで1ブロック中のタイルがどのパレットを使うかブロック単位で指定する
            // バイトの内訳としては0b(右下)(左下)(右上)(左上)
            // なので属性をブロックIDx2だけビットシフトして0b11でマスクする
            var paletteId = (attribute >> (blockId * 2)) & 0x03;
            // スプライトを組み立て
            var sprite = BuildSprite(spriteId, maskRegister.BackgroundPatternTableAddress);
            return new Tile
            {
                Sprite = sprite,
                PaletteId = paletteId,
                ScrollX = 0,
                ScrollY = 0
            };
        }

        /// <summary>
        /// キャラクタROMにアクセスしてスプライトデータを取得し、スプライトデータを組み立てる
        /// </summary>
        /// <param name="spriteId"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private Sprite BuildSprite(int spriteId, Address offset)
        {
            var spriteData = new int[8, 8];
            for (var y = 0; y < 16; y++)
            {
                var address = (Address) (spriteId * 16 + y + offset);
                var data = characterROM.Read(address);
                for (var x = 0; x < 8; x++)
                {
                    if ((data & (0x80 >> x)) == 1)
                    {
                        spriteData[y % 8, x] += (0x01 << (y / 8));
                    }
                }
            }

            return new Sprite(spriteData);
        }

        /// <summary>
        /// 指定しされたタイル座標において、自分がどのブロックに所属するタイルなのかを調べる。
        /// なお、ひとかたまりは4x4タイル
        /// +--------+--------+
        /// |        |        |
        /// |    0   |    1   |
        /// |        |        |
        /// +--------+--------+
        /// |        |        |
        /// |    2   |    3   |
        /// |        |        |
        /// +--------+--------+
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <returns></returns>
        private int GetBlockId(int tileX, int tileY)
        {
            return (tileX % 4 / 2) + (tileY % 4 / 2 * 2);
        }

        /// <summary>
        /// ネームテーブルにアクセスして、タイルに紐づくスプライト番号を取得する
        /// ネームテーブルは0x0000 - 0x03BF (3CO = 960 = 32x30)
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private int GetSpriteId(int tileX, int tileY, int offset)
        {
            var tileNumber = tileY * 32 + tileX;
            return videoRam.Read((Address) tileNumber);
        }

        /// <summary>
        /// 属性テーブルにアクセスして、4ブロック分のパレット設定値を取得する
        /// 属性テーブルは0x03C0 - 0x3FF (0x40 = 64Byte)
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private int GetAttribute(int tileX, int tileY, int offset)
        {
            var address = (tileX / 4) + (tileY / 4 * 8) + 0x03C0 + offset;
            return videoRam.Read((Address) address);
        }
    }
}