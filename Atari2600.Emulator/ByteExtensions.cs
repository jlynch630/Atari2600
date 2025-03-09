namespace Atari2600.Emulator;

internal static class ByteExtensions {
    public static byte AddWithWraparound(this byte x, byte y) => unchecked((byte)(x + y));

    public static byte BuildByte(bool d0, bool d1, bool d2, bool d3, bool d4, bool d5, bool d6, bool d7) {
        byte Val = 0;
        Add(d0, 0);
        Add(d1, 1);
        Add(d2, 2);
        Add(d3, 3);
        Add(d4, 4);
        Add(d5, 5);
        Add(d6, 6);
        Add(d7, 7);
        return Val;

        void Add(bool value, int offset) => Val |= (byte)((value ? 1 : 0) << offset);
    }

    public static bool IsNegative(this byte b) => (b & 0x80) == 0x80;
}