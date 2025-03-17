namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Sbc : GroupOneOperation {
    public override OperationType Type => OperationType.Sbc;

    public override void ApplyOperation(EmulationState state, byte memoryValue) {
        int CarryValue = state.StatusRegister.Carry ? 1 : 0;

        if (state.StatusRegister.DecimalMode) {
            int Result = GroupOneOperation.GetBcdValue(state.Accumulator) - GroupOneOperation.GetBcdValue(memoryValue) -
                         (1 - CarryValue);
            state.Accumulator.Value = GroupOneOperation.ToBcdValue(Result < 0 ? 100 + Result : Result % 100);
            state.StatusRegister.Carry = Result >= 0;
            state.StatusRegister.Overflow = Result is > 99 or < 0;
        }
        else {
            int Result = state.Accumulator - memoryValue - (1 - CarryValue);
            state.Accumulator.SetFromInt(Result);
            state.StatusRegister.Carry = Result >= 0;
            state.StatusRegister.Overflow = Result > 255;
        }

        state.StatusRegister.SetNegativeFromByteResult(state.Accumulator.Value);
        state.StatusRegister.SetZeroFromByteResult(state.Accumulator.Value);
    }
}