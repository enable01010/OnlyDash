using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using static Player;
/// <summary>
/// SwitchBlock
/// </summary>

public class DebugTool : MonoBehaviour
{
#if UNITY_EDITOR

    #region Field

    [SerializeField] private bool isStartOpen = false;
    private bool isDebugToolOpen = false;
    [SerializeField] private GameObject uiWindow;

    private enum PlayerPrefsEnum
    {
        TimeScale = 0,
        PlayerMove,
        SavePosName,
        SavePosX,
        SavePosY,
        SavePosZ,
        SaveCameraPosX,
        SaveCameraPosY,
    }

    private float timeScale;
    [SerializeField] private Slider timeScaleSlider;
    [SerializeField] private Text timeScaleText;
    [SerializeField] private Button timeScaleResetButton;
    private const int CONSTANT_VELOCITY = 10;
    private const string TIME_SCALE_TEXT_FORMAT = "x {0}�{";

    private enum PlayerMove
    {
        ControlledMove = 0,
        AutoMove,
    }
    private PlayerMove playerMove;
    [SerializeField] private Button[] moveButton;


    [SerializeField] private Transform content;
    [SerializeField] private InputField inputField;
    [SerializeField] private Button savePosButton;

    public struct SavePosData
    {
        public string saveName;
        public Vector3 playerPos;
        public Vector3 cameraEulerAngle;
        public GameObject buttonObj;
        public Button button;
        public Text buttonText;
    }

    private List<SavePosData> savePosData = new();
    private GameObject baseButton;

    #endregion


    #region MonoBehaviourMethod

    private void Awake()
    {
        DataLoad();
        SetUiActions();
    }

    private void Start()
    {
        DataSet();
        ResetColorPlayerMoveButton();

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
    /// �f�[�^�̃��[�h
    /// </summary>
    private void DataLoad()
    {
        timeScale = PlayerPrefs.GetFloat(PlayerPrefsEnum.TimeScale.ToString(), 1f);
        playerMove = (PlayerMove)PlayerPrefs.GetInt(PlayerPrefsEnum.PlayerMove.ToString());
        SavePosLoad();
    }

    /// <summary>
    /// UI�̃{�^���������ꂽ�ۂ̏����̐ݒ�
    /// </summary>
    private void SetUiActions()
    {
        // �^�C���X�P�[���̃X���C�_�[���ύX���ꂽ�ۂ̏���
        timeScaleSlider.onValueChanged.AddListener(value => {
            timeScale = value / CONSTANT_VELOCITY;
            timeScaleText.text = LibFuncUtility.TextFormatBuilder(TIME_SCALE_TEXT_FORMAT, timeScale.ToString("F1"));
        });

        // �^�C���X�P�[���̃��Z�b�g�{�^���������ꂽ�ۂ̏���
        timeScaleResetButton.onClick.AddListener(() => timeScaleSlider.value = CONSTANT_VELOCITY);

        // �{�^���Ɋ֐���o�^
        for (int i = 0; i < moveButton.Length; i++)
        {
            int count = i;
            moveButton[i].onClick
                .AddListener(() => PlayerMoveButton((PlayerMove)count));
        }
    }

    /// <summary>
    /// �f�[�^�̔��f
    /// </summary>
    private void DataSet()
    {
        TimeScaleChange();
        PlayerMoveChange();
    }

    /// <summary>
    /// �f�[�^�̃Z�[�u
    /// </summary>
    private void DataSave()
    {
        PlayerPrefs.SetFloat(PlayerPrefsEnum.TimeScale.ToString(), timeScale);
        PlayerPrefs.SetInt(PlayerPrefsEnum.PlayerMove.ToString(), (int)playerMove);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// DebugTool��ON�EOFF�؂�ւ�
    /// </summary>
    private void ChangeDebugTool()
    {
        if (isDebugToolOpen == false)
        {
            DebugToolOpen();
        }
        else
        {
            DebugToolClose();
        }

        isDebugToolOpen = !isDebugToolOpen;
        uiWindow.SetActive(isDebugToolOpen);
    }

    /// <summary>
    /// DebugTool���J������
    /// </summary>
    private void DebugToolOpen()
    {
        Time.timeScale = 0f;
        Player.instance.GetController().enabled = false;
    }

    /// <summary>
    /// DebugTool����鏈��
    /// </summary>
    private void DebugToolClose()
    {
        DataSave();
        DataSet();

        Player.instance.GetController().enabled = true;
    }

    /// <summary>
    /// TimeScale�̕ύX
    /// </summary>
    private void TimeScaleChange()
    {
        Time.timeScale = timeScale;
        timeScaleSlider.value = timeScale * CONSTANT_VELOCITY;
    }

    /// <summary>
    /// �ړ����@�̕ύX
    /// </summary>
    private void PlayerMoveChange()
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
    /// �ړ����@��؂�ւ���{�^��
    /// </summary>
    private void PlayerMoveButton(PlayerMove playerMove)
    {
        this.playerMove = playerMove;
        ResetColorPlayerMoveButton();
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

    /// <summary>
    /// �{�^���𐶐����ă��X�g�ɒǉ�
    /// </summary>
    /// <param name="saveName"></param>
    /// <param name="playerPos"></param>
    /// <param name="cameraEulerAngle"></param>
    private void InstantiateSaveButton(string saveName, Vector3 playerPos, Vector3 cameraEulerAngle)
    {
        // �{�^������
        GameObject @obj = Instantiate(baseButton, content);
        @obj.SetActive(true);

        // �{�^���̊֐��ݒ�
        Button @button = @obj.GetComponent<Button>();
        @button.onClick.AddListener(() => SavePosButtonWarp(playerPos, cameraEulerAngle));

        // �{�^����Text�ύX
        Text @text = @obj.GetComponentInChildren<Text>();
        @text.text = saveName;

        // Delete�{�^���̊֐��ݒ�
        obj.transform.GetChild(1).GetComponent<Button>().onClick
            .AddListener(() => SavePosDeleteButton(saveName));

        // List�ɒǉ�
        SavePosData @tempSavePosData;
        @tempSavePosData.saveName = saveName;
        @tempSavePosData.playerPos = playerPos;
        @tempSavePosData.cameraEulerAngle = cameraEulerAngle;
        @tempSavePosData.buttonObj = @obj;
        @tempSavePosData.button = @button;
        @tempSavePosData.buttonText = @text;
        savePosData.Add(@tempSavePosData);
    }

    /// <summary>
    /// �N�����̃{�^���̐���
    /// </summary>
    private void SavePosLoad()
    {
        baseButton = content.GetChild(0).gameObject;

        int index = 0;
        while (true)
        {
            // ���O���擾
            string name = PlayerPrefs.GetString(PlayerPrefsEnum.SavePosName.ToString() + index);
            
            // �ۑ��f�[�^�������Ȃ��break
            if (string.IsNullOrEmpty(name)) break;

            // Player�̈ʒu���擾
            Vector3 playerPos = Vector3.zero;
            playerPos.x = PlayerPrefs.GetFloat(PlayerPrefsEnum.SavePosX.ToString() + index);
            playerPos.y = PlayerPrefs.GetFloat(PlayerPrefsEnum.SavePosY.ToString() + index);
            playerPos.z = PlayerPrefs.GetFloat(PlayerPrefsEnum.SavePosZ.ToString() + index);

            // Camera�̊p�x���擾
            Vector3 cameraAngle = Vector3.zero;
            cameraAngle.x = PlayerPrefs.GetFloat(PlayerPrefsEnum.SaveCameraPosX.ToString() + index);
            cameraAngle.y = PlayerPrefs.GetFloat(PlayerPrefsEnum.SaveCameraPosY.ToString() + index);

            // �{�^���̐���
            InstantiateSaveButton(name, playerPos, cameraAngle);

            index++;
        }
    }

    /// <summary>
    /// �V���ȃZ�[�u�|�C���g�̒ǉ�
    /// </summary>
    public void SavePosSaveButton()
    {
        int index = savePosData.Count;

        // ���O�̕ۑ�
        string @string = inputField.text;
        inputField.text = "";
        PlayerPrefs.SetString(PlayerPrefsEnum.SavePosName.ToString() + index, @string);

        // Player�̈ʒu�̕ۑ�
        Vector3 @playerPos = Player.instance.transform.position;
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosX.ToString() + index, @playerPos.x);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosY.ToString() + index, @playerPos.y);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosZ.ToString() + index, @playerPos.z);

        // Camera�̈ʒu�̕ۑ�
        Vector3 @cameraAngle = Player.instance.DebugCameraAngleSGet();
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosX.ToString() + index, @cameraAngle.x);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosY.ToString() + index, @cameraAngle.y);

        PlayerPrefs.Save();

        // �{�^���̐���
        InstantiateSaveButton(@string, @playerPos, @cameraAngle);
    }

    /// <summary>
    /// �Z�[�u������
    /// </summary>
    /// <param name="name"></param>
    public void SavePosDeleteButton(string name)
    {
        // �f�[�^�����X�g����폜
        int index = 0;
        for(; index < savePosData.Count; index++)
        {
            if (savePosData[index].saveName == name)
            {
                break;
            }
        }
        Destroy(savePosData[index].buttonObj);
        savePosData.RemoveAt(index);

        // PlayerPrefs�̖������폜
        int length = savePosData.Count;
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SavePosName.ToString() + length);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SavePosX.ToString() + length);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SavePosY.ToString() + length);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SavePosZ.ToString() + length);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SaveCameraPosX.ToString() + length);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SaveCameraPosY.ToString() + length);

        // PlayerPrefs�̍X�V(�㏑��)
        for (int i = 0; i < savePosData.Count; i++)
        {
            PlayerPrefs.SetString(PlayerPrefsEnum.SavePosName.ToString() + i, savePosData[i].saveName);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosX.ToString() + i, savePosData[i].playerPos.x);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosY.ToString() + i, savePosData[i].playerPos.y);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosZ.ToString() + i, savePosData[i].playerPos.z);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosX.ToString() + i, savePosData[i].cameraEulerAngle.x);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosY.ToString() + i, savePosData[i].cameraEulerAngle.y);
        }
    }

    /// <summary>
    /// �{�^�����������烏�[�v����
    /// </summary>
    /// <param name="playerPos"></param>
    /// <param name="cameraPos"></param>
    private void SavePosButtonWarp(Vector3 playerPos, Vector3 cameraAngle)
    {
        Player.instance.transform.position = playerPos;
        Player.instance.DebugCameraAngleSet(cameraAngle);
    }

    //public static class PlayerPrefsManager
    //{
    //    public static void Load(List<SavePosData> savePosData)
    //    {
    //        
    //    }

    //    public static void Save()
    //    {

    //    }

    //    public static void Delete()
    //    {

    //    }
    //}


    #endregion

#else

    private void Awake()
    {
        Destroy(this.gameObject);
    }

#endif


}
