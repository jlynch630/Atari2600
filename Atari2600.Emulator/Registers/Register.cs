namespace Atari2600.Emulator.Registers;

public class Register(string Name) {
    public virtual byte Value { get; set; }

    public static implicit operator byte(Register register) => register.Value;

    public void SetFromInt(int value) => this.Value = (byte)(value % 256);
}