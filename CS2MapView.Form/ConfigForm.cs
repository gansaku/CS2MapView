using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CS2MapView.Data;
using CS2MapView.Drawing.Terrain;
using System.Globalization;
using log4net;
using Gfw.Common;
using CS2MapView.Drawing.Layer;
using Gfw.Ui.WindowsForms;
using CS2MapView.Config;
using System.DirectoryServices.ActiveDirectory;
using CS2MapView.Drawing.Labels;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace CS2MapView.Form
{
    /// <summary>
    /// 設定フォーム
    /// </summary>
    public partial class ConfigForm : System.Windows.Forms.Form
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConfigForm));
        /// <summary>
        /// クライアントアプリケーション
        /// </summary>
        public required MainForm AppRoot { get; init; }
        private RenderContext Context => AppRoot.Context;


        private IList<LayerSettings> EdittingLayerOrder;
#nullable disable
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfigForm()
        {
            InitializeComponent();
        }
#nullable restore

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            var us = Context.UserSettings;
            ThemeCombobox.Items.Clear();
            ThemeCombobox.Items.AddRange(Context.ThemeCandidates.Select(x => x.Name!).ToArray<object>());
            ThemeCombobox.Text = Context.Theme.Name;

            StringThemeCombobox.Items.Clear();
            StringThemeCombobox.Items.AddRange(Context.StringThemeCandidates.Select(x => x.Name!).ToArray<object>());
            StringThemeCombobox.Text = Context.StringTheme.Name;

            ContourIntervalCombobox.Items.Clear();
            ContourIntervalCombobox.Items.AddRange(Context.ContourHeightsCandidate.Select(c => c.Name!).ToArray<object>());
            ContourIntervalCombobox.Text = Context.ContourHeights.Name;

            VectorizeLandCheckbox.Checked = us.VectorizeTerrainLand;
            VectorizeWaterCheckbox.Checked = us.VectorizeTerrainWater;
            TerrainMaxResolutionTextbox.Text = us.TerrainMaxResolution.ToString(CultureInfo.InvariantCulture);
            WaterMaxResolutionTextbox.Text = us.WaterMaxResolution.ToString(CultureInfo.InvariantCulture);

            DefaultMapSymbolStyleCombobox.Items.Add("ja+");
            DefaultMapSymbolStyleCombobox.Items.Add("ja");
            DefaultMapSymbolStyleCombobox.Items.Add("maki");

            DefaultMapSymbolStyleCombobox.Text = us.DefaultMapSymbolType;

            //レイヤー
            EdittingLayerOrder = new PropertyUtil<LayerSettings>().ListPropertyClone<LayerSettings>(Context.UserSettings.LayerDrawingOrder?.OrderBy(t => t.Order)) ?? new List<LayerSettings>();

            var colFactory = new SortableBindingGridViewPartsFactory<LayerSettings>();

            LayerConfigGridView.Columns.Add(colFactory.CreateTextboxColumn(nameof(LayerSettings.Name), "Name", 100, true, align: DataGridViewContentAlignment.MiddleLeft));
            LayerConfigGridView.Columns.Add(colFactory.CreateCheckBoxColumn(nameof(LayerSettings.Visible), "Visible", 50, false));
            LayerConfigGridView.Columns.Add(colFactory.CreateTextboxColumn(nameof(LayerSettings.Order), "Order", 50, false, format: "0", align: DataGridViewContentAlignment.MiddleRight));

            LayerConfigGridView.DataSource = colFactory.CreateBindingDataSource(EdittingLayerOrder);

            DisableBuildingBorderCheckbox.Checked = us.DisableBuildingBorder;
            HideRICOCheckbox.Checked = us.HideRICOBuildingsShape;

            RenderRailwaysCheckbox.Checked = us.RenderRailTrain;
            RenderMetroCheckbox.Checked = us.RenderRailMetro;
            RenderTramCheckbox.Checked = us.RenderRailTram;
            RenderMonorailCheckbox.Checked = us.RenderRailMonorail;
            RenderCableCarCheckbox.Checked = us.RenderRailCableCar;

            RenderBuildingNameLabelCheckbox.Checked = us.RenderBuildingNameLabel;
            RenderStreetNameLabelCheckbox.Checked = us.RenderStreetNames;
            RenderMapSymbolCheckbox.Checked = us.RenderMapSymbol;
            RenderDistrictNameLabelCheckbox.Checked = us.RenderDistrictNameLabel;
            UserPrefabNameCheckbox.Checked = us.UsePrefabBuildingName;

            RenderBusLinesCheckbox.Checked = us.TransportRouteMapConfig.RenderBusLine;
            RenderTrainLinesCheckbox.Checked = us.TransportRouteMapConfig.RenderTrainLine;
            RenderTramLinesCheckbox.Checked = us.TransportRouteMapConfig.RenderTramLine;
            RenderMetroLinesCheckbox.Checked = us.TransportRouteMapConfig.RenderMetroLine;
            RenderMonorailLinesCheckbox.Checked = us.TransportRouteMapConfig.RenderMonorailLine;
            RenderShipLinesCheckbox.Checked = us.TransportRouteMapConfig.RenderShipLine;
            RenderAirplaneLinesCheckbox.Checked = us.TransportRouteMapConfig.RenderAirplaneLine;
            RenderBusStopsCheckbox.Checked = us.TransportRouteMapConfig.RenderBusStop;
            RenderTrainStopsCheckbox.Checked = us.TransportRouteMapConfig.RenderTrainStop;
            RenderTramStopsCheckbox.Checked = us.TransportRouteMapConfig.RenderTramStop;
            RenderMetroStopsCheckbox.Checked = us.TransportRouteMapConfig.RenderMetroStop;
            RenderMonorailStopsCheckbox.Checked = us.TransportRouteMapConfig.RenderMonorailStop;
            RenderShipStopsCheckbox.Checked = us.TransportRouteMapConfig.RenderShipStop;
            RenderAirplaneStopsCheckbox.Checked = us.TransportRouteMapConfig.RenderAirplaneStop;

            HideCargoLinesCheckbox.Checked = us.TransportRouteMapConfig.HideCargoLines;

        }
        private bool ValidateContent()
        {
            //レイヤー
            if (Context.UserSettings.LayerDrawingOrder is null || EdittingLayerOrder.Count != Context.UserSettings.LayerDrawingOrder.Count)
            {
                throw new InvalidOperationException();
            }
            if (EdittingLayerOrder.Select(t => t.Order).Distinct().Count() != EdittingLayerOrder.Count)
            {
                //TODO
                MessageBox.Show("Duplicate order value found.");
                return false;
            }
            return true;
        }


        private async void SubmitButton_Click(object sender, EventArgs e)
        {
            SubmitButton.Enabled = false;

            if (!ValidateContent())
            {
                SubmitButton.Enabled = true;
                return;
            }
            List<string> layers = [
                ILayer.LayerNameBuildings,
                ILayer.LayerNameDistricts,
                ILayer.LayerNameGrid,
                ILayer.LayerNameLabels,
                ILayer.LayerNameRailways,
                ILayer.LayerNameRoads,
                ILayer.LayerNameTerrain,
                ILayer.LayerNameTransportLines
            ];

            var us = Context.UserSettings;
            bool layerOrderRequiresRedraw = false;
            bool themeChanged = false;
            Dictionary<string, bool> layerChangedFlags = layers.ToDictionary(t => t, t => false);

            //テーマ変更
            {
                string? prevStringValue = Context.Theme.Name;
                string? newStringValue = ThemeCombobox.Text;
                if (prevStringValue != newStringValue)
                {
                    var newObj = Context.ThemeCandidates.First(t => t.Name == newStringValue);
                    if (newObj is not null)
                    {
                        Context.Theme = newObj;
                        Context.UserSettings.Theme = newObj.Name;
                        themeChanged = true;
                    }
                    else
                    {
                        Logger.Error($"Theme {newStringValue} was not found.");
                    }
                }

                prevStringValue = Context.StringTheme.Name;
                newStringValue = StringThemeCombobox.Text;
                if (prevStringValue != newStringValue)
                {
                    var newObj = Context.StringThemeCandidates.First(t => t.Name == newStringValue);
                    if (newObj is not null)
                    {
                        Context.StringTheme = newObj;
                        us.StringTheme = newObj.Name;
                        themeChanged = true;
                    }
                    else
                    {
                        Logger.Error($"Theme {newStringValue} was not found.");
                    }
                }
            }
            var layerResult = SetLayerValues();
            layerOrderRequiresRedraw |= layerResult.requiresRedraw;
            void LayerFlagOr(string layerName, bool value)
            {
                layerChangedFlags[layerName] = layerChangedFlags[layerName] | value;
            }
            void HandleLayerResult(string layerName)
            {
                if (layerResult.visibilityChangedLayer.Contains(layerName))
                {
                    layerChangedFlags[layerName] = true;
                }
            }
            layers.ForEach(HandleLayerResult);

            LayerFlagOr(ILayer.LayerNameTerrain, SetTerrainValues());

            if (us.DisableBuildingBorder != DisableBuildingBorderCheckbox.Checked)
            {
                us.DisableBuildingBorder = DisableBuildingBorderCheckbox.Checked;
                LayerFlagOr(ILayer.LayerNameBuildings, true);
            }
            if (us.HideRICOBuildingsShape != HideRICOCheckbox.Checked)
            {
                us.HideRICOBuildingsShape = HideRICOCheckbox.Checked;
                LayerFlagOr(ILayer.LayerNameBuildings, true);
            }
            if (us.RenderRailTrain != RenderRailwaysCheckbox.Checked)
            {
                us.RenderRailTrain = RenderRailwaysCheckbox.Checked;
                LayerFlagOr(ILayer.LayerNameRailways, true);
            }
            if (us.RenderRailTram != RenderTramCheckbox.Checked)
            {
                us.RenderRailTram = RenderTramCheckbox.Checked;
                LayerFlagOr(ILayer.LayerNameRailways, true);
            }
            if (us.RenderRailMetro != RenderMetroCheckbox.Checked)
            {
                us.RenderRailMetro = RenderMetroCheckbox.Checked;
                LayerFlagOr(ILayer.LayerNameRailways, true);
            }
            if (us.RenderRailMonorail != RenderMonorailCheckbox.Checked)
            {
                us.RenderRailMonorail = RenderMonorailCheckbox.Checked;
                LayerFlagOr(ILayer.LayerNameRailways, true);
            }
            if (us.RenderRailCableCar != RenderCableCarCheckbox.Checked)
            {
                us.RenderRailCableCar = RenderCableCarCheckbox.Checked;
                LayerFlagOr(ILayer.LayerNameRailways, true);
            }

            if (us.RenderBuildingNameLabel != RenderBuildingNameLabelCheckbox.Checked)
            {
                us.RenderBuildingNameLabel = RenderBuildingNameLabelCheckbox.Checked;
                LayerFlagOr(ILayer.LayerNameLabels, true);
            }
            if (us.RenderDistrictNameLabel != RenderDistrictNameLabelCheckbox.Checked)
            {
                us.RenderDistrictNameLabel = RenderDistrictNameLabelCheckbox.Checked;
                LayerFlagOr(ILayer.LayerNameLabels, true);
            }
            if (us.RenderStreetNames != RenderStreetNameLabelCheckbox.Checked)
            {
                us.RenderStreetNames = RenderStreetNameLabelCheckbox.Checked;
                LayerFlagOr(ILayer.LayerNameLabels, true);
            }
            if (us.RenderMapSymbol != RenderMapSymbolCheckbox.Checked)
            {
                us.RenderMapSymbol = RenderMapSymbolCheckbox.Checked;
                LayerFlagOr(ILayer.LayerNameLabels, true);
            }
            if( us.UsePrefabBuildingName != UserPrefabNameCheckbox.Checked)
            {
                us.UsePrefabBuildingName = UserPrefabNameCheckbox.Checked;
                LayerFlagOr(ILayer.LayerNameLabels, true);
            }

            if (us.DefaultMapSymbolType != DefaultMapSymbolStyleCombobox.Text)
            {
                us.DefaultMapSymbolType = DefaultMapSymbolStyleCombobox.Text;
                LayerFlagOr(ILayer.LayerNameLabels, true);
            }

            var tsold = us.TransportRouteMapConfig;
            void setTransportCheckbox(string propertyName, bool newValue)
            {
                PropertyInfo? pi = typeof(TransportRouteMapConfig).GetProperty(propertyName);
                if(pi is null)
                {
                    Debug.Print($"property TransportRouteMapConfig.{newValue} not found.");
                    return;
                }

                if ((bool)pi.GetValue(tsold)! != newValue)
                {
                    pi.SetValue(tsold, newValue);
                    LayerFlagOr(ILayer.LayerNameTransportLines, true);
                }
            }
            setTransportCheckbox(nameof(tsold.RenderBusLine), RenderBusLinesCheckbox.Checked);
            setTransportCheckbox(nameof(tsold.RenderTrainLine), RenderTrainLinesCheckbox.Checked);
            setTransportCheckbox(nameof(tsold.RenderTramLine), RenderTramLinesCheckbox.Checked);
            setTransportCheckbox(nameof(tsold.RenderMetroLine), RenderMetroLinesCheckbox.Checked);
            setTransportCheckbox(nameof(tsold.RenderMonorailLine), RenderMonorailLinesCheckbox.Checked);
            setTransportCheckbox(nameof(tsold.RenderShipLine), RenderShipLinesCheckbox.Checked);
            setTransportCheckbox(nameof(tsold.RenderAirplaneLine), RenderAirplaneLinesCheckbox.Checked);

            setTransportCheckbox(nameof(tsold.RenderBusStop), RenderBusStopsCheckbox.Checked);
            setTransportCheckbox(nameof(tsold.RenderTrainStop), RenderTrainStopsCheckbox.Checked);
            setTransportCheckbox(nameof(tsold.RenderTramStop), RenderTramStopsCheckbox.Checked);
            setTransportCheckbox(nameof(tsold.RenderMetroStop), RenderMetroStopsCheckbox.Checked);
            setTransportCheckbox(nameof(tsold.RenderMonorailStop), RenderMonorailStopsCheckbox.Checked);
            setTransportCheckbox(nameof(tsold.RenderShipStop), RenderShipStopsCheckbox.Checked);
            setTransportCheckbox(nameof(tsold.RenderAirplaneStop), RenderAirplaneStopsCheckbox.Checked);

            setTransportCheckbox(nameof(tsold.HideCargoLines), HideCargoLinesCheckbox.Checked);

            //再描画判定
            if (AppRoot.MapData is not null && Context.SourceMapType is not null)
            {
                if (themeChanged)
                {
                    AppRoot.MapData.Dispose();
                    string? oldfn = AppRoot.MapData.FileName;
                    AppRoot.MapData = await Context.SourceMapType.BuildAll(AppRoot, null);
                    AppRoot.MapData.FileName = oldfn;
                }
                else
                {
                    List<Task<ILayer>> tasks = [];
                    void RebuildLayerIfChanged(string name, Func<Task<ILayer>> act) {
                        if (!layerChangedFlags[name])
                        {
                            return;
                        }
                        var oldLayer =AppRoot.MapData!.Layers.FirstOrDefault(l => l.LayerName == name);
                        if(oldLayer is not null)
                        {
                            oldLayer.Dispose();
                            AppRoot.MapData.Layers.Remove(oldLayer);
                        }
                        if (AppRoot.Context.UserSettings.IsLayerVisible(name))
                        {
                            tasks.Add(act());
                        }
                    }
                    RebuildLayerIfChanged(ILayer.LayerNameTerrain, () => Context.SourceMapType.BuildTerrainLayerAsync(AppRoot, null));
                    RebuildLayerIfChanged(ILayer.LayerNameBuildings, () => Context.SourceMapType.BuildBuildingsLayerAsync(AppRoot, null));
                    RebuildLayerIfChanged(ILayer.LayerNameDistricts, () => Context.SourceMapType.BuildDistrictLayerAsync(AppRoot, null));
                    RebuildLayerIfChanged(ILayer.LayerNameGrid, () => Context.SourceMapType.BuildGridLayerAsync(AppRoot, null));
                    RebuildLayerIfChanged(ILayer.LayerNameRailways, ()=>Context.SourceMapType.BuildRailwaysLayerAsync(AppRoot, null));
                    RebuildLayerIfChanged(ILayer.LayerNameRoads, () => Context.SourceMapType.BuildRoadsLayerAsync(AppRoot, null));
                    RebuildLayerIfChanged(ILayer.LayerNameTransportLines, () => Context.SourceMapType.BuildTransportLinesLayerAsync(AppRoot, null));

                    if (layerChangedFlags[ILayer.LayerNameLabels])
                    {
                        if (AppRoot.MapData?.LabelContentsManager is not null)
                        {
                            await AppRoot.MapData.LabelContentsManager.RebuildAsync(AppRoot.Context.ViewContext);
                            RebuildLayerIfChanged(ILayer.LayerNameLabels, () => Context.SourceMapType.BuildLabelLayerAsync(AppRoot, AppRoot.MapData.LabelContentsManager, null));

                        }
                    }

                    foreach(var task in tasks)
                    {
                        AppRoot.MapData!.Layers.Add( await task );
                    }

                   
                }
            }


            if (themeChanged | layerOrderRequiresRedraw | layerChangedFlags.ContainsValue(true))
            {
                await AppRoot.Context.UserSettings.SaveAsync();
            }
            DialogResult = DialogResult.OK;
            AppRoot.InvalidateSkia();
            Close();
        }

        private (bool requiresRedraw, List<string> visibilityChangedLayer) SetLayerValues()
        {
            var prevList = AppRoot.Context.UserSettings.LayerDrawingOrder!;
            var result = new List<string>();
            bool changed = false;
            foreach (var newValue in EdittingLayerOrder)
            {
                var prev = prevList.First(t => t.Name == newValue.Name);

                if (newValue.Visible != prev.Visible || newValue.Order != prev.Order)
                {
                    changed = true;
                }
                if (newValue.Visible != prev.Visible)
                {
                    result.Add(newValue.Name ?? "");
                }
            }
            if (changed)
            {
                AppRoot.Context.UserSettings.LayerDrawingOrder = [.. EdittingLayerOrder];
            }
            return (changed, result);
        }

        private bool SetTerrainValues()
        {
            bool terrainChanged = false;
            string? prevStringValue = Context.ContourHeights.Name;
            string? newStringValue = ContourIntervalCombobox.Text;
            if (prevStringValue != newStringValue)
            {
                var newObj = Context.ContourHeightsCandidate.First(t => t.Name == newStringValue);
                if (newObj is not null)
                {
                    Context.ContourHeights = newObj;
                    Context.UserSettings.ContourHeights = newObj.Name;
                    terrainChanged = true;
                }
                else
                {
                    Logger.Error($"Contour intervals {newStringValue} was not found.");
                }
            }

            bool prevBoolValue = Context.UserSettings.VectorizeTerrainLand;
            bool newBoolValue = VectorizeLandCheckbox.Checked;
            if (prevBoolValue != newBoolValue)
            {
                Context.UserSettings.VectorizeTerrainLand = newBoolValue; ;
                terrainChanged = true;
            }
            prevBoolValue = Context.UserSettings.VectorizeTerrainWater;
            newBoolValue = VectorizeWaterCheckbox.Checked;
            if (prevBoolValue != newBoolValue)
            {
                Context.UserSettings.VectorizeTerrainWater = newBoolValue;
                terrainChanged = true;
            }

            int prevIntValue = Context.UserSettings.TerrainMaxResolution;
            int? newIntValue = TerrainMaxResolutionTextbox.Text.ParseIntInvariant();
            if (newIntValue.HasValue && newIntValue.Value != prevIntValue)
            {
                Context.UserSettings.TerrainMaxResolution = newIntValue.Value;
                terrainChanged = true;
            }
            prevIntValue = Context.UserSettings.WaterMaxResolution;
            newIntValue = WaterMaxResolutionTextbox.Text.ParseIntInvariant();
            if (newIntValue.HasValue && newIntValue.Value != prevIntValue)
            {
                Context.UserSettings.WaterMaxResolution = newIntValue.Value;
                terrainChanged = true;
            }
            return terrainChanged;
        }

        private void FormCancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void MaxResolution_TextChanged(object sender, EventArgs e)
        {
            if (sender is not TextBox t)
            {
                return;
            }
            var fv = t.Text.ParseIntInvariant();
            if (fv.HasValue)
            {
                if (fv.Value <= 64)
                {
                    t.Text = "64";
                }
                else if (fv.Value > 8192)
                {
                    t.Text = "8192";
                }
            }
            else
            {
                t.Text = "4096";
            }

        }

        private void LayerConfigTabPage_Click(object sender, EventArgs e)
        {

        }
    }
}
