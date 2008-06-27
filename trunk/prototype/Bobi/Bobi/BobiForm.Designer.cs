namespace Bobi
{
    partial class BobiForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BobiForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.file_NewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.file_OpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.file_CloseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.file_SaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separator1 = new System.Windows.Forms.ToolStripSeparator();
            this.file_ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.edit_UndoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edit_RedoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separator2 = new System.Windows.Forms.ToolStripSeparator();
            this.edit_CutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edit_CopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edit_PasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edit_DeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separator3 = new System.Windows.Forms.ToolStripSeparator();
            this.edit_SelectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edit_SelectNothingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.view_ZoomInMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.view_ZoomOutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.view_NormalSizeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separator4 = new System.Windows.Forms.ToolStripSeparator();
            this.view_AudioZoomInMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.view_AudioZoomOutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.view_AudioNormalSizeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.audioMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.audio_NewTrackMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.audio_ImportAudioMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.transportBar = new Bobi.View.TransportBar();
            this.projectView = new Bobi.View.ProjectView();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.editMenu,
            this.viewMenu,
            this.audioMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(632, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.file_NewMenuItem,
            this.file_OpenMenuItem,
            this.file_CloseMenuItem,
            this.file_SaveMenuItem,
            this.separator1,
            this.file_ExitMenuItem});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(35, 20);
            this.fileMenu.Text = "&File";
            // 
            // file_NewMenuItem
            // 
            this.file_NewMenuItem.Name = "file_NewMenuItem";
            this.file_NewMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.file_NewMenuItem.Size = new System.Drawing.Size(142, 22);
            this.file_NewMenuItem.Text = "&New";
            this.file_NewMenuItem.Click += new System.EventHandler(this.file_NewMenuItem_Click);
            // 
            // file_OpenMenuItem
            // 
            this.file_OpenMenuItem.Name = "file_OpenMenuItem";
            this.file_OpenMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.file_OpenMenuItem.Size = new System.Drawing.Size(142, 22);
            this.file_OpenMenuItem.Text = "&Open";
            this.file_OpenMenuItem.Click += new System.EventHandler(this.file_OpenMenuItem_Click);
            // 
            // file_CloseMenuItem
            // 
            this.file_CloseMenuItem.Name = "file_CloseMenuItem";
            this.file_CloseMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.file_CloseMenuItem.Size = new System.Drawing.Size(142, 22);
            this.file_CloseMenuItem.Text = "&Close";
            this.file_CloseMenuItem.Click += new System.EventHandler(this.file_CloseMenuItem_Click);
            // 
            // file_SaveMenuItem
            // 
            this.file_SaveMenuItem.Name = "file_SaveMenuItem";
            this.file_SaveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.file_SaveMenuItem.Size = new System.Drawing.Size(142, 22);
            this.file_SaveMenuItem.Text = "&Save";
            this.file_SaveMenuItem.Click += new System.EventHandler(this.file_SaveMenuItem_Click);
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            this.separator1.Size = new System.Drawing.Size(139, 6);
            // 
            // file_ExitMenuItem
            // 
            this.file_ExitMenuItem.Name = "file_ExitMenuItem";
            this.file_ExitMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.file_ExitMenuItem.Size = new System.Drawing.Size(142, 22);
            this.file_ExitMenuItem.Text = "E&xit";
            this.file_ExitMenuItem.Click += new System.EventHandler(this.file_ExitMenuItem_Click);
            // 
            // editMenu
            // 
            this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.edit_UndoMenuItem,
            this.edit_RedoMenuItem,
            this.separator2,
            this.edit_CutMenuItem,
            this.edit_CopyMenuItem,
            this.edit_PasteMenuItem,
            this.edit_DeleteMenuItem,
            this.separator3,
            this.edit_SelectAllMenuItem,
            this.edit_SelectNothingMenuItem});
            this.editMenu.Name = "editMenu";
            this.editMenu.Size = new System.Drawing.Size(37, 20);
            this.editMenu.Text = "&Edit";
            // 
            // edit_UndoMenuItem
            // 
            this.edit_UndoMenuItem.Name = "edit_UndoMenuItem";
            this.edit_UndoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.edit_UndoMenuItem.Size = new System.Drawing.Size(211, 22);
            this.edit_UndoMenuItem.Text = "&Undo";
            this.edit_UndoMenuItem.Click += new System.EventHandler(this.edit_UndoMenuItem_Click);
            // 
            // edit_RedoMenuItem
            // 
            this.edit_RedoMenuItem.Name = "edit_RedoMenuItem";
            this.edit_RedoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.edit_RedoMenuItem.Size = new System.Drawing.Size(211, 22);
            this.edit_RedoMenuItem.Text = "&Redo";
            this.edit_RedoMenuItem.Click += new System.EventHandler(this.edit_RedoMenuItem_Click);
            // 
            // separator2
            // 
            this.separator2.Name = "separator2";
            this.separator2.Size = new System.Drawing.Size(208, 6);
            // 
            // edit_CutMenuItem
            // 
            this.edit_CutMenuItem.Name = "edit_CutMenuItem";
            this.edit_CutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.edit_CutMenuItem.Size = new System.Drawing.Size(211, 22);
            this.edit_CutMenuItem.Text = "Cu&t";
            this.edit_CutMenuItem.Click += new System.EventHandler(this.edit_CutMenuItem_Click);
            // 
            // edit_CopyMenuItem
            // 
            this.edit_CopyMenuItem.Name = "edit_CopyMenuItem";
            this.edit_CopyMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.edit_CopyMenuItem.Size = new System.Drawing.Size(211, 22);
            this.edit_CopyMenuItem.Text = "&Copy";
            this.edit_CopyMenuItem.Click += new System.EventHandler(this.edit_CopyMenuItem_Click);
            // 
            // edit_PasteMenuItem
            // 
            this.edit_PasteMenuItem.Name = "edit_PasteMenuItem";
            this.edit_PasteMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.edit_PasteMenuItem.Size = new System.Drawing.Size(211, 22);
            this.edit_PasteMenuItem.Text = "&Paste";
            this.edit_PasteMenuItem.Click += new System.EventHandler(this.edit_PasteMenuItem_Click);
            // 
            // edit_DeleteMenuItem
            // 
            this.edit_DeleteMenuItem.Name = "edit_DeleteMenuItem";
            this.edit_DeleteMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.edit_DeleteMenuItem.Size = new System.Drawing.Size(211, 22);
            this.edit_DeleteMenuItem.Text = "&Delete";
            this.edit_DeleteMenuItem.Click += new System.EventHandler(this.edit_DeleteMenuItem_Click);
            // 
            // separator3
            // 
            this.separator3.Name = "separator3";
            this.separator3.Size = new System.Drawing.Size(208, 6);
            // 
            // edit_SelectAllMenuItem
            // 
            this.edit_SelectAllMenuItem.Name = "edit_SelectAllMenuItem";
            this.edit_SelectAllMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.edit_SelectAllMenuItem.Size = new System.Drawing.Size(211, 22);
            this.edit_SelectAllMenuItem.Text = "Select &all";
            this.edit_SelectAllMenuItem.Click += new System.EventHandler(this.edit_SelectAllMenuItem_Click);
            // 
            // edit_SelectNothingMenuItem
            // 
            this.edit_SelectNothingMenuItem.Name = "edit_SelectNothingMenuItem";
            this.edit_SelectNothingMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.A)));
            this.edit_SelectNothingMenuItem.Size = new System.Drawing.Size(211, 22);
            this.edit_SelectNothingMenuItem.Text = "Select &nothing";
            this.edit_SelectNothingMenuItem.Click += new System.EventHandler(this.edit_SelectNothingMenuItem_Click);
            // 
            // viewMenu
            // 
            this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.view_ZoomInMenuItem,
            this.view_ZoomOutMenuItem,
            this.view_NormalSizeMenuItem,
            this.separator4,
            this.view_AudioZoomInMenuItem,
            this.view_AudioZoomOutMenuItem,
            this.view_AudioNormalSizeMenuItem});
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(41, 20);
            this.viewMenu.Text = "&View";
            // 
            // view_ZoomInMenuItem
            // 
            this.view_ZoomInMenuItem.Name = "view_ZoomInMenuItem";
            this.view_ZoomInMenuItem.ShortcutKeyDisplayString = "Ctrl++";
            this.view_ZoomInMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemplus)));
            this.view_ZoomInMenuItem.Size = new System.Drawing.Size(225, 22);
            this.view_ZoomInMenuItem.Text = "Zoom &in";
            this.view_ZoomInMenuItem.Click += new System.EventHandler(this.view_ZoomInMenuItem_Click);
            // 
            // view_ZoomOutMenuItem
            // 
            this.view_ZoomOutMenuItem.Name = "view_ZoomOutMenuItem";
            this.view_ZoomOutMenuItem.ShortcutKeyDisplayString = "Ctrl+-";
            this.view_ZoomOutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemMinus)));
            this.view_ZoomOutMenuItem.Size = new System.Drawing.Size(225, 22);
            this.view_ZoomOutMenuItem.Text = "Zoom &out";
            this.view_ZoomOutMenuItem.Click += new System.EventHandler(this.view_ZoomOutMenuItem_Click);
            // 
            // view_NormalSizeMenuItem
            // 
            this.view_NormalSizeMenuItem.Name = "view_NormalSizeMenuItem";
            this.view_NormalSizeMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D0)));
            this.view_NormalSizeMenuItem.Size = new System.Drawing.Size(225, 22);
            this.view_NormalSizeMenuItem.Text = "&Normal size";
            this.view_NormalSizeMenuItem.Click += new System.EventHandler(this.view_NormalSizeMenuItem_Click);
            // 
            // separator4
            // 
            this.separator4.Name = "separator4";
            this.separator4.Size = new System.Drawing.Size(222, 6);
            // 
            // view_AudioZoomInMenuItem
            // 
            this.view_AudioZoomInMenuItem.Name = "view_AudioZoomInMenuItem";
            this.view_AudioZoomInMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift++";
            this.view_AudioZoomInMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Oemplus)));
            this.view_AudioZoomInMenuItem.Size = new System.Drawing.Size(225, 22);
            this.view_AudioZoomInMenuItem.Text = "Audio &zoom in";
            this.view_AudioZoomInMenuItem.Click += new System.EventHandler(this.view_AudioZoomInMenuItem_Click);
            // 
            // view_AudioZoomOutMenuItem
            // 
            this.view_AudioZoomOutMenuItem.Name = "view_AudioZoomOutMenuItem";
            this.view_AudioZoomOutMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+-";
            this.view_AudioZoomOutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.OemMinus)));
            this.view_AudioZoomOutMenuItem.Size = new System.Drawing.Size(225, 22);
            this.view_AudioZoomOutMenuItem.Text = "Audio zoom o&ut";
            this.view_AudioZoomOutMenuItem.Click += new System.EventHandler(this.view_AudioZoomOutMenuItem_Click);
            // 
            // view_AudioNormalSizeMenuItem
            // 
            this.view_AudioNormalSizeMenuItem.Name = "view_AudioNormalSizeMenuItem";
            this.view_AudioNormalSizeMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.D0)));
            this.view_AudioNormalSizeMenuItem.Size = new System.Drawing.Size(225, 22);
            this.view_AudioNormalSizeMenuItem.Text = "Audio normal &size";
            this.view_AudioNormalSizeMenuItem.Click += new System.EventHandler(this.view_AudioNormalSizeMenuItem_Click);
            // 
            // audioMenu
            // 
            this.audioMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.audio_NewTrackMenuItem,
            this.audio_ImportAudioMenuItem});
            this.audioMenu.Name = "audioMenu";
            this.audioMenu.Size = new System.Drawing.Size(46, 20);
            this.audioMenu.Text = "&Audio";
            // 
            // audio_NewTrackMenuItem
            // 
            this.audio_NewTrackMenuItem.Name = "audio_NewTrackMenuItem";
            this.audio_NewTrackMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.audio_NewTrackMenuItem.Size = new System.Drawing.Size(171, 22);
            this.audio_NewTrackMenuItem.Text = "&New track";
            this.audio_NewTrackMenuItem.Click += new System.EventHandler(this.audio_NewTrackMenuItem_Click);
            // 
            // audio_ImportAudioMenuItem
            // 
            this.audio_ImportAudioMenuItem.Name = "audio_ImportAudioMenuItem";
            this.audio_ImportAudioMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.audio_ImportAudioMenuItem.Size = new System.Drawing.Size(171, 22);
            this.audio_ImportAudioMenuItem.Text = "&Import audio";
            this.audio_ImportAudioMenuItem.Click += new System.EventHandler(this.audio_ImportAudioMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.statusProgressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 431);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(632, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(515, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "Current status";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusProgressBar
            // 
            this.statusProgressBar.Name = "statusProgressBar";
            this.statusProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // transportBar
            // 
            this.transportBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.transportBar.Location = new System.Drawing.Point(0, 393);
            this.transportBar.Margin = new System.Windows.Forms.Padding(0);
            this.transportBar.Name = "transportBar";
            this.transportBar.Size = new System.Drawing.Size(632, 38);
            this.transportBar.TabIndex = 3;
            // 
            // projectView
            // 
            this.projectView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.projectView.AudioScale = 1;
            this.projectView.AutoScroll = true;
            this.projectView.BackColor = System.Drawing.SystemColors.Control;
            this.projectView.Location = new System.Drawing.Point(0, 24);
            this.projectView.Name = "projectView";
            this.projectView.Project = null;
            this.projectView.Size = new System.Drawing.Size(632, 366);
            this.projectView.TabIndex = 2;
            this.projectView.Text = "projectView1";
            this.projectView.Zoom = 1;
            // 
            // BobiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Controls.Add(this.transportBar);
            this.Controls.Add(this.projectView);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "BobiForm";
            this.Text = "Bobi";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BobiForm_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem file_ExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem file_OpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem file_NewMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripMenuItem file_SaveMenuItem;
        private System.Windows.Forms.ToolStripSeparator separator1;
        private System.Windows.Forms.ToolStripMenuItem audioMenu;
        private System.Windows.Forms.ToolStripMenuItem audio_NewTrackMenuItem;
        private Bobi.View.ProjectView projectView;
        private System.Windows.Forms.ToolStripMenuItem editMenu;
        private System.Windows.Forms.ToolStripMenuItem edit_UndoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem edit_RedoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem view_ZoomInMenuItem;
        private System.Windows.Forms.ToolStripMenuItem view_ZoomOutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem view_NormalSizeMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripMenuItem audio_ImportAudioMenuItem;
        private System.Windows.Forms.ToolStripMenuItem file_CloseMenuItem;
        private System.Windows.Forms.ToolStripSeparator separator2;
        private System.Windows.Forms.ToolStripMenuItem edit_CutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem edit_CopyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem edit_PasteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem edit_DeleteMenuItem;
        private System.Windows.Forms.ToolStripSeparator separator3;
        private System.Windows.Forms.ToolStripMenuItem edit_SelectAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem edit_SelectNothingMenuItem;
        private System.Windows.Forms.ToolStripProgressBar statusProgressBar;
        private System.Windows.Forms.ToolStripSeparator separator4;
        private System.Windows.Forms.ToolStripMenuItem view_AudioZoomInMenuItem;
        private System.Windows.Forms.ToolStripMenuItem view_AudioZoomOutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem view_AudioNormalSizeMenuItem;
        private Bobi.View.TransportBar transportBar;
    }
}

