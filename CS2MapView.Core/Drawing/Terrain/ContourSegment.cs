using System.Numerics;

namespace CS2MapView.Drawing.Terrain;

/// <summary>
/// 等高線の線分の一領域分のコレクション
/// </summary>
public class ContourSegments : List<ContourSegment>
{
    /// <summary>
    /// 何らかの問題により線分が閉じられていないことを示します。
    /// </summary>
    public bool NotClosed = false;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ContourSegments() : base() { }
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="segments"></param>
    public ContourSegments(IEnumerable<ContourSegment> segments) : base(segments) { }
}
/// <summary>
/// 等高線の線分
/// </summary>
public class ContourSegment
{
    /// <summary>
    /// 始点
    /// </summary>
    public Vector3 Start;
    /// <summary>
    /// 終点
    /// </summary>
    public Vector3 End;

    /// <summary>
    /// 始点と終点を入れ替えます。
    /// </summary>
    public void Swap()
    {
        (End, Start) = (Start, End);
    }
    /// <summary>
    /// デバッグ用の文字列表現
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Start}-{End}";
    }
}

