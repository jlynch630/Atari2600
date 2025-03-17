namespace Atari2600.FormsTest;

using Emulator;
using Emulator.Debugging;
using Emulator.Pia.IO;
using Emulator.Tia;
using unvell.D2DLib;
using unvell.D2DLib.WinForm;

public partial class Form1 : D2DForm, IGraphicsBackend {
    private const int AtariHeight = Form1.WithOverscan ? 262 : 192; // want: 262!!! off by 23??
    private const int AtariWidth = Form1.WithOverscan ? 228 : 160; // want: 228 off by 5?
    private const int ScaleFactor = 1; //6:5 ratio
    private const bool WithOverscan = false;
    private readonly Dictionary<string, D2DBrush> BrushMap = new();
    private readonly Joystick Joystick = new();
    private readonly string[,] PaintColors = new string[Form1.AtariWidth, Form1.AtariHeight];
    private readonly int ScaleFactorX = 10 * Form1.ScaleFactor;
    private readonly int ScaleFactorY = 5 * Form1.ScaleFactor;

    public Form1() {
        this.InitializeComponent();
        this.Width = Form1.AtariWidth * this.ScaleFactorX + 22;
        this.Height = Form1.AtariHeight * this.ScaleFactorY + 83;
        this.AnimationDraw = true;
    }

    public void SetColor(int x, int y, AtariColor color) {
        this.PaintColors[x, y] = color.Hex ?? "#000000";
    }

    protected override void OnRender(D2DGraphics g) {
        for (int x = 0; x < this.PaintColors.GetLength(0); x++)
            for (int y = 0; y < this.PaintColors.GetLength(1); y++)
                g.FillRectangle(
                    new D2DRect(x * this.ScaleFactorX, y * this.ScaleFactorY, this.ScaleFactorX, this.ScaleFactorY),
                    this.BrushMap[this.PaintColors[x, y] ?? "#000000"]);
    }

    private void Form1_KeyDown(object sender, KeyEventArgs e) {
        switch (e.KeyCode) {
            case Keys.Left:
                this.Joystick.PlayerOne |= Joystick.JoystickDirection.Left;
                break;
            case Keys.Right:
                this.Joystick.PlayerOne |= Joystick.JoystickDirection.Right;
                break;
            case Keys.Up:
                this.Joystick.PlayerOne |= Joystick.JoystickDirection.Up;
                break;
            case Keys.Down:
                this.Joystick.PlayerOne |= Joystick.JoystickDirection.Down;
                break;
            case Keys.Space:
                this.Joystick.PlayerOneButton = true;
                break;
        }
    }

    private void Form1_KeyUp(object sender, KeyEventArgs e) {
        switch (e.KeyCode) {
            case Keys.Left:
                this.Joystick.PlayerOne ^= Joystick.JoystickDirection.Left;
                break;
            case Keys.Right:
                this.Joystick.PlayerOne ^= Joystick.JoystickDirection.Right;
                break;
            case Keys.Up:
                this.Joystick.PlayerOne ^= Joystick.JoystickDirection.Up;
                break;
            case Keys.Down:
                this.Joystick.PlayerOne ^= Joystick.JoystickDirection.Down;
                break;
            case Keys.Space:
                this.Joystick.PlayerOneButton = false;
                break;
        }
    }

    private void Form1_Load(object sender, EventArgs e) {
        foreach (string Color in AtariColor.AllColors) {
            byte[] Bytes = Convert.FromHexString(Color[1..]);
            D2DBrush? Brush = this.Invoke(() =>
                this.Device.CreateSolidColorBrush(new D2DColor(Bytes[0] / 255f, Bytes[1] / 255f, Bytes[2] / 255f)));
            this.BrushMap[Color] = Brush ?? throw new ApplicationException("Could not make brush");
        }

        string FilePath =
            @"C:\Users\jlync\source\repos\Atari2600\Atari2600.Test\road.a26";

        Emulator Emulator = Emulator.FromFile(FilePath, this.Joystick, this);
        Task.Run(() => {
            while (true) Emulator.Step();
        });
        Debugger.StartDebugging(Emulator, 59240);
    }
}