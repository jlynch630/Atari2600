namespace Atari2600.Emulator.Registers;

public class Register(string name) {
    public virtual byte Value { get; set; }

    public static implicit operator byte(Register register) => register.Value;

    public void SetFromInt(int value) => this.Value = unchecked((byte)(value % 256));

    public override string ToString() => $"{name}: {this.Value}";
}