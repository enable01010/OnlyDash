using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : SingletonActionListener<MenuManager>
{
	private bool isMenu;

	public static bool GetIsMenu() { return instance.isMenu; }

	//アクションリスナー用関数
	public override void OnMenu(InputAction.CallbackContext context)
	{
		LibDebug.Log("Menuが押されたよ",DebugUser.Taniyama);
	}

	//開かれていないステート

	//開かれているステート
}
