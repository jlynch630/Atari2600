namespace Atari2600.Emulator.Memory;

public interface IMemory {
    public (byte Value, ushort StackPointer) PopStack(ushort ptr);

    public (ushort Address, ushort StackPointer) PopStackAddress(ushort ptr);

    public ushort PushStack(ushort ptr, byte b);

    public ushort PushStack(ushort ptr, ushort b);

    public ushort ReadAddress(ushort address);

    public byte ReadByte(ushort address);

    public void WriteByte(ushort address, byte b);
}