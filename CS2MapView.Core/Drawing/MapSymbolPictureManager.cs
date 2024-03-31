using Svg.Skia;

namespace CS2MapView.Drawing
{
    /// <summary>
    /// 地図記号のイメージを管理します。
    /// </summary>
    public sealed class MapSymbolPictureManager : IDisposable
    {
        private static MapSymbolPictureManager? _inst = null;
        /// <summary>
        /// 唯一のインスタンスを取得します。
        /// </summary>
        public static MapSymbolPictureManager Instance => _inst ??= new();

        /// <summary>
        /// 画像読み込み済みであればすべてDisposeします。
        /// </summary>
        public static void DisposeIfInitialized()
        {
            if (_inst is not null)
            {
                _inst.Dispose();
                _inst = null;
            }
        }

        private readonly Dictionary<string, Dictionary<string, SKSvg>> Symbols = [];

        private MapSymbolPictureManager()
        {
            LoadImages();
        }
        /// <summary>
        /// SVGの内容を取得します。
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public SKSvg? GetSvg(string dir, string name)
        {
            if (Symbols.TryGetValue(dir, out var sub))
            {
                if (sub.TryGetValue(name, out var svg))
                {
                    return svg;
                }
            }
            return null;
        }
        /// <summary>
        /// SVGの内容を dir/name 形式で指定して取得します。
        /// </summary>
        /// <param name="dirAndName"></param>
        /// <returns></returns>
        public SKSvg? GetSvg(string dirAndName)
        {
            string[] token = dirAndName.Split('/');
            if (token.Length != 2) { return null; }
            return GetSvg(token[0], token[1]);
        }
        /// <summary>
        /// 指定したディレクトリの地図記号データを取得します。
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public IDictionary<string, SKSvg>? GetSymbols(string dir)
        {
            if (Symbols.TryGetValue(dir, out var sub))
            {
                return sub.AsReadOnly();
            }
            return null;
        }


        private void LoadImages(string? dir = null)
        {
            dir ??= Path.Combine(AppContext.BaseDirectory, "images");
            Symbols.Clear();

            foreach (var subdir in Directory.GetDirectories(dir))
            {
                var dirName = new DirectoryInfo(subdir).Name;
                Symbols.Add(dirName, []);
                foreach (var file in Directory.GetFiles(subdir))
                {
                    var fn = Path.GetFileNameWithoutExtension(file);
                    SKSvg svg = new();
                    if (svg.Load(file) is not null)
                    {
                        Symbols[dirName].Add(fn, svg);
                        //   Debug.Print($"{dirName}/{fn} size={svg.Picture!.CullRect}");
                    }

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            foreach (var k in Symbols.Values)
            {
                foreach (var v in k.Values)
                {
                    v?.Dispose();
                }
            }
            Symbols.Clear();
        }
    }
}
