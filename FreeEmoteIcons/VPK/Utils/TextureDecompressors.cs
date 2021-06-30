﻿// <copyright file="TextureDecompressors.cs" company="Ensage">
//    Copyright (c) 2019 Ensage.
// </copyright>

namespace FreeEmoteIcons.VPK.Utils
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;

    internal static class TextureDecompressors
    {
        public static Bitmap ReadBGRA8888(BinaryReader r, int w, int h)
        {
            var res = new Bitmap(w, h);
            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    var rawColor = r.ReadInt32();

                    var color = Color.FromArgb((rawColor >> 24) & 0x0FF, (rawColor >> 16) & 0x0FF, (rawColor >> 8) & 0x0FF, rawColor & 0x0FF);

                    res.SetPixel(x, y, color);
                }
            }

            return res;
        }

        public static Bitmap ReadRGBA16161616F(BinaryReader r, int w, int h)
        {
            var res = new Bitmap(w, h);

            while (h-- > 0)
            {
                while (w-- > 0)
                {
                    var red = (int)r.ReadDouble();
                    var green = (int)r.ReadDouble();
                    var blue = (int)r.ReadDouble();
                    var alpha = (int)r.ReadDouble();

                    res.SetPixel(w, h, Color.FromArgb(alpha, red, green, blue));
                }
            }

            return res;
        }

        public static Bitmap ReadRGBA8888(BinaryReader r, int w, int h)
        {
            var res = new Bitmap(w, h);
            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    var rawColor = r.ReadInt32();

                    var color = Color.FromArgb((rawColor >> 24) & 0x0FF, rawColor & 0x0FF, (rawColor >> 8) & 0x0FF, (rawColor >> 16) & 0x0FF);

                    res.SetPixel(x, y, color);
                }
            }

            return res;
        }

        public static Bitmap UncompressDXT1(BinaryReader r, int w, int h)
        {
            var rect = new Rectangle(0, 0, w, h);
            var res = new Bitmap(w, h, PixelFormat.Format32bppRgb);

            var blockCountX = (w + 3) / 4;
            var blockCountY = (h + 3) / 4;

            var lockBits = res.LockBits(rect, ImageLockMode.WriteOnly, res.PixelFormat);

            var data = new byte[lockBits.Stride * lockBits.Height];

            for (var j = 0; j < blockCountY; j++)
            {
                for (var i = 0; i < blockCountX; i++)
                {
                    var blockStorage = r.ReadBytes(8);
                    DecompressBlockDXT1(i * 4, j * 4, w, blockStorage, ref data, lockBits.Stride);
                }
            }

            Marshal.Copy(data, 0, lockBits.Scan0, data.Length);
            res.UnlockBits(lockBits);

            return res;
        }

        public static Bitmap UncompressDXT5(BinaryReader r, int w, int h, int w2, bool yCoCg)
        {
            var rect = new Rectangle(0, 0, w2, h);
            var res = new Bitmap(w2, h, PixelFormat.Format32bppArgb);

            var blockCountX = w / 4;
            var blockCountY = h / 4;

            var lockBits = res.LockBits(rect, ImageLockMode.WriteOnly, res.PixelFormat);

            var data = new byte[lockBits.Stride * lockBits.Height];

            for (var j = 0; j < blockCountY; j++)
            {
                for (var i = 0; i < blockCountX; i++)
                {
                    var blockStorage = r.ReadBytes(16);
                    DecompressBlockDXT5(i * 4, j * 4, w2, blockStorage, ref data, lockBits.Stride, yCoCg);
                }
            }

            Marshal.Copy(data, 0, lockBits.Scan0, data.Length);
            res.UnlockBits(lockBits);

            return res;
        }

        private static byte ClampColor(int a)
        {
            if (a > 255)
            {
                return 255;
            }

            return a < 0 ? (byte)0 : (byte)a;
        }

        private static void DecompressBlockDXT1(int x, int y, int width, byte[] blockStorage, ref byte[] pixels, int stride)
        {
            var color0 = (ushort)(blockStorage[0] | (blockStorage[1] << 8));
            var color1 = (ushort)(blockStorage[2] | (blockStorage[3] << 8));

            int temp;

            temp = ((color0 >> 11) * 255) + 16;
            var r0 = (byte)(((temp / 32) + temp) / 32);
            temp = (((color0 & 0x07E0) >> 5) * 255) + 32;
            var g0 = (byte)(((temp / 64) + temp) / 64);
            temp = ((color0 & 0x001F) * 255) + 16;
            var b0 = (byte)(((temp / 32) + temp) / 32);

            temp = ((color1 >> 11) * 255) + 16;
            var r1 = (byte)(((temp / 32) + temp) / 32);
            temp = (((color1 & 0x07E0) >> 5) * 255) + 32;
            var g1 = (byte)(((temp / 64) + temp) / 64);
            temp = ((color1 & 0x001F) * 255) + 16;
            var b1 = (byte)(((temp / 32) + temp) / 32);

            uint c1 = blockStorage[4];
            var c2 = (uint)blockStorage[5] << 8;
            var c3 = (uint)blockStorage[6] << 16;
            var c4 = (uint)blockStorage[7] << 24;
            var code = c1 | c2 | c3 | c4;

            for (var j = 0; j < 4; j++)
            {
                for (var i = 0; i < 4; i++)
                {
                    var positionCode = (byte)((code >> (2 * ((4 * j) + i))) & 0x03);

                    byte finalR = 0, finalG = 0, finalB = 0;

                    switch (positionCode)
                    {
                        case 0:
                            finalR = r0;
                            finalG = g0;
                            finalB = b0;
                            break;
                        case 1:
                            finalR = r1;
                            finalG = g1;
                            finalB = b1;
                            break;
                        case 2:
                            if (color0 > color1)
                            {
                                finalR = (byte)(((2 * r0) + r1) / 3);
                                finalG = (byte)(((2 * g0) + g1) / 3);
                                finalB = (byte)(((2 * b0) + b1) / 3);
                            }
                            else
                            {
                                finalR = (byte)((r0 + r1) / 2);
                                finalG = (byte)((g0 + g1) / 2);
                                finalB = (byte)((b0 + b1) / 2);
                            }

                            break;
                        case 3:
                            if (color0 < color1)
                            {
                                break;
                            }

                            finalR = (byte)(((2 * r1) + r0) / 3);
                            finalG = (byte)(((2 * g1) + g0) / 3);
                            finalB = (byte)(((2 * b1) + b0) / 3);
                            break;
                    }

                    if ((x + i) < width)
                    {
                        var pixelIndex = ((y + j) * stride) + ((x + i) * 4);
                        pixels[pixelIndex] = finalB;
                        pixels[pixelIndex + 1] = finalG;
                        pixels[pixelIndex + 2] = finalR;
                    }
                }
            }
        }

        private static void DecompressBlockDXT5(int x, int y, int width, byte[] blockStorage, ref byte[] pixels, int stride, bool yCoCg)
        {
            var alpha0 = blockStorage[0];
            var alpha1 = blockStorage[1];

            uint a1 = blockStorage[4];
            var a2 = (uint)blockStorage[5] << 8;
            var a3 = (uint)blockStorage[6] << 16;
            var a4 = (uint)blockStorage[7] << 24;
            var alphaCode1 = a1 | a2 | a3 | a4;

            var alphaCode2 = (ushort)(blockStorage[2] | (blockStorage[3] << 8));

            var color0 = (ushort)(blockStorage[8] | (blockStorage[9] << 8));
            var color1 = (ushort)(blockStorage[10] | (blockStorage[11] << 8));

            int temp;

            temp = ((color0 >> 11) * 255) + 16;
            var r0 = (byte)(((temp / 32) + temp) / 32);
            temp = (((color0 & 0x07E0) >> 5) * 255) + 32;
            var g0 = (byte)(((temp / 64) + temp) / 64);
            temp = ((color0 & 0x001F) * 255) + 16;
            var b0 = (byte)(((temp / 32) + temp) / 32);

            temp = ((color1 >> 11) * 255) + 16;
            var r1 = (byte)(((temp / 32) + temp) / 32);
            temp = (((color1 & 0x07E0) >> 5) * 255) + 32;
            var g1 = (byte)(((temp / 64) + temp) / 64);
            temp = ((color1 & 0x001F) * 255) + 16;
            var b1 = (byte)(((temp / 32) + temp) / 32);

            uint c1 = blockStorage[12];
            var c2 = (uint)blockStorage[13] << 8;
            var c3 = (uint)blockStorage[14] << 16;
            var c4 = (uint)blockStorage[15] << 24;
            var code = c1 | c2 | c3 | c4;

            for (var j = 0; j < 4; j++)
            {
                for (var i = 0; i < 4; i++)
                {
                    var alphaCodeIndex = 3 * ((4 * j) + i);
                    int alphaCode;

                    if (alphaCodeIndex <= 12)
                    {
                        alphaCode = (alphaCode2 >> alphaCodeIndex) & 0x07;
                    }
                    else if (alphaCodeIndex == 15)
                    {
                        alphaCode = (int)(((uint)alphaCode2 >> 15) | ((alphaCode1 << 1) & 0x06));
                    }
                    else
                    {
                        alphaCode = (int)((alphaCode1 >> (alphaCodeIndex - 16)) & 0x07);
                    }

                    byte finalAlpha;
                    if (alphaCode == 0)
                    {
                        finalAlpha = alpha0;
                    }
                    else if (alphaCode == 1)
                    {
                        finalAlpha = alpha1;
                    }
                    else
                    {
                        if (alpha0 > alpha1)
                        {
                            finalAlpha = (byte)((((8 - alphaCode) * alpha0) + ((alphaCode - 1) * alpha1)) / 7);
                        }
                        else
                        {
                            if (alphaCode == 6)
                            {
                                finalAlpha = 0;
                            }
                            else if (alphaCode == 7)
                            {
                                finalAlpha = 255;
                            }
                            else
                            {
                                finalAlpha = (byte)((((6 - alphaCode) * alpha0) + ((alphaCode - 1) * alpha1)) / 5);
                            }
                        }
                    }

                    var colorCode = (byte)((code >> (2 * ((4 * j) + i))) & 0x03);

                    byte finalR = 0, finalG = 0, finalB = 0;

                    switch (colorCode)
                    {
                        case 0:
                            finalR = r0;
                            finalG = g0;
                            finalB = b0;
                            break;
                        case 1:
                            finalR = r1;
                            finalG = g1;
                            finalB = b1;
                            break;
                        case 2:
                            finalR = (byte)(((2 * r0) + r1) / 3);
                            finalG = (byte)(((2 * g0) + g1) / 3);
                            finalB = (byte)(((2 * b0) + b1) / 3);
                            break;
                        case 3:
                            finalR = (byte)(((2 * r1) + r0) / 3);
                            finalG = (byte)(((2 * g1) + g0) / 3);
                            finalB = (byte)(((2 * b1) + b0) / 3);
                            break;
                    }

                    if ((x + i) < width)
                    {
                        if (yCoCg)
                        {
                            var s = (finalB >> 3) + 1;
                            var co = (finalR - 128) / s;
                            var cg = (finalG - 128) / s;

                            finalR = ClampColor((finalAlpha + co) - cg);
                            finalG = ClampColor(finalAlpha + cg);
                            finalB = ClampColor(finalAlpha - co - cg);
                        }

                        var pixelIndex = ((y + j) * stride) + ((x + i) * 4);
                        pixels[pixelIndex] = finalB;
                        pixels[pixelIndex + 1] = finalG;
                        pixels[pixelIndex + 2] = finalR;
                        pixels[pixelIndex + 3] = byte.MaxValue;
                    }
                }
            }
        }
    }
}