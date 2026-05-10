using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nep3ArchipelagoClient.src.Neptunia_3_Data
{

    public struct ComboAttack
    {
        public short RushCharacterId;
        public short RushAttackId;
        public short BreakCharacterId;
        public short BreakAttackId;
        public short PowerCharacterId;
        public short PowerAttackId;
    }
    [InlineArray(5)]
    public struct ComboAttackEntry
    {
        ComboAttack entry;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Character
    {
        [FieldOffset(0x0)]
        public byte CurrentForm;
        [FieldOffset(0x8)]
        public byte Cha;
        [FieldOffset(4)]
        public int Exp; 
        [FieldOffset(12)]
        public fixed byte CharacterName[32];
        [FieldOffset(68)]
        public int CurrentHP;
        [FieldOffset(76)]
        public int CurrentSP;
        [FieldOffset(176)]
        public int Weapon;
        [FieldOffset(180)]
        public int Armor;
        [FieldOffset(184)]
        public int Ornament;
        [FieldOffset(188)]
        public int ClothingBody;
        [FieldOffset(192)]
        public int ClothingHead;
        [FieldOffset(196)]
        public int CpuC;
        [FieldOffset(200)]
        public int CpuH;
        [FieldOffset(204)]
        public int CpuB;
        [FieldOffset(208)]
        public int CpuS;
        [FieldOffset(212)]
        public int CpuW;
        [FieldOffset(216)]
        public int CpuL;
        [FieldOffset(1172)]
        public ComboAttackEntry ComboAttacks;
    }
}
