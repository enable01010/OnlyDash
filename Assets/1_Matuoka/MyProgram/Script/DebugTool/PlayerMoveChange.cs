#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using static Player;

public class PlayerMoveChange : DebugToolBase
{
    #region Field

    public enum PlayerPrefsEnum
    {
        PlayerMove = 0,
    }

    private enum PlayerMove
    {
        ControlledMove = 0,
        AutoMove,
        DroneMove,
    }

    private PlayerMove playerMove;
    private List<Button> moveButton = new ();

    #endregion

    #region DebugToolBaseMethod

    public override void DebugToolAwake()
    {
        SetUiAction();
        DataLoad();
        ResetColorPlayerMoveButton();
    }

    public override void DebugToolStart()
    {
        DataSet();
    }

    //public override void DebugToolOpen()
    //{
        
    //}

    public override void DebugToolClose()
    {
        DataSave();
        DataSet();
    }

    #endregion

    #region CustomMethod

    /// <summary>
    /// UIのボタンが押された際の処理の設定
    /// </summary>
    private void SetUiAction()
    {
        // Buttonを全て取得
        foreach (Transform child in this.transform)
        {
            moveButton.Add(child.GetComponent<Button>());
        }

        // 移動方法を受け取り、ボタンの色を調整する
        for (int i = 0; i < moveButton.Count; i++)
        {
            int count = i;
            moveButton[i].onClick.AddListener(() => {
                playerMove = (PlayerMove)count;
                ResetColorPlayerMoveButton();
            });
        }
    }

    /// <summary>
    /// データのロード
    /// </summary>
    private void DataLoad()
    {
        playerMove = (PlayerMove)PlayerPrefs.GetInt(PlayerPrefsEnum.PlayerMove.ToString());
    }

    /// <summary>
    /// データの反映
    /// </summary>
    private void DataSet()
    {
        switch (playerMove)
        {
            case PlayerMove.ControlledMove:
                Player.instance.ChangeMove(new ControlledMove());
                Player.instance.GetController().enabled = true;
                break;

            case PlayerMove.AutoMove:
                Player.instance.ChangeMove(new AutoMove());
                Player.instance.GetController().enabled = true;
                break;

            case PlayerMove.DroneMove:
                Player.instance.ChangeMove(new DroneMove());
                Player.instance.GetController().enabled = false;
                break;

            default:
                Debug.LogError("moveを設定できません");
                break;
        }
    }

    /// <summary>
    /// データのセーブ
    /// </summary>
    private void DataSave()
    {
        PlayerPrefs.SetInt(PlayerPrefsEnum.PlayerMove.ToString(), (int)playerMove);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// プレイヤーの移動方法指定ボタンの色を調整する処理
    /// </summary>
    private void ResetColorPlayerMoveButton()
    {
        int length = Enum.GetValues(typeof(PlayerMove)).Length;
        for (int i = 0; i < length; i++)
        {
            Color col = (i == (int)playerMove) ? Color.yellow : Color.white;

            ColorBlock colorBlock = moveButton[i].colors;
            colorBlock.normalColor = col;
            colorBlock.highlightedColor = col;
            colorBlock.selectedColor = col;

            moveButton[i].colors = colorBlock;
        }
    }

    #endregion
}

#endif