namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Bcs : BranchOperation {
    public override OperationType Type => OperationType.Bcs;

    protected override bool ShouldBranch(EmulationState state) => state.StatusRegister.Carry;
}