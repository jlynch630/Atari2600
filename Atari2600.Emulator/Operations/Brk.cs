namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Brk : IOperation {
    public OperationType Type => OperationType.Brk;

    public int Apply(EmulationState state, Instruction instruction) {
        byte ProcessorState = state.StatusRegister;
        state.StatusRegister.BreakCommand = true;
        state.StackPointer = state.Memory.PushStack(state.StackPointer, state.ProgramCounter);

        state.StackPointer = state.Memory.PushStack(state.StackPointer, ProcessorState);
        state.ProgramCounter = state.Memory.ReadAddress(0xfffe); // todo: constants? (this is interrupt vector)
        return 7;
    }
}