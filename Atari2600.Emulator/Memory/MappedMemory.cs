namespace Atari2600.Emulator.Memory;

internal class MappedMemory : MemoryBase {
    private readonly List<(ushort, ushort, IMemory)> Memories = new();

    public void AddMemory(ushort offset, ushort capacity, IMemory memory) =>
        this.Memories.Add((offset, capacity, memory));

    public override byte ReadByte(ushort address) {
        (IMemory Memory, _, ushort RelativeAddress) = this.FindMemory(address);
        return Memory.ReadByte(RelativeAddress);
    }

    public override void WriteByte(ushort address, byte b) {
        (IMemory Memory, _, ushort RelativeAddress) = this.FindMemory(address);
        Memory.WriteByte(RelativeAddress, b);
    }

    private (IMemory, ushort, ushort) FindMemory(ushort address) {
        // todo: seriously? this is called every single operation and you do this
        foreach ((ushort Offset, ushort Capacity, IMemory Memory) in this.Memories)
            if (address >= Offset && address < Offset + Capacity)
                return (Memory, Offset, (ushort)(address - Offset));

        throw new ApplicationException("Illegal memory access: " + address);
    }
}