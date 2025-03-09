namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Php : IOperation {
    public OperationType Type => OperationType.Php;

    public int Apply(EmulationState state, Instruction instruction) {
        state.StackPointer = state.Memory.PushStack(state.StackPointer, state.StatusRegister);
        return 3;
    }
}