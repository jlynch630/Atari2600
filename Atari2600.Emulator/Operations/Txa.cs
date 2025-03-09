namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Txa : IOperation {
    public OperationType Type => OperationType.Txa;

    public int Apply(EmulationState state, Instruction instruction) {
        state.Accumulator.Value = state.XIndex;

        state.StatusRegister.SetZeroFromByteResult(state.Accumulator);
        state.StatusRegister.SetNegativeFromByteResult(state.Accumulator);

        return 2;
    }
}