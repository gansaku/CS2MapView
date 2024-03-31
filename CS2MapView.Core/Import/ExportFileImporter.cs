using CS2MapView.Data;
using CS2MapView.Drawing;
using CS2MapView.Import.CS1;
using CS2MapView.Import.CS2;
using Gfw.Common;
using log4net;
using System.IO.Compression;

namespace CS2MapView.Import
{
    /// <summary>
    /// ゲームからエクスポートしたファイルのインポートクラス
    /// </summary>
    public class ExportFileImporter
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ExportFileImporter));
        /// <summary>
        /// クライアントアプリケーション
        /// </summary>
        public ICS2MapViewRoot AppRoot { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="appRoot"></param>
        public ExportFileImporter(ICS2MapViewRoot appRoot)
        {
            AppRoot = appRoot;
        }
        /// <summary>
        /// インポートの実行
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<string?> ImportAsync(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            Logger.Debug($"opening {path}");

            LoadProgressInfo lpi = new();
            lpi.RegistProcess(LoadProgressInfo.Process.OpenFileTotal);
            lpi.LoadProgress += AppRoot.ReceiveProgressChanged;

            SourceMapType? sourceMapType = null;
            string? msg = null;
            if (ext == ".gz" || ext == ".cslmap")
            {

                var exData = CS1XMLImporter.Import(path);
                lpi.Progress(this, LoadProgressInfo.Process.OpenFile, 1f, $"Loaded {path}.");
                AppRoot.MapData?.Dispose();

                sourceMapType = new CS1MapType(exData);

                AppRoot.MapData = await sourceMapType.BuildAll(AppRoot, lpi);
                AppRoot.MapData.FileName = path;

            }
            else if (ext == ".cs2map" || ext == ".zip")
            {
                using var stream = new FileStream(path, FileMode.Open);
                using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
                var exData = await CS2MapDataDeserializer.Deserialize(zip);
                if (exData is null)
                {
                    return Resources.DataImport_DeserializeError;
                }

                lpi.Progress(this, LoadProgressInfo.Process.OpenFile, 1f, $"Loaded {path}.");
                sourceMapType = new CS2MapType(exData);
                AppRoot.MapData = await sourceMapType.BuildAll(AppRoot, lpi);
                AppRoot.MapData.FileName = path;
                if (Version.TryParse(exData.MainData!.FileVersion, out var fileVersion))
                {
                    if( fileVersion > CS2MapDataDeserializer.SupportedDataVersion)
                    {
                        msg = Resources.DataImport_FileVersionWarning;
                    }
                }
                else
                {
                    msg = Resources.DataImport_FileVersionWarning;
                }
            }
            else
            {
                return string.Format(Resources.DataImport_ExtensionNotSupported, ext);
            }
            //SKPaintクリア
            SKPaintCache.Instance.DisposeAll();
            //サイズをセット
            AppRoot.Context.SourceMapType = sourceMapType;
            //完了表示
            lpi.Progress(this, LoadProgressInfo.Process.Finalize, 1f, null);
            return msg;
        }

    }
}
