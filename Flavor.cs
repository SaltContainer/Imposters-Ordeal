using System;
using System.Collections.Generic;

namespace ImpostersOrdeal
{
    /// <summary>
    ///  Responsible for ensuring an enjoyable experience for the user. Don't worry about it.
    /// </summary>
    public class Flavor
    {
        private Random rng = new();
        private Controller controller;

        private readonly string[] verbs = new string[]
        {
            "Inspectin'", "Inspectigatin'", "Inspectimigatin'",
            "Perusin'", "Perusigatin'", "Perusulatin'",
            "Checkin'", "Checkin' out",
            "Examinin'", "Examigatin'",
            "Reviewin'",
            "Scannin'", "Doin' the scan on",
            "Scrutinizin'", "Scrutinatin'", "Scrutilatin'",
            "Porin' over",
            "Researchin'", "Researchigatin'",
            "Analyzin'", "Analysisin'",
            "Parsin'",
            "Delvin' into", "Doin' a delve into",
            "Explorin'", "Exploratin'",
            "Investigatin'", "Investimigatin'", "Investigaterin'",
            "Probin'",
            "Studyin'", "Studifyin'",
            "Categorizin'", "Categorygatin'",
            "Classifyin'", "Classificationifyin'",
            "Goin' over", "Goin' into",
            "Skimmin' through", "Skimmin' over",
            "Determinin'",
            "Separatin'", "Separationifyin'",
            "Configurin'", "Configuratin'",
            "Structurin'", "Structurifyin'",
            "Settin' up", "Setupin'",
            "Makin' ready", "Ready-makin'",
            "Gettin' ready", "Ready-gettin'",
            "Puttin' together",
            "Arrangin'", "Arrangementin'",
            "Assemblin'",
            "Workin' out", "Workin' on",
            "Preparin'", "Preparatin'", "Preparationin'",
            "Formulatin'",
            "Plannin'", "Plannin' out",
            "Doin'",
            "Sortin'", "Sortin' out", "Sortin' up",
            "Extractin'"
        };

        private readonly string[] articles = new string[]
        {
            "The", "Your", "Yer", "All the", "That", "All this", "Some"
        };

        private readonly string[] nouns = new string[]
        {
            "Stuff", "Data", "Info", "Input", "Delicious data", "Scrumptious info"
        };

        private readonly (string, string)[] thoughts = new (string, string)[]
        {
            ("You know what? I like ", "."),
            ("How about I put ", " everywhere?"),
            ("You know what? Screw ", ". Imma delete it."),
            ("How many places can I place ", " I wonder?"),
            ("Hmmm, now where should I place ", "?"),
            ("You don't happen to like ", ", do you?"),
            ("I wonder where I should place this ", "..."),
            ("Hmmm... ", "? Yeah, let's place one here. Why not?")
        };

        private readonly string[] quotes = new string[]
        {
            "\"Do you all think these editors grow on trees?\" - Nifyr",
            "\"Man, what an ordeal...\" - Nifyr",
            "\"And if there's still more bugs, I am going to...\" - Nifyr"
        };

        public Flavor(Controller controller)
        {
            this.controller = controller;
        }

        public string GetSubTask()
        {
            if (rng.Next(10) != 0)
                return verbs[rng.Next(verbs.Length)] + " " + articles[rng.Next(articles.Length)].ToLower() + " " + nouns[rng.Next(nouns.Length)].ToLower() + ".";
            return GetQuote();
        }

        private string GetRandomName()
        {
            List<List<string>> strLists = new();

            // TODO: For items and abilities, check validity
            strLists.Add(controller.GetGameData().GetAllLabels(Constants.ABILITY_MESSAGEFILE_NAME));
            strLists.Add(controller.GetGameData().GetAllLabels(Constants.POKEMONSPECIES_MESSAGEFILE_NAME));
            strLists.Add(controller.GetGameData().GetAllLabels(Constants.ITEM_MESSAGEFILE_NAME));
            strLists.Add(controller.GetGameData().GetAllLabels(Constants.ABILITY_MESSAGEFILE_NAME));

            int listIdx = rng.Next(strLists.Count);
            return strLists[listIdx][rng.Next(strLists[listIdx].Count)];
        }

        public string GetThought()
        {
            if (rng.Next(10) != 0)
            {
                int thoughtIdx = rng.Next(thoughts.Length);
                return thoughts[thoughtIdx].Item1 + GetRandomName() + thoughts[thoughtIdx].Item2;
            }
            return GetQuote();
        }

        private string GetQuote()
        {
            return quotes[rng.Next(quotes.Length)];
        }
    }
}
