namespace Atari2600.Emulator.Tia;

internal class GraphicsObject {
    public AtariColor? Color { get; set; }

    public int Copies { get; set; }

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

    public void Move() {
        this.HorizontalDecode += this.Speed;
    }

    public void SetSpeedFromByte(byte value) {
        this.Speed = (sbyte)value >> 4;
    }

    public AtariColor? ShouldDraw() {
        if (!this.Enabled) return null;

        if (this.Spacing == 0) return this.GetColorAtIndex(this.HorizontalDecode / this.Copies);

        for (int i = 0, j = 0; i < this.Copies; i++, j += this.Spacing * 8) {
            int Offset = this.Spacing == 0 ? this.HorizontalDecode / this.Copies : this.HorizontalDecode - j;
            if (Offset >= 8) continue;

            return this.GetColorAtIndex(Offset);
        }

        return null;
    }

    public void Step() {
        this.HorizontalDecode++;
        if (this.HorizontalDecode >= Tia.HorizontalScan - Tia.HorizontalBlank) this.HorizontalDecode = 0;
    }

    private AtariColor? GetColorAtIndex(int index) {
        if (index >= 8) return null;
        byte Pattern = this.Reflect ? this.GraphicsPattern.Reverse() : this.GraphicsPattern;

        // we're within our bounds!
        int Shifted = 1 << index;
        return (Pattern & Shifted) == Shifted ? this.Color : null;
    }
}