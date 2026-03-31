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

        private BattleTowerTrainerEditorForm bttef;

        private List<string> dexEntries;
        private List<string> natures;
        private List<string> abilities;
        private List<string> moves;
        private List<string> items;

        private TrainerTable.SheetTrainerData trainer = null;
        //private List<BattleTowerTrainerPokemon> bttMons;

        public List<TrainerTable.SheetTrainerData.TrainerPoke> TrainerPokesResult { get; set; } = null;

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

        public TrainerShowdownEditorForm(TrainerTable.SheetTrainerData trainer, GameDataSet gameData, string trainerName)
        {
            this.trainer = trainer;
            this.gameData = gameData;

            Init(trainerName);
        }

        public TrainerShowdownEditorForm(BattleTowerTrainerEditorForm bttef, GameDataSet gameData, string trainerName)
        {
            this.bttef = bttef;
            this.gameData = gameData;

            Init(trainerName);
        }

        private void Init(string trainerName)
        {
            InitializeComponent();

            dexEntries = gameData.GetAllLabels(Constants.POKEMONSPECIES_MESSAGEFILE_NAME);
            natures = gameData.GetAllLabels(Constants.NATURE_MESSAGEFILE_NAME);
            abilities = gameData.GetAllLabels(Constants.ABILITY_MESSAGEFILE_NAME);
            moves = gameData.GetAllLabels(Constants.MOVE_MESSAGEFILE_NAME);
            items = gameData.GetAllLabels(Constants.ITEM_MESSAGEFILE_NAME);

            Text = string.Format("Trainer Pokémon Editor: {0}", trainerName);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            if (trainer != null) rtxtInput.Text = TpToShowdownText(trainer.Pokes);
            //else richTextBox1.Text = BttpToShowdownText(bttMons);
            rtxtPreview.Text = "Preview should match up with copied text.";
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (trainer != null) TrainerPokesResult = ShowdownToTpData(rtxtInput.Text);
                //else ShowdownToBttpData(richTextBox1.Text, bttMons);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception)
            {
                rtxtPreview.Text = "Error in parsing";
            }
        }

        private void Preview_Click(object sender, EventArgs e)
        {
            try
            {
                if (trainer != null) 
                {
                    rtxtPreview.Text = TpToShowdownText(ShowdownToTpData(rtxtInput.Text));
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
                rtxtPreview.Text = "Error in parsing" + excp.ToString();
            }
        }

        private List<TrainerTable.SheetTrainerData.TrainerPoke> ShowdownToTpData(string showdownText)
        {
            var showdownMons = ConvertShowdownTextToShowdownTeam(showdownText);
            var trainerMons = new List<TrainerTable.SheetTrainerData.TrainerPoke>();
            foreach (var mon in showdownMons)
            {
                var (monsno, formno) = gameData.GetFormIndex(mon.Species, checkShowdown: true);
                if (monsno != 0)
                {
                    if (formno > gameData.pokemonDataTable.Data[monsno].personal.form_max - 1)
                        formno = 0;
                    var sex = mon.Sex switch
                    {
                        "M" => 0,
                        "F" => 1,
                        _ => 3,
                    };
                    var nature = !string.IsNullOrEmpty(mon.Nature) ? gameData.GetLabelIndexByValue(Constants.NATURE_MESSAGEFILE_NAME, mon.Nature) : -1;
                    var ability = !string.IsNullOrEmpty(mon.Ability) ? gameData.GetLabelIndexByValue(Constants.ABILITY_MESSAGEFILE_NAME, mon.Ability) : -1;
                    var item = !string.IsNullOrEmpty(mon.Item) ? gameData.GetLabelIndexByValue(Constants.ITEM_MESSAGEFILE_NAME, mon.Item) : -1;
                    trainerMons.Add(new TrainerTable.SheetTrainerData.TrainerPoke()
                    {
                        MonsNo = (ushort)monsno,
                        FormNo = (ushort)formno,
                        IsRare = mon.Shiny ?? false,
                        Level = (byte)(mon.Level ?? 100),
                        Sex = (byte)sex,
                        Seikaku = (byte)(nature != -1 ? nature : 0),
                        Tokusei = (ushort)(ability != -1 ? ability : 0),
                        Moves = mon.Moves.Select(m => { var moveIndex = gameData.GetLabelIndexByValue(Constants.MOVE_MESSAGEFILE_NAME, m); return (ushort)(moveIndex != -1 ? moveIndex : 0); }).ToArray(),
                        Item = (ushort)(item != -1 ? item : 0),
                        Ball = 28, // TODO
                        Seal = -1,
                        IVs = mon.IVs.Count != 0 ? mon.IVs.Select(i => (byte)i).ToArray() : [31, 31, 31, 31, 31, 31],
                        EVs = mon.EVs.Count != 0 ? mon.EVs.Select(i => (byte)i).ToArray() : [0, 0, 0, 0, 0, 0],
                    });
                }
            }

            return trainerMons;
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

        private string TpToShowdownText(List<TrainerTable.SheetTrainerData.TrainerPoke> trainerMons)
        {
            return ConvertShowdownTeamToShowdownText(trainerMons.Where(m => m.MonsNo != 0).Select(m => {
                var speciesName = gameData.GetLabelByIndex(Constants.POKEMONSPECIES_MESSAGEFILE_NAME, m.MonsNo);
                var sex = m.Sex switch
                {
                    0 => "M",
                    1 => "F",
                    _ => string.Empty,
                };
                return new ShowdownPokemonData()
                {
                    Species = speciesName + (m.FormNo != 0 ? ("-" + gameData.GetFormName(m.MonsNo, m.FormNo, checkShowdown: true).Replace(speciesName, string.Empty).Trim()) : string.Empty),
                    Shiny = m.IsRare,
                    Level = m.Level,
                    Sex = sex,
                    Nature = gameData.GetLabelByIndex(Constants.NATURE_MESSAGEFILE_NAME, m.Seikaku),
                    Ability = gameData.GetLabelByIndex(Constants.ABILITY_MESSAGEFILE_NAME, m.Tokusei),
                    Moves = m.Moves.Where(w => w != 0).Select(w => gameData.GetLabelByIndex(Constants.MOVE_MESSAGEFILE_NAME, w)).ToList(),
                    Item = m.Item != 0 ? gameData.GetLabelByIndex(Constants.ITEM_MESSAGEFILE_NAME, m.Item) : string.Empty,
                    IVs = m.IVs.Select(i => (int)i).ToList(),
                    EVs = m.EVs.Select(e => (int)e).ToList(),
                };
            }).ToList());
        }

        private string BttpToShowdownText(List<BattleTowerTrainerPokemon> trainerMons)
        {
            string showdownText = "";
            return showdownText;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private List<ShowdownPokemonData> ConvertShowdownTextToShowdownTeam(string showdownText)
        {
            var list = new List<ShowdownPokemonData>();

            // Null, Empty, or just Whitespace
            if (string.IsNullOrWhiteSpace(showdownText))
                return list;

            var lines = showdownText.Split('\n') // Split on new line
                .SkipWhile(string.IsNullOrWhiteSpace) // Remove all empty lines at the front
                .Reverse().SkipWhile(string.IsNullOrWhiteSpace).Reverse(); // Remove all empty lines at the back

            var currentMonText = string.Empty;
            foreach (var line in lines)
            {
                var workingLine = line.Trim();
                if (string.IsNullOrEmpty(workingLine) || workingLine == "---")
                {
                    list.Add(new ShowdownPokemonData(currentMonText));
                    currentMonText = string.Empty;
                }
                else
                {
                    currentMonText += workingLine + "\n";
                }
            }

            if (currentMonText != string.Empty)
                list.Add(new ShowdownPokemonData(currentMonText));

            return list;
        }

        private string ConvertShowdownTeamToShowdownText(List<ShowdownPokemonData> showdownMons)
        {
            var strOut = string.Empty;

            foreach (var mon in showdownMons)
                strOut += mon.ConvertToText() + "\n";

            return strOut;
        }
    }
}
