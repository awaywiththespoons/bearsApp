using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Drawing
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Color : IEquatable<Color>
    {
        [FieldOffset(0)] public int Value;
        [FieldOffset(0)] public byte A;
        [FieldOffset(1)] public byte R;
        [FieldOffset(2)] public byte G;
        [FieldOffset(3)] public byte B;

        public Color(int color)
        {
            Value = 0; 
            A = 255;
            R = (byte)((color & 0x00FF0000) >> 16);
            G = (byte)((color & 0x0000FF00) >> 8);
            B = (byte)((color & 0x000000FF));
        }

        public Color(byte r, byte g, byte b)
        {
            Value = 0;
            A = 255;
            R = r;
            G = g;
            B = b;
        }

        public Color(byte a, byte r, byte g, byte b)
        {
            Value = 0;
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public override bool Equals(object obj)
        {
            if (obj is Color)
            {
                return Equals((Color)obj); 
            }

            return base.Equals(obj);
        }

        public bool Equals(Color other)
        {
            return Value == other.Value; 
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public override string ToString()
        {
            return string.Format("Red: {0}, Green: {1}, Blue: {2}, Alpha: {3}", R, G, B, A);
        }

        public static bool operator ==(Color x, Color y)
        {
            return x.Value == y.Value; 
        }

        public static bool operator !=(Color x, Color y)
        {
            return x.Value != y.Value;
        }

        public static readonly Color Black = new Color(0x000000);
        public static readonly Color DarkBlue = new Color(0x00008B);
        public static readonly Color DarkGreen = new Color(0x006400);
        public static readonly Color DarkCyan = new Color(0x008B8B);
        public static readonly Color DarkRed = new Color(0x8B0000);
        public static readonly Color DarkMagenta = new Color(0x8B008B);
        public static readonly Color DarkKhaki = new Color(0xBDB76B);
        public static readonly Color LightGray = new Color(0xD3D3D3);
        public static readonly Color Gray = new Color(0x808080);
        public static readonly Color Blue = new Color(0x0000FF);
        public static readonly Color Green = new Color(0x008000);
        public static readonly Color Cyan = new Color(0x00FFFF);
        public static readonly Color White = new Color(0xFFFFFF);
        public static readonly Color Yellow = new Color(0xFFFF00);
        public static readonly Color Magenta = new Color(0xFF00FF);
        public static readonly Color Red = new Color(0xFF0000);

        public static Color FromArgb(byte alpha, byte red, byte green, byte blue)
        {
            return new Color(alpha, red, green, blue);
        }

        public static Color FromArgb(byte r, byte g, byte b)
        {
            return new Color(r, g, b); 
        }

        public static Color FromArgb(int r, int g, int b)
        {
            return new Color((byte)r, (byte)g, (byte)b);
        }
    }
}
