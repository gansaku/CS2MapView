using Gfw.Common;
using System.Collections.Concurrent;

namespace CS2MapView
{
    /// <summary>
    /// 進捗報告イベントパラメータ
    /// </summary>
    public class LoadProgressEventArgs
    {
        /// <summary>
        /// メッセージ
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// 進捗率
        /// </summary>
        public double Progress { get; set; }
    }
    /// <summary>
    /// 進捗報告受信デリゲート
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void LoadProgressEventHandler(object sender, LoadProgressEventArgs e);
    /// <summary>
    /// 読み込みプログレスバー表示用クラス
    /// </summary>
    public class LoadProgressInfo
    {
        /// <summary>
        /// 処理
        /// </summary>
        public enum Process : uint
        {
            OpenFile = 0x1,
            BuildTerrain = 0x2,
            BuildBuildings = 0x4,
            BuildRoads = 0x8,
            BuildRailways = 0x10,
            BuildStreetNames = 0x20,
            BuildLabels = 0x40,
            BuildGrid = 0x80,
            BuildTransportLines = 0x100,
            BuildDistricts = 0x200,
            SaveFile = 0x400,
            Finalize = 0x800,
            OpenFileTotal = OpenFile | BuildTerrain | BuildBuildings | BuildRoads | BuildRailways | BuildStreetNames | BuildLabels | BuildTransportLines | BuildGrid | BuildDistricts | Finalize
        }
        private static readonly Dictionary<Process, float> ProcessWeights = new() {
            { Process.OpenFile , 10f},
            { Process.BuildTerrain, 50f},
            { Process.BuildBuildings , 10f },
            { Process.BuildRoads , 20f },
            { Process.BuildRailways, 20f },
            { Process.BuildLabels, 50f },
            { Process.BuildTransportLines, 20f },
            { Process.BuildDistricts, 10f },
            { Process.SaveFile ,10f },
            { Process.Finalize , 1f }
        };
        /// <summary>
        /// 進捗割合のウェイトを返します。
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static float ProcessWeight(Process process) => ProcessWeights[process];

        private readonly ConcurrentDictionary<Process, (float weight, float progress)> Processes = new();
        /// <summary>
        /// 実行予定の処理を登録します。
        /// </summary>
        /// <param name="process"></param>
        public void RegistProcess(Process process)
        {
            ProcessWeights.Where(t => t.Key != Process.OpenFileTotal && (t.Key & process) > 0).ForEach(pw =>
            {
                Processes.TryAdd(pw.Key, (pw.Value, 0f));

            });
        }
        /// <summary>
        /// 進捗を通知します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="process"></param>
        /// <param name="progress"></param>
        /// <param name="message"></param>
        public void Progress(object sender, Process process, float progress, string? message)
        {
            if (Processes.TryGetValue(process, out var p))
            {
                p.progress = progress;

                Processes.AddOrUpdate(process, (p.weight, progress), (name, orig) => (orig.weight, progress));
            }
            float sum = Processes.Sum(t => t.Value.progress * t.Value.weight);
            float total = Processes.Sum(t => t.Value.weight);
            float rate = sum / total;
            LoadProgress?.Invoke(sender, new LoadProgressEventArgs { Message = message, Progress = rate });

        }
        /// <summary>
        /// 進捗が変更された際に呼ばれるイベント
        /// </summary>
        public event LoadProgressEventHandler? LoadProgress;



    }
}
