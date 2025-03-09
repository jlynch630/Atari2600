namespace Atari2600.Emulator.Memory;

internal static class AtariMap {
    public static MappedMemory CreateAtariMap(IMemory tiaRegisters, IMemory piaRegisters, IMemory ram,
                                              IMemory programData) {
        MappedMemory Map = new();

        /*
         ***************************************************
         * $0000-$003F = TIA Addresses $00-$3F (zero page) *
         * ----------------------------------------------- *
         *                                                 *
         *     mirror: $xyz0                               *
         *                                                 *
         *     x = {even}                                  *
         *     y = {anything}                              *
         *     z = {0, 4}                                  *
         *                                                 *
         ***************************************************
         */

        for (int X = 0; X < 16; X += 2)
            for (int Y = 0; Y < 16; Y++) {
                Map.AddMemory((ushort)(X * 4096 + Y * 256 + 0x00), 0x40, tiaRegisters);
                Map.AddMemory((ushort)(X * 4096 + Y * 256 + 0x40), 0x40, tiaRegisters);
            }

        /*
         **************************************
         * $0080-$00FF = RIOT RAM (zero page) *
         * ---------------------------------- *
         *                                    *
         *     mirror: $xy80                  *
         *                                    *
         *     x = {even}                     *
         *     y = {0,1,4,5,8,9,$C,$D}        *
         *                                    *
         **************************************
         */
        int[] RamYs = [0, 1, 4, 5, 8, 9, 0xc, 0xd];
        for (int X = 0; X < 16; X += 2)
            foreach (int Y in RamYs)
                Map.AddMemory((ushort)(X * 4096 + Y * 256 + 0x80), 0x80, ram);

        /*
         *****************************************
         * $1000-$1FFF = ROM Addresses $000-$FFF *
         * ------------------------------------- *
         *                                       *
         *     mirror: $x000                     *
         *                                       *
         *     x = {odd}                         *
         *                                       *
         *****************************************
         */
        for (int i = 0x1000; i < 0xffff; i += 0x2000)
            Map.AddMemory((ushort)i, 0x1000, programData);

        /*
         ****************************************
         * $0280-$029F = RIOT Addresses $00-$1F *
         * ------------------------------------ *
         *                                      *
         *     mirror: $xyz0                    *
         *                                      *
         *     x = {even}                       *
         *     y = {2,3,6,7,$A,$B,$E,$F}        *
         *     z = {8,$A,$C,$E}                 *
         *                                      *
         ****************************************
         */
        int[] PiaYs = [2, 3, 6, 7, 0xa, 0xb, 0xe, 0xf];
        int[] PiaZs = [8, 0xa, 0xc, 0xe];
        for (int X = 0; X < 16; X += 2)
            foreach (int Y in PiaYs)
                foreach (int Z in PiaZs)
                    Map.AddMemory((ushort)(X * 4096 + Y * 256 + Z * 16), 32, piaRegisters);
        return Map;
    }
}