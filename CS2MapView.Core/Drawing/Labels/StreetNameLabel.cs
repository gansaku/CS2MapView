namespace CS2MapView.Drawing.Labels
{
    internal class StreetNameLabel : AbstractTextOnPathLabel
    {
        internal override bool Freezed => true;

        internal override bool MayYield => true;

        internal override int YieldPriority => 1;


    }
}
