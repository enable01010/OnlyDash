using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : SingletonActionListener<MenuManager>
{
	private bool isMenu;

	public static bool GetIsMenu() { return instance.isMenu; }

	//�A�N�V�������X�i�[�p�֐�
	public override void OnMenu(InputAction.CallbackContext context)
	{
		LibDebug.Log("Menu�������ꂽ��",DebugUser.Taniyama);
	}

	//�J����Ă��Ȃ��X�e�[�g

	//�J����Ă���X�e�[�g
}
