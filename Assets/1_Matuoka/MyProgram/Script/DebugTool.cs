using UnityEngine;
using UnityEngine.UI;
using System;
using static Player;
/// <summary>
/// isPause
/// DebugToolOff
/// DebugToolOn
/// SwitchBlock
/// </summary>

public class DebugTool : MonoBehaviour
{
#if UNITY_EDITOR

    #region Field
    private bool isPause = false;
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
    [SerializeField, Range(1, 10)] private int SAVE_LIMIT = 10;
    private GameObject[] buttonsObj;
    private Button[] buttons;
    private Text[] buttonsText;

    #endregion


    #region MonoBehaviourMethod

    private void Awake()
    {
        DataLoad();
        SetUiActions();
        SavePosInit();
    }

    private void Start()
    {
        DataSet();
        ReSetColorPlayerMoveButton();
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
        if (isPause == false)
        {
            DebugToolOn();
        }
        else
        {
            DebugToolOff();
        }

        isPause = !isPause;
        uiWindow.SetActive(isPause);
    }

    /// <summary>
    /// DebugToolをONにする処理
    /// </summary>
    private void DebugToolOn()
    {
        Time.timeScale = 0f;
    }

    /// <summary>
    /// DebugToolをOFFにする処理
    /// </summary>
    private void DebugToolOff()
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

    /// <summary>
    /// 移動方法を切り替えるボタン
    /// </summary>
    private void PlayerMoveButton(PlayerMove playerMove)
    {
        this.playerMove = playerMove;
        ReSetColorPlayerMoveButton();
    }


    /// <summary>
    /// プレイヤーの移動方法指定ボタンの色を調整する処理
    /// </summary>
    private void ReSetColorPlayerMoveButton()
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
        buttonsObj = new GameObject[SAVE_LIMIT];
        buttons = new Button[SAVE_LIMIT];
        buttonsText = new Text[SAVE_LIMIT];

        buttonsObj[0] = content.transform.GetChild(0).gameObject;
        for (int i = 0; i < SAVE_LIMIT; i++)
        {
            if(i != 0) buttonsObj[i] = Instantiate(buttonsObj[0], content.transform);

            buttons[i] = buttonsObj[i].GetComponent<Button>();
            buttonsText[i] = buttonsObj[i].GetComponentInChildren<Text>();

            buttonsText[i].text = PlayerPrefs.GetString(PlayerPrefsEnum.SavePosName.ToString() + i, "None");
        }
    }

    #endregion

#else

    private void Awake()
    {
        Destroy(this.gameObject);
    }

#endif


}
