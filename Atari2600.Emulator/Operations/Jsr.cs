﻿namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Jsr : IOperation {
    public OperationType Type => OperationType.Jsr;

    public int Apply(EmulationState state, Instruction instruction) {
        (ushort Address, bool _) = AddressUtils.AddressFromInstruction(instruction, state);
        state.StackPointer = state.Memory.PushStack(state.StackPointer, (ushort)(state.ProgramCounter - 1));

        ushort MemoryValue = Address;
        state.ProgramCounter = MemoryValue;

        return instruction.AddressingMode switch {
            AddressingMode.Absolute => 6,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}