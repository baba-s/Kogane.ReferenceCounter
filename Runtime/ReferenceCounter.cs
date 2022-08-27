using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kogane
{
    /// <summary>
    /// 参照カウンタを管理するクラス
    /// </summary>
    public sealed class ReferenceCounter
    {
        //================================================================================
        // 変数(readonly)
        //================================================================================
        private readonly List<ReferenceCounterHandle> m_list = new(); // ハンドルのリスト

        //================================================================================
        // プロパティ
        //================================================================================
        /// <summary>
        /// タグ名を返します
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// 参照数を返します
        /// </summary>
        public int Count => m_list.Count;

        /// <summary>
        /// 参照されている場合 true を返します
        /// </summary>
        public bool IsReferenced => 1 <= Count;

        /// <summary>
        /// 参照されていない場合 true を返します
        /// </summary>
        public bool IsNotReferenced => !IsReferenced;

        /// <summary>
        /// ハンドルのリストを返します
        /// </summary>
        public IReadOnlyList<IReferenceCounterHandle> List => m_list;

        //================================================================================
        // イベント
        //================================================================================
        /// <summary>
        /// 参照数が変化された時に呼び出されます
        /// </summary>
        public event Action<ReferenceCounter> OnChange;

        //================================================================================
        // 関数
        //================================================================================
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ReferenceCounter( string tag )
        {
            Tag = tag;
        }

        /// <summary>
        /// 参照します
        /// </summary>
        [MustUseReturnValue]
        public ReferenceCounterHandle Refer( string name )
        {
            var blocker = new ReferenceCounterHandle
            (
                tag: Tag,
                name: name,
                referenceCounter: this,
                list: m_list,
                onChange: OnChange
            );

            return blocker;
        }

        /// <summary>
        /// すべての参照を解除します
        /// </summary>
        public void Clear()
        {
            Clear( false );
        }

        /// <summary>
        /// すべての参照を解除します
        /// </summary>
        public void Clear( bool callEvent )
        {
            if ( !IsReferenced ) return;

            var onChange = OnChange;
            OnChange = null;

            for ( var i = m_list.Count - 1; i >= 0; i-- )
            {
                var referenceCounter = m_list[ i ];
                referenceCounter.Dispose();
            }

            OnChange = onChange;

            if ( !callEvent ) return;
            OnChange?.Invoke( this );
        }
    }
}