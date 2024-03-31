using Svg.Skia;

namespace CS2MapView.Drawing.Labels
{
    internal class MapSymbol : AbstractMapLabel
    {
        internal required SKSvg Svg { get; init; }
        /// <summary>
        /// 元画像に対しての倍率
        /// </summary>
        internal float ImageScale { get; init; }

        internal override bool Freezed => true;

        internal override bool MayYield => true;

        internal override int YieldPriority => 20;


    }
}
