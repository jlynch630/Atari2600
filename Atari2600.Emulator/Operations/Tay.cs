namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Tay : IOperation {
    public OperationType Type => OperationType.Tay;

    public int Apply(EmulationState state, Instruction instruction) {
        state.YIndex.Value = state.Accumulator;

        state.StatusRegister.SetZeroFromByteResult(state.YIndex);
        state.StatusRegister.SetNegativeFromByteResult(state.YIndex);

        return 2;
    }
}