namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Cpy : IOperation {
    public OperationType Type => OperationType.Cpy;

    public int Apply(EmulationState state, Instruction instruction) {
        byte MemoryValue;
        if (instruction.AddressingMode == AddressingMode.Immediate) {
            if (instruction.Operands is null)
                throw new ApplicationException("No operands set in immediate addressing mode");
            MemoryValue = instruction.Operands[0];
        }
        else {
            (ushort Address, _) = AddressUtils.AddressFromInstruction(instruction, state);
            MemoryValue = state.Memory.ReadByte(Address);
        }

        state.StatusRegister.ZeroResult = state.YIndex == MemoryValue;
        state.StatusRegister.Carry = state.YIndex >= MemoryValue;
        state.StatusRegister.SetNegativeFromByteResult(unchecked((byte)(state.YIndex - MemoryValue)));

        return instruction.AddressingMode switch {
            AddressingMode.Immediate => 2,
            AddressingMode.ZeroPage => 3,
            AddressingMode.Absolute => 4,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}