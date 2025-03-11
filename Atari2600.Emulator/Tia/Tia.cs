namespace Atari2600.Emulator.Tia;

using Memory;

internal class Tia(IGraphicsBackend graphics) : MemoryBase {
    public const int HorizontalBlank = 68;
    public const int HorizontalScan = 228;
    private const int VerticalBlank = 40;
    private const int VerticalScan = 262;
    private readonly GraphicsObject Ball = new();

    private readonly GraphicsObject Missile0 = new();
    private readonly GraphicsObject Missile1 = new();
    private readonly GraphicsObject Player0 = new();
    private readonly GraphicsObject Player1 = new();

    private AtariColor ColorBackground = new(0);
    private AtariColor ColorPlayfield = new(0);

    private int ElectronBeamX;
    private int ElectronBeamY;

    private uint Playfield;
    private bool PlayfieldPriority;
    private bool PlayfieldScoreMode;

    private bool ReflectPlayfield;

    public bool IsWaitingForSync { get; private set; }

    public override byte ReadByte(ushort address) => 0;

    public void Step() {
        this.CountGraphicsObjects();
        this.AdvanceElectronBeam();

        if (this.ElectronBeamX > Tia.HorizontalBlank && this.ElectronBeamY is > Tia.VerticalBlank and <= 232) {
            graphics.SetColor(this.ElectronBeamX - (Tia.HorizontalBlank + 1),
                this.ElectronBeamY - (Tia.VerticalBlank + 1), this.GetColor());
        }
    }

    public override void WriteByte(ushort address, byte b) {
        switch (address) {
            case Tia.WSYNC:
                this.IsWaitingForSync = true;
                break;
            case Tia.VSYNC:
                if ((b & 0b10) == 0b10)
                    this.ElectronBeamY = 0;
                break;
            case Tia.COLUMP0:
                AtariColor ColorP0 = new(b);
                this.Missile0.Color = ColorP0;
                this.Player0.Color = ColorP0;
                break;
            case Tia.COLUMP1:
                AtariColor ColorP1 = new(b);
                this.Missile1.Color = ColorP1;
                this.Player1.Color = ColorP1;
                break;
            case Tia.COLUMPF:
                AtariColor ColorPf = new(b);
                this.Ball.Color = ColorPf;
                this.ColorPlayfield = ColorPf;
                break;
            case Tia.COLUMBK:
                this.ColorBackground = new AtariColor(b);
                break;
            case Tia.PF0:
                this.Playfield = (uint)((b >>> 4) + (this.Playfield & ~0xf));
                break;
            case Tia.PF1:
                this.Playfield = (uint)((b.Reverse() << 4) + (this.Playfield & ~0xff0));
                break;
            case Tia.PF2:
                this.Playfield = (uint)((b << 12) + (this.Playfield & ~0xff000));
                break;
            case Tia.RESBL:
                this.Ball.HorizontalDecode = 0;
                break;
            case Tia.RESM0:
                this.Missile0.HorizontalDecode = 0;
                break;
            case Tia.RESM1:
                this.Missile1.HorizontalDecode = 0;
                break;
            case Tia.RESP0:
                this.Player0.HorizontalDecode = 0;
                break;
            case Tia.RESP1:
                this.Player1.HorizontalDecode = 0;
                break;
            case Tia.CTRLPF:
                this.Ball.GraphicsPattern = GraphicsObject.GetGraphicsPattern((b >> 4) & 0x3);
                ByteExtensions.SetFromByte(b, out this.ReflectPlayfield, out this.PlayfieldScoreMode,
                    out this.PlayfieldPriority, out _, out _, out _, out _, out _);
                break;
            case Tia.HMP0:
                this.Player0.SetSpeedFromByte(b);
                break;
            case Tia.HMP1:
                this.Player1.SetSpeedFromByte(b);
                break;
            case Tia.HMM0:
                this.Missile0.SetSpeedFromByte(b);
                break;
            case Tia.HMM1:
                this.Missile1.SetSpeedFromByte(b);
                break;
            case Tia.HMBL:
                this.Ball.SetSpeedFromByte(b);
                break;
            case Tia.HMCLR:
                this.Player0.Speed = 0;
                this.Player1.Speed = 0;
                this.Missile0.Speed = 0;
                this.Missile1.Speed = 0;
                this.Ball.Speed = 0;
                break;
            case Tia.HMOVE:
                this.Missile0.Move();
                this.Missile1.Move();
                this.Player0.Move();
                this.Player1.Move();
                this.Ball.Move();
                break;

            case Tia.NUSIZ0:
                this.Missile0.GraphicsPattern = GraphicsObject.GetGraphicsPattern((b >> 4) & 0x3);
                (this.Player0.Copies, this.Player0.Spacing) = GraphicsObject.DecodeCopiesAndSpacing(b);
                break;
            case Tia.NUSIZ1:
                this.Missile1.GraphicsPattern = GraphicsObject.GetGraphicsPattern((b >> 4) & 0x3);
                (this.Player1.Copies, this.Player1.Spacing) = GraphicsObject.DecodeCopiesAndSpacing(b);
                break;
            case Tia.REFP0:
                this.Player0.Reflect = (b & 0b1000) == 0b1000;
                break;
            case Tia.REFP1:
                this.Player1.Reflect = (b & 0b1000) == 0b1000;
                break;
            case Tia.ENAM0:
                this.Missile0.Enabled = (b & 0b10) == 0b10;
                break;
            case Tia.ENAM1:
                this.Missile1.Enabled = (b & 0b10) == 0b10;
                break;
            case Tia.ENABL:
                this.Ball.Enabled = (b & 0b10) == 0b10;
                break;
            case Tia.GRP0:
                this.Player0.GraphicsPattern = b;
                break;
            case Tia.GRP1:
                this.Player1.GraphicsPattern = b;
                break;
        }
    }

    private void AdvanceElectronBeam() {
        // clock counts:
        // 228 w, first 68 are blank
        // 262 h, first 40 are blank, last 30 are blank
        this.ElectronBeamX++;
        if (this.ElectronBeamX >= 228) {
            // advance to next line
            this.IsWaitingForSync = false;
            this.ElectronBeamX = 0;
            this.ElectronBeamY++;
        }

        // restart!
        if (this.ElectronBeamY >= 262) this.ElectronBeamY = 0;
    }

    private void CountGraphicsObjects() {
        if (this.ElectronBeamX <= Tia.HorizontalBlank) return;
        this.Missile0.Step();
        this.Missile1.Step();
        this.Player0.Step();
        this.Player1.Step();
        this.Ball.Step();
    }

    private AtariColor GetColor() {
        if (this.GetPlayfieldBit()) {
            return (this.PlayfieldScoreMode
                ? this.ElectronBeamX - Tia.HorizontalBlank > 80 ? this.Player1.Color : this.Player0.Color
                : this.ColorPlayfield) ?? new AtariColor(0);
        }

        return this.ColorBackground;
    }

    private bool GetPlayfieldBit() {
        // playfield is 20 bits and covers half the screen.
        // so:
        int PlayfieldPixels = (Tia.HorizontalScan - Tia.HorizontalBlank) / 2;
        int PlayfieldPixelSize = PlayfieldPixels / 20; // = 4
        int XOffset = this.ElectronBeamX - Tia.HorizontalBlank;
        int PlayfieldIndex = XOffset % PlayfieldPixels / PlayfieldPixelSize;

        // reverse if needed
        if (this.ReflectPlayfield && XOffset > PlayfieldPixels) PlayfieldIndex = 19 - PlayfieldIndex;
        int Target = 1 << PlayfieldIndex;
        return (this.Playfield & Target) == Target;
    }

    #region Addresses

    private const ushort COLUMBK = 0x09;
    private const ushort COLUMP0 = 0x06;
    private const ushort COLUMP1 = 0x07;
    private const ushort COLUMPF = 0x08;

    private const ushort PF0 = 0x0D;
    private const ushort PF1 = 0x0E;
    private const ushort PF2 = 0x0F;
    private const ushort CTRLPF = 0x0A;

    private const ushort VSYNC = 0x0;
    private const ushort WSYNC = 0x02;

    private const ushort RESP0 = 0x10;
    private const ushort RESP1 = 0x11;
    private const ushort RESM0 = 0x12;
    private const ushort RESM1 = 0x13;
    private const ushort RESBL = 0x14;

    private const ushort HMP0 = 0x20;
    private const ushort HMP1 = 0x21;
    private const ushort HMM0 = 0x22;
    private const ushort HMM1 = 0x23;
    private const ushort HMBL = 0x24;

    private const ushort HMOVE = 0x2A;
    private const ushort HMCLR = 0x2B;

    private const ushort NUSIZ0 = 0x04;
    private const ushort NUSIZ1 = 0x05;
    private const ushort REFP0 = 0x0b;
    private const ushort REFP1 = 0x0c;

    private const ushort ENAM0 = 0x1d;
    private const ushort ENAM1 = 0x1e;
    private const ushort ENABL = 0x1f;

    private const ushort GRP0 = 0x1b;
    private const ushort GRP1 = 0x1c;

    #endregion
}