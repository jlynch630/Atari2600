namespace Atari2600.Emulator.Memory;

internal static class AtariMap {
    public static MappedMemory CreateAtariMap(IMemory tiaRegisters, IMemory piaRegisters, IMemory ram,
                                              ReadOnlyMemory programData) {
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
        Map.AddMemory(a => (a & 0x1080) == 0 ? (ushort)(a & 0x3f) : null, tiaRegisters);

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
        Map.AddMemory(a => (a & 0x1280) == 0x80 ? (ushort)(a & 0x7f) : null, ram);

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
        Map.AddMemory(a => a >= 0x1000 ? (ushort)(a % programData.Length) : null, programData);

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
        Map.AddMemory(a => (a & 0x1280) == 0x280 ? (ushort)(a & 0x1f) : null, piaRegisters);
        return Map;
    }
}