namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Tsx : IOperation {
    public OperationType Type => OperationType.Tsx;

    public int Apply(EmulationState state, Instruction instruction) {
        state.XIndex.SetFromInt(state.StackPointer);

        state.StatusRegister.SetZeroFromByteResult(state.XIndex);
        state.StatusRegister.SetNegativeFromByteResult(state.XIndex);

        return 2;
    }
}