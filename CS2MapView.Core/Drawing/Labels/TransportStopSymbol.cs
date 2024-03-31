using SkiaSharp;
using Svg.Skia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2MapView.Drawing.Labels
{
    internal class TransportStopSymbol : AbstractMapLabel
    {
        internal required SKPicture Picture { get; init; }
        /// <summary>
        /// 元画像に対しての倍率
        /// </summary>
        internal float ImageScale { get; init; }

        internal override SKRect Bounds => new(DisplayPosition.X - Size.width*ImageScale / 2f, DisplayPosition.Y - Size.height * ImageScale / 2f, DisplayPosition.X + Size.width * ImageScale / 2f, DisplayPosition.Y + Size.height * ImageScale / 2f);

        internal override bool Freezed => true;

        internal override bool MayYield => true;

        internal override int YieldPriority => 80;
    }
}
