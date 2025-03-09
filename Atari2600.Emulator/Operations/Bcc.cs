namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Bcc : BranchOperation {
    public override OperationType Type => OperationType.Bcc;

    protected override bool ShouldBranch(EmulationState state) => !state.StatusRegister.Carry;
}