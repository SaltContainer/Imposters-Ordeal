using System;
using System.Collections.Generic;
using System.Data;
using static ImpostersOrdeal.Analyzer;
using static ImpostersOrdeal.GameDataTypes;

namespace ImpostersOrdeal
{
    /// <summary>
    ///  Stores data for easy access.
    /// </summary>
    public static class GlobalData
    {
        public static FileManager fileManager;
        public static Dictionary<PathEnum, string> randomizerPaths = new();
        public static DataTable absoluteBoundaries = new();

        public static DataRow GetBoundaries(AbsoluteBoundary a)
        {
            if ((int)a == -1)
                return null;
            return absoluteBoundaries.Rows[(int)a];
        }

        public static bool IsWithin(AbsoluteBoundary a, int x)
        {
            if ((int)a == -1)
                return true;
            DataRow b = GetBoundaries(a);
            return x >= (int)b[1] && x <= (int)b[2];
        }

        public static int Conform(AbsoluteBoundary a, int x)
        {
            if ((int)a == -1)
                return x;
            DataRow b = GetBoundaries(a);
            x = (int)(Math.Round((double)x / (int)b[3]) * (int)b[3]);
            return Math.Clamp(x, (int)b[1], (int)b[2]);
        }

        public static string GetZoneName(int index)
        {
            if (index > -1 && index < Zones.zoneNames.Length)
                return Zones.zoneNames[index];

            return Zones.defaultName;
        }

        public enum PathEnum
        {
            EvScript,
            DprMasterdatas,
            Gamesettings,
            CommonMsbt,
            English,
            French,
            German,
            Italian,
            Jpn,
            JpnKanji,
            Korean,
            SimpChinese,
            Spanish,
            TradChinese,
            PersonalMasterdatas,
            Ugdata,
            BattleMasterdatas,
            UIMasterdatas,
            ContestMasterdatas
        }

        public enum AbsoluteBoundary
        {
            Level,
            BaseStat,
            CatchRate,
            EvYield,
            EvYieldTotal,
            InitialFriendship,
            ExpYield,
            LevelUpMoveCount,
            EggMoveCount,
            Power,
            Accuracy,
            Pp,
            TrainerPokemonCount,
            Iv,
            Ev,
            EvTotal,
            Price,
            None = -1
        }

        public static void Initialize()
        {
            randomizerPaths[PathEnum.BattleMasterdatas] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Battle\\battle_masterdatas";
            randomizerPaths[PathEnum.ContestMasterdatas] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Contest\\md\\contest_masterdatas";
            randomizerPaths[PathEnum.EvScript] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Dpr\\ev_script";
            randomizerPaths[PathEnum.DprMasterdatas] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Dpr\\masterdatas";
            randomizerPaths[PathEnum.Gamesettings] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Dpr\\scriptableobjects\\gamesettings";
            randomizerPaths[PathEnum.CommonMsbt] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Message\\common_msbt";
            randomizerPaths[PathEnum.English] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Message\\english";
            randomizerPaths[PathEnum.French] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Message\\french";
            randomizerPaths[PathEnum.German] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Message\\german";
            randomizerPaths[PathEnum.Italian] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Message\\italian";
            randomizerPaths[PathEnum.Jpn] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Message\\jpn";
            randomizerPaths[PathEnum.JpnKanji] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Message\\jpn_kanji";
            randomizerPaths[PathEnum.Korean] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Message\\korean";
            randomizerPaths[PathEnum.SimpChinese] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Message\\simp_chinese";
            randomizerPaths[PathEnum.Spanish] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Message\\spanish";
            randomizerPaths[PathEnum.TradChinese] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Message\\trad_chinese";
            randomizerPaths[PathEnum.PersonalMasterdatas] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Pml\\personal_masterdatas";
            randomizerPaths[PathEnum.UIMasterdatas] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\UIs\\masterdatas\\uimasterdatas";
            randomizerPaths[PathEnum.Ugdata] = "romfs\\Data\\StreamingAssets\\AssetAssistant\\UnderGround\\data\\ugdata";
            DataColumn[] columns = { new DataColumn("Value", typeof(string)), new DataColumn("Minimum", typeof(int)), new DataColumn("Maximum", typeof(int)), new DataColumn("Increment", typeof(int)) };
            absoluteBoundaries.Columns.AddRange(columns);
            columns[0].ReadOnly = true;
            absoluteBoundaries.Rows.Add(new Object[] { "Level", 1, 100, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Base Stat", 1, 255, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Catch Rate", 1, 255, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "EV Yield", 0, 3, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "EV Yield Total", 1, 3, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Initial Friendship", 0, 250, 10 });
            absoluteBoundaries.Rows.Add(new Object[] { "EXP Yield", 1, 65535, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Level Up Move Count", 1, 255, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Egg Move Count", 0, 255, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Power", 5, 255, 5 });
            absoluteBoundaries.Rows.Add(new Object[] { "Accuracy", 5, 100, 5 });
            absoluteBoundaries.Rows.Add(new Object[] { "PP", 5, 40, 5 });
            absoluteBoundaries.Rows.Add(new Object[] { "Trainer Pokémon Count", 1, 6, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "IV", 0, 31, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "EV", 0, 255, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "EV Total", 0, 510, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Price", 10, 999990, 10 });
        }

        /// <summary>
        ///  Finds the particular correlation of typings between two Pokémon.
        /// </summary>
        public static TypingCorrelation CompareTyping(this Pokemon p1, Pokemon p2)
        {
            List<int> typing1 = p1.GetTyping();
            List<int> typing2 = p2.GetTyping();
            int matches = 0;
            if (typing1.Contains(typing2[0]))
                matches++;
            if (typing2.Count > 1 && typing1.Contains(typing2[1]))
                matches++;
            if (matches == 0 && !(typing1.Count == 1 && typing2.Count == 1))
                return TypingCorrelation.NoCorrelation;
            if (matches == 2 || typing1.Count == 1 && typing2.Count == 1 && matches == 1)
                return TypingCorrelation.Identical;
            if (typing1.Count != typing2.Count)
                return TypingCorrelation.Addition;
            return TypingCorrelation.Swap;
        }
    }
}
