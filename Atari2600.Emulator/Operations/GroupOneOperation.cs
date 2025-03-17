namespace Atari2600.Emulator.Operations;

using Disassembler;

internal abstract class GroupOneOperation : IOperation {
    public abstract OperationType Type { get; }

    public int Apply(EmulationState state, Instruction instruction) {
        if (instruction.AddressingMode is AddressingMode.Immediate) {
            if (instruction.Operands is null) throw new ApplicationException("Immediate instruction had no operands");
            this.ApplyOperation(state, instruction.Operands[0]);
            return 2;
        }

        (ushort Address, bool PageCross) = AddressUtils.AddressFromInstruction(instruction, state);
        byte MemoryValue = state.Memory.ReadByte(Address);

        this.ApplyOperation(state, MemoryValue);

        int Cycles = instruction.AddressingMode switch {
            AddressingMode.ZeroPage => 3,
            AddressingMode.ZeroPageX => 4,
            AddressingMode.ZeroPageY => 4,
            AddressingMode.Absolute => 4,
            AddressingMode.AbsoluteX => 4,
            AddressingMode.AbsoluteY => 4,
            AddressingMode.IndexedIndirect => 6,
            AddressingMode.IndirectIndexed => 5,
            _ => throw new ArgumentOutOfRangeException()
        } + (PageCross ? 1 : 0);

        return Cycles;
    }

    public abstract void ApplyOperation(EmulationState state, byte memoryValue);

    protected static int GetBcdValue(byte b) {
        int Low = b & 0xf;
        int High = (b & 0xf0) >>> 4;
        if (Low >= 10 || High >= 10) throw new ApplicationException("Invalid BCD value: " + b);

        return High * 10 + Low;
    }

    protected static byte ToBcdValue(int b) {
        if (b >= 100) throw new ApplicationException("Integer cannot be converted to BCD: " + b);
        int Low = b % 10;
        int High = b / 10;
        return (byte)(High * 16 + Low);
    }
}