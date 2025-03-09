namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Tya : IOperation {
    public OperationType Type => OperationType.Tya;

    public int Apply(EmulationState state, Instruction instruction) {
        state.Accumulator.Value = state.YIndex;

        state.StatusRegister.SetZeroFromByteResult(state.Accumulator);
        state.StatusRegister.SetNegativeFromByteResult(state.Accumulator);

        return 2;
    }
}