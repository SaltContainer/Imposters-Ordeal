namespace ImpostersOrdeal
{
    public class ShowdownForm
    {
        public string SpeciesName { get; set; }
        public string FormName { get; set; }
        public int SpeciesIndex { get; set; }
        public int FormIndex { get; set; }

        public ShowdownForm(string speciesName, string formName, int speciesIndex, int formIndex)
        {
            SpeciesName = speciesName;
            FormName = formName;
            SpeciesIndex = speciesIndex;
            FormIndex = formIndex;
        }
    }
}
