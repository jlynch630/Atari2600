namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class And : GroupOneOperation {
    public override OperationType Type => OperationType.And;

    public override void ApplyOperation(EmulationState state, byte memoryValue) {
        int Result = state.Accumulator & memoryValue;
        state.Accumulator.SetFromInt(Result);

        state.StatusRegister.SetNegativeFromByteResult(state.Accumulator.Value);
        state.StatusRegister.SetZeroFromByteResult(state.Accumulator.Value);
    }
}