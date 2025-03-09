namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Ldx : IOperation {
    public OperationType Type => OperationType.Ldx;

    public int Apply(EmulationState state, Instruction instruction) {
        if (instruction.AddressingMode is AddressingMode.Immediate) {
            if (instruction.Operands is null) throw new ApplicationException("Immediate instruction had no operands");
            state.XIndex.Value = instruction.Operands[0];
            state.StatusRegister.SetNegativeFromByteResult(instruction.Operands[0]);
            state.StatusRegister.SetZeroFromByteResult(instruction.Operands[0]);
            return 2;
        }

        (ushort Address, bool PageCross) = AddressUtils.AddressFromInstruction(instruction, state);
        byte MemoryValue = state.Memory.ReadByte(Address);
        state.XIndex.Value = MemoryValue;
        state.StatusRegister.SetNegativeFromByteResult(MemoryValue);
        state.StatusRegister.SetZeroFromByteResult(MemoryValue);

        return instruction.AddressingMode switch {
            AddressingMode.ZeroPage => 3,
            AddressingMode.ZeroPageY => 4,
            AddressingMode.Absolute => 4,
            AddressingMode.AbsoluteY => 4,
            _ => throw new ArgumentOutOfRangeException()
        } + (PageCross ? 0 : 1);
    }
}