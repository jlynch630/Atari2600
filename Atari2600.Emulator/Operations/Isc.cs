namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Isc : IllegalOperation {
    public override OperationType Type => OperationType.Isc;

    protected override IOperation[] Operations { get; } = [new Inc(), new Sbc()];
}