namespace Atari2600.Emulator.Debugging;

using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;

public class Debugger : DebugAdapterBase, IDisposable, IAsyncDisposable {
    private readonly int[] AddressToLineMap;
    private readonly Emulator Emulator;
    private readonly int[] LineToAddressMap;
    private StoppedEvent.ReasonValue? PendingReason;

    public Debugger(Emulator emulator, Stream stdIn, Stream stdOut) {
        this.Emulator = emulator;
        this.LineToAddressMap = new int[this.Emulator.Instructions.Length];
        this.AddressToLineMap = new int[this.Emulator.Instructions.Sum(a => a.ByteLength)];
        int Total = 0;
        for (int i = 0; i < this.Emulator.Instructions.Length; i++) {
            this.LineToAddressMap[i] = Total;
            this.AddressToLineMap[Total] = i;
            Total += this.Emulator.Instructions[i].ByteLength;
        }

        this.Emulator.Broken += this.Emulator_Broken;
        this.InitializeProtocolClient(stdIn, stdOut);
    }

    public async ValueTask DisposeAsync() {
        await this.Emulator.DisposeAsync();
    }

    public void Dispose() {
        this.Emulator.Broken -= this.Emulator_Broken;
    }

    public static Task StartDebugging(Emulator emulator, int port) {
        return Task.Run(() => {
            TcpListener Listener = new(IPAddress.Loopback, port);
            Listener.Start();
            while (Listener.AcceptSocket() is { } ClientSocket) {
                using NetworkStream Stream = new(ClientSocket);
                Debugger Debugger = new(emulator, Stream, Stream);
                Debugger.Protocol.LogMessage += (_, e) => Console.WriteLine(e.Message);
                Debugger.Protocol.DispatcherError += (_, e) => Console.Error.WriteLine(e.Exception.Message);
                Debugger.Protocol.Run();
                Debugger.Protocol.WaitForReader();
            }
        });
    }

    protected override AttachResponse HandleAttachRequest(AttachArguments arguments) =>
        // this.Protocol.SendEvent(new LoadedSourceEvent(LoadedSourceEvent.ReasonValue.New,
        //     new Source("AtariGame", null, 1, Source.PresentationHintValue.Emphasize, "Disassembly")));
        new();

    protected override BreakpointLocationsResponse HandleBreakpointLocationsRequest(
        BreakpointLocationsArguments arguments) =>
        new() {
                  Breakpoints = [new BreakpointLocation(arguments.Line)]
              };

    protected override ContinueResponse HandleContinueRequest(ContinueArguments arguments) {
        this.Emulator.Continue();
        this.Protocol.SendEvent(new ContinuedEvent(0) { AllThreadsContinued = true });
        return new ContinueResponse();
    }

    protected override DataBreakpointInfoResponse
        HandleDataBreakpointInfoRequest(DataBreakpointInfoArguments arguments) =>
        new() {
                  AccessTypes = [DataBreakpointAccessType.Read],
                  CanPersist = false,
                  DataId = null,
                  Description = "Unavailable"
              };

    protected override DisconnectResponse HandleDisconnectRequest(DisconnectArguments arguments) => new();

    protected override EvaluateResponse HandleEvaluateRequest(EvaluateArguments arguments) {
        if (String.IsNullOrWhiteSpace(arguments.Expression)) {
            return new EvaluateResponse {
                                            VariablesReference = 0,
                                            Result = ""
                                        };
        }

        byte[] Out = new byte[2];
        Convert.FromHexString(arguments.Expression[2..], Out, out _, out _);
        return new EvaluateResponse {
                                        VariablesReference = 0,
                                        MemoryReference = arguments.Expression,
                                        Result = this.Emulator.State.Memory
                                                     .ReadByte(
                                                         BitConverter.ToUInt16(Out))
                                                     .ToString("X2")
                                    };
    }

    protected override InitializeResponse HandleInitializeRequest(InitializeArguments arguments) {
        this.Protocol.SendEvent(new InitializedEvent());
        return new InitializeResponse {
                                          SupportsANSIStyling = true,
                                          SupportsBreakpointLocationsRequest = true,
                                          SupportsDataBreakpoints = true,
                                          SupportsDataBreakpointBytes = true,
                                          SupportsDisassembleRequest = true,
                                          SupportsEvaluateForHovers = true,
                                          SupportsLoadedSourcesRequest = true,
                                          SupportsReadMemoryRequest = true,
                                          SupportsSetVariable = true,
                                          SupportsTerminateRequest = true,
                                          SupportsWriteMemoryRequest = true,
                                          SupportSuspendDebuggee = true,
                                          SupportsExceptionConditions = false,
                                          SupportsFunctionBreakpoints = false,
                                          SupportsConditionalBreakpoints = false,
                                          SupportsHitConditionalBreakpoints = false,
                                          SupportsValueFormattingOptions = true
                                      };
    }

    protected override LoadedSourcesResponse HandleLoadedSourcesRequest(LoadedSourcesArguments arguments) =>
        new() {
                  Sources = [
                      new Source {
                                     Name = "Disassembly",
                                     Path = "Disassembly",
                                     Origin = "Disassembly",
                                     PresentationHint = Source.PresentationHintValue.Emphasize,
                                     SourceReference = 1
                                 }
                  ]
              };

    protected override PauseResponse HandlePauseRequest(PauseArguments arguments) {
        this.PendingReason = StoppedEvent.ReasonValue.Pause;
        this.Emulator.Break();
        return new PauseResponse();
    }

    protected override ScopesResponse HandleScopesRequest(ScopesArguments arguments) =>
        new() {
                  Scopes = [
                      new Scope("Status", 1, false) {
                                                        NamedVariables = 7,
                                                        PresentationHint = Scope.PresentationHintValue.AutoRegisters
                                                    },
                      new Scope("Registers", 2, false) {
                                                           NamedVariables = 6,
                                                           PresentationHint = Scope.PresentationHintValue.Registers
                                                       },
                      new Scope("TIA", 4, true) {
                                                    NamedVariables = 1,
                                                    PresentationHint = Scope.PresentationHintValue.Locals
                                                },
                      new Scope("Memory", 3, true) {
                                                       IndexedVariables = 128,
                                                       PresentationHint = Scope.PresentationHintValue.Locals
                                                   }
                  ]
              };

    protected override SetBreakpointsResponse HandleSetBreakpointsRequest(SetBreakpointsArguments arguments) {
        if (arguments.Source.SourceReference != 1) {
            return new SetBreakpointsResponse {
                                                  Breakpoints = arguments.Breakpoints.Select(a =>
                                                                             new Breakpoint(false)
                                                                             { Reason = Breakpoint.ReasonValue.Failed })
                                                                         .ToList()
                                              };
        }

        this.Emulator.BreakAt = arguments.Breakpoints.Select(b => (ushort)this.LineToAddressMap[b.Line - 1]).ToArray();
        return new SetBreakpointsResponse {
                                              Breakpoints = arguments.Breakpoints.Select(a => new Breakpoint(true) {
                                                                                 Line = a.Line,
                                                                                 InstructionReference =
                                                                                     this.LineToAddressMap[a.Line - 1]
                                                                                         .ToString("X2"),
                                                                                 Id = this.LineToAddressMap[a.Line - 1],
                                                                                 Offset = 0,
                                                                                 Source = arguments.Source
                                                                             })
                                                                     .ToList()
                                          };
    }

    protected override SetDataBreakpointsResponse
        HandleSetDataBreakpointsRequest(SetDataBreakpointsArguments arguments) =>
        new() {
                  Breakpoints = arguments.Breakpoints.Select(a => new Breakpoint(true))
                                         .ToList()
              };

    protected override SetExceptionBreakpointsResponse
        HandleSetExceptionBreakpointsRequest(SetExceptionBreakpointsArguments arguments) =>
        new() { Breakpoints = [] };

    protected override SourceResponse HandleSourceRequest(SourceArguments arguments) {
        if (arguments.SourceReference != 1) throw new ApplicationException("Source not found");
        return new SourceResponse {
                                      MimeType = "text/plain",
                                      Content = String.Join('\n',
                                          this.Emulator.Instructions.Select((i, idx) =>
                                              $"L{this.LineToAddressMap[idx]:X4}: {i}"))
                                  };
    }

    protected override StackTraceResponse HandleStackTraceRequest(StackTraceArguments arguments) =>
        new() {
                  TotalFrames = 1,
                  StackFrames = [
                      new StackFrame(0, "Root", 0, 0) {
                                                          InstructionPointerReference =
                                                              (this.Emulator.State.ProgramCounter & 0xfff).ToString(
                                                                  "X2"),
                                                          Line = this.AddressToLineMap[
                                                              this.Emulator.State.ProgramCounter & 0xfff] + 1,
                                                          PresentationHint = StackFrame.PresentationHintValue.Label,
                                                          Source = new Source {
                                                                                  Name = "Disassembly",
                                                                                  Path = "Disassembly",
                                                                                  Origin = "Disassembly",
                                                                                  PresentationHint =
                                                                                      Source.PresentationHintValue
                                                                                          .Emphasize,
                                                                                  SourceReference = 1
                                                                              }
                                                      }
                  ]
              };

    protected override StepInResponse HandleStepInRequest(StepInArguments arguments) {
        this.PendingReason = StoppedEvent.ReasonValue.Step;
        this.Emulator.DebugStepNext();
        return new StepInResponse();
    }

    protected override ThreadsResponse HandleThreadsRequest(ThreadsArguments arguments) =>
        new() {
                  Threads = [new Thread(0, "Main")]
              };

    protected override VariablesResponse HandleVariablesRequest(VariablesArguments arguments) {
        string FormatStr = arguments.Format?.Hex ?? false ? "X2" : "X2";
        return arguments.VariablesReference switch {
            1 => new VariablesResponse {
                                           Variables = [
                                               new Variable("Carry",
                                                   this.Emulator.State.StatusRegister.Carry ? "True" : "False", 0),
                                               new Variable("Zero",
                                                   this.Emulator.State.StatusRegister.ZeroResult ? "True" : "False", 0),
                                               new Variable("Interrupt Disable",
                                                   this.Emulator.State.StatusRegister.InterruptDisable
                                                       ? "True"
                                                       : "False", 0),
                                               new Variable("Decimal Mode",
                                                   this.Emulator.State.StatusRegister.DecimalMode ? "True" : "False",
                                                   0),
                                               new Variable("Break",
                                                   this.Emulator.State.StatusRegister.BreakCommand ? "True" : "False",
                                                   0),
                                               new Variable("Overflow",
                                                   this.Emulator.State.StatusRegister.Overflow ? "True" : "False", 0),
                                               new Variable("Negative",
                                                   this.Emulator.State.StatusRegister.NegativeResult ? "True" : "False",
                                                   0)
                                           ]
                                       },
            2 => new VariablesResponse {
                                           Variables = [
                                               new Variable("Status Register",
                                                   this.Emulator.State.StatusRegister.Value.ToString(FormatStr), 0),
                                               new Variable("Accumulator",
                                                   this.Emulator.State.Accumulator.Value.ToString(FormatStr), 0),
                                               new Variable("Program Counter",
                                                   (this.Emulator.State.ProgramCounter & 0xfff).ToString(FormatStr), 0),
                                               new Variable("Stack Pointer",
                                                   this.Emulator.State.StackPointer.ToString(FormatStr), 0),
                                               new Variable("X",
                                                   this.Emulator.State.XIndex.Value.ToString(FormatStr), 0),
                                               new Variable("Y",
                                                   this.Emulator.State.YIndex.Value.ToString(FormatStr), 0)
                                           ]
                                       },
            3 => new VariablesResponse {
                                           Variables = Enumerable.Range(0x80, 128).Select(i =>
                                               new Variable(i.ToString("X2"),
                                                   this.Emulator.State.Memory.ReadByte((ushort)i).ToString(FormatStr),
                                                   0)).ToList()
                                       },

            4 => new VariablesResponse {
                                           Variables = [
                                               new Variable("Beam Position",
                                                   this.Emulator.Tia.ElectronBeamX + ", " +
                                                   this.Emulator.Tia.ElectronBeamY, 0)
                                           ]
                                       },
            _ => throw new ApplicationException("Unsupported value")
        };
    }

    private void Emulator_Broken(object? sender, EventArgs e) {
        this.Protocol.SendEvent(new StoppedEvent(this.PendingReason ?? StoppedEvent.ReasonValue.Breakpoint) {
                                        AllThreadsStopped = true,
                                        HitBreakpointIds = [this.Emulator.State.ProgramCounter & 0xfff],
                                        ThreadId = 0
                                    });
        this.PendingReason = null;
    }
}