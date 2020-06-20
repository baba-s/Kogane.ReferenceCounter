using System;
using System.Collections.Generic;

namespace Kogane
{
	/// <summary>
	/// 参照カウンタのハンドル
	/// </summary>
	public sealed class ReferenceCounterHandle :
		DisposableChecker,
		IReferenceCounterHandle
	{
		//================================================================================
		// 変数(readonly)
		//================================================================================
		private readonly ReferenceCounter             m_referenceCounter; // このハンドルを管理している参照カウンタ
		private readonly List<ReferenceCounterHandle> m_list;             // このハンドルを管理している参照カウンタが管理しているハンドルのリスト
		private readonly Action<ReferenceCounter>     m_onChange;         // 参照数が変化した時に呼び出すコールバック

		//================================================================================
		// 変数
		//================================================================================
		private bool m_isDispose; // 既に破棄されている場合 true

		//================================================================================
		// プロパティ
		//================================================================================
		/// <summary>
		/// タグ名を返します
		/// </summary>
		public string Tag { get; }

		/// <summary>
		/// インスタンス名を返します
		/// </summary>
		public string Name { get; }
		
		//================================================================================
		// イベント(static)
		//================================================================================
		/// <summary>
		/// Dispose が呼び出されていないインスタンスが見つかった時に呼び出されます
		/// </summary>
		public static event Action<string, string> OnHandleException; 

		//================================================================================
		// 関数
		//================================================================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ReferenceCounterHandle
		(
			string                       tag,
			string                       name,
			ReferenceCounter             referenceCounter,
			List<ReferenceCounterHandle> list,
			Action<ReferenceCounter>     onChange
		)
		{
			Tag                = tag;
			Name               = name;
			m_referenceCounter = referenceCounter;
			m_list             = list;
			m_onChange         = onChange;

			list.Add( this );
			onChange?.Invoke( referenceCounter );
		}

		/// <summary>
		/// 参照カウンタを減らします
		/// </summary>
		protected override void DoDispose()
		{
			if ( m_isDispose ) return;
			m_isDispose = true;
			m_list.Remove( this );
			m_onChange?.Invoke( m_referenceCounter );
		}

		/// <summary>
		/// Dispose が呼び出されていないインスタンスが見つかった時に呼び出されます
		/// </summary>
		protected override void HandleException()
		{
			OnHandleException?.Invoke( Tag, Name );
		}
	}
}