namespace Atari2600.Emulator.Tia;

internal class GraphicsObject(int processingTime) {
    private const int MaxDecodeValue = Tia.HorizontalScan - Tia.HorizontalBlank;

    public AtariColor? Color { get; set; }

    public int Copies { get; set; } = 1;

    public bool Enabled { get; set; }

    /// <summary>
    ///     The pattern to display when enabled, e.g. 0b10110001 makes the blocks "X XX   X" on the screen (unless reflected)
    /// </summary>
    public byte GraphicsPattern { get; set; }

    public int HorizontalDecode { get; set; }

    public bool Reflect { get; set; }

    public int Spacing { get; set; }

    public int Speed { get; set; }

    public static (int Copies, int Spacing) DecodeCopiesAndSpacing(byte value) {
        int Option = value & 0b111;

        return Option switch {
            0 => (1, 0),
            1 => (2, 1),
            2 => (2, 2),
            3 => (3, 1),
            4 => (2, 7),
            5 => (2, 0),
            6 => (3, 3),
            7 => (4, 0),
            _ => (0, 0)
        };
    }

    public static byte GetGraphicsPattern(int width) => (byte)(Math.Pow(2, width) - 1);

    public AtariColor? GetDrawColor() {
        if (!this.Enabled) return null;

        if (this.Spacing == 0) return this.GetColorAtIndex(this.HorizontalDecode / this.Copies);

        for (int i = 0, j = 0; i < this.Copies; i++, j += 8 + this.Spacing * 8) {
            int Offset = this.HorizontalDecode - j;
            if (Offset >= 8) continue;

            return this.GetColorAtIndex(Offset);
        }

        return null;
    }

    public void Move() {
        this.HorizontalDecode += this.Speed;

        if (this.HorizontalDecode >= GraphicsObject.MaxDecodeValue)
            this.HorizontalDecode -= GraphicsObject.MaxDecodeValue;
        if (this.HorizontalDecode < 0) this.HorizontalDecode += GraphicsObject.MaxDecodeValue;
    }

    public void Reset(int xPos) {
        // strobing reset usually needs some processing time, we can just introduce that
        // by decoding earlier
        this.HorizontalDecode = GraphicsObject.MaxDecodeValue - (processingTime + (this.Copies > 1 ? 1 : 0));
    }

    public void SetSpeedFromByte(byte value) {
        this.Speed = (sbyte)value >> 4;
    }

    public void Step() {
        this.HorizontalDecode++;
        if (this.HorizontalDecode >= GraphicsObject.MaxDecodeValue)
            this.HorizontalDecode -= GraphicsObject.MaxDecodeValue;
    }

    private AtariColor? GetColorAtIndex(int index) {
        if (index >= 8) return null;
        byte Pattern = this.Reflect ? this.GraphicsPattern.Reverse() : this.GraphicsPattern;

        // we're within our bounds!
        int Shifted = 1 << (7 - index);
        return (Pattern & Shifted) == Shifted ? this.Color : null;
    }
}