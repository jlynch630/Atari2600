namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Cmp : GroupOneOperation {
    public override OperationType Type => OperationType.Cmp;

    public override void ApplyOperation(EmulationState state, byte memoryValue) {
        state.StatusRegister.ZeroResult = state.Accumulator == memoryValue;
        state.StatusRegister.Carry = state.Accumulator >= memoryValue;
        state.StatusRegister.SetNegativeFromByteResult(unchecked((byte)(state.Accumulator - memoryValue)));
    }
}