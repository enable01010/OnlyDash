#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;

public class DebugToolManager : MonoBehaviour
{
    #region Field

    [SerializeField] private bool isStartOpen = false;
    private bool isDebugToolOpen = false;
    [SerializeField] private GameObject uiWindow;

    [SerializeField] private List<DebugToolBase> debugToolBases;

    #endregion


    #region MonoBehaviourMethod

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        foreach (var debugToolBase in debugToolBases) debugToolBase.DebugToolAwake();
    }

    private void Start()
    {
        foreach (var debugToolBase in debugToolBases) debugToolBase.DebugToolStart();

        if (isStartOpen) ChangeDebugTool();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) == false) return;
        ChangeDebugTool();
    }

    #endregion


    #region CustomMethod

    /// <summary>
    /// DebugToolÇÃONÅEOFFêÿÇËë÷Ç¶
    /// </summary>
    private void ChangeDebugTool()
    {
        if (isDebugToolOpen == false)
        {
            Time.timeScale = 0f;
            foreach (var debugToolBase in debugToolBases) debugToolBase.DebugToolOpen();
        }
        else
        {
            foreach (var debugToolBase in debugToolBases) debugToolBase.DebugToolClose();
        }

        isDebugToolOpen = !isDebugToolOpen;
        uiWindow.SetActive(isDebugToolOpen);
    }

    #endregion
}

#endif