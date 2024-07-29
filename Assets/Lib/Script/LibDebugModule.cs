using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.PackageManager.UI;

public class LibDebug
{

    static private DebugEditor _window;
    static private DebugEditor window 
    { get 
        { 
            if (_window == null) 
                _window = (DebugEditor)EditorWindow.GetWindow(typeof(DebugEditor), false, null, false);
            return _window; 
        } 
    }

    static public void ButtonLog(object obj, Action action, bool isStopAndView = false)
    {
#if UNITY_EDITOR
        //window.AddButtonLog(obj, action);

        if (isStopAndView == true) StopAndView();
#endif
    }

    static public void LogIf(object obj, DebugUser user, Func<bool> whenUse, bool isStopAndView = false)
    {
#if UNITY_EDITOR
        if (whenUse.Invoke() == false) return;

        window.Log(obj, user);

        if (isStopAndView == true) StopAndView();
#endif
    }

    static public void Log(object obj, DebugUser user,bool isStopAndView = false)
    {
#if UNITY_EDITOR
        window.Log(obj, user);

        if (isStopAndView == true) StopAndView();
#endif
    }

    static private void StopAndView()
    {
#if UNITY_EDITOR
        EditorWindow.GetWindow(typeof(DebugEditor));
        UnityEditor.EditorApplication.isPaused = true;
#endif
    }

}

public enum DebugUser
{
    All,
    Taniyama,
    Matuoka,
}
