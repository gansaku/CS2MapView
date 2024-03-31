namespace CS2MapView.Drawing.Terrain
{
    internal class TerrainProgress
    {
        internal int LayersCount { get; set; }

        private int _completedCount = 0;
        internal int CompletedCount { get => _completedCount; }
        internal void IncrementCompleted()
        {
            Interlocked.Increment(ref _completedCount);
            RaiseProgressUpdated();
        }

        internal event Action? ProgressUpdated;
        internal void RaiseProgressUpdated()
        {
            ProgressUpdated?.Invoke();
        }
    }
}
