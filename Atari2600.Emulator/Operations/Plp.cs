namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Plp : IOperation {
    public OperationType Type => OperationType.Plp;

    public int Apply(EmulationState state, Instruction instruction) {
        (byte Value, state.StackPointer) = state.Memory.PopStack(state.StackPointer);
        state.StatusRegister.Value = Value;
        return 4;
    }
}