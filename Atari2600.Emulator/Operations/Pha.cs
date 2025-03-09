namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Pha : IOperation {
    public OperationType Type => OperationType.Pha;

    public int Apply(EmulationState state, Instruction instruction) {
        state.StackPointer = state.Memory.PushStack(state.StackPointer, state.Accumulator);
        return 3;
    }
}