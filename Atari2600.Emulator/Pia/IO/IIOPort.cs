namespace Atari2600.Emulator.Pia.IO;

public interface IIOPort {
    public void ConfigureInputOutput(byte mask);

    public byte Read();

    public void Write(byte value);
}