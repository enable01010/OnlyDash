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


    [SerializeField] private GameObject content;
    [SerializeField] private InputField inputField;
    [SerializeField] private Button savePosButton;
    [SerializeField, Range(1, 10)] private int SAVE_LIMIT = 10;
    private List<string> posNames = new();
    public List<Vector3> posPlayerVectors = new();
    public List<Vector3> posCameraVectors = new();
    private List<GameObject> posButtonsObj = new();
    private GameObject baseButton;
    private List<Button> posButtons = new();
    private List<Text> posButtonsText = new();
    private GameObject playerFollowCamera;

    #endregion


    #region MonoBehaviourMethod

    private void Awake()
    {
        DataLoad();
        SetUiActions();
        baseButton = content.transform.GetChild(0).gameObject;
        SavePosButtonInstantiate();
    }

    private void Start()
    {
        DataSet();
        ResetColorPlayerMoveButton();

        playerFollowCamera = GameObject.Find("PlayerFollowCamera");
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
    /// �N������PlayerPrefs����̃f�[�^�擾
    /// </summary>
    private void SavePosLoad()
    {
        for (int i = 0; i < SAVE_LIMIT; i++)
        {
            // �f�[�^���Ȃ�������break
            string @string = PlayerPrefs.GetString(PlayerPrefsEnum.SavePosName.ToString() + i);
            if (string.IsNullOrEmpty(@string)) break;

            // List�ɖ��O���i�[
            posNames.Add(@string);

            // List��Player�̈ʒu���i�[
            Vector3 @player = Vector3.zero;
            @player.x = PlayerPrefs.GetFloat(PlayerPrefsEnum.SavePosX.ToString() + i);
            @player.y = PlayerPrefs.GetFloat(PlayerPrefsEnum.SavePosY.ToString() + i);
            @player.z = PlayerPrefs.GetFloat(PlayerPrefsEnum.SavePosZ.ToString() + i);
            posPlayerVectors.Add(@player);

            // List��Camera�̈ʒu���i�[
            Vector3 @camera = Vector3.zero;
            @camera.x = PlayerPrefs.GetFloat(PlayerPrefsEnum.SaveCameraPosX.ToString() + i);
            @camera.y = PlayerPrefs.GetFloat(PlayerPrefsEnum.SaveCameraPosY.ToString() + i);
            posCameraVectors.Add(@camera);
        }  
    }

    /// <summary>
    /// �N�����̃{�^������
    /// </summary>
    private void SavePosButtonInstantiate()
    {
        for (int i = 0; i < posNames.Count; i++)
        {
            var obj = Instantiate(baseButton, content.transform);
            posButtonsObj.Add(obj);

            // ��A�N�e�B�u�̃I�u�W�F�N�g�𕡐����邽�߃A�N�e�B�u�ɕύX
            obj.SetActive(true);

            // �{�^���̊֐��ݒ�
            Vector3 tempPlayerPos = posPlayerVectors[i];
            Vector3 tempCameraPos = posCameraVectors[i];
            obj.GetComponent<Button>().onClick
                .AddListener(() => 
                {
                    SavePosButtonWarp(tempPlayerPos, tempCameraPos);
                });

            // List�Ɋi�[
            posButtons.Add(obj.GetComponent<Button>());
            posButtonsText.Add(obj.GetComponentInChildren<Text>());

            // �{�^����Text�ύX
            posButtonsText[i].text = posNames[i];

            // Delete�{�^���̊֐��ݒ�
            string name = posNames[i];
            obj.transform.GetChild(1).GetComponent<Button>().onClick
                .AddListener(() => SavePosDeleteButton(name));
        }
    }

    /// <summary>
    /// �V���ȃZ�[�u�|�C���g�̒ǉ�
    /// </summary>
    public void SavePosSaveButton()
    {
        // ���O�̕ۑ�
        posNames.Add(inputField.text);
        inputField.text = "";
        PlayerPrefs.SetString(PlayerPrefsEnum.SavePosName.ToString() + (posNames.Count - 1), posNames[^1]);

        // Player�̈ʒu�̕ۑ�
        posPlayerVectors.Add(Player.instance.transform.position);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosX.ToString() + (posPlayerVectors.Count - 1), posPlayerVectors[^1].x);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosY.ToString() + (posPlayerVectors.Count - 1), posPlayerVectors[^1].y);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosZ.ToString() + (posPlayerVectors.Count - 1), posPlayerVectors[^1].z);

        // Camera�̈ʒu�̕ۑ�
        posCameraVectors.Add(Player.instance.DebugCameraAngleSGet());
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosX.ToString() + (posCameraVectors.Count - 1), posCameraVectors[^1].x);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosY.ToString() + (posCameraVectors.Count - 1), posCameraVectors[^1].y);


        PlayerPrefs.Save();

        posButtonsObj.Add(Instantiate(baseButton,content.transform));

        posButtonsObj[^1].SetActive(true);

        // �{�^���̊֐��ݒ�
        Vector3 tempPlayerPos = posPlayerVectors[^1];
        Vector3 tempCameraPos = posCameraVectors[^1];
        posButtonsObj[^1].GetComponent<Button>().onClick
            .AddListener(() => 
            {
                SavePosButtonWarp(tempPlayerPos, tempCameraPos);
            });

        // List�Ɋi�[
        posButtons.Add(posButtonsObj[^1].GetComponent<Button>());
        posButtonsText.Add(posButtonsObj[^1].GetComponentInChildren<Text>());

        // �{�^����Text�ύX
        posButtonsText[^1].text = posNames[^1];

        // Delete�{�^���̊֐��ݒ�
        string name = posNames[^1];
        posButtonsObj[^1].transform.GetChild(1).GetComponent<Button>().onClick
                .AddListener(() => SavePosDeleteButton(name));
    }

    /// <summary>
    /// �Z�[�u������
    /// </summary>
    /// <param name="name"></param>
    public void SavePosDeleteButton(string name)
    {
        // �f�[�^�����X�g����폜
        int index = posNames.IndexOf(name);

        posNames.RemoveAt(index);
        posPlayerVectors.RemoveAt(index);
        posCameraVectors.RemoveAt(index);


        // PlayerPrefs�̖������폜
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SavePosName.ToString() + (posNames.Count));
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SavePosX.ToString() + (posPlayerVectors.Count));
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SavePosY.ToString() + (posPlayerVectors.Count));
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SavePosZ.ToString() + (posPlayerVectors.Count));
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SaveCameraPosX.ToString() + (posCameraVectors.Count));
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SaveCameraPosY.ToString() + (posCameraVectors.Count));


        // PlayerPrefs�̍X�V(�㏑��)
        for (int i = 0; i < posNames.Count; i++)
        {
            PlayerPrefs.SetString(PlayerPrefsEnum.SavePosName.ToString() + i, posNames[i]);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosX.ToString() + i, posPlayerVectors[i].x);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosY.ToString() + i, posPlayerVectors[i].y);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosZ.ToString() + i, posPlayerVectors[i].z);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosX.ToString() + i, posCameraVectors[i].x);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosY.ToString() + i, posCameraVectors[i].y);
        }


        // �{�^����0�ɂȂ�Ȃ��悤�ɂ���
        if (posButtonsObj.Count == 1)
        {
            posButtonsObj[0].SetActive(false);
            return;
        }

        // �{�^���̍폜
        Destroy(posButtonsObj[index]);

        posButtonsObj.RemoveAt(index);
        posButtons.RemoveAt(index);
        posButtonsText.RemoveAt(index);
    }

    /// <summary>
    /// �{�^�����������烏�[�v����
    /// </summary>
    /// <param name="playerPos"></param>
    /// <param name="cameraPos"></param>
    private void SavePosButtonWarp(Vector3 playerPos, Vector3 cameraPos)
    {
        Player.instance.transform.position = playerPos;
        Player.instance.DebugCameraAngleSet(cameraPos);
    }

    #endregion

#else

    private void Awake()
    {
        Destroy(this.gameObject);
    }

#endif


}
