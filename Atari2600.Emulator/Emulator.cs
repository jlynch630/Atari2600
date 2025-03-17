namespace Atari2600.Emulator;

using System.Reflection;
using Disassembler;
using Memory;
using Operations;
using Pia.IO;
using Tia;

public interface IController : ITiaInput, IIOPort { }

public class Emulator : IDisposable, IAsyncDisposable {
    private static readonly Dictionary<OperationType, IOperation> Operations;

    public bool IsBroken;
    public int ShouldBreakIn = -1;
    internal readonly Tia.Tia Tia;

    private readonly Pia.Pia Pia;
    private readonly int ProgramDataLength;
    private readonly MemoryStream ProgramDataStream;
    private bool ForceSkipBreak;
    private Instruction? Prev;
    private int WaitCycles;

    static Emulator() {
        Emulator.Operations = new Dictionary<OperationType, IOperation>(Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t is { Namespace: "Atari2600.Emulator.Operations", Name.Length: 3 }).Select(t => {
                IOperation? Op = (IOperation?)Activator.CreateInstance(t);
                if (Op is null) throw new Exception("Null operation found: " + t.Name);
                return new KeyValuePair<OperationType, IOperation>(Op.Type, Op);
            }));
    }

    public Emulator(byte[] programData, IController controller, IGraphicsBackend graphics) {
        ConsoleSwitches Switches = new();
        this.ProgramDataStream = new MemoryStream(programData);
        this.Instructions = Disassembler.ReadFromStream(this.ProgramDataStream);
        this.ProgramDataStream.Seek(0, SeekOrigin.Begin);
        this.ProgramDataLength = programData.Length;
        this.Pia = new Pia.Pia(Switches, controller);
        this.Tia = new Tia.Tia(graphics, controller);
        this.State = this.InitializeState(programData);
    }

    public ushort[] BreakAt { get; set; } = [];

    public Instruction[] Instructions { get; }

    public EmulationState State { get; }

    public async ValueTask DisposeAsync() {
        await this.ProgramDataStream.DisposeAsync();
    }

    public void Dispose() {
        this.ProgramDataStream.Dispose();
    }

    public static Emulator FromFile(string path, IController controller, IGraphicsBackend graphics) =>
        new(File.ReadAllBytes(path), controller, graphics);

    public void Break() {
        this.ShouldBreakIn = 0;
    }

    public void Continue() {
        this.ForceSkipBreak = true;
        this.IsBroken = false;
    }

    public void DebugStepNext() {
        this.ForceSkipBreak = true;
        this.ShouldBreakIn = 1;
        this.IsBroken = false;
    }

    public void Step() {
        if (this.IsBroken) return;
        this.Pia.Step(); // todo, does this pause on wsync?
        this.Tia.Step();
        this.Tia.Step();
        this.Tia.Step();
        if (this.WaitCycles > 0) {
            // Console.WriteLine("wait");
            this.WaitCycles--;
            return;
        }

        if (this.Tia.IsWaitingForSync) return;

        // great!
        Instruction? Next = this.GetNextInstruction();
        if (Next is null) return;
        //Console.WriteLine("{0}: {1}", this.State.ProgramCounter & 0x0FFF, Next);
        if (this.ShouldBreakIn == 0 || (!this.ForceSkipBreak &&
                                        this.BreakAt.Any(b => (this.State.ProgramCounter & 0x0fff) == (b & 0x0fff)))) {
            //Debugger.Break();
            this.ShouldBreakIn = -1;
            this.IsBroken = true;
            this.Broken?.Invoke(null, EventArgs.Empty);
            return;
        }

        this.ForceSkipBreak = false;

        this.WaitCycles = this.ExecuteInstruction(Next) - 1; // this, of course, counts as one cycle
        this.Prev = Next;

        if (this.ShouldBreakIn > 0) this.ShouldBreakIn--;
    }

    private int ExecuteInstruction(Instruction instruction) {
        this.State.ProgramCounter = (ushort)(this.State.ProgramCounter + instruction.ByteLength);
        IOperation MatchingOp = Emulator.Operations[instruction.OperationType];
        return MatchingOp.Apply(this.State, instruction);
    }

    private Instruction? GetNextInstruction() {
        this.ProgramDataStream.Seek(this.State.ProgramCounter % this.ProgramDataLength, SeekOrigin.Begin);
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

    public event EventHandler? Broken;
}