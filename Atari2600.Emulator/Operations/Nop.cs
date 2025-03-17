namespace Atari2600.Emulator.Operations;

using System.Diagnostics;
using Disassembler;

internal class Nop : IOperation {
    public OperationType Type => OperationType.Nop;

    public int Apply(EmulationState state, Instruction instruction) {
        // todo obviously
        if (instruction.OpCode is not (0xea or 0x04 or 0x44 or 0x64 or 0x14 or 0x34 or 0x54 or 0x74 or 0xd4 or 0xf4
         or 0x0c or 0x1c or 0x3c or 0x5c or 0x7c or 0xdc or 0xfc)) {
            Debug.WriteLine(
                $"Warning: nonstandard opcode encountered in execution: {instruction.OpCode:X2} PC: {state.ProgramCounter - 1:X2}");
        }

        if (instruction.AddressingMode is not AddressingMode.AbsoluteX) {
            return instruction.AddressingMode switch {
                AddressingMode.Implicit => 2,
                AddressingMode.Immediate => 2,
                AddressingMode.ZeroPage => 3,
                AddressingMode.ZeroPageX => 4,
                AddressingMode.Absolute => 4,
                _ => 2
            };
        }

        (_, bool PageCross) = AddressUtils.AddressFromInstruction(instruction, state);
        return PageCross ? 5 : 4;
    }
}