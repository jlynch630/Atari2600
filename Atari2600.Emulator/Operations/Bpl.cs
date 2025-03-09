namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Bpl : BranchOperation {
    public override OperationType Type => OperationType.Bpl;

    protected override bool ShouldBranch(EmulationState state) => !state.StatusRegister.NegativeResult;
}