namespace Atari2600.Emulator.Operations;

using Disassembler;

internal class Clc : IOperation {
    public OperationType Type => OperationType.Clc;

    public int Apply(EmulationState state, Instruction instruction) {
        state.StatusRegister.Carry = false;
        return 2;
    }
}