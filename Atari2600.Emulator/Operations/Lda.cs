namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Lda : GroupOneOperation {
    public override OperationType Type => OperationType.Lda;

    public override void ApplyOperation(EmulationState state, byte memoryValue) {
        state.Accumulator.Value = memoryValue;

        state.StatusRegister.SetNegativeFromByteResult(state.Accumulator.Value);
        state.StatusRegister.SetZeroFromByteResult(state.Accumulator.Value);
    }
}