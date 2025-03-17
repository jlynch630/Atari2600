namespace Atari2600.Emulator.Tia;

using System.Diagnostics;
using Memory;

internal class Tia(IGraphicsBackend graphics, ITiaInput input) : MemoryBase {
    public const int HorizontalBlank = 68;
    public const int HorizontalScan = 228;
    private const int VerticalBlank = 40;
    private const int VerticalScan = 262;
    private readonly GraphicsObject Ball = new(4);

    private readonly CollisionRegistry Collisions = new();

    private readonly GraphicsObject Missile0 = new(4);
    private readonly GraphicsObject Missile1 = new(4);
    private readonly GraphicsObject Player0 = new(5);
    private readonly GraphicsObject Player1 = new(5);

    private readonly DelayedValue<byte> Playfield0 = new(8);
    private readonly DelayedValue<byte> Playfield1 = new(8);
    private readonly DelayedValue<byte> Playfield2 = new(8);

    private AtariColor ColorBackground = new(0);
    private AtariColor ColorPlayfield = new(0);

    private bool Missile0Follows;
    private bool Missile1Follows;

    private bool PlayfieldPriority;
    private bool PlayfieldScoreMode;

    private bool ReflectPlayfield;

    public bool IsWaitingForSync { get; private set; }

    internal int ElectronBeamX { get; private set; }

    internal int ElectronBeamY { get; private set; }

    private uint Playfield =>
        (uint)((this.Playfield0 >>> 4) + (this.Playfield1.Value.Reverse() << 4) + (this.Playfield2 << 12));

    public override byte ReadByte(ushort address) =>
        address >= 8 ? input.ReadInput(address - 8) : this.Collisions.ReadAddress(address);

    public void Step() {
        this.AdvanceElectronBeam();
        this.Playfield0.Step();
        this.Playfield1.Step();
        this.Playfield2.Step();

        const bool WithOverscan = false;
        if (WithOverscan) {
            graphics.SetColor(this.ElectronBeamX,
                this.ElectronBeamY, this.GetColor());
        }

        if (this.ElectronBeamX > Tia.HorizontalBlank && this.ElectronBeamY is > Tia.VerticalBlank and <= 232) {
            graphics.SetColor(this.ElectronBeamX - (Tia.HorizontalBlank + 1),
                this.ElectronBeamY - (Tia.VerticalBlank + 1), this.GetColor());
        }

        this.CountGraphicsObjects();
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
                this.Playfield0.Value = b;
                break;
            case Tia.PF1:
                this.Playfield1.Value = b;
                break;
            case Tia.PF2:
                this.Playfield2.Value = b;
                break;
            case Tia.RESBL:
                this.Ball.Reset(this.ElectronBeamX);
                break;
            case Tia.RESM0:
                this.Missile0.Reset(this.ElectronBeamX);
                break;
            case Tia.RESM1:
                this.Missile1.Reset(this.ElectronBeamX);
                break;
            case Tia.RESP0:
                this.Player0.Reset(this.ElectronBeamX);
                break;
            case Tia.RESP1:
                this.Player1.Reset(this.ElectronBeamX);
                break;
            case Tia.CTRLPF:
                this.Ball.GraphicsPattern =
                    GraphicsObject.GetGraphicsPattern((int)Math.Pow(2, (1 + (b >> 4)) & 0x3));
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
                this.Missile0.GraphicsPattern =
                    GraphicsObject.GetGraphicsPattern((int)Math.Pow(2, (1 + (b >> 4)) & 0x3));
                (this.Player0.Copies, this.Player0.Spacing) = GraphicsObject.DecodeCopiesAndSpacing(b);
                break;
            case Tia.NUSIZ1:
                this.Missile1.GraphicsPattern =
                    GraphicsObject.GetGraphicsPattern((int)Math.Pow(2, (1 + (b >> 4)) & 0x3));
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
                this.Player0.Enabled = b != 0;
                this.Player0.GraphicsPattern = b;
                break;
            case Tia.GRP1:
                this.Player1.Enabled = b != 0;
                this.Player1.GraphicsPattern = b;
                break;
            case Tia.RESMP0:
                this.Missile0Follows = (b & 0b10) == 0b10;
                break;
            case Tia.RESMP1:
                this.Missile1Follows = (b & 0b10) == 0b10;
                break;
            case Tia.CXCLR:
                this.Collisions.ResetCollisions();
                break;
            case Tia.VBLANK:
                // ignored
                break;
            default:
                if (b != 0)
                    Debug.WriteLine("Woah! Writing to unsupported register: {0}", address);
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
        if (this.ElectronBeamX < Tia.HorizontalBlank) return;
        this.Missile0.Step();
        this.Missile1.Step();
        this.Player0.Step();
        this.Player1.Step();
        this.Ball.Step();

        if (this.Missile0Follows) this.Missile0.HorizontalDecode = this.Player0.HorizontalDecode;
        if (this.Missile1Follows) this.Missile1.HorizontalDecode = this.Player1.HorizontalDecode;
    }

    private AtariColor GetColor() {
        AtariColor? BallColor = this.Ball.GetDrawColor();
        AtariColor? Player0Color = this.Player0.GetDrawColor();
        AtariColor? Player1Color = this.Player1.GetDrawColor();
        AtariColor? Missile0Color = this.Missile0.GetDrawColor();
        AtariColor? Missile1Color = this.Missile1.GetDrawColor();
        AtariColor? PlayfieldColor = this.GetPlayfieldColor();
        this.Collisions.DetermineCollisions(BallColor != null, Player0Color != null, Player1Color != null,
            Missile0Color != null,
            Missile1Color != null, PlayfieldColor != null);
        AtariColor? P0M0Color = Player0Color ?? Missile0Color;
        AtariColor? P1M1Color = Player1Color ?? Missile1Color;
        AtariColor? PfBallColor = PlayfieldColor ?? BallColor;

        if (this.PlayfieldPriority) return PfBallColor ?? P0M0Color ?? P1M1Color ?? this.ColorBackground;
        return P0M0Color ?? P1M1Color ?? PfBallColor ?? this.ColorBackground;
    }

    private AtariColor? GetPlayfieldColor() {
        // playfield is 20 bits and covers half the screen.
        // so:
        int PlayfieldPixels = (Tia.HorizontalScan - Tia.HorizontalBlank) / 2;
        int PlayfieldPixelSize = PlayfieldPixels / 20; // = 4
        int XOffset = this.ElectronBeamX - Tia.HorizontalBlank;
        int PlayfieldIndex = XOffset % PlayfieldPixels / PlayfieldPixelSize;

        // reverse if needed
        if (this.ReflectPlayfield && XOffset >= PlayfieldPixels) PlayfieldIndex = 19 - PlayfieldIndex;
        int Target = 1 << PlayfieldIndex;
        if ((this.Playfield & Target) != Target) return null;

        return (this.PlayfieldScoreMode
            ? this.ElectronBeamX - Tia.HorizontalBlank > 80 ? this.Player1.Color : this.Player0.Color
            : this.ColorPlayfield) ?? new AtariColor(0);
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
    private const ushort VBLANK = 0x1;
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

    private const ushort RESMP0 = 0x28;
    private const ushort RESMP1 = 0x29;

    private const ushort CXCLR = 0x2c;

    private const ushort INPT0 = 0x8;
    private const ushort INPT1 = 0x9;
    private const ushort INPT2 = 0xa;
    private const ushort INPT3 = 0xb;
    private const ushort INPT4 = 0xc;
    private const ushort INPT5 = 0xd;

    #endregion
}