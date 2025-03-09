namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Adc : GroupOneOperation {
    public override OperationType Type => OperationType.Adc;

    public override void ApplyOperation(EmulationState state, byte memoryValue) {
        byte OldValue = state.Accumulator;
        int CarryValue = state.StatusRegister.Carry ? 1 : 0;

        if (state.StatusRegister.DecimalMode) {
            int Result = GroupOneOperation.GetBcdValue(state.Accumulator) + GroupOneOperation.GetBcdValue(memoryValue) +
                         CarryValue;
            state.Accumulator.Value = GroupOneOperation.ToBcdValue(Result % 100);
            state.StatusRegister.Carry = Result >= 100;
        }
        else {
            int Result = state.Accumulator + memoryValue + CarryValue;
            state.Accumulator.SetFromInt(Result);
            state.StatusRegister.SetCarryFromIntResult(Result);
        }

        state.StatusRegister.SetOverflowFromByteResult(OldValue, state.Accumulator.Value);
        state.StatusRegister.SetNegativeFromByteResult(state.Accumulator.Value);
        state.StatusRegister.SetZeroFromByteResult(state.Accumulator.Value);
    }
}