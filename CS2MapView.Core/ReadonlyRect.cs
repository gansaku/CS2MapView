using SkiaSharp;

namespace CS2MapView
{
    /// <summary>
    /// 読み取り専用の長方形　（いる？）
    /// </summary>
    public readonly struct ReadonlyRect
    {
        /// <summary>
        /// 左
        /// </summary>
        public readonly float Left;
        /// <summary>
        /// 上
        /// </summary>
        public readonly float Top;
        /// <summary>
        /// 右
        /// </summary>
        public readonly float Right;
        /// <summary>
        /// 下
        /// </summary>
        public readonly float Bottom;
        /// <summary>
        /// 幅
        /// </summary>
        public readonly float Width => Right - Left;
        /// <summary>
        /// 高さ
        /// </summary>
        public readonly float Height => Bottom - Top;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public ReadonlyRect(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
        /// <summary>
        /// SKRectへの暗黙キャスト
        /// </summary>
        /// <param name="r"></param>
        public static implicit operator SKRect(ReadonlyRect r) => new(r.Left, r.Top, r.Right, r.Bottom);
        /// <summary>
        /// SKRectからの暗黙キャスト
        /// </summary>
        /// <param name="r"></param>
        public static implicit operator ReadonlyRect(SKRect r) => new(r.Left, r.Top, r.Right, r.Bottom);
    }
}
