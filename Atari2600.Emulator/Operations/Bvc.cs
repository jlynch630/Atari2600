namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Bvc : BranchOperation {
    public override OperationType Type => OperationType.Bvc;

    protected override bool ShouldBranch(EmulationState state) => !state.StatusRegister.Overflow;
}