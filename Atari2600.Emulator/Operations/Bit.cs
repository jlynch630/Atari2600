namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Bit : IOperation {
    public OperationType Type => OperationType.Bit;

    public int Apply(EmulationState state, Instruction instruction) {
        (ushort Address, bool _) = AddressUtils.AddressFromInstruction(instruction, state);
        byte MemoryValue = state.Memory.ReadByte(Address);
        byte Result = (byte)(state.Accumulator & MemoryValue);

        state.StatusRegister.Overflow = (MemoryValue & 0x40) == 0x40; // bit 6
        state.StatusRegister.NegativeResult = MemoryValue.IsNegative(); // bit 7
        state.StatusRegister.SetZeroFromByteResult(Result);

        return instruction.AddressingMode switch {
            AddressingMode.ZeroPage => 3,
            AddressingMode.Absolute => 4,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}