namespace Atari2600.Emulator.Tia;

using Memory;

internal class Tia(IGraphicsBackend graphics) : MemoryBase {
    private const ushort COLUMBK = 0x09;
    private const ushort COLUMP0 = 0x06;
    private const ushort COLUMP1 = 0x07;
    private const ushort COLUMPF = 0x08;
    private const ushort WSYNC = 0x02;

    private AtariColor ColorBackground = new(0);
    private AtariColor ColorPlayerMissile0 = new(0);
    private AtariColor ColorPlayerMissile1 = new(0);
    private AtariColor ColorPlayfieldBall = new(0);

    private int ElectronBeamX;
    private int ElectronBeamY;

    private int Playfield;

    public bool IsWaitingForSync { get; private set; }

    public override byte ReadByte(ushort address) => 0;

    public void Step() {
        this.AdvanceElectronBeam();

        if (this.ElectronBeamX > 68 && this.ElectronBeamY is > 37 and <= 232)
            graphics.SetColor(this.ElectronBeamX - 69, this.ElectronBeamY - 38, this.GetColor());
    }

    public override void WriteByte(ushort address, byte b) {
        switch (address) {
            case Tia.WSYNC:
                this.IsWaitingForSync = true;
                break;
            case Tia.COLUMP0:
                this.ColorPlayerMissile0 = new AtariColor(b);
                break;
            case Tia.COLUMP1:
                this.ColorPlayerMissile1 = new AtariColor(b);
                break;
            case Tia.COLUMPF:
                this.ColorPlayfieldBall = new AtariColor(b);
                break;
            case Tia.COLUMBK:
                this.ColorBackground = new AtariColor(b);
                break;
        }
    }

    private void AdvanceElectronBeam() {
        // clock counts:
        // 228 w, first 68 are blank
        // 262 h, first 37 are blank, last 30 are blank
        this.ElectronBeamX++;
        if (this.ElectronBeamX > 228) {
            // advance to next line
            this.IsWaitingForSync = false;
            this.ElectronBeamX = 0;
            this.ElectronBeamY++;
        }

        // restart!
        if (this.ElectronBeamY > 262) this.ElectronBeamY = 0;
    }

    private AtariColor GetColor() => this.ColorBackground;
}