using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ImpostersOrdeal.Plugins;

namespace ImpostersOrdeal
{
    /// <summary>
    /// Form for selecting a data source provider on startup.
    /// </summary>
    public class ProviderSelectionForm : Form
    {
        private readonly List<IDataSourceProvider> providers;
        private ListBox providerListBox;
        private Label descriptionLabel;
        private Button okButton;
        private Button cancelButton;

        /// <summary>
        /// The selected data source provider.
        /// </summary>
        public IDataSourceProvider SelectedProvider { get; private set; }

        public ProviderSelectionForm(List<IDataSourceProvider> providers)
        {
            this.providers = providers;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Select Data Source";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title label
            var titleLabel = new Label
            {
                Text = "Select how to load game data:",
                Location = new Point(15, 15),
                Size = new Size(400, 25),
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Bold)
            };
            this.Controls.Add(titleLabel);

            // Provider list box
            providerListBox = new ListBox
            {
                Location = new Point(15, 45),
                Size = new Size(400, 120),
                DisplayMember = "ProviderName"
            };
            providerListBox.SelectedIndexChanged += ProviderListBox_SelectedIndexChanged;
            providerListBox.DoubleClick += (s, e) => { if (SelectedProvider != null) { DialogResult = DialogResult.OK; Close(); } };

            foreach (var provider in providers)
            {
                providerListBox.Items.Add(provider);
            }

            this.Controls.Add(providerListBox);

            // Description group box
            var descriptionGroup = new GroupBox
            {
                Text = "Description",
                Location = new Point(15, 175),
                Size = new Size(400, 80)
            };

            descriptionLabel = new Label
            {
                Location = new Point(10, 20),
                Size = new Size(380, 50),
                Text = "Select a provider to see its description."
            };
            descriptionGroup.Controls.Add(descriptionLabel);
            this.Controls.Add(descriptionGroup);

            // OK button
            okButton = new Button
            {
                Text = "OK",
                Location = new Point(255, 270),
                Size = new Size(75, 25),
                Enabled = false,
                DialogResult = DialogResult.OK
            };
            okButton.Click += (s, e) => Close();
            this.Controls.Add(okButton);

            // Cancel button
            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(340, 270),
                Size = new Size(75, 25),
                DialogResult = DialogResult.Cancel
            };
            cancelButton.Click += (s, e) => Close();
            this.Controls.Add(cancelButton);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;

            // Select first provider by default
            if (providerListBox.Items.Count > 0)
            {
                providerListBox.SelectedIndex = 0;
            }
        }

        private void ProviderListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (providerListBox.SelectedItem is IDataSourceProvider provider)
            {
                SelectedProvider = provider;
                descriptionLabel.Text = provider.ProviderDescription ?? provider.Description ?? "No description available.";
                okButton.Enabled = true;
            }
            else
            {
                SelectedProvider = null;
                descriptionLabel.Text = "Select a provider to see its description.";
                okButton.Enabled = false;
            }
        }
    }
}
