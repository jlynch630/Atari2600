namespace Atari2600.Emulator.Memory;

internal class MappedMemory : MemoryBase {
    private readonly List<(Func<ushort, ushort?>, IMemory)> Memories = new();

    public void AddMemory(ushort offset, ushort capacity, IMemory memory) =>
        this.Memories.Add((
            address => address >= offset && address < offset + capacity ? (ushort)(address - offset) : null, memory));

    public void AddMemory(Func<ushort, ushort?> matcher, IMemory memory) =>
        this.Memories.Add((matcher, memory));

    public override byte ReadByte(ushort address) {
        (IMemory Memory, ushort RelativeAddress) = this.FindMemory(address);
        return Memory.ReadByte(RelativeAddress);
    }

    public override void WriteByte(ushort address, byte b) {
        (IMemory Memory, ushort RelativeAddress) = this.FindMemory(address);
        Memory.WriteByte(RelativeAddress, b);
    }

    private (IMemory, ushort) FindMemory(ushort address) {
        foreach ((Func<ushort, ushort?> Matcher, IMemory Memory) in this.Memories) {
            ushort? Match = Matcher(address);
            if (Match.HasValue)
                return (Memory, Match.Value);
        }

        throw new ApplicationException("Illegal memory access: " + address);
    }
}