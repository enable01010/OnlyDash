#if UNITY_EDITOR

using UnityEngine;

public class DebugToolGenerater : MonoBehaviour
{
    private static bool isGenerate = true;

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]// �Q�[���N�����i�V�[�����[�h�O�j�̏���
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]// �ŏ��̃V�[�����[�h��̏���
    private static void OnGameStart()
    {
        if (!isGenerate) return;

        DontDestroyOnLoad(Instantiate(LibResourceLoader._debugToolPref));
        DontDestroyOnLoad(Instantiate(LibResourceLoader._uiEventSystem));
    }
}

#endif