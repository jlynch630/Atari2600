using Atari2600.Emulator;
using Atari2600.Emulator.Debugging;
using Atari2600.Emulator.Pia.IO;
using Atari2600.Emulator.Tia;

string FilePath =
    @"C:\Users\jlync\source\repos\Atari2600\Atari2600.Test\kernel_01.bin";
//@"C:\Users\jlync\source\repos\Atari2600\Atari2600.Test\Basic Math (USA).a26";
//
// Instruction[] AllInstructions = Disassembler.ReadFromFile(FilePath);
//
// Console.WriteLine(String.Join('\n', AllInstructions.Select(i => i.ToString())));
Emulator Emulator = Emulator.FromFile(FilePath, new Joystick(), new ConsoleGraphics());
Task.Run(() => {
    while (true) Emulator.Step();
});
await Debugger.StartDebugging(Emulator, 59240);

internal class ConsoleGraphics : IGraphicsBackend {
    public void SetColor(int x, int y, AtariColor color) {
        // disaster!
    }
}