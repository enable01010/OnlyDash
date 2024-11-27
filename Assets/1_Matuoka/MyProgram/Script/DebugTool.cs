using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DebugTool : MonoBehaviour
{
#if UNITY_EDITOR

    #region Field

    [SerializeField] private EventSystem eventSystem;

    private bool isPause = false;
    [SerializeField] private GameObject UI_Window;

    private enum PP
    {
        TimeScale = 0,
        PlayerMove,
    }

    private float timeScale;
    [SerializeField] private Slider timeScaleSlider;
    [SerializeField] private Text timeScaleText;
    [SerializeField] private Button timeScaleResetButton;
    private const int CONSTANT_VELOCITY = 10;


    [System.Serializable]
    public enum PlayerMove
    {
        ControlledMove = 0,
        AutoMove,
    }
    private int playerMove;
    [SerializeField] private Button[] moveButton;
    private Button nowPlayButton;


    #endregion


    #region MonoBehaviourMethod

    private void Awake()
    {
        DataLoad();
    }

    private void Start()
    {
        DataSet();

        timeScaleSlider.onValueChanged.AddListener(TimeScaleSliderChange);
        timeScaleResetButton.onClick.AddListener(TimeScaleReset);

        moveButton[(int)PlayerMove.ControlledMove].onClick.
            AddListener(() => PlayerMoveButton(PlayerMove.ControlledMove));
        moveButton[(int)PlayerMove.AutoMove].onClick.
            AddListener(() => PlayerMoveButton(PlayerMove.AutoMove));
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
        timeScale = PlayerPrefs.GetFloat(PP.TimeScale.ToString(), 1f);
        playerMove = PlayerPrefs.GetInt(PP.PlayerMove.ToString(), (int)PlayerMove.ControlledMove);
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
        PlayerPrefs.SetFloat(PP.TimeScale.ToString(), timeScale);
        PlayerPrefs.SetInt(PP.PlayerMove.ToString(), playerMove);

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
        UI_Window.SetActive(isPause);
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
    /// TimeScaleをTextに表示する
    /// </summary>
    /// <param name="num"></param>
    private void TimeScaleSliderChange(float num)
    {
        timeScale = num / 10f;
        timeScaleText.text = "x " + timeScale.ToString("F1");
        //timeScaleText.text = "x " + num.ToString("0.0");
    }

    /// <summary>
    /// TimeScaleを1に戻す
    /// </summary>
    private void TimeScaleReset()
    {
        timeScaleSlider.value = CONSTANT_VELOCITY;
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
            case (int)PlayerMove.ControlledMove:
                Player.instance.ChangeMove(new Player.ControlledMove());
                break;

            case (int)PlayerMove.AutoMove:
                Player.instance.ChangeMove(new Player.AutoMove());
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
        this.playerMove = (int)playerMove;


        // ボタンの色を白に戻す
        if (nowPlayButton != null)
        {
            ColorBlock colorBlock = nowPlayButton.colors;
            colorBlock.normalColor = Color.white;
            colorBlock.highlightedColor = Color.white;
            colorBlock.selectedColor = Color.white;

            nowPlayButton.colors = colorBlock;
        }

        // ボタンの色を黄色にする
        eventSystem.currentSelectedGameObject.TryGetComponent(out nowPlayButton);
        if (nowPlayButton != null)
        {
            ColorBlock colorBlock = nowPlayButton.colors;
            colorBlock.normalColor = Color.yellow;
            colorBlock.highlightedColor = Color.yellow;
            colorBlock.selectedColor = Color.yellow;
            nowPlayButton.colors = colorBlock;
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
