namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Bvs : BranchOperation {
    public override OperationType Type => OperationType.Bvs;

    protected override bool ShouldBranch(EmulationState state) => state.StatusRegister.Overflow;
}