using SharpYaml;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class TrainerTable : MonoBehaviour
    {
        [YamlMember("TrainerType", 10)]
        public SheetTrainerType[] TrainerType { get; set; }

        [YamlMember("TrainerData", 11)]
        public SheetTrainerData[] TrainerData { get; set; }

        [YamlMember("TrainerPoke", 12)]
        public SheetTrainerPoke[] TrainerPoke { get; set; }

        [YamlMember("TrainerRematch", 13)]
        public SheetTrainerRematch[] TrainerRematch { get; set; }

        [YamlMember("SealTemplate", 14)]
        public SheetSealTemplate[] SealTemplate { get; set; }

        [YamlMember("SkirtGraphicsChara", 15)]
        public SheetSkirtGraphicsChara[] SkirtGraphicsChara { get; set; }
    }

    public class SheetTrainerType
    {
        [YamlMember("TrainerID", 0)]
        public int TrainerID { get; set; }

        [YamlMember("LabelTrType", 1)]
        public string LabelTrType { get; set; }

        [YamlMember("Sex", 2)]
        public byte Sex { get; set; }

        [YamlMember("Group", 3)]
        public byte Group { get; set; }

        [YamlMember("BallId", 4)]
        public byte BallId { get; set; }

        [YamlMember("FieldEncount", 5)]
        public string[] FieldEncount { get; set; }

        [YamlMember("BtlEffId", 6)]
        public int[] BtlEffId { get; set; }

        [YamlMember("EyeBgm", 7)]
        public string EyeBgm { get; set; }

        [YamlMember("ModelID", 8)]
        public string ModelID { get; set; }

        [YamlMember("Hand", 9)]
        public byte Hand { get; set; }

        [YamlMember("HoldBallHand", 10)]
        public byte HoldBallHand { get; set; }

        [YamlMember("HelpHand", 11)]
        public byte HelpHand { get; set; }

        [YamlMember("HelpHoldBallHand", 12)]
        public byte HelpHoldBallHand { get; set; }

        [YamlMember("ThrowTime", 13)]
        public float ThrowTime { get; set; }

        [YamlMember("CaptureThrowTime", 14)]
        public float CaptureThrowTime { get; set; }

        [YamlMember("LoseLoopTime", 15)]
        public float LoseLoopTime { get; set; }

        [YamlMember("TrainerEffect", 16)]
        public string TrainerEffect { get; set; }

        [YamlMember("Age", 17)]
        public byte Age { get; set; }
    }

    public class SheetTrainerData
    {
        [YamlMember("TypeID", 0)]
        public int TypeID { get; set; }

        [YamlMember("ColorID", 1)]
        public byte ColorID { get; set; }

        [YamlMember("FightType", 2)]
        public byte FightType { get; set; }

        [YamlMember("ArenaID", 3)]
        public int ArenaID { get; set; }

        [YamlMember("EffectID", 4)]
        public int EffectID { get; set; }

        [YamlMember("Gold", 5)]
        public byte Gold { get; set; }

        [YamlMember("UseItem1", 6)]
        public ushort UseItem1 { get; set; }

        [YamlMember("UseItem2", 7)]
        public ushort UseItem2 { get; set; }

        [YamlMember("UseItem3", 8)]
        public ushort UseItem3 { get; set; }

        [YamlMember("UseItem4", 9)]
        public ushort UseItem4 { get; set; }

        [YamlMember("HpRecoverFlag", 10)]
        public byte HPRecoverFlag { get; set; }

        [YamlMember("GiftItem", 11)]
        public ushort GiftItem { get; set; }

        [YamlMember("NameLabel", 12)]
        public string NameLabel { get; set; }

        [YamlMember("MsgFieldPokeOne", 13)]
        public string MsgFieldPokeOne { get; set; }

        [YamlMember("MsgFieldBefore", 14)]
        public string MsgFieldBefore { get; set; }

        [YamlMember("MsgFieldRevenge", 15)]
        public string MsgFieldRevenge { get; set; }

        [YamlMember("MsgFieldAfter", 16)]
        public string MsgFieldAfter { get; set; }

        [YamlMember("MsgBattle", 17)]
        public string[] MsgBattle { get; set; }

        [YamlMember("SeqBattle", 18)]
        public string[] SeqBattle { get; set; }

        [YamlMember("AIBit", 19)]
        public uint AIBit { get; set; }
    }

    public class SheetTrainerPoke
    {
        [YamlMember("ID", 0)]
        public int ID { get; set; }

        [YamlMember("P1MonsNo", 1)]
        public ushort P1MonsNo { get; set; }

        [YamlMember("P1FormNo", 2)]
        public ushort P1FormNo { get; set; }

        [YamlMember("P1IsRare", 3)]
        public byte P1IsRare { get; set; }

        [YamlMember("P1Level", 4)]
        public byte P1Level { get; set; }

        [YamlMember("P1Sex", 5)]
        public byte P1Sex { get; set; }

        [YamlMember("P1Seikaku", 6)]
        public byte P1Seikaku { get; set; }

        [YamlMember("P1Tokusei", 7)]
        public ushort P1Tokusei { get; set; }

        [YamlMember("P1Waza1", 8)]
        public ushort P1Waza1 { get; set; }

        [YamlMember("P1Waza2", 9)]
        public ushort P1Waza2 { get; set; }

        [YamlMember("P1Waza3", 10)]
        public ushort P1Waza3 { get; set; }

        [YamlMember("P1Waza4", 11)]
        public ushort P1Waza4 { get; set; }

        [YamlMember("P1Item", 12)]
        public ushort P1Item { get; set; }

        [YamlMember("P1Ball", 13)]
        public byte P1Ball { get; set; }

        [YamlMember("P1Seal", 14)]
        public int P1Seal { get; set; }

        [YamlMember("P1TalentHp", 15)]
        public byte P1TalentHp { get; set; }

        [YamlMember("P1TalentAtk", 16)]
        public byte P1TalentAtk { get; set; }

        [YamlMember("P1TalentDef", 17)]
        public byte P1TalentDef { get; set; }

        [YamlMember("P1TalentSpAtk", 18)]
        public byte P1TalentSpAtk { get; set; }

        [YamlMember("P1TalentSpDef", 19)]
        public byte P1TalentSpDef { get; set; }

        [YamlMember("P1TalentAgi", 20)]
        public byte P1TalentAgi { get; set; }

        [YamlMember("P1EffortHp", 21)]
        public byte P1EffortHp { get; set; }

        [YamlMember("P1EffortAtk", 22)]
        public byte P1EffortAtk { get; set; }

        [YamlMember("P1EffortDef", 23)]
        public byte P1EffortDef { get; set; }

        [YamlMember("P1EffortSpAtk", 24)]
        public byte P1EffortSpAtk { get; set; }

        [YamlMember("P1EffortSpDef", 25)]
        public byte P1EffortSpDef { get; set; }

        [YamlMember("P1EffortAgi", 26)]
        public byte P1EffortAgi { get; set; }

        [YamlMember("P2MonsNo", 27)]
        public ushort P2MonsNo { get; set; }

        [YamlMember("P2FormNo", 28)]
        public ushort P2FormNo { get; set; }

        [YamlMember("P2IsRare", 29)]
        public byte P2IsRare { get; set; }

        [YamlMember("P2Level", 30)]
        public byte P2Level { get; set; }

        [YamlMember("P2Sex", 31)]
        public byte P2Sex { get; set; }

        [YamlMember("P2Seikaku", 32)]
        public byte P2Seikaku { get; set; }

        [YamlMember("P2Tokusei", 33)]
        public ushort P2Tokusei { get; set; }

        [YamlMember("P2Waza1", 34)]
        public ushort P2Waza1 { get; set; }

        [YamlMember("P2Waza2", 35)]
        public ushort P2Waza2 { get; set; }

        [YamlMember("P2Waza3", 36)]
        public ushort P2Waza3 { get; set; }

        [YamlMember("P2Waza4", 37)]
        public ushort P2Waza4 { get; set; }

        [YamlMember("P2Item", 38)]
        public ushort P2Item { get; set; }

        [YamlMember("P2Ball", 39)]
        public byte P2Ball { get; set; }

        [YamlMember("P2Seal", 40)]
        public int P2Seal { get; set; }

        [YamlMember("P2TalentHp", 41)]
        public byte P2TalentHp { get; set; }

        [YamlMember("P2TalentAtk", 42)]
        public byte P2TalentAtk { get; set; }

        [YamlMember("P2TalentDef", 43)]
        public byte P2TalentDef { get; set; }

        [YamlMember("P2TalentSpAtk", 44)]
        public byte P2TalentSpAtk { get; set; }

        [YamlMember("P2TalentSpDef", 45)]
        public byte P2TalentSpDef { get; set; }

        [YamlMember("P2TalentAgi", 46)]
        public byte P2TalentAgi { get; set; }

        [YamlMember("P2EffortHp", 47)]
        public byte P2EffortHp { get; set; }

        [YamlMember("P2EffortAtk", 48)]
        public byte P2EffortAtk { get; set; }

        [YamlMember("P2EffortDef", 49)]
        public byte P2EffortDef { get; set; }

        [YamlMember("P2EffortSpAtk", 50)]
        public byte P2EffortSpAtk { get; set; }

        [YamlMember("P2EffortSpDef", 51)]
        public byte P2EffortSpDef { get; set; }

        [YamlMember("P2EffortAgi", 52)]
        public byte P2EffortAgi { get; set; }

        [YamlMember("P3MonsNo", 53)]
        public ushort P3MonsNo { get; set; }

        [YamlMember("P3FormNo", 54)]
        public ushort P3FormNo { get; set; }

        [YamlMember("P3IsRare", 55)]
        public byte P3IsRare { get; set; }

        [YamlMember("P3Level", 56)]
        public byte P3Level { get; set; }

        [YamlMember("P3Sex", 57)]
        public byte P3Sex { get; set; }

        [YamlMember("P3Seikaku", 58)]
        public byte P3Seikaku { get; set; }

        [YamlMember("P3Tokusei", 59)]
        public ushort P3Tokusei { get; set; }

        [YamlMember("P3Waza1", 60)]
        public ushort P3Waza1 { get; set; }

        [YamlMember("P3Waza2", 61)]
        public ushort P3Waza2 { get; set; }

        [YamlMember("P3Waza3", 62)]
        public ushort P3Waza3 { get; set; }

        [YamlMember("P3Waza4", 63)]
        public ushort P3Waza4 { get; set; }

        [YamlMember("P3Item", 64)]
        public ushort P3Item { get; set; }

        [YamlMember("P3Ball", 65)]
        public byte P3Ball { get; set; }

        [YamlMember("P3Seal", 66)]
        public int P3Seal { get; set; }

        [YamlMember("P3TalentHp", 67)]
        public byte P3TalentHp { get; set; }

        [YamlMember("P3TalentAtk", 68)]
        public byte P3TalentAtk { get; set; }

        [YamlMember("P3TalentDef", 69)]
        public byte P3TalentDef { get; set; }

        [YamlMember("P3TalentSpAtk", 70)]
        public byte P3TalentSpAtk { get; set; }

        [YamlMember("P3TalentSpDef", 71)]
        public byte P3TalentSpDef { get; set; }

        [YamlMember("P3TalentAgi", 72)]
        public byte P3TalentAgi { get; set; }

        [YamlMember("P3EffortHp", 73)]
        public byte P3EffortHp { get; set; }

        [YamlMember("P3EffortAtk", 74)]
        public byte P3EffortAtk { get; set; }

        [YamlMember("P3EffortDef", 75)]
        public byte P3EffortDef { get; set; }

        [YamlMember("P3EffortSpAtk", 76)]
        public byte P3EffortSpAtk { get; set; }

        [YamlMember("P3EffortSpDef", 77)]
        public byte P3EffortSpDef { get; set; }

        [YamlMember("P3EffortAgi", 78)]
        public byte P3EffortAgi { get; set; }

        [YamlMember("P4MonsNo", 79)]
        public ushort P4MonsNo { get; set; }

        [YamlMember("P4FormNo", 80)]
        public ushort P4FormNo { get; set; }

        [YamlMember("P4IsRare", 81)]
        public byte P4IsRare { get; set; }

        [YamlMember("P4Level", 82)]
        public byte P4Level { get; set; }

        [YamlMember("P4Sex", 83)]
        public byte P4Sex { get; set; }

        [YamlMember("P4Seikaku", 84)]
        public byte P4Seikaku { get; set; }

        [YamlMember("P4Tokusei", 85)]
        public ushort P4Tokusei { get; set; }

        [YamlMember("P4Waza1", 86)]
        public ushort P4Waza1 { get; set; }

        [YamlMember("P4Waza2", 87)]
        public ushort P4Waza2 { get; set; }

        [YamlMember("P4Waza3", 88)]
        public ushort P4Waza3 { get; set; }

        [YamlMember("P4Waza4", 89)]
        public ushort P4Waza4 { get; set; }

        [YamlMember("P4Item", 90)]
        public ushort P4Item { get; set; }

        [YamlMember("P4Ball", 91)]
        public byte P4Ball { get; set; }

        [YamlMember("P4Seal", 92)]
        public int P4Seal { get; set; }

        [YamlMember("P4TalentHp", 93)]
        public byte P4TalentHp { get; set; }

        [YamlMember("P4TalentAtk", 94)]
        public byte P4TalentAtk { get; set; }

        [YamlMember("P4TalentDef", 95)]
        public byte P4TalentDef { get; set; }

        [YamlMember("P4TalentSpAtk", 96)]
        public byte P4TalentSpAtk { get; set; }

        [YamlMember("P4TalentSpDef", 97)]
        public byte P4TalentSpDef { get; set; }

        [YamlMember("P4TalentAgi", 98)]
        public byte P4TalentAgi { get; set; }

        [YamlMember("P4EffortHp", 99)]
        public byte P4EffortHp { get; set; }

        [YamlMember("P4EffortAtk", 100)]
        public byte P4EffortAtk { get; set; }

        [YamlMember("P4EffortDef", 101)]
        public byte P4EffortDef { get; set; }

        [YamlMember("P4EffortSpAtk", 102)]
        public byte P4EffortSpAtk { get; set; }

        [YamlMember("P4EffortSpDef", 103)]
        public byte P4EffortSpDef { get; set; }

        [YamlMember("P4EffortAgi", 104)]
        public byte P4EffortAgi { get; set; }

        [YamlMember("P5MonsNo", 105)]
        public ushort P5MonsNo { get; set; }

        [YamlMember("P5FormNo", 106)]
        public ushort P5FormNo { get; set; }

        [YamlMember("P5IsRare", 107)]
        public byte P5IsRare { get; set; }

        [YamlMember("P5Level", 108)]
        public byte P5Level { get; set; }

        [YamlMember("P5Sex", 109)]
        public byte P5Sex { get; set; }

        [YamlMember("P5Seikaku", 110)]
        public byte P5Seikaku { get; set; }

        [YamlMember("P5Tokusei", 111)]
        public ushort P5Tokusei { get; set; }

        [YamlMember("P5Waza1", 112)]
        public ushort P5Waza1 { get; set; }

        [YamlMember("P5Waza2", 113)]
        public ushort P5Waza2 { get; set; }

        [YamlMember("P5Waza3", 114)]
        public ushort P5Waza3 { get; set; }

        [YamlMember("P5Waza4", 115)]
        public ushort P5Waza4 { get; set; }

        [YamlMember("P5Item", 116)]
        public ushort P5Item { get; set; }

        [YamlMember("P5Ball", 117)]
        public byte P5Ball { get; set; }

        [YamlMember("P5Seal", 118)]
        public int P5Seal { get; set; }

        [YamlMember("P5TalentHp", 119)]
        public byte P5TalentHp { get; set; }

        [YamlMember("P5TalentAtk", 120)]
        public byte P5TalentAtk { get; set; }

        [YamlMember("P5TalentDef", 121)]
        public byte P5TalentDef { get; set; }

        [YamlMember("P5TalentSpAtk", 122)]
        public byte P5TalentSpAtk { get; set; }

        [YamlMember("P5TalentSpDef", 123)]
        public byte P5TalentSpDef { get; set; }

        [YamlMember("P5TalentAgi", 124)]
        public byte P5TalentAgi { get; set; }

        [YamlMember("P5EffortHp", 125)]
        public byte P5EffortHp { get; set; }

        [YamlMember("P5EffortAtk", 126)]
        public byte P5EffortAtk { get; set; }

        [YamlMember("P5EffortDef", 127)]
        public byte P5EffortDef { get; set; }

        [YamlMember("P5EffortSpAtk", 128)]
        public byte P5EffortSpAtk { get; set; }

        [YamlMember("P5EffortSpDef", 129)]
        public byte P5EffortSpDef { get; set; }

        [YamlMember("P5EffortAgi", 130)]
        public byte P5EffortAgi { get; set; }

        [YamlMember("P6MonsNo", 131)]
        public ushort P6MonsNo { get; set; }

        [YamlMember("P6FormNo", 132)]
        public ushort P6FormNo { get; set; }

        [YamlMember("P6IsRare", 133)]
        public byte P6IsRare { get; set; }

        [YamlMember("P6Level", 134)]
        public byte P6Level { get; set; }

        [YamlMember("P6Sex", 135)]
        public byte P6Sex { get; set; }

        [YamlMember("P6Seikaku", 136)]
        public byte P6Seikaku { get; set; }

        [YamlMember("P6Tokusei", 137)]
        public ushort P6Tokusei { get; set; }

        [YamlMember("P6Waza1", 138)]
        public ushort P6Waza1 { get; set; }

        [YamlMember("P6Waza2", 139)]
        public ushort P6Waza2 { get; set; }

        [YamlMember("P6Waza3", 140)]
        public ushort P6Waza3 { get; set; }

        [YamlMember("P6Waza4", 141)]
        public ushort P6Waza4 { get; set; }

        [YamlMember("P6Item", 142)]
        public ushort P6Item { get; set; }

        [YamlMember("P6Ball", 143)]
        public byte P6Ball { get; set; }

        [YamlMember("P6Seal", 144)]
        public int P6Seal { get; set; }

        [YamlMember("P6TalentHp", 145)]
        public byte P6TalentHp { get; set; }

        [YamlMember("P6TalentAtk", 146)]
        public byte P6TalentAtk { get; set; }

        [YamlMember("P6TalentDef", 147)]
        public byte P6TalentDef { get; set; }

        [YamlMember("P6TalentSpAtk", 148)]
        public byte P6TalentSpAtk { get; set; }

        [YamlMember("P6TalentSpDef", 149)]
        public byte P6TalentSpDef { get; set; }

        [YamlMember("P6TalentAgi", 150)]
        public byte P6TalentAgi { get; set; }

        [YamlMember("P6EffortHp", 151)]
        public byte P6EffortHp { get; set; }

        [YamlMember("P6EffortAtk", 152)]
        public byte P6EffortAtk { get; set; }

        [YamlMember("P6EffortDef", 153)]
        public byte P6EffortDef { get; set; }

        [YamlMember("P6EffortSpAtk", 154)]
        public byte P6EffortSpAtk { get; set; }

        [YamlMember("P6EffortSpDef", 155)]
        public byte P6EffortSpDef { get; set; }

        [YamlMember("P6EffortAgi", 156)]
        public byte P6EffortAgi { get; set; }
    }

    public class SheetTrainerRematch
    {
        [YamlMember("BaseTrainerID", 0)]
        public int BaseTrainerID { get; set; }

        [YamlMember("Rematch_01", 1)]
        public int Rematch1 { get; set; }

        [YamlMember("Rematch_02", 2)]
        public int Rematch2 { get; set; }

        [YamlMember("Rematch_03", 3)]
        public int Rematch3 { get; set; }

        [YamlMember("Rematch_04", 4)]
        public int Rematch4 { get; set; }

        [YamlMember("Rematch_05", 5)]
        public int Rematch5 { get; set; }
    }

    public class SheetSealTemplate
    {
        [YamlMember("SealID1", 0)]
        public int SealID1 { get; set; }

        [YamlMember("Pos1", 1)]
        public UnityVector3 Pos1 { get; set; }

        [YamlMember("SealID2", 2)]
        public int SealID2 { get; set; }

        [YamlMember("Pos2", 3)]
        public UnityVector3 Pos2 { get; set; }

        [YamlMember("SealID3", 4)]
        public int SealID3 { get; set; }

        [YamlMember("Pos3", 5)]
        public UnityVector3 Pos3 { get; set; }

        [YamlMember("SealID4", 6)]
        public int SealID4 { get; set; }

        [YamlMember("Pos4", 7)]
        public UnityVector3 Pos4 { get; set; }

        [YamlMember("SealID5", 8)]
        public int SealID5 { get; set; }

        [YamlMember("Pos5", 9)]
        public UnityVector3 Pos5 { get; set; }

        [YamlMember("SealID6", 10)]
        public int SealID6 { get; set; }

        [YamlMember("Pos6", 11)]
        public UnityVector3 Pos6 { get; set; }

        [YamlMember("SealID7", 12)]
        public int SealID7 { get; set; }

        [YamlMember("Pos7", 13)]
        public UnityVector3 Pos7 { get; set; }

        [YamlMember("SealID8", 14)]
        public int SealID8 { get; set; }

        [YamlMember("Pos8", 15)]
        public UnityVector3 Pos8 { get; set; }

        [YamlMember("SealID9", 16)]
        public int SealID9 { get; set; }

        [YamlMember("Pos9", 17)]
        public UnityVector3 Pos9 { get; set; }

        [YamlMember("SealID10", 18)]
        public int SealID10 { get; set; }

        [YamlMember("Pos10", 19)]
        public UnityVector3 Pos10 { get; set; }

        [YamlMember("SealID11", 20)]
        public int SealID11 { get; set; }

        [YamlMember("Pos11", 21)]
        public UnityVector3 Pos11 { get; set; }

        [YamlMember("SealID12", 22)]
        public int SealID12 { get; set; }

        [YamlMember("Pos12", 23)]
        public UnityVector3 Pos12 { get; set; }

        [YamlMember("SealID13", 24)]
        public int SealID13 { get; set; }

        [YamlMember("Pos13", 25)]
        public UnityVector3 Pos13 { get; set; }

        [YamlMember("SealID14", 26)]
        public int SealID14 { get; set; }

        [YamlMember("Pos14", 27)]
        public UnityVector3 Pos14 { get; set; }

        [YamlMember("SealID15", 28)]
        public int SealID15 { get; set; }

        [YamlMember("Pos15", 29)]
        public UnityVector3 Pos15 { get; set; }

        [YamlMember("SealID16", 30)]
        public int SealID16 { get; set; }

        [YamlMember("Pos16", 31)]
        public UnityVector3 Pos16 { get; set; }

        [YamlMember("SealID17", 32)]
        public int SealID17 { get; set; }

        [YamlMember("Pos17", 33)]
        public UnityVector3 Pos17 { get; set; }

        [YamlMember("SealID18", 34)]
        public int SealID18 { get; set; }

        [YamlMember("Pos18", 35)]
        public UnityVector3 Pos18 { get; set; }

        [YamlMember("SealID19", 36)]
        public int SealID19 { get; set; }

        [YamlMember("Pos19", 37)]
        public UnityVector3 Pos19 { get; set; }

        [YamlMember("SealID20", 38)]
        public int SealID20 { get; set; }

        [YamlMember("Pos20", 39)]
        public UnityVector3 Pos20 { get; set; }
    }

    public class SheetSkirtGraphicsChara
    {
        [YamlMember("SkirtGraphicsID", 0)]
        public string SkirtGraphicsID { get; set; }
    }
}
