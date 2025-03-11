namespace Atari2600.Emulator.Memory;

internal abstract class MemoryBase : IMemory {
    public (byte Value, ushort StackPointer) PopStack(ushort ptr) {
        byte Value = this.ReadByte(ptr);
        int NewPointer = ptr + 1;
        return (Value, NewPointer > 255 ? (ushort)0 : (ushort)NewPointer);
    }

    public (ushort Address, ushort StackPointer) PopStackAddress(ushort ptr) {
        (byte Low, ushort OutPtr) = this.PopStack(ptr);
        (byte High, OutPtr) = this.PopStack(OutPtr);

        return (AddressUtils.AddressFromBytes([Low, High]), OutPtr);
    }

    public ushort PushStack(ushort ptr, byte b) {
        this.WriteByte(ptr, b);
        int NewPointer = ptr - 1;

        return NewPointer < 0 ? (ushort)0xff : (ushort)NewPointer;
    }

    public ushort PushStack(ushort ptr, ushort b) {
        byte High = (byte)(b >>> 8);
        byte Low = (byte)(b & 0xFF);
        ushort OutPtr = this.PushStack(ptr, High);
        OutPtr = this.PushStack(OutPtr, Low);
        return OutPtr;
    }

    public ushort ReadAddress(ushort address) {
        byte Low = this.ReadByte(address);
        byte High = this.ReadByte((ushort)(address + 1));
        return AddressUtils.AddressFromBytes([Low, High]);
    }

    public abstract byte ReadByte(ushort address);

    public abstract void WriteByte(ushort address, byte b);
}