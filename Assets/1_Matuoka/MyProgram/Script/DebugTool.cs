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
    private bool isDebugTool = false;
    [SerializeField] private GameObject uiWindow;

    private enum PlayerPrefsEnum
    {
        TimeScale = 0,
        PlayerMove,
        SavePosName,
        SavePosX,
        SavePosY,
        SavePosZ,
    }

    private float timeScale;
    [SerializeField] private Slider timeScaleSlider;
    [SerializeField] private Text timeScaleText;
    [SerializeField] private Button timeScaleResetButton;
    private const int CONSTANT_VELOCITY = 10;
    private const string TIME_SCALE_TEXT_FORMAT = "x {0}倍";

    private enum PlayerMove
    {
        ControlledMove = 0,
        AutoMove,
    }
    private PlayerMove playerMove;
    [SerializeField] private Button[] moveButton;
    private Button nowPlayButton;


    [SerializeField] private GameObject content;
    [SerializeField] private InputField inputField;
    [SerializeField] private Button savePosButton;
    [SerializeField, Range(1, 10)] private int SAVE_LIMIT = 10;
    public List<string> posNames = new();
    public List<Vector3> posVectors = new(); 
    public List<GameObject> posButtonsObj = new();
    public List<Button> posButtons = new();
    public List<Text> posButtonsText = new();
    public Vector3 playerPos;

    #endregion


    #region MonoBehaviourMethod

    private void Awake()
    {
        DataLoad();
        SetUiActions();
        posButtonsObj.Add(content.transform.GetChild(0).gameObject);
        SavePosButtonChange();
        SavePosInit();
    }

    private void Start()
    {
        DataSet();
        ResetColorPlayerMoveButton();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) == false) return;
        InputKey();
    }

    #endregion


    #region CustomMethod

    /// <summary>
    /// データのロード
    /// </summary>
    private void DataLoad()
    {
        timeScale = PlayerPrefs.GetFloat(PlayerPrefsEnum.TimeScale.ToString(), 1f);
        playerMove = (PlayerMove)PlayerPrefs.GetInt(PlayerPrefsEnum.PlayerMove.ToString());
        SavePosLoad();
    }

    /// <summary>
    /// UIのボタンが押された際の処理の設定
    /// </summary>
    private void SetUiActions()
    {
        // タイムスケールのスライダーが変更された際の処理
        timeScaleSlider.onValueChanged.AddListener(value => {
            timeScale = value / CONSTANT_VELOCITY;
            timeScaleText.text = LibFuncUtility.TextFormatBuilder(TIME_SCALE_TEXT_FORMAT, timeScale.ToString("F1"));
        });

        // タイムスケールのリセットボタンが押された際の処理
        timeScaleResetButton.onClick.AddListener(() => timeScaleSlider.value = CONSTANT_VELOCITY);

        // ボタンに関数を登録
        for (int i = 0; i < moveButton.Length; i++)
        {
            int count = i;
            moveButton[i].onClick
                .AddListener(() => PlayerMoveButton((PlayerMove)count));
        }
    }

    /// <summary>
    /// データの反映
    /// </summary>
    private void DataSet()
    {
        TimeScaleChange();
        PlayerMoveChange();
        PlayerPosChange();
    }

    /// <summary>
    /// データのセーブ
    /// </summary>
    private void DataSave()
    {
        PlayerPrefs.SetFloat(PlayerPrefsEnum.TimeScale.ToString(), timeScale);
        PlayerPrefs.SetInt(PlayerPrefsEnum.PlayerMove.ToString(), (int)playerMove);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// DebugToolのON・OFF切り替え
    /// </summary>
    private void InputKey()
    {
        if (isDebugTool == false)
        {
            DebugToolOpen();
        }
        else
        {
            DebugToolClose();
        }

        isDebugTool = !isDebugTool;
        uiWindow.SetActive(isDebugTool);
    }

    /// <summary>
    /// DebugToolをONにする処理
    /// </summary>
    private void DebugToolOpen()
    {
        Time.timeScale = 0f;

        playerPos = Player.instance.transform.position;
    }

    /// <summary>
    /// DebugToolをOFFにする処理
    /// </summary>
    private void DebugToolClose()
    {
        DataSave();
        DataSet();
    }

    /// <summary>
    /// TimeScaleの変更
    /// </summary>
    private void TimeScaleChange()
    {
        Time.timeScale = timeScale;
        timeScaleSlider.value = timeScale * CONSTANT_VELOCITY;
    }

    /// <summary>
    /// 移動方法の変更
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
                Debug.LogError("moveを設定できません");
                break;
        }
    }

    private void PlayerPosChange()
    {
        Player.instance.transform.position = playerPos;
    }

    /// <summary>
    /// 移動方法を切り替えるボタン
    /// </summary>
    private void PlayerMoveButton(PlayerMove playerMove)
    {
        this.playerMove = playerMove;
        ResetColorPlayerMoveButton();
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

    private void SavePosInit()
    {
        playerPos = Player.instance.transform.position;
    }

    private void SavePosLoad()
    {
        for (int i = 0; i < SAVE_LIMIT; i++)
        {
            string @tempString = PlayerPrefs.GetString(PlayerPrefsEnum.SavePosName.ToString() + i, "None");

            if (@tempString == "None") break;

            posNames.Add(@tempString);

            Vector3 @tempVector3 = Vector3.zero;

            @tempVector3.x = PlayerPrefs.GetFloat(PlayerPrefsEnum.SavePosX.ToString() + i);
            @tempVector3.y = PlayerPrefs.GetFloat(PlayerPrefsEnum.SavePosY.ToString() + i);
            @tempVector3.z = PlayerPrefs.GetFloat(PlayerPrefsEnum.SavePosZ.ToString() + i);

            posVectors.Add(tempVector3);
        }  
    }


    private void SavePosButtonChange()
    {
        for (int i = 0; i < posNames.Count; i++)
        {
            if(i != 0) posButtonsObj.Add(Instantiate(posButtonsObj[0], content.transform));

            posButtons.Add(posButtonsObj[i].GetComponent<Button>());
            posButtonsText.Add(posButtonsObj[i].GetComponentInChildren<Text>());

            posButtonsText[i].text = PlayerPrefs.GetString(PlayerPrefsEnum.SavePosName.ToString() + i, "None");
        }

        posButtonsObj[0].SetActive(posNames.Count != 0);
    }

    public void SavePosSaveButton()
    {
        posNames.Add(inputField.text);
        inputField.text = "";
        PlayerPrefs.SetString(PlayerPrefsEnum.SavePosName.ToString() + (posNames.Count - 1), posNames[^1]);

        posVectors.Add(Player.instance.transform.position);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosX.ToString() + (posVectors.Count - 1), posVectors[^1].x);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosY.ToString() + (posVectors.Count - 1), posVectors[^1].y);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosZ.ToString() + (posVectors.Count - 1), posVectors[^1].z);

        PlayerPrefs.Save();


        posButtonsObj[0].SetActive(true);

        if (posButtonsObj.Count != posNames.Count) posButtonsObj.Add(Instantiate(posButtonsObj[0], content.transform));

        posButtons.Add(posButtonsObj[^1].GetComponent<Button>());
        posButtonsText.Add(posButtonsObj[^1].GetComponentInChildren<Text>());

        posButtonsText[^1].text = posNames[^1];
    }

    #endregion

#else

    private void Awake()
    {
        Destroy(this.gameObject);
    }

#endif


}
