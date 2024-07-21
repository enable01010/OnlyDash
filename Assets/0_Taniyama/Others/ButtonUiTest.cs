using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUiTest : MonoBehaviour
{
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            LibButtonUIInfoManager.PopIcon(ButtonType.Climbing);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            LibButtonUIInfoManager.RemoveIcon(ButtonType.Climbing);
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            LibButtonUIInfoManager.PopIcon(ButtonType.ZipLine);
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            LibButtonUIInfoManager.RemoveIcon(ButtonType.ZipLine);
        }
    }
}
