namespace CS2MapView.Drawing.Terrain
{
    internal interface ITerrainDrawingsBuiderProcedure
    {
        void Execute();
        IEnumerable<IDrawCommand> GetResult();
    }
}
