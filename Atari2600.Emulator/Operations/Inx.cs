namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Inx : IOperation {
    public OperationType Type => OperationType.Inx;

    public int Apply(EmulationState state, Instruction instruction) {
        state.XIndex.Value++;
        state.StatusRegister.SetZeroFromByteResult(state.XIndex);
        state.StatusRegister.SetNegativeFromByteResult(state.XIndex);
        return 2;
    }
}