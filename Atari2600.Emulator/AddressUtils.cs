namespace Atari2600.Emulator;

using Disassembler;

internal static class AddressUtils {
    public static ushort AddressFromBytes(byte[] bytes) => bytes.Length == 2
        ? (ushort)((bytes[1] << 8) + bytes[0])
        : throw new ArgumentException("Invalid number of bytes when converting to address", nameof(bytes));

    public static (ushort, bool) AddressFromInstruction(Instruction instruction, EmulationState state) =>
        instruction.Operands is not null
            ? AddressUtils.AddressFromOperand(instruction.Operands, instruction.AddressingMode, state)
            : throw new ApplicationException("Instruction operands cannot be null for address resolution");

    public static (ushort, bool) AddressFromOperand(byte[] operands, AddressingMode mode, EmulationState state) {
        if (mode is AddressingMode.Accumulator or AddressingMode.Immediate or AddressingMode.Implicit)
            throw new ApplicationException("Invalid addressing mode for memory address: " + mode);

        int ExpectedBytes = mode is AddressingMode.ZeroPage or AddressingMode.ZeroPageX or AddressingMode.ZeroPageY
         or AddressingMode.IndexedIndirect or AddressingMode.IndirectIndexed or AddressingMode.Relative
            ? 1
            : 2;

        if (operands.Length != ExpectedBytes)
            throw new ApplicationException("Invalid number of bytes passed for address resolution");
        return mode switch {
            AddressingMode.ZeroPage => (operands[0], false),
            AddressingMode.ZeroPageX => (operands[0].AddWithWraparound(state.XIndex.Value), false),
            AddressingMode.ZeroPageY => (operands[0].AddWithWraparound(state.YIndex.Value), false),
            AddressingMode.Relative => AddressUtils.AddWithPageCrossSigned(state.ProgramCounter, operands[0]),
            AddressingMode.Absolute => (AddressUtils.AddressFromBytes(operands), false),
            AddressingMode.AbsoluteX =>
                AddressUtils.AddWithPageCross(AddressUtils.AddressFromBytes(operands), state.XIndex.Value),
            AddressingMode.AbsoluteY =>
                AddressUtils.AddWithPageCross(AddressUtils.AddressFromBytes(operands), state.YIndex.Value),
            AddressingMode.Indirect => (
                state.Memory.ReadAddress(AddressUtils.AddressFromBytes(operands)),
                false), // todo: broken on original 6502, break here?
            AddressingMode.IndexedIndirect => (
                state.Memory.ReadAddress(operands[0].AddWithWraparound(state.XIndex.Value)), false),
            AddressingMode.IndirectIndexed => AddressUtils.AddWithPageCross(
                state.Memory.ReadAddress(operands[0]), state.YIndex.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    private static (ushort, bool) AddWithPageCross(ushort a, byte b) {
        ushort Result = unchecked((ushort)(a + b));
        return (Result, (a & 0xff00) != (Result & 0xff00));
    }

    private static (ushort, bool) AddWithPageCrossSigned(ushort a, byte b) {
        ushort Result = unchecked((ushort)(a + (sbyte)b));
        return (Result, (a & 0xff00) != (Result & 0xff00));
    }
}