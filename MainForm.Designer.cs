namespace Recorder
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.stStripDonate = new System.Windows.Forms.StatusStrip();
            this.tsDonateLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tplMain = new System.Windows.Forms.TableLayoutPanel();
            this.btnRecord = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnFinish = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.stStripDonate.SuspendLayout();
            this.tplMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // stStripDonate
            // 
            this.stStripDonate.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.stStripDonate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsDonateLabel});
            this.stStripDonate.Location = new System.Drawing.Point(0, 101);
            this.stStripDonate.Name = "stStripDonate";
            this.stStripDonate.Size = new System.Drawing.Size(241, 22);
            this.stStripDonate.TabIndex = 1;
            this.stStripDonate.Text = "Donate";
            // 
            // tsDonateLabel
            // 
            this.tsDonateLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.tsDonateLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.tsDonateLabel.IsLink = true;
            this.tsDonateLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.tsDonateLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(20)))), ((int)(((byte)(60)))));
            this.tsDonateLabel.Name = "tsDonateLabel";
            this.tsDonateLabel.Size = new System.Drawing.Size(195, 17);
            this.tsDonateLabel.Spring = true;
            this.tsDonateLabel.Text = "❤ SUPPORT PROJECT";
            this.tsDonateLabel.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(20)))), ((int)(((byte)(60)))));
            this.tsDonateLabel.Click += new System.EventHandler(this.tsDonateLabel_Click);
            // 
            // tplMain
            // 
            this.tplMain.ColumnCount = 2;
            this.tplMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tplMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tplMain.Controls.Add(this.btnRecord, 0, 0);
            this.tplMain.Controls.Add(this.btnPlay, 1, 0);
            this.tplMain.Controls.Add(this.btnFinish, 0, 1);
            this.tplMain.Controls.Add(this.btnSettings, 1, 1);
            this.tplMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tplMain.Location = new System.Drawing.Point(0, 0);
            this.tplMain.Name = "tplMain";
            this.tplMain.RowCount = 2;
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tplMain.Size = new System.Drawing.Size(241, 101);
            this.tplMain.TabIndex = 2;
            // 
            // btnRecord
            // 
            this.btnRecord.BackColor = System.Drawing.SystemColors.HighlightText;
            this.btnRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRecord.Image = global::Recorder.Properties.Resources.record;
            this.btnRecord.Location = new System.Drawing.Point(3, 3);
            this.btnRecord.Margin = new System.Windows.Forms.Padding(3, 3, 1, 1);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(116, 46);
            this.btnRecord.TabIndex = 0;
            this.btnRecord.Text = "Record (Ctrl+F7)";
            this.btnRecord.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRecord.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRecord.UseVisualStyleBackColor = false;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.BackColor = System.Drawing.SystemColors.HighlightText;
            this.btnPlay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPlay.Image = global::Recorder.Properties.Resources.play;
            this.btnPlay.Location = new System.Drawing.Point(121, 3);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(1, 3, 3, 1);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(117, 46);
            this.btnPlay.TabIndex = 1;
            this.btnPlay.Text = "Play (Ctrl+F9)";
            this.btnPlay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPlay.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPlay.UseVisualStyleBackColor = false;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnFinish
            // 
            this.btnFinish.BackColor = System.Drawing.SystemColors.HighlightText;
            this.btnFinish.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFinish.Image = global::Recorder.Properties.Resources.finish;
            this.btnFinish.Location = new System.Drawing.Point(3, 51);
            this.btnFinish.Margin = new System.Windows.Forms.Padding(3, 1, 1, 3);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(116, 47);
            this.btnFinish.TabIndex = 2;
            this.btnFinish.Text = "Stop (Ctrl+F8)";
            this.btnFinish.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFinish.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFinish.UseVisualStyleBackColor = false;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.BackColor = System.Drawing.SystemColors.HighlightText;
            this.btnSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSettings.Location = new System.Drawing.Point(121, 51);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(1, 1, 3, 3);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(117, 47);
            this.btnSettings.TabIndex = 4;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = false;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 123);
            this.Controls.Add(this.tplMain);
            this.Controls.Add(this.stStripDonate);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Activity Recorder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.stStripDonate.ResumeLayout(false);
            this.stStripDonate.PerformLayout();
            this.tplMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip stStripDonate;
        private System.Windows.Forms.TableLayoutPanel tplMain;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnFinish;
        private System.Windows.Forms.ToolStripStatusLabel tsDonateLabel;
    }
}

