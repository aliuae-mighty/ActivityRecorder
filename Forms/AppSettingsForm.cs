using Recorder.Configuration;
using Recorder.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder.Forms
{
    public partial class AppSettingsForm : Form
    {
        public string ControlName { get; }
        public new MainForm ParentForm { get; }

        public AppSettingsForm(MainForm mainForm, string controlName)
        {
            InitializeComponent();

            this.ParentForm = mainForm;
            this.ControlName = controlName;

            // Enable manual positioning for the child form
            this.StartPosition = FormStartPosition.Manual;
        }

        private void AppSettingsForm_Load(object sender, EventArgs e)
        {
            AppConfig config = ConfigManager.Load();
            ConfigManager.ApplyToForm(this, config);

            // Calculate and set the form's location on load
            this.PositionRelativeTo(ParentForm);
        }

        private void btnSettingsSave_Click(object sender, EventArgs e)
        {
            AppConfig config = ConfigManager.ExtractFromForm(this);
            ConfigManager.Save(config);
            ParentForm.ConfigManager = config;

            if (!string.IsNullOrWhiteSpace(config.SettingsFormTitle))
                this.ParentForm.Text = config.SettingsFormTitle;

            this.ParentForm.TopMost = config.SettingsAlwaysOnTop;
            this.Hide();
        }
    }
}