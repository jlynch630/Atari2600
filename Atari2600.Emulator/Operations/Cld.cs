namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Cld : IOperation {
    public OperationType Type => OperationType.Cld;

    public int Apply(EmulationState state, Instruction instruction) {
        state.StatusRegister.DecimalMode = false;
        return 2;
    }
}