using System.Numerics;
using System.Collections.Generic;
using ImpostersOrdeal.Utils;
using static ImpostersOrdeal.JsonConverterStructs;
using System.Linq;

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
