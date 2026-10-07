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
    public partial class DonateForm : Form
    {
        public string ControlName { get; }
        public new MainForm ParentForm { get; }

        public DonateForm(MainForm mainForm, string controlName)
        {
            InitializeComponent();

            this.ParentForm = mainForm;
            this.ControlName = controlName;

            // Enable manual positioning for the child form
            this.StartPosition = FormStartPosition.Manual;
        }

        private void AppSettingsForm_Load(object sender, EventArgs e)
        {

            // Calculate and set the form's location on load
            this.PositionRelativeTo(ParentForm);

        }

        private void btnVisitWebsite_Click(object sender, EventArgs e)
        {
            string url = "https://useractemu.com/#donate";
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open the link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Hide();
        }

        private void linkToWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            btnVisitWebsite_Click(null, null);
        }
    }
}