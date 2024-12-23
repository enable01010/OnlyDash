#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.UI;
using System;
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
    }
    private PlayerMove playerMove;
    [SerializeField] private Button[] moveButton;

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
    /// UI�̃{�^���������ꂽ�ۂ̏����̐ݒ�
    /// </summary>
    private void SetUiAction()
    {
        // �ړ����@���󂯎��A�{�^���̐F�𒲐�����
        for (int i = 0; i < moveButton.Length; i++)
        {
            int count = i;
            moveButton[i].onClick.AddListener(() => {
                playerMove = (PlayerMove)count;
                ResetColorPlayerMoveButton();
            });
        }
    }

    /// <summary>
    /// �f�[�^�̃��[�h
    /// </summary>
    private void DataLoad()
    {
        playerMove = (PlayerMove)PlayerPrefs.GetInt(PlayerPrefsEnum.PlayerMove.ToString());
    }

    /// <summary>
    /// �f�[�^�̔��f
    /// </summary>
    private void DataSet()
    {
        switch (playerMove)
        {
            case PlayerMove.ControlledMove:
                Player.instance.ChangeMove(new ControlledMove());
                break;

            case PlayerMove.AutoMove:
                Player.instance.ChangeMove(new AutoMove());
                break;

            default:
                Debug.LogError("move��ݒ�ł��܂���");
                break;
        }
    }

    /// <summary>
    /// �f�[�^�̃Z�[�u
    /// </summary>
    private void DataSave()
    {
        PlayerPrefs.SetInt(PlayerPrefsEnum.PlayerMove.ToString(), (int)playerMove);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// �v���C���[�̈ړ����@�w��{�^���̐F�𒲐����鏈��
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