using CS2MapView.Import;

namespace CS2MapView.Form.Command
{
    internal class FileCommands
    {
        private MainForm MainForm { get; init; }

        internal FileCommands(MainForm mainForm)
        {
            MainForm = mainForm;
        }

        internal async Task OpenFileAsync()
        {
            var ofd = new OpenFileDialog
            {
                CheckFileExists = true,
#if DEBUG
                Filter = "CS2/CSLMap(*.cslmap,*.cslmap.gz,*.cs2map, *.cs2map.zip)|*.cslmap;*.cslmap.gz;*.cs2map;*.cs2map.zip|CS2Map(*.cs2map,*.cs2map.zip)|*.cs2map;*.cs2map.zip|CSLMap(*.cslmap,*.cslmap.gz|*.cslmap;*.cslmap.gz"
#else
                Filter = "CS2/CSLMap(*.cslmap,*.cslmap.gz,*.cs2map)|*.cslmap;*.cslmap.gz;*.cs2map|CS2Map(*.cs2map)|*.cs2map|CSLMap(*.cslmap,*.cslmap.gz|*.cslmap;*.cslmap.gz"
#endif
            };
            if( MainForm.Context.UserSettings.OpenFileDir is not null)
            {
                if (Directory.Exists(MainForm.Context.UserSettings.OpenFileDir))
                {
                    ofd.InitialDirectory = MainForm.Context.UserSettings.OpenFileDir;
                }
            }
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                await new ExportFileImporter(MainForm).ImportAsync(ofd.FileName);
                MainForm.OnZoomOrViewPositionChanged();
                MainForm.InvalidateSkia();
                if(ofd.FileName is not null)
                {
                    MainForm.Context.UserSettings.OpenFileDir = Path.GetDirectoryName(ofd.FileName)!;
                    await MainForm.Context.UserSettings.SaveAsync();
                }
            }
        }
    }
}
