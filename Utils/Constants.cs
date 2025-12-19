using System.IO;

namespace ImpostersOrdeal
{
    public static class Constants
    {
        // Folder names
        private static readonly string ROMFS_FOLDER = "romfs";
        private static readonly string DATA_FOLDER = "Data";
        private static readonly string STREAMINGASSETS_FOLDER = "StreamingAssets";
        private static readonly string ASSETASSISTANT_FOLDER = "AssetAssistant";

        private static readonly string BATTLE_FOLDER = "Battle";
        private static readonly string CONTEST_FOLDER = "Contest";
        private static readonly string DPR_FOLDER = "Dpr";
        private static readonly string MESSAGE_FOLDER = "Message";
        private static readonly string PML_FOLDER = "Pml";
        private static readonly string UIS_FOLDER = "UIs";
        private static readonly string UNDERGROUND_FOLDER = "UnderGround";

        private static readonly string LOWERCASE_DATA_FOLDER = "data";
        private static readonly string MASTERDATAS_FOLDER = "masterdatas";
        private static readonly string MD_FOLDER = "md";
        private static readonly string SCRIPTABLEOBJECTS_FOLDER = "scriptableobjects";

        public static readonly string OUTPUT_FOLDER = "Output";
        public static readonly string TEMP_FOLDER = "Temp";

        // Bundle names
        private static readonly string BATTLEMASTERDATAS_BUNDLE = "battle_masterdatas";
        private static readonly string COMMONMSBT_BUNDLE = "common_msbt";
        private static readonly string CONTESTMASTERDATAS_BUNDLE = "contest_masterdatas";
        private static readonly string ENGLISH_BUNDLE = "english";
        private static readonly string EVSCRIPT_BUNDLE = "ev_script";
        private static readonly string FRENCH_BUNDLE = "french";
        private static readonly string GAMESETTINGS_BUNDLE = "gamesettings";
        private static readonly string GERMAN_BUNDLE = "german";
        private static readonly string ITALIAN_BUNDLE = "italian";
        private static readonly string JAPANESE_BUNDLE = "jpn";
        private static readonly string JAPANESEKANJI_BUNDLE = "jpn_kanji";
        private static readonly string KOREAN_BUNDLE = "korean";
        private static readonly string MASTERDATAS_BUNDLE = "masterdatas";
        private static readonly string PERSONALMASTERDATAS_BUNDLE = "personal_masterdatas";
        private static readonly string SIMPLIFIEDCHINESE_BUNDLE = "simp_chinese";
        private static readonly string SPANISH_BUNDLE = "spanish";
        private static readonly string TRADITIONALCHINESE_BUNDLE = "trad_chinese";
        private static readonly string UGDATA_BUNDLE = "ugdata";
        private static readonly string UIMASTERDATAS_BUNDLE = "uimasterdatas";

        // Bundle paths
        public static readonly string BATTLEMASTERDATAS_PATH =          Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, BATTLE_FOLDER,      BATTLEMASTERDATAS_BUNDLE);
        public static readonly string CONTESTMASTERDATAS_PATH =         Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, CONTEST_FOLDER,     MD_FOLDER, CONTESTMASTERDATAS_BUNDLE);
        public static readonly string EVSCRIPT_PATH =                   Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, DPR_FOLDER,         EVSCRIPT_BUNDLE);
        public static readonly string DPRMASTERDATAS_PATH =             Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, DPR_FOLDER,         MASTERDATAS_BUNDLE);
        public static readonly string GAMESETTINGS_PATH =               Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, DPR_FOLDER,         SCRIPTABLEOBJECTS_FOLDER, GAMESETTINGS_BUNDLE);
        public static readonly string COMMONMSBT_PATH =                 Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, MESSAGE_FOLDER,     COMMONMSBT_BUNDLE);
        public static readonly string ENGLISH_MESSAGE_PATH =            Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, MESSAGE_FOLDER,     ENGLISH_BUNDLE);
        public static readonly string FRENCH_MESSAGE_PATH =             Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, MESSAGE_FOLDER,     FRENCH_BUNDLE);
        public static readonly string GERMAN_MESSAGE_PATH =             Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, MESSAGE_FOLDER,     GERMAN_BUNDLE);
        public static readonly string ITALIAN_MESSAGE_PATH =            Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, MESSAGE_FOLDER,     ITALIAN_BUNDLE);
        public static readonly string JAPANESE_MESSAGE_PATH =           Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, MESSAGE_FOLDER,     JAPANESE_BUNDLE);
        public static readonly string JAPANESEKANJI_MESSAGE_PATH =      Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, MESSAGE_FOLDER,     JAPANESEKANJI_BUNDLE);
        public static readonly string KOREAN_MESSAGE_PATH =             Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, MESSAGE_FOLDER,     KOREAN_BUNDLE);
        public static readonly string SIMPLIFIEDCHINESE_MESSAGE_PATH =  Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, MESSAGE_FOLDER,     SIMPLIFIEDCHINESE_BUNDLE);
        public static readonly string SPANISH_MESSAGE_PATH =            Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, MESSAGE_FOLDER,     SPANISH_BUNDLE);
        public static readonly string TRADITIONALCHINESE_MESSAGE_PATH = Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, MESSAGE_FOLDER,     TRADITIONALCHINESE_BUNDLE);
        public static readonly string PERSONALMASTERDATAS_PATH =        Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, PML_FOLDER,         PERSONALMASTERDATAS_BUNDLE);
        public static readonly string UIMASTERDATAS_PATH =              Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, UIS_FOLDER,         MASTERDATAS_FOLDER, UIMASTERDATAS_BUNDLE);
        public static readonly string UGDATA_PATH =                     Path.Combine(ROMFS_FOLDER, DATA_FOLDER, STREAMINGASSETS_FOLDER, ASSETASSISTANT_FOLDER, UNDERGROUND_FOLDER, LOWERCASE_DATA_FOLDER, UGDATA_BUNDLE);
    }
}
