using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ImpostersOrdeal
{
    public partial class BattleTowerPokemonShowdownEditorForm : Form
    {
        private GameDataSet gameData;

        private List<string> dexEntries;
        private List<string> natures;
        private List<string> abilities;
        private List<string> moves;
        private List<string> items;
        private BattleTowerTable.TowerTrainerTable.SheetTrainerPoke bttMon;

        private readonly string[] genders = new string[]
        {
            "(M)", "(F)", "", "" 
        };

        private readonly string[] stats = new string[]
        {
            "HP", "Atk", "Def", "SpA", "SpD", "Spe"
        };

        private readonly Dictionary<string, int[]> monForms;


        public void SetBTP(BattleTowerTable.TowerTrainerTable.SheetTrainerPoke btp)
        {
            bttMon = btp;
            Text = string.Format("Battle Tower Pokémon Editor: {0} {1}", btp.ID, dexEntries[bttMon.MonsNo]);
        }

        public BattleTowerPokemonShowdownEditorForm(GameDataSet gameData)
        {
            this.gameData = gameData;

            Init();
        }

        private void Init()
        {
            dexEntries = gameData.GetAllLabels(Constants.POKEMONSPECIES_MESSAGEFILE_NAME);
            natures = gameData.GetAllLabels(Constants.NATURE_MESSAGEFILE_NAME);
            abilities = gameData.GetAllLabels(Constants.ABILITY_MESSAGEFILE_NAME);
            moves = gameData.GetAllLabels(Constants.MOVE_MESSAGEFILE_NAME);
            items = gameData.GetAllLabels(Constants.ITEM_MESSAGEFILE_NAME);

            InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            richTextBox1.Text = ToShowdownText(bttMon);
            richTextBox2.Text = "Preview should match up with copied text.";
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                ShowdownToData(richTextBox1.Text, bttMon);
            }
            catch (Exception)
            {
                richTextBox2.Text = "Error in parsing";
            }
        }

        private void Preview_Click(object sender, EventArgs e)
        {
            try
            {
                var tempMon = new BattleTowerTable.TowerTrainerTable.SheetTrainerPoke();
                ShowdownToData(richTextBox1.Text, tempMon);
                richTextBox2.Text = ToShowdownText(tempMon);
            }
            catch(Exception excp)
            {
                richTextBox2.Text = "Error in parsing" + excp.ToString();
            }
        }

        private void ShowdownToData(string showdownText, BattleTowerTable.TowerTrainerTable.SheetTrainerPoke tp)
        {
            string[] pokemon = showdownText.Trim('\r', '\n', ' ').Split("\n");
            
            //Default values
            tp.IsRare = false;
            tp.Level = 100;
            tp.Seikaku = 0;
            tp.Sex = 3; //Default to random

            //Counter Values
            int moveNum = 0;

            List<ushort> newMoves = new() { 0, 0, 0, 0 };
            List<byte> EVs = new() { 0, 0, 0, 0, 0, 0 };
            List<byte> IVs = new() { 31, 31, 31, 31, 31, 31 };

            string firstline = pokemon[0];
            string mon = firstline.Split(" ")[0];
            if (monForms.ContainsKey(mon))
            {
                tp.MonsNo = (ushort) monForms[mon][0];
                tp.FormNo = (ushort) monForms[mon][1];
            }
            else
            {
                tp.MonsNo = (ushort) dexEntries.IndexOf(mon);
                tp.FormNo = 0;
            }
            //Mr. Mime and Mime Jr. are both two word pokemon
            //-1 is 65535 for ushort numbers
            if (tp.MonsNo == 65535)
            {
                mon = firstline.Split(" ")[0] + " " + firstline.Split(" ")[1];
                tp.MonsNo = (ushort) dexEntries.IndexOf(mon);
                tp.FormNo = 0;
            }

            //Gender
            if (firstline.Contains("("))
            {
                int index = firstline.IndexOf("(");
                string genderStr = firstline.Substring(index, 3);
                tp.Sex = (byte) Array.IndexOf(genders, genderStr);
            }
            else
            {
                tp.Sex = 3; //Random Gender
            }

            //Item
            if (firstline.Contains("@"))
            {
                int Index = firstline.IndexOf("@");
                string itemStr = firstline[(Index + 1)..].Trim();
                tp.Item = (ushort) items.IndexOf(itemStr);
            }

            for (int i = 1; i < pokemon.Length; i++)
            {
                string data = pokemon[i];

                if (data.ToUpper().StartsWith("-"))
                {
                    newMoves[moveNum] = (ushort) moves.IndexOf(data[2..]);
                    if(data[2..].StartsWith("Hidden Power"))
                    {
                        newMoves[moveNum] = (ushort)237; //Hardcode for hidden power
                    }
                    moveNum++;
                }
                else if (data.ToUpper().StartsWith("ABILITY"))
                {
                    tp.Tokusei = (ushort) abilities.IndexOf(data[9..]);
                }
                else if (data.ToUpper().Contains("NATURE"))
                {
                    string nature = data.Split(' ')[0];
                    tp.Seikaku = (byte) natures.IndexOf(nature);
                }
                else if (data.ToUpper().StartsWith("SHINY"))
                {
                    tp.IsRare = true;
                }
                else if (data.ToUpper().StartsWith("LEVEL"))
                {
                    tp.Level = byte.Parse((data.Split(" ")[1]));
                }
                else if (data.ToUpper().StartsWith("EVS"))
                {
                    int statNum = 0;
                    foreach (int stat in FormatStats(data, 0))
                    {
                        EVs[statNum] = (byte) stat;
                        statNum++;
                    }
                }
                else if (data.ToUpper().StartsWith("IVS"))
                {
                    int statNum = 0;
                    foreach (int stat in FormatStats(data, 31))
                    {
                        IVs[statNum] = (byte) stat;
                        statNum++;
                    }
                } 
                else if (data.ToUpper().StartsWith("TERA"))
                {
                    //Tera types are not supported in Imposters Ordeal
                    //This is just to skip the line
                    continue;
                }
            }

            //Repack into tp
            tp.Waza1 = newMoves[0];
            tp.Waza2 = newMoves[1];
            tp.Waza3 = newMoves[2];
            tp.Waza4 = newMoves[3];

            tp.TalentHp = IVs[0];
            tp.TalentAtk = IVs[1];
            tp.TalentDef = IVs[2];
            tp.TalentSpAtk = IVs[3];
            tp.TalentSpDef = IVs[4];
            tp.TalentAgi = IVs[5];

            tp.EffortHp = EVs[0];
            tp.EffortAtk = EVs[1];
            tp.EffortDef = EVs[2];
            tp.EffortSpAtk = EVs[3];
            tp.EffortSpDef = EVs[4];
            tp.EffortAgi = EVs[5];
        }

        private string GetForm(int[] formArray)
        {
            foreach (KeyValuePair<string, int[]> entry in monForms)
            {
                if (Enumerable.SequenceEqual(formArray, entry.Value))
                {
                    return entry.Key;
                }
            }
            return null;
        }

        private string ToShowdownText(BattleTowerTable.TowerTrainerTable.SheetTrainerPoke bttp)
        {
            string showdownText = "";

            int[] formArray = new int[] { bttp.MonsNo, (int) bttp.FormNo };
            string formName = GetForm(formArray);

            if (bttp.MonsNo < dexEntries.Count) {
                if (formName != null)
                {
                    showdownText += formName;
                }
                else
                {
                    showdownText += dexEntries[bttp.MonsNo];
                }
            }
            else
            {
                showdownText += "Unrecognized";
            }
            showdownText += " ";
            if (bttp.Sex != 255) //Gender defaults to 255
            {
                showdownText += genders[bttp.Sex];

                if (genders[bttp.Sex] != "")
                {
                        showdownText += " ";
                }
            }

            if (bttp.Item != 0)
            {
                showdownText += string.Format("@ {0}", items[bttp.Item]);
            }

            if (bttp.Level != 100) //Level 100 isn't shown
            {
                showdownText += "\n";
                showdownText += string.Format("Level: {0}", bttp.Level);
            }

            showdownText += "\n";

            showdownText += "Ability: " + abilities[bttp.Tokusei];

            showdownText += "\n";

            if (bttp.IsRare)
            {
                showdownText += "Shiny: Yes\n";
            }

            showdownText += GetEVText(new List<byte>() {bttp.EffortHp, bttp.EffortAtk, bttp.EffortDef, bttp.EffortSpAtk, bttp.EffortSpDef, bttp.EffortAgi });

            showdownText += string.Format("{0} Nature", natures[bttp.Seikaku]);
            showdownText += "\n";

            showdownText += GetIVText(new List<byte>() {bttp.TalentHp, bttp.TalentAtk, bttp.TalentDef, bttp.TalentSpAtk, bttp.TalentSpDef, bttp.TalentAgi });
            
            List<int> moveList = new()
            {
                0,
                0,
                0,
                0,
                bttp.Waza1,
                bttp.Waza2,
                bttp.Waza3,
                bttp.Waza4
            };

            foreach (ushort moveID in moveList.Select(v => (ushort)v))
            {
                if (moveID != 0 && moveID != 65535)
                {
                    showdownText += string.Format("- {0}\n", moves[moveID]);
                }
            }

            showdownText += "\n";
            return showdownText;
        }

        //0 Is implied EVs
        private string GetEVText(List<byte> EVList)
        {
            bool hasEV = false;
            string returnString = "";
            byte b;

            //Format a HP / b Atk / c Def / d SpA / e SpD / f Spe  
            for (int i = 0; i < EVList.Count; i++)
            {
                b = EVList[i];
                if(b != 0)
                {
                    if(!hasEV)
                    {
                        returnString += "EVs: ";
                        hasEV = true;
                    }
                    returnString = returnString += string.Format("{0} {1} / ", b, stats[i]);
                }
            }

            if(hasEV)
            {
                returnString = returnString.Remove(returnString.Length - 3 , 3);
                returnString += "\n";
            }

            return returnString;
        }

        //31 Is implied IVs
        private string GetIVText(List<byte> IVList)
        {
            bool hasIV = false;
            string returnString = "";
            byte b;

            //Format a HP / b Atk / c Def / d SpA / e SpD / f Spe  
            for (int i = 0; i < IVList.Count; i++)
            {
                b = IVList[i];
                if (b != 31)
                {
                    if (!hasIV)
                    {
                        returnString += "IVs: ";
                        hasIV = true;
                    }
                    returnString += string.Format("{0} {1} / ", b, stats[i]);
                }
            }

            if (hasIV)
            {
                returnString = returnString.Remove(returnString.Length - 3, 3);
                returnString += "\n";
            }

            return returnString;
        }

        private List<int> FormatStats(string statString, int defaultVal)
        {
            List<int> returnList = new();

            for(int i = 0; i < 6; i++)
            {
                returnList.Add(defaultVal);
            }

            string[] statList = statString[4..].TrimEnd('/', ' ').Split("/");
            foreach (string stat in statList)
            {
                string[] statSplit = stat.Trim().Split(" ");
                int statVal = int.Parse(statSplit[0]);
                string statName = statSplit[1];

                int index = Array.IndexOf(stats, statName);

                returnList[index] = statVal;
            }

            return returnList;
        }
    }
}
