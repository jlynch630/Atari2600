namespace Atari2600.Emulator;

using System.Reflection;
using Disassembler;
using Memory;
using Operations;
using Pia.IO;
using Tia;

public class Emulator : IDisposable, IAsyncDisposable {
    private static readonly Dictionary<OperationType, IOperation> Operations;

    private readonly Pia.Pia Pia;
    private readonly MemoryStream ProgramDataStream;
    private readonly EmulationState State;
    private readonly Tia.Tia Tia;
    private int WaitCycles;

    static Emulator() {
        Emulator.Operations = new Dictionary<OperationType, IOperation>(Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t is { Namespace: "Atari2600.Emulator.Operations", Name.Length: 3 }).Select(t => {
                IOperation? Op = (IOperation?)Activator.CreateInstance(t);
                if (Op is null) throw new Exception("Null operation found: " + t.Name);
                return new KeyValuePair<OperationType, IOperation>(Op.Type, Op);
            }));
    }

    public Emulator(byte[] programData, IGraphicsBackend graphics) {
        ConsoleSwitches Switches = new();
        Joystick Joystick = new();
        this.ProgramDataStream = new MemoryStream(programData);
        this.Pia = new Pia.Pia(Switches, Joystick);
        this.Tia = new Tia.Tia(graphics);
        this.State = this.InitializeState(programData);
    }

    public async ValueTask DisposeAsync() {
        await this.ProgramDataStream.DisposeAsync();
    }

    public void Dispose() {
        this.ProgramDataStream.Dispose();
    }

    public static Emulator FromFile(string path, IGraphicsBackend graphics) => new(File.ReadAllBytes(path), graphics);

    public void Step() {
        this.Pia.Step(); // todo, does this pause on wsync?
        this.Tia.Step();
        this.Tia.Step();
        this.Tia.Step();
        if (this.Tia.IsWaitingForSync) return;
        if (this.WaitCycles > 0) {
            // Console.WriteLine("wait");
            this.WaitCycles--;
            return;
        }

        // great!
        Instruction? Next = this.GetNextInstruction();
        if (Next is null) return;
        //Console.WriteLine("{0}: {1}", this.State.ProgramCounter & 0x0FFF, Next);

        this.WaitCycles = this.ExecuteInstruction(Next) - 1; // this, of course, counts as one cycle
    }

    private int ExecuteInstruction(Instruction instruction) {
        this.State.ProgramCounter = (ushort)(this.State.ProgramCounter + instruction.ByteLength);
        IOperation MatchingOp = Emulator.Operations[instruction.OperationType];
        return MatchingOp.Apply(this.State, instruction);
    }

    private Instruction? GetNextInstruction() {
        this.ProgramDataStream.Seek(this.State.ProgramCounter & 0x0FFF, SeekOrigin.Begin);
        return InstructionParser.ParseInstruction(this.ProgramDataStream);
    }

    private EmulationState InitializeState(byte[] program) {
        ReadWriteMemory Ram = new(0x80);
        ReadOnlyMemory ProgramData = new(program);
        MappedMemory Map = AtariMap.CreateAtariMap(this.Tia, this.Pia, Ram, ProgramData);

        return new EmulationState(Map) {
                                           ProgramCounter = Map.ReadAddress(0xFFFC),
                                           StackPointer = 0xFF
                                       };
    }
}