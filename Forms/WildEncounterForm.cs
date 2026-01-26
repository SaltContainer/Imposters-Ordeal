using System;
using System.Windows.Forms;

namespace ImpostersOrdeal
{
    public partial class WildEncounterForm : Form
    {
        private GameDataSet gameData;

        public WildEncounterForm(GameDataSet gameData)
        {
            this.gameData = gameData;

            InitializeComponent();
        }

        private void OpenEcounterTableEditor(object sender, EventArgs e)
        {
            EncounterTableEditorForm etef = new(gameData);
            etef.Show();
            gameData.SetModified(typeof(FieldEncountTable));
        }

        private void OpenUndergroundEncounterEditor(object sender, EventArgs e)
        {
            UgEncounterEditorForm ueef = new(gameData);
            ueef.Show();
            gameData.SetModified(typeof(UgHideawayTable));
            gameData.SetModified(typeof(UgEncounterTableCollection));
            gameData.SetModified(typeof(UgEncounterLevelTable));
            gameData.SetModified(typeof(UgPokemonDataTable));
        }

        private void OpenMiscEncounterEditor(object sender, EventArgs e)
        {
            MiscEncounterEditorForm meef = new(gameData);
            meef.Show();
            gameData.SetModified(typeof(FieldEncountTable));
        }
    }
}
