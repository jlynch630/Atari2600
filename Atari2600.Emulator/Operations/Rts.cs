namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Rts : IOperation {
    public OperationType Type => OperationType.Rts;

    public int Apply(EmulationState state, Instruction instruction) {
        (state.ProgramCounter, state.StackPointer) = state.Memory.PopStackAddress(state.StackPointer);
        state.ProgramCounter++;
        return 6;
    }
}