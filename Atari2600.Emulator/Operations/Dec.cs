namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Dec : IOperation {
    public OperationType Type => OperationType.Dec;

    public int Apply(EmulationState state, Instruction instruction) {
        (ushort Address, bool _) = AddressUtils.AddressFromInstruction(instruction, state);
        byte MemoryValue = state.Memory.ReadByte(Address);
        MemoryValue--;
        state.Memory.WriteByte(Address, MemoryValue);
        state.StatusRegister.SetZeroFromByteResult(MemoryValue);
        state.StatusRegister.SetNegativeFromByteResult(MemoryValue);

        return instruction.AddressingMode switch {
            AddressingMode.ZeroPage => 5,
            AddressingMode.ZeroPageX => 6,
            AddressingMode.Absolute => 6,
            AddressingMode.AbsoluteX => 7,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}