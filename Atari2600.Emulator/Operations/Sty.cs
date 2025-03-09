namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Sty : IOperation {
    public OperationType Type => OperationType.Sty;

    public int Apply(EmulationState state, Instruction instruction) {
        (ushort Address, _) = AddressUtils.AddressFromInstruction(instruction, state);
        state.Memory.WriteByte(Address, state.YIndex);

        return instruction.AddressingMode switch {
            AddressingMode.ZeroPage => 3,
            AddressingMode.ZeroPageX => 4,
            AddressingMode.Absolute => 4,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}