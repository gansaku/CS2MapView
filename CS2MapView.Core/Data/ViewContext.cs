using CS2MapView.Util;
using Gfw.Common;
using SkiaSharp;

namespace CS2MapView.Data
{
    /// <summary>
    /// 表示状態
    /// </summary>
    public class ViewContext
    {
        /// <summary>
        /// ズーム
        /// </summary>
        public float ScaleFactor { get; set; } = 1f;

        /// <summary>
        /// データの座標をもとにした補正スケール倍率
        /// </summary>
        public float WorldScaleFactor { get; set; } = 0.2f;

        private float _angle = 0f;
        /// <summary>
        /// 表示角度 (rad)
        /// </summary>
        public float Angle
        {
            get => _angle;
            // 0 ~ 2π の範囲に設定
            set
            {
                double v = value;
                v += Math.PI * 2;
                v %= Math.PI * 2;
                _angle = (float)v;

            }
        }
        /// <summary>
        /// 表示基準点（角度0の場合は左上)
        /// </summary>
        public (float x, float y) ViewLeftTop { get; set; } = (0f, 0f);

        /// <summary>
        /// データの座標範囲
        /// インポート時に設定されます。
        /// </summary>
        public ReadonlyRect WorldRect { get; set; }


        /// <summary>
        /// ユーザ設定拡大率*元データからの拡大率
        /// </summary>
        public float ViewScaleFromWorld => ScaleFactor * WorldScaleFactor;
        /// <summary>
        /// ビュー用のTransform
        /// </summary>
        public SKMatrix ViewTransform => SKMatrix.CreateRotation(Angle).PostConcat(
            SKMatrix.CreateTranslation(-ViewLeftTop.x, -ViewLeftTop.y)).PostConcat(
                SKMatrix.CreateScale(ScaleFactor * WorldScaleFactor, ScaleFactor * WorldScaleFactor));
        /// <summary>
        /// テキストオブジェクトを配置するワールド座標への変換
        /// </summary>
        public SKMatrix TextWorldTransform => SKMatrix.CreateRotation(Angle).PostConcat(
                          SKMatrix.CreateScale(ViewScaleFromWorld, ViewScaleFromWorld));
        /// <summary>
        /// テキストオブジェクト配置枠
        /// </summary>
        public SKRect TextWorldRect => SKPathEx.GetTransformedRectBound(WorldRect, TextWorldTransform);


        /// <summary>
        /// オブジェクトを複製します。
        /// </summary>
        public ViewContext Clone() => new PropertyUtil<ViewContext>().PropertyClone<ViewContext>(this);

    }
}
