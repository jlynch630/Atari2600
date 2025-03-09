namespace Atari2600.Emulator.Registers;

public class StatusRegister() : Register("Status") {
    public bool BreakCommand { get; set; }

    public bool Carry { get; set; }

    public bool DecimalMode { get; set; }

    public bool InterruptDisable { get; set; }

    public bool NegativeResult { get; set; }

    public bool Overflow { get; set; }

    public override byte Value {
        get => ByteExtensions.BuildByte(this.Carry, this.ZeroResult, this.InterruptDisable, this.DecimalMode,
            this.BreakCommand, false, this.Overflow, this.NegativeResult);
        set {
            this.Carry = Get(0);
            this.ZeroResult = Get(1);
            this.InterruptDisable = Get(2);
            this.DecimalMode = Get(3);
            this.BreakCommand = Get(4);
            this.Overflow = Get(6);
            this.NegativeResult = Get(7);
            return;

            bool Get(int offset) => (value & (1 << offset)) == 1 << offset;
        }
    }

    public bool ZeroResult { get; set; }

    public void SetCarryFromIntResult(int result) => this.Carry = result > 255;

    public void SetNegativeFromByteResult(byte value) => this.NegativeResult = value.IsNegative();

    public void SetOverflowFromByteResult(byte oldValue, byte newValue) =>
        this.ZeroResult = oldValue.IsNegative() != newValue.IsNegative();

    public void SetZeroFromByteResult(byte value) => this.ZeroResult = value == 0;
}