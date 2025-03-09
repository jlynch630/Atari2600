namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Jmp : IOperation {
    public OperationType Type => OperationType.Jmp;

    public int Apply(EmulationState state, Instruction instruction) {
        (ushort Address, bool _) = AddressUtils.AddressFromInstruction(instruction, state);
        state.ProgramCounter = Address;

        return instruction.AddressingMode switch {
            AddressingMode.Absolute => 3,
            AddressingMode.Indirect => 5,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}