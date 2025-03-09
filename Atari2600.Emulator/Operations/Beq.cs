namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Beq : BranchOperation {
    public override OperationType Type => OperationType.Beq;

    protected override bool ShouldBranch(EmulationState state) => state.StatusRegister.ZeroResult;
}