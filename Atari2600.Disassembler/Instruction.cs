namespace Atari2600.Disassembler;

public record Instruction(
    byte OpCode,
    OperationType OperationType,
    AddressingMode AddressingMode,
    byte[]? Operands) {
    public int ByteLength => 1 + (this.Operands?.Length ?? 0);

    public override string ToString() {
        if ((this.OperationType is OperationType.Nop && this.OpCode != 0xEA) || this.OpCode == 0xff)
            return $".byte ${this.OpCode:X2}";
        return
            $"{this.OperationType.ToString().ToUpper()}    {Instruction.FormatOperands(this.AddressingMode, this.Operands)}";
    }

    private static string FormatOperands(AddressingMode mode, byte[]? operands) {
        if (operands is null) return "";
        return mode switch {
            AddressingMode.Implicit => "",
            AddressingMode.Accumulator => "A",
            AddressingMode.Immediate => $"#${operands[0]:X2}",
            AddressingMode.ZeroPage => $"${operands[0]:X2}",
            AddressingMode.ZeroPageX => $"${operands[0]:X2},x",
            AddressingMode.ZeroPageY => $"${operands[0]:X2},y",
            AddressingMode.Relative => ((sbyte)operands[0]).ToString(),
            AddressingMode.Absolute => "$" + FormatAddress(),
            AddressingMode.AbsoluteX => $"${FormatAddress()},x",
            AddressingMode.AbsoluteY => $"${FormatAddress()},y",
            AddressingMode.Indirect => $"(${FormatAddress()})",
            AddressingMode.IndexedIndirect => $"(${operands[0]:X2},X)",
            AddressingMode.IndirectIndexed => $"(${operands[0]:X2}),Y",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        string FormatAddress() => operands[0].ToString("X2") + operands[1].ToString("X2");
    }
}