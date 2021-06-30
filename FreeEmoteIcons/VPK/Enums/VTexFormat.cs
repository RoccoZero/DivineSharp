﻿// <copyright file="VTexFormat.cs" company="Ensage">
//    Copyright (c) 2019 Ensage.
// </copyright>

namespace FreeEmoteIcons.VPK.Enums
{
    // ReSharper disable InconsistentNaming
    internal enum VTexFormat : byte
    {
        IMAGE_FORMAT_NONE = 0,

        IMAGE_FORMAT_DXT1 = 1,

        IMAGE_FORMAT_DXT5 = 2,

        IMAGE_FORMAT_RGBA8888 = 4,

        IMAGE_FORMAT_RGBA16161616F = 10,

        IMAGE_FORMAT_PNG = 16,

        IMAGE_FORMAT_BGRA8888 = 28
    }
}