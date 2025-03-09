namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Sei : IOperation {
    public OperationType Type => OperationType.Sei;

    public int Apply(EmulationState state, Instruction instruction) {
        state.StatusRegister.InterruptDisable = true;
        return 2;
    }
}