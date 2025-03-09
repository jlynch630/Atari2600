namespace Atari2600.Emulator.Operations;

using Disassembler;

internal abstract class BranchOperation : IOperation {
    public abstract OperationType Type { get; }

    public int Apply(EmulationState state, Instruction instruction) {
        (ushort Address, bool PageCross) = AddressUtils.AddressFromInstruction(instruction, state);
        if (this.ShouldBranch(state)) {
            state.ProgramCounter = Address;
            return 3 + (PageCross ? 1 : 0);
        }

        return 2;
    }

    protected abstract bool ShouldBranch(EmulationState state);
}