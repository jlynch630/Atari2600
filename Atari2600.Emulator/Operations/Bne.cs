namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Bne : BranchOperation {
    public override OperationType Type => OperationType.Bne;

    protected override bool ShouldBranch(EmulationState state) => !state.StatusRegister.ZeroResult;
}