namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Pla : IOperation {
    public OperationType Type => OperationType.Pla;

    public int Apply(EmulationState state, Instruction instruction) {
        (byte Value, state.StackPointer) = state.Memory.PopStack(state.StackPointer);
        state.Accumulator.Value = Value;
        state.StatusRegister.SetZeroFromByteResult(Value);
        state.StatusRegister.SetNegativeFromByteResult(Value);
        return 4;
    }
}