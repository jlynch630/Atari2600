namespace Atari2600.Emulator.Tia;

internal class CollisionRegistry {
    private bool BLPF;
    private bool M0BL;
    private bool M0M1;
    private bool M0P0;
    private bool M0P1;
    private bool M0PF;
    private bool M1BL;
    private bool M1P0;
    private bool M1P1;
    private bool M1PF;
    private bool P0BL;
    private bool P0P1;
    private bool P0PF;
    private bool P1BL;
    private bool P1PF;

    public void DetermineCollisions(bool ball, bool player0, bool player1, bool missile0, bool missile1,
                                    bool playField) {
        if (missile0 && player1) this.M0P1 = true;
        if (missile0 && player0) this.M0P0 = true;
        if (missile1 && player0) this.M1P0 = true;
        if (missile1 && player1) this.M1P1 = true;

        if (player0 && playField) this.P0PF = true;
        if (player0 && ball) this.P0BL = true;
        if (player1 && playField) this.P1PF = true;
        if (player1 && ball) this.P1BL = true;

        if (missile0 && playField) this.M0PF = true;
        if (missile0 && ball) this.M0BL = true;
        if (missile1 && playField) this.M1PF = true;
        if (missile1 && ball) this.M1BL = true;

        if (ball && playField) this.BLPF = true;
        if (player0 && player1) this.P0P1 = true;
        if (missile0 && missile1) this.M0M1 = true;
    }

    public byte ReadAddress(int address) {
        return address switch {
            0 => CollisionRegistry.BuildByte(this.M0P1, this.M0P0),
            1 => CollisionRegistry.BuildByte(this.M1P0, this.M1P1),
            2 => CollisionRegistry.BuildByte(this.P0PF, this.P0BL),
            3 => CollisionRegistry.BuildByte(this.P1PF, this.P1BL),
            4 => CollisionRegistry.BuildByte(this.M0PF, this.M0BL),
            5 => CollisionRegistry.BuildByte(this.M1PF, this.M1BL),
            6 => CollisionRegistry.BuildByte(this.BLPF, false),
            7 => CollisionRegistry.BuildByte(this.P0P1, this.M0M1),
            _ => 0
        };
    }

    public void ResetCollisions() {
        this.BLPF = false;
        this.M0BL = false;
        this.M0M1 = false;
        this.M0P0 = false;
        this.M0P1 = false;
        this.M0PF = false;
        this.M1BL = false;
        this.M1P0 = false;
        this.M1P1 = false;
        this.M1PF = false;
        this.P0BL = false;
        this.P0P1 = false;
        this.P0PF = false;
        this.P1BL = false;
        this.P1PF = false;
    }

    private static byte BuildByte(bool d7, bool d6) =>
        ByteExtensions.BuildByte(false, false, false, false, false, false, d6, d7);
}