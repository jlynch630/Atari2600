using Atari2600.Emulator;
using Atari2600.Emulator.Tia;

string FilePath =
    @"C:\Users\jlync\source\repos\Atari2600\Atari2600.Test\kernel_01.bin";
//@"C:\Users\jlync\source\repos\Atari2600\Atari2600.Test\Basic Math (USA).a26";
//
// Instruction[] AllInstructions = Disassembler.ReadFromFile(FilePath);
//
// Console.WriteLine(String.Join('\n', AllInstructions.Select(i => i.ToString())));
Emulator Emulator = Emulator.FromFile(FilePath, new ConsoleGraphics());
while (true) Emulator.Step();

internal class ConsoleGraphics : IGraphicsBackend {
    public void SetColor(int x, int y, AtariColor color) {
        // disaster!
    }
}