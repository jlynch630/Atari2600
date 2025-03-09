namespace Atari2600.Disassembler;

public record Instruction(
    byte OpCode,
    OperationType OperationType,
    AddressingMode AddressingMode,
    byte[]? Operands) {
    public int ByteLength => 1 + (this.Operands?.Length ?? 0);
}