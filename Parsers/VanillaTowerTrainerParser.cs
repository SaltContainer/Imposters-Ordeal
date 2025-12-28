using ImpostersOrdeal.Utils;
using System.Linq;

namespace ImpostersOrdeal
{
    public class VanillaTowerTrainerParser : IParser<BattleTowerTable>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (BattleTowerTable)data);

        // Monos
        private static readonly string TOWERTRAINERTABLE_MONONAME = "TowerTrainerTable";
        private static readonly string TOWERSINGLESTOCKTABLE_MONONAME = "TowerSingleStockTable";
        private static readonly string TOWERDOUBLESTOCKTABLE_MONONAME = "TowerDoubleStockTable";

        // Arrays
        private static readonly string TOWERDOUBLESTOCK_FIELD = "TowerDoubleStock";
        private static readonly string TOWERSINGLESTOCK_FIELD = "TowerSingleStock";
        private static readonly string TRAINERDATA_FIELD = "TrainerData";
        private static readonly string TRAINERPOKE_FIELD = "TrainerPoke";

        // Fields
        private static readonly string BALL_FIELD = "Ball";
        private static readonly string BATTLEBGM_FIELD = "BattleBGM";
        private static readonly string COLORID_FIELD = "ColorID";
        private static readonly string DOUBLEENC_FIELD = "doubleEnc";
        private static readonly string EFFORTAGI_FIELD = "EffortAgi";
        private static readonly string EFFORTATK_FIELD = "EffortAtk";
        private static readonly string EFFORTDEF_FIELD = "EffortDef";
        private static readonly string EFFORTHP_FIELD = "EffortHp";
        private static readonly string EFFORTSPATK_FIELD = "EffortSpAtk";
        private static readonly string EFFORTSPDEF_FIELD = "EffortSpDef";
        private static readonly string FLDGRAPHIC_FIELD = "fldGraphic";
        private static readonly string FORMNO_FIELD = "FormNo";
        private static readonly string ID_FIELD = "ID";
        private static readonly string ISRARE_FIELD = "IsRare";
        private static readonly string ITEM_FIELD = "Item";
        private static readonly string LEVEL_FIELD = "Level";
        private static readonly string MNAME_FIELD = "m_Name";
        private static readonly string MONSNO_FIELD = "MonsNo";
        private static readonly string MSGBATTLE_FIELD = "MsgBattle";
        private static readonly string MSGFIELDBEFORE_FIELD = "MsgFieldBefore";
        private static readonly string NAMELABEL_FIELD = "NameLabel";
        private static readonly string POKEID_FIELD = "PokeID";
        private static readonly string SEAL_FIELD = "Seal";
        private static readonly string SEQBATTLE_FIELD = "SeqBattle";
        private static readonly string SEIKAKU_FIELD = "Seikaku";
        private static readonly string SEX_FIELD = "Sex";
        private static readonly string SINGLEENC_FIELD = "singleEnc";
        private static readonly string TALENTAGI_FIELD = "TalentAgi";
        private static readonly string TALENTATK_FIELD = "TalentAtk";
        private static readonly string TALENTDEF_FIELD = "TalentDef";
        private static readonly string TALENTHP_FIELD = "TalentHp";
        private static readonly string TALENTSPATK_FIELD = "TalentSpAtk";
        private static readonly string TALENTSPDEF_FIELD = "TalentSpDef";
        private static readonly string TOKUSEI_FIELD = "Tokusei";
        private static readonly string TRAINERID_FIELD = "TrainerID";
        private static readonly string TRAINERTYPE_FIELD = "TrainerType";
        private static readonly string WAZA1_FIELD = "Waza1";
        private static readonly string WAZA2_FIELD = "Waza2";
        private static readonly string WAZA3_FIELD = "Waza3";
        private static readonly string WAZA4_FIELD = "Waza4";
        private static readonly string WINBGM_FIELD = "WinBGM";

        public BattleTowerTable ParseFromSources(FileManager fileManager)
        {
            var data = new BattleTowerTable();

            var dprMasterdatasBundle = fileManager.GetDprMasterdatasBundle();
            var (trainerPathID, trainerMono) = dprMasterdatasBundle.GetMonoByName(TOWERTRAINERTABLE_MONONAME);
            var (singlePathID, singleMono) = dprMasterdatasBundle.GetMonoByName(TOWERSINGLESTOCKTABLE_MONONAME);
            var (doublePathID, doubleMono) = dprMasterdatasBundle.GetMonoByName(TOWERDOUBLESTOCKTABLE_MONONAME);

            data.TowerTrainer = new BattleTowerTable.TowerTrainerTable();
            data.TowerTrainer.pathID = trainerPathID;
            data.TowerTrainer.m_Name = trainerMono[MNAME_FIELD].AsString;

            data.TowerTrainer.TrainerData = new();
            var trainerFields = trainerMono[TRAINERDATA_FIELD].GetArrayElements();
            foreach (var trainerField in trainerFields)
            {
                var trainer = new BattleTowerTable.TowerTrainerTable.SheetTrainerData();

                trainer.TrainerType = trainerField[TRAINERTYPE_FIELD].AsInt;
                trainer.NameLabel = trainerField[NAMELABEL_FIELD].AsString;
                trainer.MsgFieldBefore = trainerField[MSGFIELDBEFORE_FIELD].AsString;
                trainer.MsgBattle = trainerField[MSGBATTLE_FIELD].GetArrayElements().Select(f => f.AsString).ToList();
                trainer.SeqBattle = trainerField[SEQBATTLE_FIELD].GetArrayElements().Select(f => f.AsString).ToList();
                trainer.ColorID = trainerField[COLORID_FIELD].AsByte;
                trainer.fldGraphic = trainerField[FLDGRAPHIC_FIELD].AsInt;
                trainer.singleEnc = trainerField[SINGLEENC_FIELD].AsString;
                trainer.doubleEnc = trainerField[DOUBLEENC_FIELD].AsString;

                data.TowerTrainer.TrainerData.Add(trainer);
            }

            data.TowerTrainer.TrainerPoke = new();
            var pokeFields = trainerMono[TRAINERPOKE_FIELD].GetArrayElements();
            foreach (var pokeField in pokeFields)
            {
                var poke = new BattleTowerTable.TowerTrainerTable.SheetTrainerPoke();

                poke.ID = pokeField[ID_FIELD].AsUInt;
                poke.MonsNo = pokeField[MONSNO_FIELD].AsInt;
                poke.FormNo = pokeField[FORMNO_FIELD].AsUShort;
                poke.IsRare = pokeField[ISRARE_FIELD].AsBool;
                poke.Level = pokeField[LEVEL_FIELD].AsByte;
                poke.Sex = pokeField[SEX_FIELD].AsByte;
                poke.Seikaku = pokeField[SEIKAKU_FIELD].AsInt;
                poke.Tokusei = pokeField[TOKUSEI_FIELD].AsInt;
                poke.Waza1 = pokeField[WAZA1_FIELD].AsInt;
                poke.Waza2 = pokeField[WAZA2_FIELD].AsInt;
                poke.Waza3 = pokeField[WAZA3_FIELD].AsInt;
                poke.Waza4 = pokeField[WAZA4_FIELD].AsInt;
                poke.Item = pokeField[ITEM_FIELD].AsUShort;
                poke.Ball = pokeField[BALL_FIELD].AsByte;
                poke.Seal = pokeField[SEAL_FIELD].AsInt;
                poke.TalentHp = pokeField[TALENTHP_FIELD].AsByte;
                poke.TalentAtk = pokeField[TALENTATK_FIELD].AsByte;
                poke.TalentDef = pokeField[TALENTDEF_FIELD].AsByte;
                poke.TalentSpAtk = pokeField[TALENTSPATK_FIELD].AsByte;
                poke.TalentSpDef = pokeField[TALENTSPDEF_FIELD].AsByte;
                poke.TalentAgi = pokeField[TALENTAGI_FIELD].AsByte;
                poke.EffortHp = pokeField[EFFORTHP_FIELD].AsByte;
                poke.EffortAtk = pokeField[EFFORTATK_FIELD].AsByte;
                poke.EffortDef = pokeField[EFFORTDEF_FIELD].AsByte;
                poke.EffortSpAtk = pokeField[EFFORTSPATK_FIELD].AsByte;
                poke.EffortSpDef = pokeField[EFFORTSPDEF_FIELD].AsByte;
                poke.EffortAgi = pokeField[EFFORTAGI_FIELD].AsByte;

                data.TowerTrainer.TrainerPoke.Add(poke);
            }

            data.TowerSingleStock = new BattleTowerTable.TowerSingleStockTable();
            data.TowerSingleStock.pathID = trainerPathID;
            data.TowerSingleStock.m_Name = trainerMono[MNAME_FIELD].AsString;

            data.TowerSingleStock.Items = new();
            var singleFields = singleMono[TOWERSINGLESTOCK_FIELD].GetArrayElements();
            foreach (var singleField in singleFields)
            {
                var single = new BattleTowerTable.TowerSingleStockTable.SheetTowerSingleStock();

                single.ID = singleField[ID_FIELD].AsUInt;
                single.TrainerID = singleField[TRAINERID_FIELD].AsInt;
                single.PokeID = singleField[POKEID_FIELD].GetArrayElements().Select(f => f.AsUInt).ToList();
                single.BattleBGM = singleField[BATTLEBGM_FIELD].AsString;
                single.WinBGM = singleField[WINBGM_FIELD].AsString;

                data.TowerSingleStock.Items.Add(single);
            }

            data.TowerDoubleStock = new BattleTowerTable.TowerDoubleStockTable();
            data.TowerDoubleStock.pathID = trainerPathID;
            data.TowerDoubleStock.m_Name = trainerMono[MNAME_FIELD].AsString;

            data.TowerDoubleStock.Items = new();
            var doubleFields = doubleMono[TOWERDOUBLESTOCK_FIELD].GetArrayElements();
            foreach (var doubleField in doubleFields)
            {
                var doubleStock = new BattleTowerTable.TowerDoubleStockTable.SheetTowerDoubleStock();

                doubleStock.ID = doubleField[ID_FIELD].AsUInt;
                doubleStock.TrainerID = doubleField[TRAINERID_FIELD].GetArrayElements().Select(f => f.AsInt).ToList();
                doubleStock.PokeID = doubleField[POKEID_FIELD].GetArrayElements().Select(f => f.AsUInt).ToList();
                doubleStock.BattleBGM = doubleField[BATTLEBGM_FIELD].AsString;
                doubleStock.WinBGM = doubleField[WINBGM_FIELD].AsString;

                data.TowerDoubleStock.Items.Add(doubleStock);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, BattleTowerTable data)
        {
            var dprMasterdatasBundle = fileManager.GetDprMasterdatasBundle();
            var (trainerPathID, trainerMono) = dprMasterdatasBundle.GetMonoByName(TOWERTRAINERTABLE_MONONAME);
            var (singlePathID, singleMono) = dprMasterdatasBundle.GetMonoByName(TOWERSINGLESTOCKTABLE_MONONAME);
            var (doublePathID, doubleMono) = dprMasterdatasBundle.GetMonoByName(TOWERDOUBLESTOCKTABLE_MONONAME);

            trainerMono[TRAINERTYPE_FIELD].SetArrayElementsAndInit(data.TowerTrainer.TrainerData, (trainerField, trainer) =>
            {
                trainerField[TRAINERTYPE_FIELD].AsInt = trainer.TrainerType;
                trainerField[NAMELABEL_FIELD].AsString = trainer.NameLabel;
                trainerField[MSGFIELDBEFORE_FIELD].AsString = trainer.MsgFieldBefore;
                trainerField[MSGBATTLE_FIELD].SetArrayElementsAndInit(trainer.MsgBattle, (f, v) => f.AsString = v);
                trainerField[SEQBATTLE_FIELD].SetArrayElementsAndInit(trainer.SeqBattle, (f, v) => f.AsString = v);
                trainerField[COLORID_FIELD].AsByte = trainer.ColorID;
                trainerField[FLDGRAPHIC_FIELD].AsInt = trainer.fldGraphic;
                trainerField[SINGLEENC_FIELD].AsString = trainer.singleEnc;
                trainerField[DOUBLEENC_FIELD].AsString = trainer.doubleEnc;
            });

            trainerMono[TRAINERPOKE_FIELD].SetArrayElementsAndInit(data.TowerTrainer.TrainerPoke, (pokeField, poke) =>
            {
                pokeField[ID_FIELD].AsUInt = poke.ID;
                pokeField[MONSNO_FIELD].AsInt = poke.MonsNo;
                pokeField[FORMNO_FIELD].AsUShort = poke.FormNo;
                pokeField[ISRARE_FIELD].AsBool = poke.IsRare;
                pokeField[LEVEL_FIELD].AsByte = poke.Level;
                pokeField[SEX_FIELD].AsByte = poke.Sex;
                pokeField[SEIKAKU_FIELD].AsInt = poke.Seikaku;
                pokeField[TOKUSEI_FIELD].AsInt = poke.Tokusei;
                pokeField[WAZA1_FIELD].AsInt = poke.Waza1;
                pokeField[WAZA2_FIELD].AsInt = poke.Waza2;
                pokeField[WAZA3_FIELD].AsInt = poke.Waza3;
                pokeField[WAZA4_FIELD].AsInt = poke.Waza4;
                pokeField[ITEM_FIELD].AsUShort = poke.Item;
                pokeField[BALL_FIELD].AsByte = poke.Ball;
                pokeField[SEAL_FIELD].AsInt = poke.Seal;
                pokeField[TALENTHP_FIELD].AsByte = poke.TalentHp;
                pokeField[TALENTATK_FIELD].AsByte = poke.TalentAtk;
                pokeField[TALENTDEF_FIELD].AsByte = poke.TalentDef;
                pokeField[TALENTSPATK_FIELD].AsByte = poke.TalentSpAtk;
                pokeField[TALENTSPDEF_FIELD].AsByte = poke.TalentSpDef;
                pokeField[TALENTAGI_FIELD].AsByte = poke.TalentAgi;
                pokeField[EFFORTHP_FIELD].AsByte = poke.EffortHp;
                pokeField[EFFORTATK_FIELD].AsByte = poke.EffortAtk;
                pokeField[EFFORTDEF_FIELD].AsByte = poke.EffortDef;
                pokeField[EFFORTSPATK_FIELD].AsByte = poke.EffortSpAtk;
                pokeField[EFFORTSPDEF_FIELD].AsByte = poke.EffortSpDef;
                pokeField[EFFORTAGI_FIELD].AsByte = poke.EffortAgi;
            });

            singleMono[TOWERSINGLESTOCK_FIELD].SetArrayElementsAndInit(data.TowerSingleStock.Items, (singleField, single) =>
            {
                singleField[ID_FIELD].AsUInt = single.ID;
                singleField[TRAINERID_FIELD].AsInt = single.TrainerID;
                singleField[POKEID_FIELD].SetArrayElementsAndInit(single.PokeID, (f, v) => f.AsUInt = v);
                singleField[BATTLEBGM_FIELD].AsString = single.BattleBGM;
                singleField[WINBGM_FIELD].AsString = single.WinBGM;
            });

            doubleMono[TOWERDOUBLESTOCK_FIELD].SetArrayElementsAndInit(data.TowerDoubleStock.Items, (doubleField, doubleStock) =>
            {
                doubleField[ID_FIELD].AsUInt = doubleStock.ID;
                doubleField[TRAINERID_FIELD].SetArrayElementsAndInit(doubleStock.TrainerID, (f, v) => f.AsInt = v);
                doubleField[POKEID_FIELD].SetArrayElementsAndInit(doubleStock.PokeID, (f, v) => f.AsUInt = v);
                doubleField[BATTLEBGM_FIELD].AsString = doubleStock.BattleBGM;
                doubleField[WINBGM_FIELD].AsString = doubleStock.WinBGM;
            });

            dprMasterdatasBundle.SetMonoByPathID(trainerPathID, trainerMono);
            dprMasterdatasBundle.SetMonoByPathID(singlePathID, singleMono);
            dprMasterdatasBundle.SetMonoByPathID(doublePathID, doubleMono);
        }
    }
}
