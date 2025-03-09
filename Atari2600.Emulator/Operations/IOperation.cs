namespace Atari2600.Emulator.Operations;

using Disassembler;

internal interface IOperation {
    public OperationType Type { get; }

    public int Apply(EmulationState state, Instruction instruction);
}