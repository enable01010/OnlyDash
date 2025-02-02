#if UNITY_EDITOR

using UnityEngine;

public class DebugToolBase : MonoBehaviour
{
    /// <summary>
    /// Awakeでの処理
    /// </summary>
    public virtual void DebugToolAwake()
    {

    }

    /// <summary>
    /// Startでの処理
    /// </summary>
    public virtual void DebugToolStart()
    {

    }

    /// <summary>
    /// DebugToolを開くときの処理
    /// </summary>
    public virtual void DebugToolOpen()
    {

    }

    /// <summary>
    /// DebugToolを閉じるときの処理
    /// </summary>
    public virtual void DebugToolClose()
    {

    }
}

#endif