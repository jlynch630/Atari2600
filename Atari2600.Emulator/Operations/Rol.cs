namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Rol : IOperation {
    public OperationType Type => OperationType.Rol;

    public int Apply(EmulationState state, Instruction instruction) {
        byte Result;
        bool PageCross = false;
        int BitZero = state.StatusRegister.Carry ? 1 : 0;
        if (instruction.AddressingMode is AddressingMode.Accumulator) {
            state.StatusRegister.Carry = (state.Accumulator & 0x80) == 0x80;
            state.Accumulator.Value = (byte)((state.Accumulator << 1) | BitZero);
            Result = state.Accumulator.Value;
        }
        else {
            (ushort Address, PageCross) = AddressUtils.AddressFromInstruction(instruction, state);
            byte MemoryValue = state.Memory.ReadByte(Address);
            state.StatusRegister.Carry = (MemoryValue & 0x80) == 0x80;
            Result = (byte)((MemoryValue << 1) | BitZero);
            state.Memory.WriteByte(Address, Result);
        }

        state.StatusRegister.SetNegativeFromByteResult(Result);
        state.StatusRegister.SetZeroFromByteResult(Result);

        return instruction.AddressingMode switch {
            AddressingMode.Accumulator => 2,
            AddressingMode.ZeroPage => 5,
            AddressingMode.ZeroPageX => 6,
            AddressingMode.Absolute => 6,
            AddressingMode.AbsoluteX => 7,
            _ => throw new ArgumentOutOfRangeException()
        } + (PageCross ? 0 : 1);
    }
}