namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Ora : GroupOneOperation {
    public override OperationType Type => OperationType.Ora;

    public override void ApplyOperation(EmulationState state, byte memoryValue) {
        int Result = state.Accumulator | memoryValue;
        state.Accumulator.SetFromInt(Result);

        state.StatusRegister.SetNegativeFromByteResult(state.Accumulator.Value);
        state.StatusRegister.SetZeroFromByteResult(state.Accumulator.Value);
    }
}