namespace CS2MapView.Drawing.Labels
{
    internal class DistrictNameLabel : AbstractTextLabel
    {
        internal override bool Freezed => false;

        internal override bool MayYield => false;

        internal override int YieldPriority => 100;
    }
}
