namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Sed : IOperation {
    public OperationType Type => OperationType.Sed; // todo: what is decimal??

    public int Apply(EmulationState state, Instruction instruction) {
        state.StatusRegister.DecimalMode = true;
        return 2;
    }
}