#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.UI;

public class TimeScaleChange : DebugToolBase
{
    #region Field

    public enum PlayerPrefsEnum
    {
        TimeScale = 0,
    }

    private float timeScale;
    [SerializeField] private Slider timeScaleSlider;
    [SerializeField] private Text timeScaleText;
    [SerializeField] private Button timeScaleResetButton;
    private const int CONSTANT_VELOCITY = 10;
    private const string TIME_SCALE_TEXT_FORMAT = "x {0}倍";

    #endregion

    #region DebugToolBaseMethod

    public override void DebugToolAwake()
    {
        SetUiAction();
        DataLoad();
    }

    public override void DebugToolStart()
    {
        DataSet();
    }

    //public override void DebugToolOpen()
    //{
    //    Time.timeScale = 0f;
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
        // タイムスケールのスライダーが変更された際の処理
        // 値を変換して受け取り、テキストを変える
        timeScaleSlider.onValueChanged.AddListener(value => {
            timeScale = value / CONSTANT_VELOCITY;
            timeScaleText.text = LibFuncUtility.TextFormatBuilder(TIME_SCALE_TEXT_FORMAT, timeScale.ToString("F1"));
        });

        // タイムスケールのリセットボタンが押された際の処理
        // スライダーを初期状態に戻す
        timeScaleResetButton.onClick.AddListener(() => timeScaleSlider.value = CONSTANT_VELOCITY);
    }

    /// <summary>
    /// データのロード
    /// </summary>
    private void DataLoad()
    {
        timeScale = PlayerPrefs.GetFloat(PlayerPrefsEnum.TimeScale.ToString(), 1f);
        timeScaleSlider.value = timeScale * CONSTANT_VELOCITY;
    }

    /// <summary>
    /// データの反映
    /// </summary>
    private void DataSet()
    {
        Time.timeScale = timeScale;
    }

    /// <summary>
    /// データのセーブ
    /// </summary>
    private void DataSave()
    {
        PlayerPrefs.SetFloat(PlayerPrefsEnum.TimeScale.ToString(), timeScale);
        PlayerPrefs.Save();
    }

    #endregion
}

#endif