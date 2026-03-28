using System.Numerics;
using System.Collections.Generic;
using ImpostersOrdeal.Utils;
using static ImpostersOrdeal.JsonConverterStructs;
using System.Linq;
using static ImpostersOrdeal.GameDataTypes;

namespace ImpostersOrdeal
{
    public class TrainerTable : ScriptableObject
    {
        public List<SheetTrainerType> TrainerType = new List<SheetTrainerType>();
        public List<SheetTrainerData> TrainerData = new List<SheetTrainerData>();
        public List<SealTemplate> SealTemplates = new List<SealTemplate>();
        public List<SheetSkirtGraphicsChara> SkirtGraphicsChara = new List<SheetSkirtGraphicsChara>();

        public class SheetTrainerType
        {
            public int TypeID; // TrainerType
            public string LabelTrType;
            public byte Sex;
            public byte Group;
            public byte BallId; // BallId
            public List<string> FieldEncount = new List<string>();
            public List<int> BtlEffId = new List<int>(); // BattleSetupEffectId
            public string EyeBgm;
            public string ModelID;
            public byte Hand; // HandDominance
            public byte HoldBallHand; // HandDominance
            public byte HelpHand; // HandDominance
            public byte HelpHoldBallHand; // HandDominance
            public float ThrowTime;
            public float CaptureThrowTime;
            public float LoseLoopTime;
            public string TrainerEffect;
            public byte Age; // TrainerAge

            // TODO: This would be different if someone changes the trainer types, see if there's an alternative
            public int GetTypeTheme()
            {
                return TypeID switch
                {
                    80 => 1,
                    69 => 4,
                    65 => 5,
                    68 => 6,
                    81 => 7,
                    67 => 8,
                    70 => 9,
                    79 => 10,
                    78 => 11,
                    83 => 12,
                    71 => 13,
                    82 => 14,
                    _ => -1,
                };
            }
        }

        public class SheetTrainerData
        {
            public int TypeID; // TrainerType
            public byte ColorID;
            public byte FightType;
            public int ArenaID; // ArenaID
            public int EffectID; // EffectBattleID
            public byte Gold;

            // Maximum is 4
            public List<ushort> UseItem = new List<ushort>();

            public bool HpRecoverFlag;
            public ushort GiftItem;
            public string NameLabel;
            public string MsgFieldPokeOne;
            public string MsgFieldBefore;
            public string MsgFieldRevenge;
            public string MsgFieldAfter;
            public List<string> MsgBattle = new List<string>();
            public List<string> SeqBattle = new List<string>();
            public uint AIBit;

            // Maximum is 6
            public List<TrainerPoke> Pokes = new List<TrainerPoke>();

            // Maximum is 5
            public List<int> Rematches = new List<int>();

            public bool[] AIFlags { get => AIBit.GetBitArray(); set => AIBit = value.ConvertBitArrayToUint(); }

            public double AverageLevel => Pokes.Count == 0 ? 0 : Pokes.Average(p => p.Level);

            public bool ItemFlag { get => AIFlags[5]; set => AIFlags[5] = value; }

            public class TrainerPoke
            {
                public ushort MonsNo;
                public ushort FormNo;
                public bool IsRare;
                public byte Level;
                public byte Sex; // Sex
                public byte Seikaku;
                public ushort Tokusei;
                public ushort Waza1;
                public ushort Waza2;
                public ushort Waza3;
                public ushort Waza4;
                public ushort Item;
                public byte Ball; // BallId
                public int Seal; // SealTemplateID
                public byte TalentHp;
                public byte TalentAtk;
                public byte TalentDef;
                public byte TalentSpAtk;
                public byte TalentSpDef;
                public byte TalentAgi;
                public byte EffortHp;
                public byte EffortAtk;
                public byte EffortDef;
                public byte EffortSpAtk;
                public byte EffortSpDef;
                public byte EffortAgi;

                public byte[] IVs
                {
                    get => [TalentHp, TalentAtk, TalentDef, TalentSpAtk, TalentSpDef, TalentAgi];
                    set
                    {
                        TalentHp = value[0];
                        TalentAtk = value[1];
                        TalentDef = value[2];
                        TalentSpAtk = value[3];
                        TalentSpDef = value[4];
                        TalentAgi = value[5];
                    }
                }
                public byte[] EVs
                {
                    get => [EffortHp, EffortAtk, EffortDef, EffortSpAtk, EffortSpDef, EffortAgi];
                    set
                    {
                        EffortHp = value[0];
                        EffortAtk = value[1];
                        EffortDef = value[2];
                        EffortSpAtk = value[3];
                        EffortSpDef = value[4];
                        EffortAgi = value[5];
                    }
                }

                public int TotalIVs => TalentHp + TalentAtk + TalentDef + TalentSpAtk + TalentSpDef + TalentAgi;
                public int TotalEVs => EffortHp + EffortAtk + EffortDef + EffortSpAtk + EffortSpDef + EffortAgi;

                public double AverageIVs => TotalIVs / 6;
                public double AverageEVs => TotalEVs / 6;

                public ushort[] Moves
                {
                    get => [Waza1, Waza2, Waza3, Waza4];
                    set
                    {
                        Waza1 = (ushort)(value.Length >= 1 ? value[0] : 0);
                        Waza2 = (ushort)(value.Length >= 2 ? value[1] : 0);
                        Waza3 = (ushort)(value.Length >= 3 ? value[2] : 0);
                        Waza4 = (ushort)(value.Length >= 4 ? value[3] : 0);
                    }
                }

                public TrainerPoke Clone()
                {
                    return new()
                    {
                        MonsNo = MonsNo,
                        FormNo = FormNo,
                        IsRare = IsRare,
                        Level = Level,
                        Sex = Sex,
                        Seikaku = Seikaku,
                        Tokusei = Tokusei,
                        Waza1 = Waza1,
                        Waza2 = Waza2,
                        Waza3 = Waza3,
                        Waza4 = Waza4,
                        Item = Item,
                        Ball = Ball,
                        Seal = Seal,
                        TalentHp = TalentHp,
                        TalentAtk = TalentAtk,
                        TalentDef = TalentDef,
                        TalentSpAtk = TalentSpAtk,
                        TalentSpDef = TalentSpDef,
                        TalentAgi = TalentAgi,
                        EffortHp = EffortHp,
                        EffortAtk = EffortAtk,
                        EffortDef = EffortDef,
                        EffortSpAtk = EffortSpAtk,
                        EffortSpDef = EffortSpDef,
                        EffortAgi = EffortAgi,
                    };
                }
            }
        }

        public class SealTemplate
        {
            // Maximum is 20
            public List<Seal> Seals = new List<Seal>();

            public class Seal
            {
                public int SealID; // SealID
                public Vector3 Pos;
            }
        }

        public class SheetSkirtGraphicsChara
        {
            public string SkirtGraphicsID;
        }
    }
}
