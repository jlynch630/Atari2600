namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Asl : IOperation {
    public OperationType Type => OperationType.Asl;

    public int Apply(EmulationState state, Instruction instruction) {
        int Result = 0;
        if (instruction.AddressingMode is AddressingMode.Accumulator) {
            Result = state.Accumulator << 1;
            state.Accumulator.SetFromInt(Result);
        }
        else {
            (ushort Address, bool _) = AddressUtils.AddressFromInstruction(instruction, state);
            byte MemoryValue = state.Memory.ReadByte(Address);
            Result = MemoryValue << 1;
            state.Memory.WriteByte(Address, (byte)(Result % 256));
        }

        state.StatusRegister.SetCarryFromIntResult(Result);
        state.StatusRegister.SetNegativeFromByteResult((byte)(Result % 256));
        state.StatusRegister.SetZeroFromByteResult((byte)(Result % 256));

        return instruction.AddressingMode switch {
            AddressingMode.Accumulator => 2,
            AddressingMode.ZeroPage => 5,
            AddressingMode.ZeroPageX => 6,
            AddressingMode.Absolute => 6,
            AddressingMode.AbsoluteX => 7,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}