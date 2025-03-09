namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Nop : IOperation {
    public OperationType Type => OperationType.Nop;

    public int Apply(EmulationState state, Instruction instruction) => 2;
}