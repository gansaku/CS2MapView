namespace CS2MapView.Form
{
    partial class SaveImageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveImageForm));
            SubSkiaControl = new SkiaSharp.Views.Desktop.SKGLControl();
            PrintAreaLeftTextbox = new TextBox();
            PrintAreaTopTextbox = new TextBox();
            PrintAreaRightTextbox = new TextBox();
            PrintAreaBottomTextbox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label6 = new Label();
            OutputImageSizeTextbox = new TextBox();
            OutputImageSizeLabel = new Label();
            label5 = new Label();
            OutputFileFormatCombobox = new ComboBox();
            label7 = new Label();
            OutputPathTextbox = new TextBox();
            SaveButton = new Button();
            OpenFileDialogButton = new Button();
            OutputFileNameTextbox = new TextBox();
            label8 = new Label();
            SuspendLayout();
            // 
            // SubSkiaControl
            // 
            SubSkiaControl.BackColor = Color.Black;
            SubSkiaControl.Location = new Point(0, 0);
            SubSkiaControl.Margin = new Padding(0);
            SubSkiaControl.Name = "SubSkiaControl";
            SubSkiaControl.Size = new Size(540, 540);
            SubSkiaControl.TabIndex = 1;
            SubSkiaControl.VSync = true;
            SubSkiaControl.PaintSurface += SubSkiaControl_PaintSurface;
            SubSkiaControl.MouseDown += SubSkiaControl_MouseDown;
            SubSkiaControl.MouseLeave += SubSkiaControl_MouseLeave;
            SubSkiaControl.MouseMove += SubSkiaControl_MouseMove;
            SubSkiaControl.MouseUp += SubSkiaControl_MouseUp;
            // 
            // PrintAreaLeftTextbox
            // 
            PrintAreaLeftTextbox.Location = new Point(549, 31);
            PrintAreaLeftTextbox.Margin = new Padding(3, 2, 3, 2);
            PrintAreaLeftTextbox.Name = "PrintAreaLeftTextbox";
            PrintAreaLeftTextbox.Size = new Size(37, 23);
            PrintAreaLeftTextbox.TabIndex = 0;
            PrintAreaLeftTextbox.Leave += PrintAreaTextboxs_Leave;
            // 
            // PrintAreaTopTextbox
            // 
            PrintAreaTopTextbox.Location = new Point(599, 31);
            PrintAreaTopTextbox.Margin = new Padding(3, 2, 3, 2);
            PrintAreaTopTextbox.Name = "PrintAreaTopTextbox";
            PrintAreaTopTextbox.Size = new Size(37, 23);
            PrintAreaTopTextbox.TabIndex = 1;
            PrintAreaTopTextbox.Leave += PrintAreaTextboxs_Leave;
            // 
            // PrintAreaRightTextbox
            // 
            PrintAreaRightTextbox.Location = new Point(663, 31);
            PrintAreaRightTextbox.Margin = new Padding(3, 2, 3, 2);
            PrintAreaRightTextbox.Name = "PrintAreaRightTextbox";
            PrintAreaRightTextbox.Size = new Size(37, 23);
            PrintAreaRightTextbox.TabIndex = 2;
            PrintAreaRightTextbox.Leave += PrintAreaTextboxs_Leave;
            // 
            // PrintAreaBottomTextbox
            // 
            PrintAreaBottomTextbox.Location = new Point(714, 31);
            PrintAreaBottomTextbox.Margin = new Padding(3, 2, 3, 2);
            PrintAreaBottomTextbox.Name = "PrintAreaBottomTextbox";
            PrintAreaBottomTextbox.Size = new Size(37, 23);
            PrintAreaBottomTextbox.TabIndex = 3;
            PrintAreaBottomTextbox.Leave += PrintAreaTextboxs_Leave;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(548, 7);
            label1.Name = "label1";
            label1.Size = new Size(70, 15);
            label1.TabIndex = 4;
            label1.Text = "Output area";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(588, 36);
            label2.Name = "label2";
            label2.Size = new Size(10, 15);
            label2.TabIndex = 5;
            label2.Text = ",";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(703, 36);
            label3.Name = "label3";
            label3.Size = new Size(10, 15);
            label3.TabIndex = 5;
            label3.Text = ",";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(640, 37);
            label4.Name = "label4";
            label4.Size = new Size(19, 15);
            label4.TabIndex = 6;
            label4.Text = "～";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(548, 68);
            label6.Name = "label6";
            label6.Size = new Size(102, 15);
            label6.TabIndex = 4;
            label6.Text = "Output image size";
            // 
            // OutputImageSizeTextbox
            // 
            OutputImageSizeTextbox.Location = new Point(548, 89);
            OutputImageSizeTextbox.Margin = new Padding(3, 2, 3, 2);
            OutputImageSizeTextbox.Name = "OutputImageSizeTextbox";
            OutputImageSizeTextbox.Size = new Size(56, 23);
            OutputImageSizeTextbox.TabIndex = 4;
            OutputImageSizeTextbox.Leave += OutputImageSizeTextbox_Leave;
            // 
            // OutputImageSizeLabel
            // 
            OutputImageSizeLabel.AutoSize = true;
            OutputImageSizeLabel.Location = new Point(637, 91);
            OutputImageSizeLabel.Name = "OutputImageSizeLabel";
            OutputImageSizeLabel.Size = new Size(61, 15);
            OutputImageSizeLabel.TabIndex = 7;
            OutputImageSizeLabel.Text = "1024x1024";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(549, 122);
            label5.Name = "label5";
            label5.Size = new Size(63, 15);
            label5.TabIndex = 8;
            label5.Text = "File format";
            // 
            // OutputFileFormatCombobox
            // 
            OutputFileFormatCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            OutputFileFormatCombobox.FormattingEnabled = true;
            OutputFileFormatCombobox.Location = new Point(549, 142);
            OutputFileFormatCombobox.Margin = new Padding(3, 2, 3, 2);
            OutputFileFormatCombobox.Name = "OutputFileFormatCombobox";
            OutputFileFormatCombobox.Size = new Size(111, 23);
            OutputFileFormatCombobox.TabIndex = 5;
            OutputFileFormatCombobox.SelectedIndexChanged += OutputFileFormatCombobox_SelectedIndexChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(6, 545);
            label7.Name = "label7";
            label7.Size = new Size(95, 15);
            label7.TabIndex = 8;
            label7.Text = "Output directory";
            // 
            // OutputPathTextbox
            // 
            OutputPathTextbox.Location = new Point(6, 565);
            OutputPathTextbox.Margin = new Padding(3, 2, 3, 2);
            OutputPathTextbox.Name = "OutputPathTextbox";
            OutputPathTextbox.Size = new Size(739, 23);
            OutputPathTextbox.TabIndex = 6;
            // 
            // SaveButton
            // 
            SaveButton.Location = new Point(663, 611);
            SaveButton.Margin = new Padding(3, 2, 3, 2);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(82, 22);
            SaveButton.TabIndex = 9;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // OpenFileDialogButton
            // 
            OpenFileDialogButton.Location = new Point(236, 614);
            OpenFileDialogButton.Margin = new Padding(3, 2, 3, 2);
            OpenFileDialogButton.Name = "OpenFileDialogButton";
            OpenFileDialogButton.Size = new Size(37, 22);
            OpenFileDialogButton.TabIndex = 8;
            OpenFileDialogButton.Text = "...";
            OpenFileDialogButton.UseVisualStyleBackColor = true;
            OpenFileDialogButton.Click += OpenFileDialogButton_Click;
            // 
            // OutputFileNameTextbox
            // 
            OutputFileNameTextbox.Location = new Point(6, 612);
            OutputFileNameTextbox.Name = "OutputFileNameTextbox";
            OutputFileNameTextbox.Size = new Size(224, 23);
            OutputFileNameTextbox.TabIndex = 7;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(6, 593);
            label8.Name = "label8";
            label8.Size = new Size(57, 15);
            label8.TabIndex = 8;
            label8.Text = "File name";
            // 
            // SaveImageForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(762, 647);
            Controls.Add(OutputFileNameTextbox);
            Controls.Add(OpenFileDialogButton);
            Controls.Add(SaveButton);
            Controls.Add(OutputPathTextbox);
            Controls.Add(OutputFileFormatCombobox);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label5);
            Controls.Add(OutputImageSizeLabel);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label6);
            Controls.Add(label1);
            Controls.Add(PrintAreaTopTextbox);
            Controls.Add(PrintAreaBottomTextbox);
            Controls.Add(PrintAreaRightTextbox);
            Controls.Add(OutputImageSizeTextbox);
            Controls.Add(PrintAreaLeftTextbox);
            Controls.Add(SubSkiaControl);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SaveImageForm";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "CS2MapView - Save image";
            FormClosing += SaveImageForm_FormClosing;
            FormClosed += SaveImageForm_FormClosed;
            Load += SaveImageForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SkiaSharp.Views.Desktop.SKGLControl SubSkiaControl;

        private TextBox PrintAreaLeftTextbox;
        private TextBox PrintAreaTopTextbox;
        private TextBox OutputImageSizeTextbox;
        private TextBox PrintAreaRightTextbox;
       
        private TextBox PrintAreaBottomTextbox;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label6;
        private Label OutputImageSizeLabel;
        private Label label5;
        private ComboBox OutputFileFormatCombobox;
        private Label label7;
        private TextBox OutputPathTextbox;
        private Button SaveButton;
        private Button OpenFileDialogButton;
        private TextBox OutputFileNameTextbox;
        private Label label8;
    }
}