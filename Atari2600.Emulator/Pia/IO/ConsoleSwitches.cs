namespace Atari2600.Emulator.Pia.IO;

internal class ConsoleSwitches : IIOPort {
    public bool GameReset { get; set; }

    public bool GameSelect { get; set; }

    public bool PlayerOneProDifficulty { get; set; }

    public bool PlayerTwoProDifficulty { get; set; }

    public bool UseColor { get; set; }

    public void ConfigureInputOutput(byte mask) {
        throw new NotSupportedException();
    }

    public byte Read() => ByteExtensions.BuildByte(!this.GameReset, !this.GameSelect, false, this.UseColor, false,
        false, this.PlayerOneProDifficulty, this.PlayerTwoProDifficulty);

    public void Write(byte value) {
        throw new NotSupportedException();
    }
}