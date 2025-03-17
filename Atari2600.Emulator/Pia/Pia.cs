namespace Atari2600.Emulator.Pia;

using IO;
using Memory;

/// <summary>
///     The PIA chip, which includes a timer and IO ports, while obviously not "memory" in real life, can be thought of as
///     memory from the perspective of the microcontroller, since all it can do is read from or write to it
/// </summary>
internal class Pia(ConsoleSwitches switches, IIOPort portA) : MemoryBase {
    private const ushort INTIM = 0x4; // timer read
    private const ushort SWACNT = 0x1; // port a data control
    private const ushort SWCHA = 0x0; // port a read/write
    private const ushort SWCHB = 0x2; // console switch read
    private const ushort TIM1T = 0x14; // timer interval 1 cycle
    private const ushort TIM24T = 0x17; // timer interval 1024 cycles
    private const ushort TIM64T = 0x16; // timer interval 64 cycles
    private const ushort TIM8T = 0x15; // timer interval 8 cycles

    private readonly PiaTimer Timer = new();

    public override byte ReadByte(ushort address) {
        return address switch {
            Pia.INTIM => this.Timer.Value,
            Pia.SWCHA => portA.Read(),
            Pia.SWCHB => switches.Read(),
            _ => 0
        };
    }

    public void Step() {
        this.Timer.Step();
    }

    public override void WriteByte(ushort address, byte b) {
        switch (address) {
            case Pia.TIM1T:
                this.Timer.Initialize(b, 1);
                return;
            case Pia.TIM24T:
                this.Timer.Initialize(b, 1024);
                return;
            case Pia.TIM64T:
                this.Timer.Initialize(b, 64);
                return;
            case Pia.TIM8T:
                this.Timer.Initialize(b, 8);
                return;
            case Pia.SWACNT:
                portA.ConfigureInputOutput(b);
                return;
        }
    }
}