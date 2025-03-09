namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Clv : IOperation {
    public OperationType Type => OperationType.Clv;

    public int Apply(EmulationState state, Instruction instruction) {
        state.StatusRegister.Overflow = false;
        return 2;
    }
}