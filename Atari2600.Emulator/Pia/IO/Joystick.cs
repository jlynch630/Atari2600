namespace Atari2600.Emulator.Pia.IO;

internal class Joystick : IIOPort {
    [Flags]
    public enum JoystickDirection {
        Right = 0b1,
        Left = 0b10,
        Down = 0b100,
        Up = 0b1000
    }

    public JoystickDirection PlayerOne { get; set; }

    public JoystickDirection PlayerTwo { get; set; }

    public void ConfigureInputOutput(byte mask) {
        throw new NotSupportedException();
    }

    // switches are normally closed, hence the not
    // then, it goes [P1][P0], where each player is RLDU
    //              0        7 
    public byte Read() => (byte)~((byte)this.PlayerTwo + ((byte)this.PlayerOne << 4));

    public void Write(byte value) {
        throw new NotSupportedException();
    }
}