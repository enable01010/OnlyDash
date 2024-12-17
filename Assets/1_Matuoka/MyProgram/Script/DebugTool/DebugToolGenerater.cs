using UnityEngine;

public class DebugToolGenerater : MonoBehaviour
{
    private static bool isGenerate = true;

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]// ゲーム起動時（シーンロード前）の処理
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]// 最初のシーンロード後の処理
    private static void OnGameStart()
    {
        if (!isGenerate) return;

        Instantiate(LibResourceLoader._debugToolPref);
    }
}
