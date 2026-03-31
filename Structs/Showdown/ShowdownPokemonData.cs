using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public class ShowdownPokemonData
    {
        public string Nickname { get; set; }
        public string Species { get; set; }
        public string Item { get; set; }
        public string Ability { get; set; }
        public List<string> Moves { get; set; } = new List<string>();
        public string Nature { get; set; }
        public string Sex { get; set; }
        public List<int> EVs { get; set; } = new List<int>();
        public List<int> IVs { get; set; } = new List<int>();
        public int? Level { get; set; }
        public bool? Shiny { get; set; }
        public int? Friendship { get; set; }
        public string Pokeball { get; set; }
        public string HiddenPower { get; set; }
        public int? DynamaxLevel { get; set; }
        public bool? GMax { get; set; }
        public string TeraType { get; set; }

        public ShowdownPokemonData() { }

        public ShowdownPokemonData(string text)
        {
            ParseFromText(text);
        }

        private void ParseFromText(string text)
        {
            // Null, Empty, or just Whitespace
            if (string.IsNullOrWhiteSpace(text))
                return;

            var lines = text.Split('\n') // Split on new line
                .SkipWhile(string.IsNullOrWhiteSpace) // Remove all empty lines at the front
                .Reverse().SkipWhile(string.IsNullOrWhiteSpace).Reverse(); // Remove all empty lines at the back

            // Remove leading and trailing whitespace from each line
            lines = lines.Select(l => l.Trim());

            bool firstLine = true;
            foreach (var line in lines)
            {
                var workingLine = line;
                if (firstLine)
                {
                    // Item
                    var itemSplit = line.Split(" @ ");
                    workingLine = itemSplit[0];
                    if (itemSplit.Length > 1)
                        Item = itemSplit[1];

                    // Sex
                    if (workingLine.EndsWith(" (M)"))
                    {
                        Sex = "M";
                        workingLine = workingLine[..^4];
                    }
                    else if (workingLine.EndsWith(" (F)"))
                    {
                        Sex = "F";
                        workingLine = workingLine[..^4];
                    }

                    // Nickname
                    if (workingLine.EndsWith(')') && workingLine.Contains('('))
                    {
                        var nicknameSplit = workingLine[..^1].Split("(");
                        Species = nicknameSplit[1];
                        Nickname = nicknameSplit[0].Trim();
                    }
                    else
                    {
                        Species = workingLine;
                        Nickname = string.Empty;
                    }

                    firstLine = false;
                }
                else
                {
                    if (workingLine.StartsWith("Trait: "))
                    {
                        Ability = workingLine[7..];
                    }
                    else if (workingLine.StartsWith("Ability: "))
                    {
                        Ability = workingLine[9..];
                    }
                    else if (workingLine == "Shiny: Yes")
                    {
                        Shiny = true;
                    }
                    else if (workingLine.StartsWith("Level: "))
                    {
                        if (int.TryParse(workingLine[7..], out int result))
                            Level = result;
                    }
                    else if (workingLine.StartsWith("Happiness: "))
                    {
                        if (int.TryParse(workingLine[11..], out int result))
                            Friendship = result;
                    }
                    else if (workingLine.StartsWith("Pokeball: "))
                    {
                        Pokeball = workingLine[10..];
                    }
                    else if (workingLine.StartsWith("Hidden Power: "))
                    {
                        HiddenPower = workingLine[14..];
                        if (IVs.Count == 0)
                            IVs = FindHiddenPowerIVsFromType(HiddenPower);
                    }
                    else if (workingLine.StartsWith("Tera Type: "))
                    {
                        TeraType = workingLine[11..];
                    }
                    else if (workingLine == "Gigantamax: Yes")
                    {
                        GMax = true;
                    }
                    else if (workingLine.StartsWith("EVs: "))
                    {
                        workingLine = workingLine[5..];
                        var evLines = workingLine.Split('/');

                        EVs.AddRange(Enumerable.Repeat(0, 6));

                        foreach (var evLine in evLines)
                        {
                            var evSplit = evLine.Trim().Split(' ');

                            // No stat name
                            if (evSplit.Length < 2)
                                continue;

                            // Not a valid stat name
                            var index = FindIndexFromStatName(evSplit[1]);
                            if (index < 0)
                                continue;

                            if (int.TryParse(evSplit[0], out int result))
                                EVs[index] = result;
                        }
                    }
                    else if (workingLine.StartsWith("IVs: "))
                    {
                        workingLine = workingLine[5..];
                        var ivLines = workingLine.Split('/');

                        IVs.AddRange(Enumerable.Repeat(31, 6));

                        foreach (var ivLine in ivLines)
                        {
                            var ivSplit = ivLine.Trim().Split(' ');

                            // No stat name
                            if (ivSplit.Length < 2)
                                continue;

                            // Not a valid stat name
                            var index = FindIndexFromStatName(ivSplit[1]);
                            if (index < 0)
                                continue;

                            if (int.TryParse(ivSplit[0], out int result))
                                IVs[index] = result;
                        }
                    }
                    else if (workingLine.EndsWith(" Nature") || workingLine.EndsWith(" nature"))
                    {
                        Nature = workingLine[..^7];
                    }
                    else if (workingLine.StartsWith('-') || workingLine.StartsWith('~'))
                    {
                        workingLine = workingLine[1..];
                        if (workingLine[0] == ' ')
                            workingLine = workingLine[1..];

                        if (workingLine.StartsWith("Hidden Power [") && IVs.Count == 0)
                        {
                            IVs = FindHiddenPowerIVsFromType(workingLine[14..^1]);
                            workingLine = workingLine[..12];
                        }
                        else if (workingLine == "Frustration" && !Friendship.HasValue)
                        {
                            Friendship = 0;
                        }

                        Moves.Add(workingLine);
                    }
                }
            }
        }

        public string ConvertToText()
        {
            string strOut = string.Empty;

            strOut += Species;
            strOut += Sex switch
            {
                "M" => " (M)",
                "F" => " (F)",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(Item))
                strOut += string.Format(" @ {0}", Item);

            strOut += "  \n";

            if (!string.IsNullOrEmpty(Ability))
                strOut += string.Format("Ability: {0}  \n", Ability);

            if (Level.HasValue && Level != 100)
                strOut += string.Format("Level: {0}  \n", Level);

            if (Shiny.HasValue && Shiny == true)
                strOut += "Shiny: Yes  \n";

            if (Friendship.HasValue && Friendship != 255)
                strOut += string.Format("Happiness: {0}  \n", Friendship);

            if (!string.IsNullOrEmpty(Pokeball))
                strOut += string.Format("Pokeball: {0}  \n", Pokeball);

            if (!string.IsNullOrEmpty(HiddenPower))
                strOut += string.Format("Hidden Power: {0}  \n", HiddenPower);

            if (DynamaxLevel.HasValue && DynamaxLevel != 10)
                strOut += string.Format("Dynamax Level: {0}  \n", DynamaxLevel);

            if (GMax.HasValue && GMax == true)
                strOut += "Gigantamax: Yes  \n";

            if (!string.IsNullOrEmpty(TeraType))
                strOut += string.Format("Tera Type: {0}  \n", TeraType);

            if (EVs.Where(s => s != 0).Any())
                strOut += string.Format("EVs: {0}  \n", string.Join(" / ", EVs.Select((e, i) => (e, FindStatNameFromIndex(i))).Where(s => s.e != 0).Select(s => string.Format("{0} {1}", s.e, s.Item2))));

            if (!string.IsNullOrEmpty(Nature))
                strOut += string.Format("{0} Nature  \n", Nature);

            if (IVs.Where(s => s != 31).Any())
                strOut += string.Format("IVs: {0}  \n", string.Join(" / ", IVs.Select((e, i) => (e, FindStatNameFromIndex(i))).Where(s => s.e != 31).Select(s => string.Format("{0} {1}", s.e, s.Item2))));

            foreach (var move in Moves)
                strOut += string.Format("- {0}  \n", move);

            return strOut;
        }

        private int FindIndexFromStatName(string name)
        {
            return name switch
            {
                "HP" => 0,
                "Atk" => 1,
                "Def" => 2,
                "SpA" => 3,
                "SpD" => 4,
                "Spe" or "Spd" => 5, // Showdown considers "Spd" to be speed
                _ => -1,
            };
        }

        private List<int> FindHiddenPowerIVsFromType(string type)
        {
            return type switch
            {
                "Bug" =>      [31, 30, 30, 31, 30, 31], // Atk 30, Def 30, SpD 30
                "Dark" =>     [31, 31, 31, 31, 31, 31],
                "Dragon" =>   [31, 30, 31, 31, 31, 31], // Atk 30
                "Electric" => [31, 31, 31, 30, 31, 31], // SpA 30
                "Fighting" => [31, 31, 30, 30, 30, 30], // Def 30, SpA 30, SpD 30, Spe 30
                "Fire" =>     [31, 30, 31, 30, 31, 30], // Atk 30, SpA 30, Spe 30
                "Flying" =>   [30, 30, 30, 30, 30, 31], // HP  30, Atk 30, Def 30, SpA 30, SpD 30
                "Ghost" =>    [31, 31, 30, 31, 30, 31], // Def 30, SpD 30
                "Grass" =>    [31, 30, 31, 30, 31, 31], // Atk 30, SpA 30
                "Ground" =>   [31, 31, 31, 30, 30, 31], // SpA 30, SpD 30
                "Ice" =>      [31, 30, 30, 31, 31, 31], // Atk 30, Def 30
                "Poison" =>   [31, 31, 30, 30, 30, 31], // Def 30, SpA 30, SpD 30
                "Psychic" =>  [31, 30, 31, 31, 31, 30], // Atk 30, Spe 30
                "Rock" =>     [31, 31, 30, 31, 30, 30], // Def 30, SpD 30, Spe 30
                "Steel" =>    [31, 31, 31, 31, 30, 31], // SpD 30
                "Water" =>    [31, 30, 30, 30, 31, 31], // Atk 30, Def 30, SpA 30
                _ => [],
            };
        }

        private string FindStatNameFromIndex(int index)
        {
            return index switch
            {
                0 => "HP",
                1 => "Atk",
                2 => "Def",
                3 => "SpA",
                4 => "SpD",
                5 => "Spe",
                _ => string.Empty,
            };
        }
    }
}
