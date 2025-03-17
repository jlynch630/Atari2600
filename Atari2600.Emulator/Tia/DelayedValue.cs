namespace Atari2600.Emulator.Tia;

internal class DelayedValue<T>(int delay) {
    private T? CurrentValue;
    private int LeftToSwitch;
    private T? PendingValue;

    public T? Value {
        get => this.CurrentValue;
        set {
            this.PendingValue = value;
            this.LeftToSwitch = delay;
        }
    }

    public static implicit operator T?(DelayedValue<T> val) => val.Value;

    public void Step() {
        switch (this.LeftToSwitch) {
            case < 0:
                return;
            case 0:
                this.CurrentValue = this.PendingValue;
                this.PendingValue = default(T);
                break;
        }

        this.LeftToSwitch--;
    }
}