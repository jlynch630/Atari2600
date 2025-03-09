namespace Atari2600.FormsTest;

using Emulator;
using Emulator.Tia;

public partial class Form1 : Form, IGraphicsBackend {
    private const int Height = 192;
    private const int Width = 160;
    private readonly Color[] Screen = new Color[Form1.Width * Form1.Height];

    public Form1() {
        this.InitializeComponent();
    }

    public void SetColor(int x, int y, AtariColor color) {
        byte[] Colors = Convert.FromHexString(color.Hex.Substring(1));
        if (y >= Form1.Height) return;
        lock (this.Screen) this.Screen[y * Form1.Width + x] = Color.FromArgb(255, Colors[0], Colors[1], Colors[2]);
    }

    private void Emulate() {
        string FilePath =
            //@"C:\Users\jlync\source\repos\Atari2600\Atari2600.Test\kernel_01.bin";
            @"C:\Users\jlync\source\repos\Atari2600\Atari2600.Test\Basic Math (USA).a26";
        //
        // Instruction[] AllInstructions = Disassembler.ReadFromFile(FilePath);
        //
        // Console.WriteLine(String.Join('\n', AllInstructions.Select(i => i.ToString())));
        Emulator Emulator = Emulator.FromFile(FilePath, this);
        while (true) Emulator.Step();
    }

    private void Form1_Load(object sender, EventArgs e) {
        Task.Run(this.Emulate);
    }

    private void panel1_Paint(object sender, PaintEventArgs e) {
        lock (this.Screen)
            for (int x = 0; x < Form1.Width; x++) {
                for (int y = 0; y < Form1.Height; y++)
                    e.Graphics.FillRectangle(new SolidBrush(this.Screen[y * Form1.Width + x]), x, y, 1, 1);
            }
    }

    private void timer1_Tick(object sender, EventArgs e) {
        this.panel1.Refresh();
    }
}