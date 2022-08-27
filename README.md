# Kogane Reference Counter

参照カウンタ

## 使用例

```cs
using Kogane;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ReferenceCounters
{
    // タップブロック用の参照カウンタ
    public static ReferenceCounter TapBlocker { get; } = new ReferenceCounter( nameof( TapBlocker ) );
}

public class Example : MonoBehaviour
{
    public Image m_icon;
    public Image m_inputBlocker;

    private void Awake()
    {
        // タップブロック用の参照カウンタの参照数が変更された時に
        // 呼び出されるイベントにコールバックを登録
        ReferenceCounters.TapBlocker.OnChange += OnChange;
    }

    private void OnDestroy()
    {
        // タップブロック用の参照カウンタの参照数が変更された時に
        // 呼び出されるイベントからコールバックを解除
        ReferenceCounters.TapBlocker.OnChange -= OnChange;
    }

    private void OnChange( ReferenceCounter counter )
    {
        // タップブロック用の参照カウンタがどこかから参照されている場合は
        // タップをブロックするための Image をアクティブにする
        m_inputBlocker.gameObject.SetActive( counter.IsReferenced );
    }

    private void Update()
    {
        if ( Input.GetKeyDown( KeyCode.Space ) )
        {
            StartCoroutine( LoadAsync() );
        }
    }

    private IEnumerator LoadAsync()
    {
        // スプライトの読み込み中はタップをブロックするために参照カウンタを参照
        var counter = ReferenceCounters.TapBlocker.Refer( nameof( LoadAsync ) );
        var request = Resources.LoadAsync<Sprite>( "Assets/icon.png" );

        yield return request;

        m_icon.sprite = ( Sprite ) request.asset;

        // 読み込みが完了したのでタップのブロックを解除
        counter.Dispose();
    }
}
```
