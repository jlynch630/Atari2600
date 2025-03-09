namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Bmi : BranchOperation {
    public override OperationType Type => OperationType.Bmi;

    protected override bool ShouldBranch(EmulationState state) => state.StatusRegister.NegativeResult;
}