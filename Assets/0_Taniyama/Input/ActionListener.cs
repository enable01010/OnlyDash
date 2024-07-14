using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface I_ActionListener
{
	public void OnMenu(InputAction.CallbackContext context);
	public void OnPlayerMove(InputAction.CallbackContext context);
	public void OnJump(InputAction.CallbackContext context);
	public void OnSlide(InputAction.CallbackContext context);
	public void OnZipLine(InputAction.CallbackContext context);
    public void OnCamMove(InputAction.CallbackContext context);
	public void OnSlow(InputAction.CallbackContext context);
	public void OnCursor(InputAction.CallbackContext context);
	public void OnSelect(InputAction.CallbackContext context);
	public void OnBuck(InputAction.CallbackContext context);
}

public abstract class MonobehaviourActionListener : MonoBehaviour, I_ActionListener
{
	public virtual void OnMenu(InputAction.CallbackContext context) { }
	public virtual void OnPlayerMove(InputAction.CallbackContext context) { }
	public virtual void OnJump(InputAction.CallbackContext context) { }
	public virtual void OnSlide(InputAction.CallbackContext context) { }
	public virtual void OnZipLine(InputAction.CallbackContext context) { }
    public virtual void OnCamMove(InputAction.CallbackContext context) { }
	public virtual void OnSlow(InputAction.CallbackContext context) { }
	public virtual void OnCursor(InputAction.CallbackContext context) { }
	public virtual void OnSelect(InputAction.CallbackContext context) { }
	public virtual void OnBuck(InputAction.CallbackContext context) { }

	public virtual void Awake()
	{
		MainSceneManager.AddListener(this);
	}
}

public abstract class SingletonActionListener<T> : Singleton<T>,I_ActionListener where T : Singleton<T>
{
	public virtual void OnMenu(InputAction.CallbackContext context) { }
	public virtual void OnPlayerMove(InputAction.CallbackContext context) { }
	public virtual void OnJump(InputAction.CallbackContext context) { }
	public virtual void OnSlide(InputAction.CallbackContext context) { }
    public virtual void OnZipLine(InputAction.CallbackContext context) { }
    public virtual void OnCamMove(InputAction.CallbackContext context) { }
	public virtual void OnSlow(InputAction.CallbackContext context) { }
	public virtual void OnCursor(InputAction.CallbackContext context) { }
	public virtual void OnSelect(InputAction.CallbackContext context) { }
	public virtual void OnBuck(InputAction.CallbackContext context) { }

	public override void OnInitialize()
	{
		MainSceneManager.AddListener(this);
	}
}
