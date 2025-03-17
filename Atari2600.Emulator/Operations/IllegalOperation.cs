namespace Atari2600.Emulator.Operations;

using Disassembler;

internal abstract class IllegalOperation : IOperation {
    public abstract OperationType Type { get; }

    protected abstract IOperation[] Operations { get; }

    public int Apply(EmulationState state, Instruction instruction) {
        return this.Operations.Select(o => o.Apply(state, instruction)).Max();
    }
}