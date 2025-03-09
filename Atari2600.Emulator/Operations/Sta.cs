namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Sta : IOperation {
    public OperationType Type => OperationType.Sta;

    public int Apply(EmulationState state, Instruction instruction) {
        (ushort Address, _) = AddressUtils.AddressFromInstruction(instruction, state);
        state.Memory.WriteByte(Address, state.Accumulator);

        return instruction.AddressingMode switch {
            AddressingMode.ZeroPage => 3,
            AddressingMode.ZeroPageX => 4,
            AddressingMode.Absolute => 4,
            AddressingMode.AbsoluteX => 5,
            AddressingMode.AbsoluteY => 5,
            AddressingMode.IndexedIndirect => 6,
            AddressingMode.IndirectIndexed => 6,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}