﻿using System;
using System.Runtime.InteropServices;
using Ultraviolet.Core;

namespace Ultraviolet.FreeType2.Native
{
#pragma warning disable 1591
    [Preserve(AllMembers = true)]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FT_Outline32
    {
        public Int16 n_contour;
        public Int16 n_points;

        public FT_Vector32* points;
        public Byte* tags;
        public UInt16* contours;

        public Int32 flags;
    }
#pragma warning restore 1591
}