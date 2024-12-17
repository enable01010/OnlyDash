using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using static Player;
using static DebugTool;
using UnityEngine.SceneManagement;
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

    public enum PlayerPrefsEnum
    {
        TimeScale = 0,
        PlayerMove,
        PosName,
        PlayerPosX,
        PlayerPosY,
        PlayerPosZ,
        CameraEulerAngleX,
        CameraEulerAngleY,
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

    public class SavePosData
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
        DontDestroyOnLoad(gameObject);
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

        // �|�W�V�����Z�[�u�@�\�̐V�K�o�^�p�{�^��
        savePosButton.onClick.AddListener(SavePosSaveButton);
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

    private void InstantiateSaveButton(SavePosData data)
    {
        // �{�^������
        GameObject @obj = Instantiate(baseButton, content);
        @obj.SetActive(true);

        // �{�^���̊֐��ݒ�
        Button @button = @obj.GetComponent<Button>();
        @button.onClick.AddListener(() => SavePosButtonWarp(data.playerPos, data.cameraEulerAngle));

        // �{�^����Text�ύX
        Text @text = @obj.GetComponentInChildren<Text>();
        @text.text = data.saveName;

        // Delete�{�^���̊֐��ݒ�
        obj.transform.GetChild(1).GetComponent<Button>().onClick
            .AddListener(() => SavePosDeleteButton(data.saveName));

        // List�ɒǉ�
        data.buttonObj = @obj;
        data.button = @button;
        data.buttonText = @text;
    }

    /// <summary>
    /// �N�����̃{�^���̐���
    /// </summary>
    private void SavePosLoad()
    {
        baseButton = content.GetChild(0).gameObject;

        int index = 0;
        while (PlayerPrefsManager.TryLoad(index,out var data))
        {
            // �{�^���̐���
            InstantiateSaveButton(data);

            // ���X�g�ɓo�^
            savePosData.Add(data);

            index++;
        }
    }

    /// <summary>
    /// �V���ȃZ�[�u�|�C���g�̒ǉ�
    /// </summary>
    public void SavePosSaveButton()
    {
        int index = savePosData.Count;

        // �Z�[�u����f�[�^�̍쐬
        Vector3 @Vector = Player.instance.transform.position;
        SavePosData data = new SavePosData
        {
            saveName = inputField.text,
            playerPos = Player.instance.transform.position,
            cameraEulerAngle = Player.instance.DebugCameraAngleSGet()
        };

        // �Z�[�u���{
        data.Save(index);

        // �{�^���̐���
        InstantiateSaveButton(data);

        //���X�g�ɓo�^
        savePosData.Add(data);

        // ���͗�����ɂ���
        inputField.text = "";
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
            if (savePosData[index].saveName.Equals(name))
            {
                break;
            }
        }
        Destroy(savePosData[index].buttonObj);
        savePosData.RemoveAt(index);

        // PlayerPrefs�̖������폜
        PlayerPrefsManager.Delete(savePosData.Count);

        // PlayerPrefs�̍X�V(�㏑��)
        for (int i = 0; i < savePosData.Count; i++)
        {
            savePosData[i].Save(i);
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

    #endregion

#else

    private void Awake()
    {
        Destroy(this.gameObject);
    }

#endif
}

#if UNITY_EDITOR

/// <summary>
/// �f�o�b�O�p�̃v���C���[�ʒu���f�[�^����������p�̃N���X
/// </summary>
public static class PlayerPrefsManager
{

    /// <summary>
    /// �w�肳�ꂽ�C���f�b�N�X�Ƀf�[�^������ꍇ�擾
    /// </summary>
    /// <param name="index">�C���f�b�N�X</param>
    /// <param name="answer">�擾�����f�[�^</param>
    /// <returns>����������</returns>
    public static bool TryLoad(int index, out SavePosData answer)
    {
        // return�o���Ȃ��̂ňꎞ�I�ɓ��ꍞ��
        answer = null;

        // ���O���擾
        string name = PlayerPrefs.GetString(PlayerPrefsEnum.PosName.ToString() + index);
        
        // �ۑ��f�[�^��������Εԋp
        if (string.IsNullOrEmpty(name)) return false;


        answer = new SavePosData();

        // ���O�̓o�^
        answer.saveName = name;

        // Player�̈ʒu���擾
        Vector3 @Vector = Vector3.zero;
        @Vector.x = PlayerPrefs.GetFloat(PlayerPrefsEnum.PlayerPosX.ToString() + index);
        @Vector.y = PlayerPrefs.GetFloat(PlayerPrefsEnum.PlayerPosY.ToString() + index);
        @Vector.z = PlayerPrefs.GetFloat(PlayerPrefsEnum.PlayerPosZ.ToString() + index);
        answer.playerPos = @Vector;

        // Camera�̊p�x���擾
        @Vector.x = PlayerPrefs.GetFloat(PlayerPrefsEnum.CameraEulerAngleX.ToString() + index);
        @Vector.y = PlayerPrefs.GetFloat(PlayerPrefsEnum.CameraEulerAngleY.ToString() + index);
        @Vector.z = 0;
        answer.cameraEulerAngle = @Vector;

        return true;
    }

    /// <summary>
    /// �N���X���̏��Ńf�[�^��ۑ�
    /// </summary>
    /// <param name="data">�ۑ�����f�[�^</param>
    /// <param name="index">�C���f�b�N�X</param>
    public static void Save(this SavePosData data,int index)
    {
        PlayerPrefs.SetString(PlayerPrefsEnum.PosName.ToString() + index, data.saveName);

        // Player�̈ʒu�̕ۑ�
        PlayerPrefs.SetFloat(PlayerPrefsEnum.PlayerPosX.ToString() + index, data.playerPos.x);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.PlayerPosY.ToString() + index, data.playerPos.y);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.PlayerPosZ.ToString() + index, data.playerPos.z);

        // Camera�̈ʒu�̕ۑ�
        PlayerPrefs.SetFloat(PlayerPrefsEnum.CameraEulerAngleX.ToString() + index, data.cameraEulerAngle.x);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.CameraEulerAngleY.ToString() + index, data.cameraEulerAngle.y);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// �w�肵���C���f�b�N�X�̃f�[�^���폜����֐�
    /// </summary>
    /// <param name="index">�C���f�b�N�X</param>
    public static void Delete(int index)
    {
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.PosName.ToString() + index);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.PlayerPosX.ToString() + index);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.PlayerPosY.ToString() + index);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.PlayerPosZ.ToString() + index);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.CameraEulerAngleX.ToString() + index);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.CameraEulerAngleY.ToString() + index);
    }
}

#endif