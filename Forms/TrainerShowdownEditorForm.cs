using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static ImpostersOrdeal.GameDataTypes;

namespace ImpostersOrdeal
{
    // TODO: Reimplement tower stuff
    public partial class TrainerShowdownEditorForm : Form
    {
        private GameDataSet gameData;

        private TrainerEditorForm tef;
        private BattleTowerTrainerEditorForm bttef;
        private List<string> dexEntries;
        private List<string> natures;
        private List<string> abilities;
        private List<string> moves;
        private List<string> items;
        private List<TrainerTable.SheetTrainerData.TrainerPoke> tMons;
        //private List<BattleTowerTrainerPokemon> bttMons;

        private readonly string[] genders = new string[]
        {
            "(M)", "(F)", "", "" 
        };

        private readonly string[] stats = new string[]
        {
            "HP", "Atk", "Def", "SpA", "SpD", "Spe"
        };

        private readonly Dictionary<string, int[]> monForms = PokemonFormes.monForms;


        public void SetTP(TrainerTable.SheetTrainerData t)
        {
            tMons = t.Pokes;
            Text = string.Format("Trainer Pokémon Editor: {0} {1}", tef.trainerTypeNames[t.TypeID], gameData.GetLabelByName(Constants.TRAINERNAME_MESSAGEFILE_NAME, t.NameLabel));
        }

        /*public void SetBTTP(BattleTowerTrainer btt)
        {
            BattleTowerTrainerPokemon pokemon1 = gameData.battleTowerTrainerPokemons.FirstOrDefault(t1 => t1.pokemonID == btt.battleTowerPokemonID1);
            BattleTowerTrainerPokemon pokemon2 = gameData.battleTowerTrainerPokemons.FirstOrDefault(t1 => t1.pokemonID == btt.battleTowerPokemonID2);
            BattleTowerTrainerPokemon pokemon3 = gameData.battleTowerTrainerPokemons.FirstOrDefault(t1 => t1.pokemonID == btt.battleTowerPokemonID3);
            BattleTowerTrainerPokemon pokemon4 = gameData.battleTowerTrainerPokemons.FirstOrDefault(t1 => t1.pokemonID == btt.battleTowerPokemonID4);
            bttMons = new() {pokemon1, pokemon2, pokemon3};
            if (pokemon4 != null) bttMons.Add(pokemon4);
            Text = "Battle Tower Trainer Pokémon Editor: " + " " + btt.GetName();
        }*/

        public TrainerShowdownEditorForm(TrainerEditorForm tef, GameDataSet gameData)
        {
            this.tef = tef;
            this.gameData = gameData;

            Init();
        }

        public TrainerShowdownEditorForm(BattleTowerTrainerEditorForm bttef, GameDataSet gameData)
        {
            this.bttef = bttef;
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
            if (tef != null) richTextBox1.Text = TpToShowdownText(tMons);
            //else richTextBox1.Text = BttpToShowdownText(bttMons);
            richTextBox2.Text = "Preview should match up with copied text.";
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (tef != null) ShowdownToTpData(richTextBox1.Text, tMons);
                //else ShowdownToBttpData(richTextBox1.Text, bttMons);
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
                if (tef != null) 
                {
                    var trainerMons = new List<TrainerTable.SheetTrainerData.TrainerPoke>();
                    for(int i = 0; i < tMons.Count; i++)
                    {
                        trainerMons.Add(new TrainerTable.SheetTrainerData.TrainerPoke());
                    }
                    ShowdownToTpData(richTextBox1.Text, trainerMons);
                    richTextBox2.Text = TpToShowdownText(trainerMons);
                }
                else
                {
                    /*List<BattleTowerTrainerPokemon> trainerMons = new();
                    for(int i = 0; i < bttMons.Count; i++)
                    {
                        trainerMons.Add(new BattleTowerTrainerPokemon());
                    }
                    ShowdownToBttpData(richTextBox1.Text, trainerMons);
                    richTextBox2.Text = BttpToShowdownText(trainerMons);*/
                }
            }
            catch(Exception excp)
            {
                richTextBox2.Text = "Error in parsing" + excp.ToString();
            }
        }

        private void ShowdownToTpData(string showdownText, List<TrainerTable.SheetTrainerData.TrainerPoke> trainerMons)
        {
            string[] team = showdownText.Trim('\r', '\n', ' ').Split("\n");
            List<int> breakpoints = GetShowdownBreakpoints(team);
            int pokeNum = 0;
            int point;

            for(point = 0; point < breakpoints.Count - 1; point++)
            {
                while (trainerMons.Count <= point)
                {
                    trainerMons.Add(new TrainerTable.SheetTrainerData.TrainerPoke());
                }
                var tp = trainerMons[pokeNum];
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

                pokeNum++;

                string firstline = team[breakpoints[point]];


                string mon = firstline.Split(" ")[0];
                if(monForms.ContainsKey(mon))
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
                if(firstline.Contains("("))
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

                for (int i = breakpoints[point] + 1; i < breakpoints[point + 1]; i++)
                {
                    string data = team[i];

                    if(data.ToUpper().StartsWith("-"))
                    {
                        newMoves[moveNum] = (ushort) moves.IndexOf(data[2..]);
                        if(data[2..].StartsWith("Hidden Power"))
                        {
                            newMoves[moveNum] = (ushort)237; //Hardcode for hidden power
                        }
                        moveNum++;
                    }
                    else if(data.ToUpper().StartsWith("ABILITY"))
                    {
                        tp.Tokusei = (ushort) abilities.IndexOf(data[9..]);
                    }
                    else if(data.ToUpper().Contains("NATURE"))
                    {
                        string nature = data.Split(' ')[0];
                        tp.Seikaku = (byte) natures.IndexOf(nature);
                    }
                    else if(data.ToUpper().StartsWith("SHINY"))
                    {
                        tp.IsRare = true;
                    }
                    else if(data.ToUpper().StartsWith("LEVEL"))
                    {
                        tp.Level = byte.Parse((data.Split(" ")[1]));
                    }
                    else if(data.ToUpper().StartsWith("EVS"))
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

            while(point < trainerMons.Count)
            {
                trainerMons.RemoveAt(trainerMons.Count - 1);
            }
        }

        private void ShowdownToBttpData(string showdownText, List<BattleTowerTrainerPokemon> trainerMons)
        {
            /*string[] team = showdownText.Trim('\r', '\n', ' ').Split("\n");
            List<int> breakpoints = GetShowdownBreakpoints(team);
            int pokeNum = 0;
            int point;

            for(point = 0; point < breakpoints.Count - 1; point++)
            {
                while (trainerMons.Count <= point)
                {
                    trainerMons.Add(new BattleTowerTrainerPokemon());
                }
                BattleTowerTrainerPokemon tp = trainerMons[pokeNum];
                //Default values
                tp.isRare = 0;
                tp.level = 100;
                tp.natureID = 0;
                tp.sex = 3; //Default to random

                //Counter Values
                int moveNum = 0;

                List<ushort> newMoves = new() { 0, 0, 0, 0 };
                List<byte> EVs = new() { 0, 0, 0, 0, 0, 0 };
                List<byte> IVs = new() { 31, 31, 31, 31, 31, 31 };

                pokeNum++;

                String firstline = team[breakpoints[point]];


                String mon = firstline.Split(" ")[0];
                if(monForms.ContainsKey(mon))
                {
                    tp.dexID = (ushort) monForms[mon][0];
                    tp.formID = (ushort) monForms[mon][1];
                }

                else
                {
                    tp.dexID = (ushort) dexEntries.IndexOf(mon);
                    tp.formID = 0;
                }
                //Mr. Mime and Mime Jr. are both two word pokemon
                //-1 is 65535 for ushort numbers
                if (tp.dexID == 65535)
                {
                    mon = firstline.Split(" ")[0] + " " + firstline.Split(" ")[1];
                    tp.dexID = (ushort) dexEntries.IndexOf(mon);
                    tp.formID = 0;
                }

                //Gender
                if(firstline.Contains("("))
                {
                    int index = firstline.IndexOf("(");
                    String genderStr = firstline.Substring(index, 3);
                    tp.sex = (byte) Array.IndexOf(genders, genderStr);
                }
                else
                {
                    tp.sex = 3; //Random Gender
                }

                //Item
                if (firstline.Contains("@"))
                {
                    int Index = firstline.IndexOf("@");

                    String itemStr = firstline[(Index + 1)..].Trim();
                    tp.itemID = (ushort) items.IndexOf(itemStr);
                }

                for (int i = breakpoints[point] + 1; i < breakpoints[point + 1]; i++)
                {
                    String data = team[i];

                    if(data.ToUpper().StartsWith("-"))
                    {
                        newMoves[moveNum] = (ushort) moves.IndexOf(data[2..]);
                        if(data[2..].StartsWith("Hidden Power"))
                        {
                            newMoves[moveNum] = (ushort)237; //Hardcode for hidden power
                        }
                        moveNum++;
                    }
                    else if(data.ToUpper().StartsWith("ABILITY"))
                    {
                        tp.abilityID = (ushort) abilities.IndexOf(data[9..]);
                    }
                    else if(data.ToUpper().Contains("NATURE"))
                    {
                        String nature = data.Split(' ')[0];
                        tp.natureID = (byte) natures.IndexOf(nature);
                    }
                    else if(data.ToUpper().StartsWith("SHINY"))
                    {
                        tp.isRare = 1;
                    }
                    else if(data.ToUpper().StartsWith("LEVEL"))
                    {
                        tp.level = Byte.Parse((data.Split(" ")[1]));
                    }
                    else if(data.ToUpper().StartsWith("EVS"))
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
                }

                //Repack into tp
                tp.moveID1 = newMoves[0];
                tp.moveID2 = newMoves[1];
                tp.moveID3 = newMoves[2];
                tp.moveID4 = newMoves[3];

                tp.hpIV = IVs[0];
                tp.atkIV = IVs[1];
                tp.defIV = IVs[2];
                tp.spAtkIV = IVs[3];
                tp.spDefIV = IVs[4];
                tp.spdIV = IVs[5];

                tp.hpEV = EVs[0];
                tp.atkEV = EVs[1];
                tp.defEV = EVs[2];
                tp.spAtkEV = EVs[3];
                tp.spDefEV = EVs[4];
                tp.spdEV = EVs[5];
            }

            while(point < trainerMons.Count)
            {
                trainerMons.RemoveAt(trainerMons.Count - 1);
            }*/
        }

        private List<int> GetShowdownBreakpoints(String[] team)
        {
            List<int> breakpoints = new();
            if (team.Length > 1) //Makes sure it isn't just newlines
            {
                breakpoints.Add(0);
                for (int i = 0; i < team.Length; i++)
                {
                    team[i] = team[i].Trim();
                    if (team[i] == "")
                    {
                        breakpoints.Add(i + 1);
                    }
                }
            }

            if (breakpoints.Count > 0)
            {
                breakpoints.Add(team.Length);
            }

            return breakpoints;
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

        private string TpToShowdownText(List<TrainerTable.SheetTrainerData.TrainerPoke> trainerMons)
        {
            string showdownText = "";
            foreach (var tp in trainerMons)
            {
                int[] formArray = new int[] { tp.MonsNo, tp.FormNo };

                string formName = GetForm(formArray);

                if(tp.MonsNo < dexEntries.Count) {
                    if (formName != null)
                    {
                        showdownText += formName;
                    }
                    else
                    {
                        showdownText += dexEntries[tp.MonsNo];
                    }
                }
                else
                {
                    showdownText += "Unrecognized";
                }
                showdownText += " ";
                if(tp.Sex != 255) //Gender defaults to 255
                {
                    showdownText += genders[tp.Sex];

                    if (genders[tp.Sex] != "")
                    {
                        showdownText += " ";
                    }
                }

                if (tp.Item != 0)
                {
                    showdownText += string.Format("@ {0}", items[tp.Item]);
                }

                if(tp.Level != 100) //Level 100 isn't shown
                {
                    showdownText += "\n";
                    showdownText += string.Format("Level: {0}", tp.Level);
                }

                showdownText += "\n";

                showdownText += "Ability: " + abilities[tp.Tokusei];

                showdownText += "\n";

                if(tp.IsRare)
                {
                    showdownText += "Shiny: Yes\n";
                }

                showdownText += GetEVText(new List<byte>() {tp.EffortHp, tp.EffortAtk, tp.EffortDef, tp.EffortSpAtk, tp.EffortSpDef, tp.EffortAgi});

                showdownText += string.Format("{0} Nature", natures[tp.Seikaku]);
                showdownText += "\n";

                showdownText += GetIVText(new List<byte>() {tp.TalentHp, tp.TalentAtk, tp.TalentDef, tp.TalentSpAtk, tp.TalentSpDef, tp.TalentAgi});
            
                List<ushort> moveList = new() {0, 0, 0, 0};
                moveList.Add(tp.Waza1);
                moveList.Add(tp.Waza2);
                moveList.Add(tp.Waza3);
                moveList.Add(tp.Waza4);

                foreach (ushort moveID in moveList)
                {
                    if (moveID != 0 && moveID != 65535)
                    {
                        showdownText += string.Format("- {0}\n", moves[moveID]);
                    }
                }

                showdownText += "\n";
            }

            return showdownText;
        }

        private string BttpToShowdownText(List<BattleTowerTrainerPokemon> trainerMons)
        {
            string showdownText = "";
            /*foreach (BattleTowerTrainerPokemon tp in trainerMons)
            {
                int[] formArray = new int[] { tp.dexID, (int) tp.formID };

                string formName = GetForm(formArray);

                if(tp.dexID < dexEntries.Count) {
                    if (formName != null)
                    {
                        showdownText += formName;
                    }
                    else
                    {
                        showdownText += dexEntries[tp.dexID];
                    }
                }
                else
                {
                    showdownText += "Unrecognized";
                }
                showdownText += " ";
                if(tp.sex != 255) //Gender defaults to 255
                {
                    showdownText += genders[tp.sex];

                    if (genders[tp.sex] != "")
                    {
                        showdownText += " ";
                    }
                }

                if (tp.itemID != 0)
                {
                    showdownText += string.Format("@ {0}", items[tp.itemID]);
                }

                if(tp.level != 100) //Level 100 isn't shown
                {
                    showdownText += "\n";
                    showdownText += string.Format("Level: {0}", tp.level);
                }

                showdownText += "\n";

                showdownText += "Ability: " + abilities[tp.abilityID];

                showdownText += "\n";

                if(tp.isRare == 1)
                {
                    showdownText += "Shiny: Yes\n";
                }

                showdownText += GetEVText(new List<byte>() {tp.hpEV, tp.atkEV, tp.defEV, tp.spAtkEV, tp.spDefEV, tp.spdEV});

                showdownText += string.Format("{0} Nature", natures[tp.natureID]);
                showdownText += "\n";

                showdownText += GetIVText(new List<byte>() {tp.hpIV, tp.atkIV, tp.defIV, tp.spAtkIV, tp.spDefIV, tp.spdIV});
            
                List<int> moveList = new() {0, 0, 0, 0};
                moveList.Add(tp.moveID1);
                moveList.Add(tp.moveID2);
                moveList.Add(tp.moveID3);
                moveList.Add(tp.moveID4);

                foreach (ushort moveID in moveList)
                {
                    if (moveID != 0 && moveID != 65535)
                    {
                        showdownText += string.Format("- {0}\n", moves[moveID]);
                    }
                }

                showdownText += "\n";
            }*/

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
                    returnString = returnString += String.Format("{0} {1} / ", b, stats[i]);
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

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
