namespace Atari2600.FormsTest;

using Emulator;
using Emulator.Tia;
using unvell.D2DLib;
using unvell.D2DLib.WinForm;

public partial class Form1 : D2DForm, IGraphicsBackend {
    private const int AtariHeight = 192; // want: 262!!! off by 23??
    private const int AtariWidth = 160; // want: 228 off by 5?
    private readonly Dictionary<string, D2DBrush> BrushMap = new();
    private readonly string[,] PaintColors = new string[Form1.AtariWidth, Form1.AtariHeight];
    private readonly int ScaleFactorX = 12; //6:5 ratio
    private readonly int ScaleFactorY = 10;

    public Form1() {
        this.InitializeComponent();
        this.Width = Form1.AtariWidth * this.ScaleFactorX;
        this.Height = Form1.AtariHeight * this.ScaleFactorY;
        this.AnimationDraw = true;
    }

    public void SetColor(int x, int y, AtariColor color) {
        lock (this.PaintColors) this.PaintColors[x, y] = color.Hex ?? "#000000";
    }

    protected override void OnRender(D2DGraphics g) {
        lock (this.PaintColors)
            for (int x = 0; x < this.PaintColors.GetLength(0); x++)
                for (int y = 0; y < this.PaintColors.GetLength(1); y++)
                    g.FillRectangle(
                        new D2DRect(x * this.ScaleFactorX, y * this.ScaleFactorY, this.ScaleFactorX, this.ScaleFactorY),
                        this.BrushMap[this.PaintColors[x, y] ?? "#000000"]);
    }

    private void Form1_Load(object sender, EventArgs e) {
        foreach (string Color in AtariColor.AllColors) {
            byte[] Bytes = Convert.FromHexString(Color[1..]);
            D2DBrush? Brush = this.Invoke(() =>
                this.Device.CreateSolidColorBrush(new D2DColor(Bytes[0] / 255f, Bytes[1] / 255f, Bytes[2] / 255f)));
            this.BrushMap[Color] = Brush ?? throw new ApplicationException("Could not make brush");
        }

        Task.Run(() => {
            string FilePath =
                @"C:\Users\jlync\AppData\Local\Temp\MicrosoftEdgeDownloads\71794997-3244-420d-a71e-86dd10d82377\bitmap.a26";
            // @"C:\Users\jlync\source\repos\Atari2600\Atari2600.Test\kernel_15.bin";

            Emulator Emulator = Emulator.FromFile(FilePath, this);
            while (true) Emulator.Step();
        });
    }
}