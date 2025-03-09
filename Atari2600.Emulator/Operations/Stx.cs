namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Stx : IOperation {
    public OperationType Type => OperationType.Stx;

    public int Apply(EmulationState state, Instruction instruction) {
        (ushort Address, _) = AddressUtils.AddressFromInstruction(instruction, state);
        state.Memory.WriteByte(Address, state.XIndex);

        return instruction.AddressingMode switch {
            AddressingMode.ZeroPage => 3,
            AddressingMode.ZeroPageY => 4,
            AddressingMode.Absolute => 4,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}