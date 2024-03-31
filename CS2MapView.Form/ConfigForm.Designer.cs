namespace CS2MapView.Form
{
    partial class ConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            ConfigTab = new TabControl();
            LayerConfigTabPage = new TabPage();
            HideCargoLinesCheckbox = new CheckBox();
            groupBox6 = new GroupBox();
            RenderBusStopsCheckbox = new CheckBox();
            RenderAirplaneStopsCheckbox = new CheckBox();
            RenderShipStopsCheckbox = new CheckBox();
            RenderMonorailStopsCheckbox = new CheckBox();
            RenderMetroStopsCheckbox = new CheckBox();
            RenderTramStopsCheckbox = new CheckBox();
            RenderTrainStopsCheckbox = new CheckBox();
            groupBox5 = new GroupBox();
            RenderBusLinesCheckbox = new CheckBox();
            RenderAirplaneLinesCheckbox = new CheckBox();
            RenderShipLinesCheckbox = new CheckBox();
            RenderMonorailLinesCheckbox = new CheckBox();
            RenderMetroLinesCheckbox = new CheckBox();
            RenderTramLinesCheckbox = new CheckBox();
            RenderTrainLinesCheckbox = new CheckBox();
            groupBox4 = new GroupBox();
            UserPrefabNameCheckbox = new CheckBox();
            RenderDistrictNameLabelCheckbox = new CheckBox();
            RenderMapSymbolCheckbox = new CheckBox();
            RenderStreetNameLabelCheckbox = new CheckBox();
            RenderBuildingNameLabelCheckbox = new CheckBox();
            groupBox3 = new GroupBox();
            RenderMetroCheckbox = new CheckBox();
            RenderTramCheckbox = new CheckBox();
            RenderCableCarCheckbox = new CheckBox();
            RenderMonorailCheckbox = new CheckBox();
            RenderRailwaysCheckbox = new CheckBox();
            groupBox2 = new GroupBox();
            DisableBuildingBorderCheckbox = new CheckBox();
            HideRICOCheckbox = new CheckBox();
            LayerConfigGridView = new Gfw.Ui.WindowsForms.SortableBindingGridView();
            GeneralConfigTabPage = new TabPage();
            groupBox1 = new GroupBox();
            VectorizeLandCheckbox = new CheckBox();
            WaterMaxResolutionTextbox = new TextBox();
            ContourLinesIntervalsLabel = new Label();
            TerrainMaxResolutionTextbox = new TextBox();
            ContourIntervalCombobox = new ComboBox();
            WaterMaxResolutionLabel = new Label();
            VectorizeWaterCheckbox = new CheckBox();
            TerrainMaxResolutionLabel = new Label();
            DefaultMapSymbolStyleCombobox = new ComboBox();
            StringThemeCombobox = new ComboBox();
            ThemeCombobox = new ComboBox();
            DefaultMapSymbolStyleLabel = new Label();
            TextStyleLabel = new Label();
            ThemeLabel = new Label();
            SubmitButton = new Button();
            FormCancelButton = new Button();
            panel1 = new Panel();
            ConfigTab.SuspendLayout();
            LayerConfigTabPage.SuspendLayout();
            groupBox6.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LayerConfigGridView).BeginInit();
            GeneralConfigTabPage.SuspendLayout();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // ConfigTab
            // 
            resources.ApplyResources(ConfigTab, "ConfigTab");
            ConfigTab.Controls.Add(LayerConfigTabPage);
            ConfigTab.Controls.Add(GeneralConfigTabPage);
            ConfigTab.Name = "ConfigTab";
            ConfigTab.SelectedIndex = 0;
            // 
            // LayerConfigTabPage
            // 
            resources.ApplyResources(LayerConfigTabPage, "LayerConfigTabPage");
            LayerConfigTabPage.Controls.Add(HideCargoLinesCheckbox);
            LayerConfigTabPage.Controls.Add(groupBox6);
            LayerConfigTabPage.Controls.Add(groupBox5);
            LayerConfigTabPage.Controls.Add(groupBox4);
            LayerConfigTabPage.Controls.Add(groupBox3);
            LayerConfigTabPage.Controls.Add(groupBox2);
            LayerConfigTabPage.Controls.Add(LayerConfigGridView);
            LayerConfigTabPage.Name = "LayerConfigTabPage";
            LayerConfigTabPage.UseVisualStyleBackColor = true;
            LayerConfigTabPage.Click += LayerConfigTabPage_Click;
            // 
            // HideCargoLinesCheckbox
            // 
            resources.ApplyResources(HideCargoLinesCheckbox, "HideCargoLinesCheckbox");
            HideCargoLinesCheckbox.Name = "HideCargoLinesCheckbox";
            HideCargoLinesCheckbox.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            resources.ApplyResources(groupBox6, "groupBox6");
            groupBox6.Controls.Add(RenderBusStopsCheckbox);
            groupBox6.Controls.Add(RenderAirplaneStopsCheckbox);
            groupBox6.Controls.Add(RenderShipStopsCheckbox);
            groupBox6.Controls.Add(RenderMonorailStopsCheckbox);
            groupBox6.Controls.Add(RenderMetroStopsCheckbox);
            groupBox6.Controls.Add(RenderTramStopsCheckbox);
            groupBox6.Controls.Add(RenderTrainStopsCheckbox);
            groupBox6.Name = "groupBox6";
            groupBox6.TabStop = false;
            // 
            // RenderBusStopsCheckbox
            // 
            resources.ApplyResources(RenderBusStopsCheckbox, "RenderBusStopsCheckbox");
            RenderBusStopsCheckbox.Name = "RenderBusStopsCheckbox";
            RenderBusStopsCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderAirplaneStopsCheckbox
            // 
            resources.ApplyResources(RenderAirplaneStopsCheckbox, "RenderAirplaneStopsCheckbox");
            RenderAirplaneStopsCheckbox.Name = "RenderAirplaneStopsCheckbox";
            RenderAirplaneStopsCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderShipStopsCheckbox
            // 
            resources.ApplyResources(RenderShipStopsCheckbox, "RenderShipStopsCheckbox");
            RenderShipStopsCheckbox.Name = "RenderShipStopsCheckbox";
            RenderShipStopsCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderMonorailStopsCheckbox
            // 
            resources.ApplyResources(RenderMonorailStopsCheckbox, "RenderMonorailStopsCheckbox");
            RenderMonorailStopsCheckbox.Name = "RenderMonorailStopsCheckbox";
            RenderMonorailStopsCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderMetroStopsCheckbox
            // 
            resources.ApplyResources(RenderMetroStopsCheckbox, "RenderMetroStopsCheckbox");
            RenderMetroStopsCheckbox.Name = "RenderMetroStopsCheckbox";
            RenderMetroStopsCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderTramStopsCheckbox
            // 
            resources.ApplyResources(RenderTramStopsCheckbox, "RenderTramStopsCheckbox");
            RenderTramStopsCheckbox.Name = "RenderTramStopsCheckbox";
            RenderTramStopsCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderTrainStopsCheckbox
            // 
            resources.ApplyResources(RenderTrainStopsCheckbox, "RenderTrainStopsCheckbox");
            RenderTrainStopsCheckbox.Name = "RenderTrainStopsCheckbox";
            RenderTrainStopsCheckbox.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            resources.ApplyResources(groupBox5, "groupBox5");
            groupBox5.Controls.Add(RenderBusLinesCheckbox);
            groupBox5.Controls.Add(RenderAirplaneLinesCheckbox);
            groupBox5.Controls.Add(RenderShipLinesCheckbox);
            groupBox5.Controls.Add(RenderMonorailLinesCheckbox);
            groupBox5.Controls.Add(RenderMetroLinesCheckbox);
            groupBox5.Controls.Add(RenderTramLinesCheckbox);
            groupBox5.Controls.Add(RenderTrainLinesCheckbox);
            groupBox5.Name = "groupBox5";
            groupBox5.TabStop = false;
            // 
            // RenderBusLinesCheckbox
            // 
            resources.ApplyResources(RenderBusLinesCheckbox, "RenderBusLinesCheckbox");
            RenderBusLinesCheckbox.Name = "RenderBusLinesCheckbox";
            RenderBusLinesCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderAirplaneLinesCheckbox
            // 
            resources.ApplyResources(RenderAirplaneLinesCheckbox, "RenderAirplaneLinesCheckbox");
            RenderAirplaneLinesCheckbox.Name = "RenderAirplaneLinesCheckbox";
            RenderAirplaneLinesCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderShipLinesCheckbox
            // 
            resources.ApplyResources(RenderShipLinesCheckbox, "RenderShipLinesCheckbox");
            RenderShipLinesCheckbox.Name = "RenderShipLinesCheckbox";
            RenderShipLinesCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderMonorailLinesCheckbox
            // 
            resources.ApplyResources(RenderMonorailLinesCheckbox, "RenderMonorailLinesCheckbox");
            RenderMonorailLinesCheckbox.Name = "RenderMonorailLinesCheckbox";
            RenderMonorailLinesCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderMetroLinesCheckbox
            // 
            resources.ApplyResources(RenderMetroLinesCheckbox, "RenderMetroLinesCheckbox");
            RenderMetroLinesCheckbox.Name = "RenderMetroLinesCheckbox";
            RenderMetroLinesCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderTramLinesCheckbox
            // 
            resources.ApplyResources(RenderTramLinesCheckbox, "RenderTramLinesCheckbox");
            RenderTramLinesCheckbox.Name = "RenderTramLinesCheckbox";
            RenderTramLinesCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderTrainLinesCheckbox
            // 
            resources.ApplyResources(RenderTrainLinesCheckbox, "RenderTrainLinesCheckbox");
            RenderTrainLinesCheckbox.Name = "RenderTrainLinesCheckbox";
            RenderTrainLinesCheckbox.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            resources.ApplyResources(groupBox4, "groupBox4");
            groupBox4.Controls.Add(UserPrefabNameCheckbox);
            groupBox4.Controls.Add(RenderDistrictNameLabelCheckbox);
            groupBox4.Controls.Add(RenderMapSymbolCheckbox);
            groupBox4.Controls.Add(RenderStreetNameLabelCheckbox);
            groupBox4.Controls.Add(RenderBuildingNameLabelCheckbox);
            groupBox4.Name = "groupBox4";
            groupBox4.TabStop = false;
            // 
            // UserPrefabNameCheckbox
            // 
            resources.ApplyResources(UserPrefabNameCheckbox, "UserPrefabNameCheckbox");
            UserPrefabNameCheckbox.Name = "UserPrefabNameCheckbox";
            UserPrefabNameCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderDistrictNameLabelCheckbox
            // 
            resources.ApplyResources(RenderDistrictNameLabelCheckbox, "RenderDistrictNameLabelCheckbox");
            RenderDistrictNameLabelCheckbox.Name = "RenderDistrictNameLabelCheckbox";
            RenderDistrictNameLabelCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderMapSymbolCheckbox
            // 
            resources.ApplyResources(RenderMapSymbolCheckbox, "RenderMapSymbolCheckbox");
            RenderMapSymbolCheckbox.Name = "RenderMapSymbolCheckbox";
            RenderMapSymbolCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderStreetNameLabelCheckbox
            // 
            resources.ApplyResources(RenderStreetNameLabelCheckbox, "RenderStreetNameLabelCheckbox");
            RenderStreetNameLabelCheckbox.Name = "RenderStreetNameLabelCheckbox";
            RenderStreetNameLabelCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderBuildingNameLabelCheckbox
            // 
            resources.ApplyResources(RenderBuildingNameLabelCheckbox, "RenderBuildingNameLabelCheckbox");
            RenderBuildingNameLabelCheckbox.Name = "RenderBuildingNameLabelCheckbox";
            RenderBuildingNameLabelCheckbox.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            resources.ApplyResources(groupBox3, "groupBox3");
            groupBox3.Controls.Add(RenderMetroCheckbox);
            groupBox3.Controls.Add(RenderTramCheckbox);
            groupBox3.Controls.Add(RenderCableCarCheckbox);
            groupBox3.Controls.Add(RenderMonorailCheckbox);
            groupBox3.Controls.Add(RenderRailwaysCheckbox);
            groupBox3.Name = "groupBox3";
            groupBox3.TabStop = false;
            // 
            // RenderMetroCheckbox
            // 
            resources.ApplyResources(RenderMetroCheckbox, "RenderMetroCheckbox");
            RenderMetroCheckbox.Name = "RenderMetroCheckbox";
            RenderMetroCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderTramCheckbox
            // 
            resources.ApplyResources(RenderTramCheckbox, "RenderTramCheckbox");
            RenderTramCheckbox.Name = "RenderTramCheckbox";
            RenderTramCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderCableCarCheckbox
            // 
            resources.ApplyResources(RenderCableCarCheckbox, "RenderCableCarCheckbox");
            RenderCableCarCheckbox.Name = "RenderCableCarCheckbox";
            RenderCableCarCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderMonorailCheckbox
            // 
            resources.ApplyResources(RenderMonorailCheckbox, "RenderMonorailCheckbox");
            RenderMonorailCheckbox.Name = "RenderMonorailCheckbox";
            RenderMonorailCheckbox.UseVisualStyleBackColor = true;
            // 
            // RenderRailwaysCheckbox
            // 
            resources.ApplyResources(RenderRailwaysCheckbox, "RenderRailwaysCheckbox");
            RenderRailwaysCheckbox.Name = "RenderRailwaysCheckbox";
            RenderRailwaysCheckbox.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Controls.Add(DisableBuildingBorderCheckbox);
            groupBox2.Controls.Add(HideRICOCheckbox);
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // DisableBuildingBorderCheckbox
            // 
            resources.ApplyResources(DisableBuildingBorderCheckbox, "DisableBuildingBorderCheckbox");
            DisableBuildingBorderCheckbox.Name = "DisableBuildingBorderCheckbox";
            DisableBuildingBorderCheckbox.UseVisualStyleBackColor = true;
            // 
            // HideRICOCheckbox
            // 
            resources.ApplyResources(HideRICOCheckbox, "HideRICOCheckbox");
            HideRICOCheckbox.Name = "HideRICOCheckbox";
            HideRICOCheckbox.UseVisualStyleBackColor = true;
            // 
            // LayerConfigGridView
            // 
            resources.ApplyResources(LayerConfigGridView, "LayerConfigGridView");
            LayerConfigGridView.AllowUserToAddRows = false;
            LayerConfigGridView.AllowUserToDeleteRows = false;
            LayerConfigGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            LayerConfigGridView.CurrentSortColumn = null;
            LayerConfigGridView.Name = "LayerConfigGridView";
            LayerConfigGridView.RowHeadersVisible = false;
            LayerConfigGridView.RowNumberVisible = true;
            // 
            // GeneralConfigTabPage
            // 
            resources.ApplyResources(GeneralConfigTabPage, "GeneralConfigTabPage");
            GeneralConfigTabPage.Controls.Add(groupBox1);
            GeneralConfigTabPage.Controls.Add(DefaultMapSymbolStyleCombobox);
            GeneralConfigTabPage.Controls.Add(StringThemeCombobox);
            GeneralConfigTabPage.Controls.Add(ThemeCombobox);
            GeneralConfigTabPage.Controls.Add(DefaultMapSymbolStyleLabel);
            GeneralConfigTabPage.Controls.Add(TextStyleLabel);
            GeneralConfigTabPage.Controls.Add(ThemeLabel);
            GeneralConfigTabPage.Name = "GeneralConfigTabPage";
            GeneralConfigTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(VectorizeLandCheckbox);
            groupBox1.Controls.Add(WaterMaxResolutionTextbox);
            groupBox1.Controls.Add(ContourLinesIntervalsLabel);
            groupBox1.Controls.Add(TerrainMaxResolutionTextbox);
            groupBox1.Controls.Add(ContourIntervalCombobox);
            groupBox1.Controls.Add(WaterMaxResolutionLabel);
            groupBox1.Controls.Add(VectorizeWaterCheckbox);
            groupBox1.Controls.Add(TerrainMaxResolutionLabel);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // VectorizeLandCheckbox
            // 
            resources.ApplyResources(VectorizeLandCheckbox, "VectorizeLandCheckbox");
            VectorizeLandCheckbox.Name = "VectorizeLandCheckbox";
            VectorizeLandCheckbox.UseVisualStyleBackColor = true;
            // 
            // WaterMaxResolutionTextbox
            // 
            resources.ApplyResources(WaterMaxResolutionTextbox, "WaterMaxResolutionTextbox");
            WaterMaxResolutionTextbox.Name = "WaterMaxResolutionTextbox";
            WaterMaxResolutionTextbox.Leave += MaxResolution_TextChanged;
            // 
            // ContourLinesIntervalsLabel
            // 
            resources.ApplyResources(ContourLinesIntervalsLabel, "ContourLinesIntervalsLabel");
            ContourLinesIntervalsLabel.Name = "ContourLinesIntervalsLabel";
            // 
            // TerrainMaxResolutionTextbox
            // 
            resources.ApplyResources(TerrainMaxResolutionTextbox, "TerrainMaxResolutionTextbox");
            TerrainMaxResolutionTextbox.Name = "TerrainMaxResolutionTextbox";
            TerrainMaxResolutionTextbox.Leave += MaxResolution_TextChanged;
            // 
            // ContourIntervalCombobox
            // 
            resources.ApplyResources(ContourIntervalCombobox, "ContourIntervalCombobox");
            ContourIntervalCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            ContourIntervalCombobox.FormattingEnabled = true;
            ContourIntervalCombobox.Name = "ContourIntervalCombobox";
            // 
            // WaterMaxResolutionLabel
            // 
            resources.ApplyResources(WaterMaxResolutionLabel, "WaterMaxResolutionLabel");
            WaterMaxResolutionLabel.Name = "WaterMaxResolutionLabel";
            // 
            // VectorizeWaterCheckbox
            // 
            resources.ApplyResources(VectorizeWaterCheckbox, "VectorizeWaterCheckbox");
            VectorizeWaterCheckbox.Name = "VectorizeWaterCheckbox";
            VectorizeWaterCheckbox.UseVisualStyleBackColor = true;
            // 
            // TerrainMaxResolutionLabel
            // 
            resources.ApplyResources(TerrainMaxResolutionLabel, "TerrainMaxResolutionLabel");
            TerrainMaxResolutionLabel.Name = "TerrainMaxResolutionLabel";
            // 
            // DefaultMapSymbolStyleCombobox
            // 
            resources.ApplyResources(DefaultMapSymbolStyleCombobox, "DefaultMapSymbolStyleCombobox");
            DefaultMapSymbolStyleCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            DefaultMapSymbolStyleCombobox.FormattingEnabled = true;
            DefaultMapSymbolStyleCombobox.Name = "DefaultMapSymbolStyleCombobox";
            // 
            // StringThemeCombobox
            // 
            resources.ApplyResources(StringThemeCombobox, "StringThemeCombobox");
            StringThemeCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            StringThemeCombobox.FormattingEnabled = true;
            StringThemeCombobox.Name = "StringThemeCombobox";
            // 
            // ThemeCombobox
            // 
            resources.ApplyResources(ThemeCombobox, "ThemeCombobox");
            ThemeCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            ThemeCombobox.FormattingEnabled = true;
            ThemeCombobox.Name = "ThemeCombobox";
            // 
            // DefaultMapSymbolStyleLabel
            // 
            resources.ApplyResources(DefaultMapSymbolStyleLabel, "DefaultMapSymbolStyleLabel");
            DefaultMapSymbolStyleLabel.Name = "DefaultMapSymbolStyleLabel";
            // 
            // TextStyleLabel
            // 
            resources.ApplyResources(TextStyleLabel, "TextStyleLabel");
            TextStyleLabel.Name = "TextStyleLabel";
            // 
            // ThemeLabel
            // 
            resources.ApplyResources(ThemeLabel, "ThemeLabel");
            ThemeLabel.Name = "ThemeLabel";
            // 
            // SubmitButton
            // 
            resources.ApplyResources(SubmitButton, "SubmitButton");
            SubmitButton.Name = "SubmitButton";
            SubmitButton.UseVisualStyleBackColor = true;
            SubmitButton.Click += SubmitButton_Click;
            // 
            // FormCancelButton
            // 
            resources.ApplyResources(FormCancelButton, "FormCancelButton");
            FormCancelButton.Name = "FormCancelButton";
            FormCancelButton.UseVisualStyleBackColor = true;
            FormCancelButton.Click += FormCancelButton_Click;
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Controls.Add(FormCancelButton);
            panel1.Controls.Add(SubmitButton);
            panel1.Name = "panel1";
            // 
            // ConfigForm
            // 
            AcceptButton = SubmitButton;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = FormCancelButton;
            Controls.Add(panel1);
            Controls.Add(ConfigTab);
            Name = "ConfigForm";
            Load += ConfigForm_Load;
            ConfigTab.ResumeLayout(false);
            LayerConfigTabPage.ResumeLayout(false);
            LayerConfigTabPage.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)LayerConfigGridView).EndInit();
            GeneralConfigTabPage.ResumeLayout(false);
            GeneralConfigTabPage.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl ConfigTab;
        private TabPage GeneralConfigTabPage;
        private TabPage LayerConfigTabPage;
        private Button SubmitButton;
        private Button FormCancelButton;
        private Panel panel1;
        private Label ContourLinesIntervalsLabel;
        private Label ThemeLabel;
        private ComboBox ContourIntervalCombobox;
        private ComboBox ThemeCombobox;
        private Label TerrainMaxResolutionLabel;
        private CheckBox VectorizeLandCheckbox;
        private TextBox WaterMaxResolutionTextbox;
        private TextBox TerrainMaxResolutionTextbox;
        private Label WaterMaxResolutionLabel;
        private CheckBox VectorizeWaterCheckbox;
        private ComboBox StringThemeCombobox;
        private Label TextStyleLabel;
        private Gfw.Ui.WindowsForms.SortableBindingGridView LayerConfigGridView;
        private GroupBox groupBox1;
        private CheckBox HideRICOCheckbox;
        private CheckBox DisableBuildingBorderCheckbox;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private CheckBox RenderMetroCheckbox;
        private CheckBox RenderTramCheckbox;
        private CheckBox RenderCableCarCheckbox;
        private CheckBox RenderMonorailCheckbox;
        private CheckBox RenderRailwaysCheckbox;
        private GroupBox groupBox4;
        private CheckBox RenderBuildingNameLabelCheckbox;
        private ComboBox DefaultMapSymbolStyleCombobox;
        private Label DefaultMapSymbolStyleLabel;
        private CheckBox RenderDistrictNameLabelCheckbox;
        private CheckBox RenderMapSymbolCheckbox;
        private CheckBox RenderStreetNameLabelCheckbox;
        private GroupBox groupBox5;
        private CheckBox RenderTramLinesCheckbox;
        private CheckBox RenderTrainLinesCheckbox;
        private CheckBox RenderMetroLinesCheckbox;
        private CheckBox RenderBusLinesCheckbox;
        private CheckBox RenderMonorailLinesCheckbox;
        private CheckBox RenderAirplaneLinesCheckbox;
        private CheckBox RenderShipLinesCheckbox;
        private CheckBox HideCargoLinesCheckbox;
        private GroupBox groupBox6;
        private CheckBox RenderBusStopsCheckbox;
        private CheckBox RenderAirplaneStopsCheckbox;
        private CheckBox RenderShipStopsCheckbox;
        private CheckBox RenderMonorailStopsCheckbox;
        private CheckBox RenderMetroStopsCheckbox;
        private CheckBox RenderTramStopsCheckbox;
        private CheckBox RenderTrainStopsCheckbox;
        private CheckBox UserPrefabNameCheckbox;
    }
}