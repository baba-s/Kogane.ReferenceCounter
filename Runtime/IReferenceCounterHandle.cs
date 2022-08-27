namespace Kogane
{
    /// <summary>
    /// 参照カウンタのハンドルのインターフェイス
    /// </summary>
    public interface IReferenceCounterHandle
    {
        /// <summary>
        /// タグ名を返します
        /// </summary>
        string Tag { get; }

        /// <summary>
        /// インスタンス名を返します
        /// </summary>
        string Name { get; }
    }
}