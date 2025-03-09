namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Rti : IOperation {
    public OperationType Type => OperationType.Rti;

    public int Apply(EmulationState state, Instruction instruction) {
        (state.StatusRegister.Value, state.StackPointer) = state.Memory.PopStack(state.StackPointer);
        (state.ProgramCounter, state.StackPointer) = state.Memory.PopStackAddress(state.StackPointer);

        return 6;
    }
}