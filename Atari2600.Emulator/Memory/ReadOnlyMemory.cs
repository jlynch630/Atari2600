namespace Atari2600.Emulator.Memory;

internal class ReadOnlyMemory(byte[] data) : MemoryBase {
    public int Length => data.Length;

    public override byte ReadByte(ushort address) => address >= data.Length ? (byte)0 : data[address];

    public override void WriteByte(ushort address, byte b) =>
        throw new InvalidOperationException("Cannot modify readonly memory");
}