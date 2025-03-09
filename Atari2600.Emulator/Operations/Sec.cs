namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Sec : IOperation {
    public OperationType Type => OperationType.Sec;

    public int Apply(EmulationState state, Instruction instruction) {
        state.StatusRegister.Carry = true;
        return 2;
    }
}