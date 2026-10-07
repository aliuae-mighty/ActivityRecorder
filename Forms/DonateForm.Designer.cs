namespace Recorder.Forms
{
    partial class DonateForm
    {


        private void InitializeComponent()
        {
            this.tlpDontate = new System.Windows.Forms.TableLayoutPanel();
            this.btnVisitWebsite = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.linkToWebsite = new System.Windows.Forms.LinkLabel();
            this.lbDescription = new System.Windows.Forms.Label();
            this.lbTitle = new System.Windows.Forms.Label();
            this.tlpDontate.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpDontate
            // 
            this.tlpDontate.ColumnCount = 1;
            this.tlpDontate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDontate.Controls.Add(this.btnVisitWebsite, 0, 1);
            this.tlpDontate.Controls.Add(this.panel1, 0, 0);
            this.tlpDontate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDontate.Location = new System.Drawing.Point(0, 0);
            this.tlpDontate.Name = "tlpDontate";
            this.tlpDontate.RowCount = 2;
            this.tlpDontate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDontate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.tlpDontate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpDontate.Size = new System.Drawing.Size(403, 116);
            this.tlpDontate.TabIndex = 3;
            // 
            // btnVisitWebsite
            // 
            this.btnVisitWebsite.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnVisitWebsite.Location = new System.Drawing.Point(134, 78);
            this.btnVisitWebsite.Margin = new System.Windows.Forms.Padding(2, 2, 5, 2);
            this.btnVisitWebsite.Name = "btnVisitWebsite";
            this.btnVisitWebsite.Size = new System.Drawing.Size(131, 32);
            this.btnVisitWebsite.TabIndex = 4;
            this.btnVisitWebsite.Text = "Visit Website / Donate";
            this.btnVisitWebsite.UseVisualStyleBackColor = true;
            this.btnVisitWebsite.Click += new System.EventHandler(this.btnVisitWebsite_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.linkToWebsite);
            this.panel1.Controls.Add(this.lbDescription);
            this.panel1.Controls.Add(this.lbTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(397, 67);
            this.panel1.TabIndex = 8;
            // 
            // linkToWebsite
            // 
            this.linkToWebsite.AutoSize = true;
            this.linkToWebsite.Location = new System.Drawing.Point(6, 47);
            this.linkToWebsite.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.linkToWebsite.Name = "linkToWebsite";
            this.linkToWebsite.Size = new System.Drawing.Size(126, 13);
            this.linkToWebsite.TabIndex = 9;
            this.linkToWebsite.TabStop = true;
            this.linkToWebsite.Text = "https://useractemu.com/";
            this.linkToWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkToWebsite_LinkClicked);
            // 
            // lbDescription
            // 
            this.lbDescription.Location = new System.Drawing.Point(5, 20);
            this.lbDescription.Margin = new System.Windows.Forms.Padding(4);
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(371, 26);
            this.lbDescription.TabIndex = 8;
            this.lbDescription.Text = "Your support helps keep this tool free and regularly updated. Click below to supp" +
    "ort the project on our official page.";
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbTitle.Location = new System.Drawing.Point(0, 0);
            this.lbTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Padding = new System.Windows.Forms.Padding(3);
            this.lbTitle.Size = new System.Drawing.Size(123, 19);
            this.lbTitle.TabIndex = 0;
            this.lbTitle.Text = "Support the Project";
            // 
            // DonateForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(403, 116);
            this.Controls.Add(this.tlpDontate);
            this.MaximizeBox = false;
            this.Name = "DonateForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "DONATE";
            this.Load += new System.EventHandler(this.AppSettingsForm_Load);
            this.tlpDontate.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TableLayoutPanel tlpDontate;
        private System.Windows.Forms.Button btnVisitWebsite;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbDescription;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.LinkLabel linkToWebsite;
    }
}