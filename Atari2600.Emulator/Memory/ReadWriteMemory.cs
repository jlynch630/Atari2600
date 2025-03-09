namespace Atari2600.Emulator.Memory;

internal class ReadWriteMemory(ushort capacity) : MemoryBase {
    private readonly byte[] Memory = new byte[capacity];

    public ReadOnlyMemory AsReadOnly() => new(this.Memory);

    public override byte ReadByte(ushort address) => this.Memory[address];

    public override void WriteByte(ushort address, byte b) => this.Memory[address] = b;
}