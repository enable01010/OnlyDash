using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public partial class MainSceneManager:Singleton<MainSceneManager>
{
	private PlayerInput _input;
	private List<I_ActionListener> actionListeners = new List<I_ActionListener>();
	
	public void OnMenu(InputAction.CallbackContext context)
	{
		foreach (I_ActionListener listener in actionListeners)
		{
			listener.OnMenu(context);
		}
	}
	public void OnPlayerMove(InputAction.CallbackContext context)
	{
		foreach (I_ActionListener listener in actionListeners)
		{
			listener.OnPlayerMove(context);
		}
	}
	public void OnJump(InputAction.CallbackContext context)
	{
		foreach (I_ActionListener listener in actionListeners)
		{
			listener.OnJump(context);
		}
	}
	public void OnSlide(InputAction.CallbackContext context)
	{
		foreach (I_ActionListener listener in actionListeners)
		{
			listener.OnSlide(context);
		}
	}

    public void OnZipLine(InputAction.CallbackContext context)
    {
        foreach (I_ActionListener listener in actionListeners)
        {
            listener.OnZipLine(context);
        }
    }

    public void OnCamMove(InputAction.CallbackContext context)
	{
		foreach (I_ActionListener listener in actionListeners)
		{
			listener.OnCamMove(context);
		}
	}
	public void OnSlow(InputAction.CallbackContext context)
	{
		foreach (I_ActionListener listener in actionListeners)
		{
			listener.OnSlow(context);
		}
	}
	public void OnCursor(InputAction.CallbackContext context)
	{
		foreach (I_ActionListener listener in actionListeners)
		{
			listener.OnCursor(context);
		}
	}
	public void OnSelect(InputAction.CallbackContext context)
	{
		foreach (I_ActionListener listener in actionListeners)
		{
			listener.OnSelect(context);
		}
	}
	public void OnBuck(InputAction.CallbackContext context)
	{
		foreach (I_ActionListener listener in actionListeners)
		{
			listener.OnBuck(context);
		}
	}

    public override void OnInitialize()
	{
		_input = GetComponent<PlayerInput>();
		//ボタン押し始め
		_input.actions["Menu"].started += OnMenu;
		_input.actions["PlayerMove"].started += OnPlayerMove;
		_input.actions["Jump"].started += OnJump;
		_input.actions["Slide"].started += OnSlide;
		_input.actions["ZipLine"].started += OnZipLine;
		_input.actions["CamMove"].started += OnCamMove;
		_input.actions["Slow"].started += OnSlow;
		_input.actions["Cursor"].started += OnCursor;
		_input.actions["Select"].started += OnSelect;
		_input.actions["Buck"].started += OnBuck;

		//数値の切り替わり
		_input.actions["Menu"].performed += OnMenu;
		_input.actions["PlayerMove"].performed += OnPlayerMove;
		_input.actions["Jump"].performed += OnJump;
		_input.actions["Slide"].performed += OnSlide;
        _input.actions["ZipLine"].performed += OnZipLine;
        _input.actions["CamMove"].performed += OnCamMove;
		_input.actions["Slow"].performed += OnSlow;
		_input.actions["Cursor"].performed += OnCursor;
		_input.actions["Select"].performed += OnSelect;
		_input.actions["Buck"].performed += OnBuck;

		//ボタン離したとき
		_input.actions["Menu"].canceled += OnMenu;
		_input.actions["PlayerMove"].canceled += OnPlayerMove;
		_input.actions["Jump"].canceled += OnJump;
		_input.actions["Slide"].canceled += OnSlide;
        _input.actions["ZipLine"].canceled += OnZipLine;
        _input.actions["CamMove"].canceled += OnCamMove;
		_input.actions["Slow"].canceled += OnSlow;
		_input.actions["Cursor"].canceled += OnCursor;
		_input.actions["Select"].canceled += OnSelect;
		_input.actions["Buck"].canceled += OnBuck;
	}

	public static void AddListener(I_ActionListener listener)
	{
		instance.actionListeners.Add(listener);
	}

	public static string GetCurrentControlScheme()
	{
		return instance._input.currentControlScheme;
    }
}