using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTool : MonoBehaviour
{
#if UNITY_EDITOR

    private bool isPause = false;
    [SerializeField]private GameObject UI_Window;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            InputKey();
        }
    }

    private void InputKey()
    {
        isPause = !isPause;

        UI_Window.SetActive(isPause);

        Time.timeScale = isPause ? 0f : 1f;
    }

#else

    private void Awake()
    {
        Destroy(this.gameObject);
    }

#endif


}
