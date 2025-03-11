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

    public static byte Reverse(this byte b) {
        int Result = b;
        Result = ((Result & 0b11110000) >> 4) | ((Result & 0b00001111) << 4);
        Result = ((Result & 0b11001100) >> 2) | ((Result & 0b00110011) << 2);
        Result = ((Result & 0b10101010) >> 1) | ((Result & 0b01010101) << 1);
        return (byte)Result;
    }

    public static void SetFromByte(byte b, out bool d0, out bool d1, out bool d2, out bool d3, out bool d4, out bool d5,
                                   out bool d6, out bool d7) {
        d0 = Get(0);
        d1 = Get(1);
        d2 = Get(2);
        d3 = Get(3);
        d4 = Get(4);
        d5 = Get(5);
        d6 = Get(6);
        d7 = Get(7);
        return;

        bool Get(int offset) => (b & (1 << offset)) == 1 << offset;
    }
}