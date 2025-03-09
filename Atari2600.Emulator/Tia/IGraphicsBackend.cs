namespace Atari2600.Emulator.Tia;

public interface IGraphicsBackend {
    public void SetColor(int x, int y, AtariColor color);
}