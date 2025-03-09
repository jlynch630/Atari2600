namespace Atari2600.Emulator;

using Memory;
using Registers;

public class EmulationState(IMemory memory) {
    public Register Accumulator { get; } = new("Accumulator");

    public IMemory Memory { get; } = memory;

    public ushort ProgramCounter { get; set; }

    public ushort StackPointer { get; set; }

    public StatusRegister StatusRegister { get; } = new();

    public Register XIndex { get; } = new("X");

    public Register YIndex { get; } = new("Y");
}