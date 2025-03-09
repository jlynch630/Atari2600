namespace Atari2600.Disassembler;

public class Disassembler {
    public static Instruction[] ReadFromFile(string path) {
        using FileStream Stream = new(path, FileMode.Open);
        return Disassembler.ReadFromStream(Stream);
    }

    public static Instruction[] ReadFromStream(Stream stream) {
        List<Instruction> Instructions = [];
        while (true) {
            Instruction? Next = InstructionParser.ParseInstruction(stream);
            if (Next == null) break;
            Instructions.Add(Next);
        }

        return Instructions.ToArray();
    }
}