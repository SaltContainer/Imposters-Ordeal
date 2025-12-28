using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class BattleTowerTable
    {
        public TowerTrainerTable TowerTrainer = new TowerTrainerTable();
        public TowerSingleStockTable TowerSingleStock = new TowerSingleStockTable();
        public TowerDoubleStockTable TowerDoubleStock = new TowerDoubleStockTable();

        public class TowerTrainerTable : ScriptableObject
        {
            public List<SheetTrainerData> TrainerData = new List<SheetTrainerData>();
            public List<SheetTrainerPoke> TrainerPoke = new List<SheetTrainerPoke>();

            public class SheetTrainerData
            {
                public int TrainerType; // TrainerType
                public string NameLabel;
                public string MsgFieldBefore;
                public List<string> MsgBattle = new List<string>();
                public List<string> SeqBattle = new List<string>();
                public byte ColorID;
                public int fldGraphic;
                public string singleEnc;
                public string doubleEnc;
            }

            public class SheetTrainerPoke
            {
                public uint ID;
                public int MonsNo; // MonsNo
                public ushort FormNo;
                public bool IsRare;
                public byte Level;
                public byte Sex; // Sex
                public int Seikaku; // Seikaku
                public int Tokusei; // TokuseiNo
                public int Waza1; // WazaNo
                public int Waza2; // WazaNo
                public int Waza3; // WazaNo
                public int Waza4; // WazaNo
                public ushort Item; // ItemNo
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

        public class TowerSingleStockTable : ScriptableObject
        {
            public List<SheetTowerSingleStock> Items = new List<SheetTowerSingleStock>();

            public class SheetTowerSingleStock
            {
                public uint ID;
                public int TrainerID; // TowerTrID
                public List<uint> PokeID = new List<uint>();
                public string BattleBGM;
                public string WinBGM;
            }
        }

        public class TowerDoubleStockTable : ScriptableObject
        {
            public List<SheetTowerDoubleStock> Items = new List<SheetTowerDoubleStock>();

            public class SheetTowerDoubleStock
            {
                public uint ID;
                public List<int> TrainerID = new List<int>(); // TowerTrID[]
                public List<uint> PokeID = new List<uint>();
                public string BattleBGM;
                public string WinBGM;
            }
        }
    }
}
