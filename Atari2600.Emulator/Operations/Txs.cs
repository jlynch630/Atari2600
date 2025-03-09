namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Txs : IOperation {
    public OperationType Type => OperationType.Txs;

    public int Apply(EmulationState state, Instruction instruction) {
        state.StackPointer = state.XIndex;
        state.StatusRegister.SetZeroFromByteResult(state.XIndex);
        state.StatusRegister.SetNegativeFromByteResult(state.XIndex);

        return 2;
    }
}