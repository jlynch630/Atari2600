namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Cli : IOperation {
    public OperationType Type => OperationType.Cli;

    public int Apply(EmulationState state, Instruction instruction) {
        state.StatusRegister.InterruptDisable = false;
        return 2;
    }
}