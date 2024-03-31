namespace CS2MapView.Drawing.Labels
{
    internal class BuildingNameLabel : AbstractTextLabel
    {
        internal override bool Freezed => true;

        internal override bool MayYield => true;

        internal override int YieldPriority => 10;

        public BuildingNameLabel() { }


    }
}
