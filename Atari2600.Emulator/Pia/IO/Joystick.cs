namespace Atari2600.Emulator.Pia.IO;

public class Joystick : IController {
    [Flags]
    public enum JoystickDirection {
        Right = 0b1000,
        Left = 0b100,
        Down = 0b10,
        Up = 0b1
    }

    public JoystickDirection PlayerOne { get; set; }

    public bool PlayerOneButton { get; set; }

    public JoystickDirection PlayerTwo { get; set; }

    public bool PlayerTwoButton { get; set; }

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

    public byte ReadInput(int index) => (byte)(index == 4 ? this.PlayerOneButton ? 0 : 128 : 0);
}