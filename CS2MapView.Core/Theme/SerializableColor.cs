using SkiaSharp;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CS2MapView.Theme
{
    /// <summary>
    /// 色の設定ファイル用表現
    /// </summary>
    public partial struct SerializableColor
    {
        /// <summary>
        /// アルファ
        /// </summary>
        [XmlIgnore]
        public byte A { get; set; }
        /// <summary>
        /// 赤
        /// </summary>
        [XmlIgnore]
        public byte R { get; set; }
        /// <summary>
        /// 緑
        /// </summary>
        [XmlIgnore]
        public byte G { get; set; }
        /// <summary>
        /// 青
        /// </summary>
        [XmlIgnore]
        public byte B { get; set; }

        /// <summary>
        /// カラーコード
        /// </summary>
        [XmlText]
        public string HexString
        {
            readonly get => $"#{R:x2}{G:x2}{B:x2}{A:x2}";
            set
            {

                string temp = value;

                if (temp is null)
                {
                    A = 0; R = 0; B = 0; G = 0;
                    return;
                }
                temp = temp.Trim();
                if (temp.StartsWith("#"))
                {
                    temp = temp.Remove(0, 1);
                }
                if (temp.Length < 6)
                {
                    A = 0; R = 0; B = 0; G = 0;
                    return;
                }

                byte parse(int start, byte defaultValue = 0)
                {
                    if (byte.TryParse(temp.AsSpan(start, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r))
                    {
                        return r;
                    }
                    return defaultValue;
                }

                if (RegexHex6().IsMatch(temp))
                {
                    R = parse(0);
                    G = parse(2);
                    B = parse(4);
                    A = 255;
                }
                if (RegexHex8().IsMatch(temp))
                {
                    R = parse(0);
                    G = parse(2);
                    B = parse(4);
                    A = parse(6);
                }

            }
        }
        /// <summary>
        /// ARGB値からインスタンスを作成して返します。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SerializableColor FromARGB(byte a, byte r, byte g, byte b) => new() { A = a, R = r, G = g, B = b };
        /// <summary>
        /// SKColorからインスタンスを作成して返します。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static SerializableColor FromSKColor(SKColor color) => new() { A = color.Alpha, R = color.Red, G = color.Green, B = color.Blue };
        /// <summary>
        /// 不透明のグレー色を作成して返します。
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static SerializableColor GrayScale(byte g) => new() { A = 255, R = g, G = g, B = g };
        /// <summary>
        /// 透過付きのグレー色を作成して返します。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public static SerializableColor GrayScale(byte a, byte g) => new() { A = a, R = g, G = g, B = g };
        /// <summary>
        /// SKColorに変換します。
        /// </summary>
        /// <returns></returns>
        public readonly SKColor ToSKColor()
        {
            return new SKColor(R, G, B, A);
        }
        /// <summary>
        /// メモリ書き込み用の表現を返します。
        /// </summary>
        /// <returns></returns>
        public readonly uint ToBgra8888()
        {

            uint a = A;
            uint r = R;
            uint g = G;
            uint b = B;
            if (BitConverter.IsLittleEndian)
            {
                a <<= 24;
                r <<= 16;
                g <<= 8;
            }
            else
            {
                b <<= 24;
                g <<= 16;
                r <<= 8;
            }
            return a | r | g | b;
        }
        /// <summary>
        /// SKColorへの暗黙キャスト
        /// </summary>
        /// <param name="c"></param>
        public static implicit operator SKColor(SerializableColor c) => new(c.R, c.G, c.B, c.A);
        /// <summary>
        /// SKColorからの暗黙キャスト
        /// </summary>
        /// <param name="c"></param>
        public static implicit operator SerializableColor(SKColor c) => FromSKColor(c);

        [GeneratedRegex("[0-9a-fA-F]{6}")]
        private static partial Regex RegexHex6();
        [GeneratedRegex("[0-9a-fA-F]{8}")]
        private static partial Regex RegexHex8();
    }
}
