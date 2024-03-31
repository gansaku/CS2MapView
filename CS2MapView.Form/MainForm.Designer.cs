namespace CS2MapView.Form
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            MainStatusStrip = new StatusStrip();
            StatusBarMessageLabel = new ToolStripStatusLabel();
            StatusBarProgressBar = new ToolStripProgressBar();
            StatusBarDisplayInfoLabel = new ToolStripStatusLabel();
            menuStrip1 = new MenuStrip();
            FileMenuItem = new ToolStripMenuItem();
            OpenFileMenuItem = new ToolStripMenuItem();
            SaveImageToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            CloseMenuItem = new ToolStripMenuItem();
            ViewMenuItem = new ToolStripMenuItem();
            OptionMenuItem = new ToolStripMenuItem();
            HelpMenuItem = new ToolStripMenuItem();
            GitHubMenuItem = new ToolStripMenuItem();
            VersionInfoMenuItem = new ToolStripMenuItem();
            panel1 = new Panel();
            HScrollBar = new HScrollBar();
            VScrollBar = new VScrollBar();
            SkiaControl = new SkiaSharp.Views.Desktop.SKGLControl();
            MainStatusStrip.SuspendLayout();
            menuStrip1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // MainStatusStrip
            // 
            resources.ApplyResources(MainStatusStrip, "MainStatusStrip");
            MainStatusStrip.ImageScalingSize = new Size(20, 20);
            MainStatusStrip.Items.AddRange(new ToolStripItem[] { StatusBarMessageLabel, StatusBarProgressBar, StatusBarDisplayInfoLabel });
            MainStatusStrip.Name = "MainStatusStrip";
            MainStatusStrip.SizeChanged += MainStatusStrip_SizeChanged;
            // 
            // StatusBarMessageLabel
            // 
            resources.ApplyResources(StatusBarMessageLabel, "StatusBarMessageLabel");
            StatusBarMessageLabel.Name = "StatusBarMessageLabel";
            // 
            // StatusBarProgressBar
            // 
            resources.ApplyResources(StatusBarProgressBar, "StatusBarProgressBar");
            StatusBarProgressBar.ForeColor = Color.FromArgb(192, 255, 192);
            StatusBarProgressBar.Name = "StatusBarProgressBar";
            // 
            // StatusBarDisplayInfoLabel
            // 
            resources.ApplyResources(StatusBarDisplayInfoLabel, "StatusBarDisplayInfoLabel");
            StatusBarDisplayInfoLabel.Name = "StatusBarDisplayInfoLabel";
            // 
            // menuStrip1
            // 
            resources.ApplyResources(menuStrip1, "menuStrip1");
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { FileMenuItem, ViewMenuItem, HelpMenuItem });
            menuStrip1.Name = "menuStrip1";
            // 
            // FileMenuItem
            // 
            resources.ApplyResources(FileMenuItem, "FileMenuItem");
            FileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { OpenFileMenuItem, SaveImageToolStripMenuItem, toolStripSeparator1, CloseMenuItem });
            FileMenuItem.Name = "FileMenuItem";
            // 
            // OpenFileMenuItem
            // 
            resources.ApplyResources(OpenFileMenuItem, "OpenFileMenuItem");
            OpenFileMenuItem.Name = "OpenFileMenuItem";
            OpenFileMenuItem.Click += OpenFileMenuItem_Click;
            // 
            // SaveImageToolStripMenuItem
            // 
            resources.ApplyResources(SaveImageToolStripMenuItem, "SaveImageToolStripMenuItem");
            SaveImageToolStripMenuItem.Name = "SaveImageToolStripMenuItem";
            SaveImageToolStripMenuItem.Click += SaveImageMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // CloseMenuItem
            // 
            resources.ApplyResources(CloseMenuItem, "CloseMenuItem");
            CloseMenuItem.Name = "CloseMenuItem";
            CloseMenuItem.Click += CloseMenuItem_Click;
            // 
            // ViewMenuItem
            // 
            resources.ApplyResources(ViewMenuItem, "ViewMenuItem");
            ViewMenuItem.DropDownItems.AddRange(new ToolStripItem[] { OptionMenuItem });
            ViewMenuItem.Name = "ViewMenuItem";
            // 
            // OptionMenuItem
            // 
            resources.ApplyResources(OptionMenuItem, "OptionMenuItem");
            OptionMenuItem.Name = "OptionMenuItem";
            OptionMenuItem.Click += OptionMenuItem_Click;
            // 
            // HelpMenuItem
            // 
            resources.ApplyResources(HelpMenuItem, "HelpMenuItem");
            HelpMenuItem.DropDownItems.AddRange(new ToolStripItem[] { GitHubMenuItem, VersionInfoMenuItem });
            HelpMenuItem.Name = "HelpMenuItem";
            // 
            // GitHubMenuItem
            // 
            resources.ApplyResources(GitHubMenuItem, "GitHubMenuItem");
            GitHubMenuItem.Name = "GitHubMenuItem";
            GitHubMenuItem.Click += GitHubMenuItem_Click;
            // 
            // VersionInfoMenuItem
            // 
            resources.ApplyResources(VersionInfoMenuItem, "VersionInfoMenuItem");
            VersionInfoMenuItem.Name = "VersionInfoMenuItem";
            VersionInfoMenuItem.Click += VersionInfoMenuItem_Click;
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Controls.Add(HScrollBar);
            panel1.Controls.Add(VScrollBar);
            panel1.Controls.Add(SkiaControl);
            panel1.Name = "panel1";
            // 
            // HScrollBar
            // 
            resources.ApplyResources(HScrollBar, "HScrollBar");
            HScrollBar.Name = "HScrollBar";
            HScrollBar.Scroll += HScrollBar_Scroll;
            // 
            // VScrollBar
            // 
            resources.ApplyResources(VScrollBar, "VScrollBar");
            VScrollBar.Name = "VScrollBar";
            VScrollBar.Scroll += VScrollBar_Scroll;
            // 
            // SkiaControl
            // 
            resources.ApplyResources(SkiaControl, "SkiaControl");
            SkiaControl.BackColor = Color.Black;
            SkiaControl.Name = "SkiaControl";
            SkiaControl.VSync = true;
            SkiaControl.PaintSurface += SkiaControl_PaintSurface;
            SkiaControl.KeyDown += SkiaControl_KeyDown;
            SkiaControl.KeyUp += SkiaControl_KeyUp;
            SkiaControl.MouseDown += SkiaControl_MouseDown;
            SkiaControl.MouseMove += SkiaControl_MouseMove;
            SkiaControl.MouseUp += SkiaControl_MouseUp;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AllowDrop = true;
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(panel1);
            Controls.Add(MainStatusStrip);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            SizeChanged += MainForm_SizeChanged;
            DragDrop += MainForm_DragDrop;
            DragEnter += MainForm_DragEnter;
            MainStatusStrip.ResumeLayout(false);
            MainStatusStrip.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip MainStatusStrip;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem FileMenuItem;
        private Panel panel1;
        private ToolStripMenuItem OpenFileMenuItem;
        private SkiaSharp.Views.Desktop.SKGLControl SkiaControl;
        private HScrollBar HScrollBar;
        private VScrollBar VScrollBar;
        private ToolStripProgressBar StatusBarProgressBar;
        private ToolStripStatusLabel StatusBarMessageLabel;
        private ToolStripStatusLabel StatusBarDisplayInfoLabel;
        private ToolStripMenuItem ViewMenuItem;
        private ToolStripMenuItem OptionMenuItem;
        private ToolStripMenuItem SaveImageToolStripMenuItem;
        private ToolStripMenuItem CloseMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem HelpMenuItem;
        private ToolStripMenuItem VersionInfoMenuItem;
        private ToolStripMenuItem GitHubMenuItem;
    }
}