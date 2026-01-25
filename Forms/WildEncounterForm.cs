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
            gameData.SetModified(GameDataSet.DataField.EncounterTableFiles);
        }

        private void OpenUndergroundEncounterEditor(object sender, EventArgs e)
        {
            UgEncounterEditorForm ueef = new(gameData);
            ueef.Show();
            gameData.SetModified(GameDataSet.DataField.UgAreas);
            gameData.SetModified(GameDataSet.DataField.UgEncounterFiles);
            gameData.SetModified(GameDataSet.DataField.UgEncounterLevelSets);
            gameData.SetModified(GameDataSet.DataField.UgSpecialEncounters);
        }

        private void OpenMiscEncounterEditor(object sender, EventArgs e)
        {
            MiscEncounterEditorForm meef = new(gameData);
            meef.Show();
            gameData.SetModified(GameDataSet.DataField.EncounterTableFiles);
        }
    }
}
