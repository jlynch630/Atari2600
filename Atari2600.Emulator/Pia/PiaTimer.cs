namespace Atari2600.Emulator.Pia;

internal class PiaTimer {
    private int CycleInterval;
    private int NumCyclesPassed;

    public bool HasTimerElapsed { get; private set; } = true;

    public byte Value { get; private set; }

    public void Initialize(byte startValue, int cycleInterval) {
        this.Value = startValue;
        this.CycleInterval = cycleInterval;
        this.HasTimerElapsed = false;
        this.NumCyclesPassed = 0;
    }

    public void Step() {
        if (this.HasTimerElapsed) {
            /* from reference guide: It holds that 0 for one interval, then the counter
               flips to FF(HEX) and decrements once each clock cycle, rather than once per
               interval. */
            this.Value--;
            return;
        }

        this.NumCyclesPassed++;
        if (this.NumCyclesPassed < this.CycleInterval) return;

        this.NumCyclesPassed = 0;
        if (this.Value == 0) this.HasTimerElapsed = true;

        this.Value--;
    }
}