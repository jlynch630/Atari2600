namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Dey : IOperation {
    public OperationType Type => OperationType.Dey;

    public int Apply(EmulationState state, Instruction instruction) {
        state.YIndex.Value--;
        state.StatusRegister.SetZeroFromByteResult(state.YIndex);
        state.StatusRegister.SetNegativeFromByteResult(state.YIndex);
        return 2;
    }
}